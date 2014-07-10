// ==============================================================================================================
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

namespace WorkerRoleCommandProcessor
{
    using System;
    using System.Data.Entity;
    using System.Threading;
    using Infrastructure.Blob;
    using Infrastructure.Database;
    using Infrastructure.EventSourcing;
    using Infrastructure.Messaging;
    using Infrastructure.Messaging.Handling;
    using Infrastructure.Processes;
    using Infrastructure.Serialization;
    using Infrastructure.Sql.Blob;
    using Infrastructure.Sql.Database;
    using Infrastructure.Sql.EventSourcing;
    using Infrastructure.Sql.Messaging;
    using Infrastructure.Sql.Messaging.Handling;
    using Infrastructure.Sql.Messaging.Implementation;
    using Infrastructure.Sql.Processes;
    using Microsoft.Practices.Unity;

    public sealed class RentalCommandProcessor : IDisposable
    {
        private IUnityContainer container;
        private CancellationTokenSource cancellationTokenSource;

        public RentalCommandProcessor()
        {
            this.container = CreateContainer();
            RegisterHandlers(container);

            Database.SetInitializer<EventStoreDbContext>(null);
            Database.SetInitializer<BlobStorageDbContext>(null);
        }

        public void Start()
        {
            this.cancellationTokenSource = new CancellationTokenSource();
            this.container.Resolve<CommandProcessor>().Start();
            this.container.Resolve<EventProcessor>().Start();
        }

        public void Stop()
        {
            this.container.Resolve<CommandProcessor>().Stop();
            this.container.Resolve<EventProcessor>().Stop();
            this.cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            this.container.Dispose();
        }

        private static UnityContainer CreateContainer()
        {
            var container = new UnityContainer();

            // infrastructure
            var serializer = new JsonTextSerializer();
            container.RegisterInstance<ITextSerializer>(serializer);

            var commandBus = new CommandBus(new MessageSender(Database.DefaultConnectionFactory, "SqlBus", "SqlBus.Commands"), serializer);
            var eventBus = new EventBus(new MessageSender(Database.DefaultConnectionFactory, "SqlBus", "SqlBus.Events"), serializer);

            var commandProcessor = new CommandProcessor(new MessageReceiver(Database.DefaultConnectionFactory, "SqlBus", "SqlBus.Commands"), serializer);
            var eventProcessor = new EventProcessor(new MessageReceiver(Database.DefaultConnectionFactory, "SqlBus", "SqlBus.Events"), serializer);

            container.RegisterInstance<ICommandBus>(commandBus);
            container.RegisterInstance<IEventBus>(eventBus);
            container.RegisterInstance<ICommandHandlerRegistry>(commandProcessor);
            container.RegisterInstance(commandProcessor);
            container.RegisterInstance<IEventHandlerRegistry>(eventProcessor);
            container.RegisterInstance(eventProcessor);

            // repository
            container.RegisterType<EventStoreDbContext>(new TransientLifetimeManager(), new InjectionConstructor("EventStore"));
            container.RegisterType(typeof(IEventSourcedRepository<>), typeof(SqlEventSourcedRepository<>), new ContainerControlledLifetimeManager());

            container.RegisterType<IBlobStorage, SqlBlobStorage>(new ContainerControlledLifetimeManager(), new InjectionConstructor("BlobStorage"));

            // handlers

            return container;
        }

        private static void RegisterHandlers(IUnityContainer unityContainer)
        {
            var commandHandlerRegistry = unityContainer.Resolve<ICommandHandlerRegistry>();
            var eventHandlerRegistry = unityContainer.Resolve<IEventHandlerRegistry>();

            foreach (var commandHandler in unityContainer.ResolveAll<ICommandHandler>())
            {
                commandHandlerRegistry.Register(commandHandler);
            }

            foreach (var eventHandler in unityContainer.ResolveAll<IEventHandler>())
            {
                eventHandlerRegistry.Register(eventHandler);
            }
        }
    }
}