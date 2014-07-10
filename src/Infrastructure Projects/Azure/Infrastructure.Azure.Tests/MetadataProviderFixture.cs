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

namespace Infrastructure.Azure.Tests
{
    using Xunit;

    public class given_a_metadata_provider
    {
        [Fact]
        public void when_getting_metadata_then_returns_type_name()
        {
            var provider = new MetadataProvider();
            var typeName = typeof(given_a_metadata_provider).Name;

            var metadata = provider.GetMetadata(this);

            Assert.Contains(typeName, metadata.Values);
            Assert.Contains("EventType", metadata.Keys);
        }
    }
}
