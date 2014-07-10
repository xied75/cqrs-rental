﻿// ==============================================================================================================
// Microsoft patterns & practices
// CQRS Journey project
// ==============================================================================================================
// ©2012 Microsoft. All rights reserved. Certain content used with permission from contributors
// http://cqrsjourney.github.com/contributors/members
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance 
// with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software distributed under the License is 
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and limitations under the License.
// ==============================================================================================================

namespace Azure.IntegrationTests.EventSourcing.EventStoreFixture
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Infrastructure.Azure;
    using Infrastructure.Azure.EventSourcing;
    using Infrastructure.Azure.Messaging;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;
    using Xunit;

    public class given_empty_store : IDisposable
    {
        private readonly string tableName;
        private CloudStorageAccount account;
        protected EventStore sut;
        protected string sourceId;
        protected string partitionKey;
        protected EventData[] events;

        public given_empty_store()
        {
            this.tableName = "EventStoreFixture" + new Random((int) DateTime.Now.Ticks).Next();
            var settings = InfrastructureSettings.ReadEventSourcing("Settings.xml");
            this.account = CloudStorageAccount.Parse(settings.ConnectionString);
            this.sut = new EventStore(this.account, this.tableName);

            this.sourceId = Guid.NewGuid().ToString();
            this.partitionKey = Guid.NewGuid().ToString();
            this.events = new[]
                             {
                                 new EventData { Version = 1, SourceId = sourceId, SourceType = "Source", EventType = "Test1", Payload = "Payload1" },
                                 new EventData { Version = 2, SourceId = sourceId, SourceType = "Source", EventType = "Test2", Payload = "Payload2" },
                                 new EventData { Version = 3, SourceId = sourceId, SourceType = "Source", EventType = "Test3", Payload = "Payload3" },
                             };
        }

        public void Dispose()
        {
            var client = this.account.CreateCloudTableClient();
            client.DeleteTableIfExist(this.tableName);
        }
    }

    public class when_adding_items : given_empty_store
    {
        [Fact]
        public void when_adding_one_item_then_can_load_it()
        {
            sut.Save(this.partitionKey, new[] { events[0] });

            var stored = sut.Load(this.partitionKey, 0).ToList();

            Assert.Equal(1, stored.Count);
            Assert.Equal(events[0].Version, stored[0].Version);
            Assert.Equal(events[0].SourceId, stored[0].SourceId);
            Assert.Equal(events[0].EventType, stored[0].EventType);
            Assert.Equal(events[0].Payload, stored[0].Payload);
        }

        [Fact]
        public void when_adding_multiple_items_then_can_load_them_in_order()
        {
            sut.Save(this.partitionKey, events);

            var stored = sut.Load(this.partitionKey, 0).ToList();

            Assert.Equal(3, stored.Count);
            Assert.True(stored.All(x => x.SourceType == "Source"));
            Assert.True(stored.All(x => x.SourceId == this.sourceId));
            Assert.Equal(1, stored[0].Version);
            Assert.Equal(2, stored[1].Version);
            Assert.Equal(3, stored[2].Version);
            Assert.Equal("Payload1", stored[0].Payload);
            Assert.Equal("Payload2", stored[1].Payload);
            Assert.Equal("Payload3", stored[2].Payload);
        }

        [Fact]
        public void when_adding_multiple_items_at_different_times_then_can_load_them_in_order()
        {
            sut.Save(this.partitionKey, new[] { events[0], events[1] });
            sut.Save(this.partitionKey, new[] { events[2] });

            var stored = sut.Load(this.partitionKey, 0).ToList();

            Assert.Equal(3, stored.Count);
            Assert.True(stored.All(x => x.SourceType == "Source"));
            Assert.Equal(1, stored[0].Version);
            Assert.Equal(2, stored[1].Version);
            Assert.Equal(3, stored[2].Version);
            Assert.Equal("Payload1", stored[0].Payload);
            Assert.Equal("Payload2", stored[1].Payload);
            Assert.Equal("Payload3", stored[2].Payload);
        }

        [Fact]
        public void can_load_events_since_specified_version()
        {
            sut.Save(this.partitionKey, events);

            var stored = sut.Load(this.partitionKey, 2).ToList();

            Assert.Equal(2, stored.Count);
            Assert.Equal(2, stored[0].Version);
            Assert.Equal(3, stored[1].Version);
            Assert.Equal("Payload2", stored[0].Payload);
            Assert.Equal("Payload3", stored[1].Payload);
        }

        [Fact]
        public void cannot_store_same_version()
        {
            sut.Save(this.partitionKey, new[] { events[0] });

            var sameVersion = new EventData { Version = events[0].Version, EventType = "Test2", Payload = "Payload2" };
            Assert.Throws<ConcurrencyException>(() => sut.Save(this.partitionKey, new[] { sameVersion }));

            var stored = sut.Load(this.partitionKey, 0).ToList();

            Assert.Equal(1, stored.Count);
            Assert.Equal(1, stored[0].Version);
            Assert.Equal("Payload1", stored[0].Payload);
        }

        [Fact]
        public void when_storing_same_version_within_batch_then_aborts_entire_commit()
        {
            sut.Save(this.partitionKey, new[] { events[0] });

            var sameVersion = new EventData { Version = events[0].Version, EventType = "Test2", Payload = "Payload2" };
            Assert.Throws<ConcurrencyException>(() => sut.Save(this.partitionKey, new[] { sameVersion, events[1] }));

            var stored = sut.Load(this.partitionKey, 0).ToList();

            Assert.Equal(1, stored.Count);
            Assert.Equal(1, stored[0].Version);
            Assert.Equal("Payload1", stored[0].Payload);
        }
    }

    public class given_store_with_events : given_empty_store
    {
        public given_store_with_events()
        {
            sut.Save(this.partitionKey, new[] { events[0], events[1] });
        }
    }

    public class when_getting_pending_events : given_store_with_events
    {
        [Fact]
        public void can_get_all_events_for_partition_as_pending()
        {
            var pending = sut.GetPending(this.partitionKey).ToList();

            Assert.Equal(2, pending.Count);
            Assert.True(pending.All(x => x.SourceType == "Source"));
            Assert.Equal("Unpublished_" + events[0].Version.ToString("D10"), pending[0].RowKey);
            Assert.Equal("Payload1", pending[0].Payload);
            Assert.Equal("Test1", pending[0].EventType);
            Assert.Equal("Unpublished_" + events[1].Version.ToString("D10"), pending[1].RowKey);
            Assert.Equal("Payload2", pending[1].Payload);
            Assert.Equal("Test2", pending[1].EventType);
        }

        [Fact]
        public void when_deleting_pending_then_can_get_list_without_item()
        {
            var pending = sut.GetPending(this.partitionKey).ToList();
            sut.DeletePending(pending[0].PartitionKey, pending[0].RowKey);

            pending = sut.GetPending(this.partitionKey).ToList();

            Assert.Equal(1, pending.Count);
            Assert.Equal("Unpublished_" + events[1].Version.ToString("D10"), pending[0].RowKey);
            Assert.Equal("Payload2", pending[0].Payload);
            Assert.Equal("Test2", pending[0].EventType);
            Assert.Equal("Source", pending[0].SourceType);
        }

        [Fact]
        public void can_get_partition_with_pending_events()
        {
            var pending = sut.GetPartitionsWithPendingEvents().ToList();

            Assert.Equal(1, pending.Count);
            Assert.Equal(partitionKey, pending.Single());
        }
    }

    public class when_getting_pending_events_for_multiple_partitions : given_empty_store
    {
        protected string[] expectedPending;

        // use larger than 1000 in order to force getting continuation tokens, but it takes a lot of time
        private const int NumberOfPartitions = 5; //5000; 

        public when_getting_pending_events_for_multiple_partitions()
        {
            this.expectedPending = new string[NumberOfPartitions];
            for (int i = 0; i < expectedPending.Length; i++)
            {
                expectedPending[i] = "Test_" + Guid.NewGuid();
                sut.Save(expectedPending[i], new[] { events[0] });
            }
        }

        [Fact]
        public void can_get_all_partitions_with_pending_events()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var actual = sut.GetPartitionsWithPendingEvents().ToList();
            stopWatch.Stop();
            Debug.WriteLine(stopWatch.ElapsedMilliseconds);
            Assert.Equal(expectedPending.Length, actual.Distinct().Count());
            Assert.Equal(expectedPending.Length, actual.Intersect(expectedPending).Count());
        }
    }
}
