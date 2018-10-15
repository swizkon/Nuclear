using System;
using Alice.Organizational.Domain;
using Alice.Organizational.Events;
using EventStore.ClientAPI;
using Nuclear.NetCore.Aggregates;
using Nuclear.NetCore.EventStore;

namespace EventstoreConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // docker run --name eventstore-node -it --rm -p 2113:2113 -p 1113:1113 eventstore/eventstore

            Console.WriteLine("Hello EventstoreConsole!");

            var uri = new Uri("tcp://localhost:1113");

            try
            {
                var connection = EventStoreConnection.Create(uri);

                System.Console.WriteLine("Connect to Eventstore...");
                connection.ConnectAsync().Wait();
                System.Console.WriteLine("Connected!");

                var repo = new EventStoreRepository(connection);

                // WriteByEventType(repo);
                
                OrganizationTest(repo);
                
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void OrganizationTest(EventStoreRepository repo)
        {
            
                var organizationId = Guid.NewGuid();


                System.Console.WriteLine("Enter organization name:");
                var orgname = Console.ReadLine();

                var orgCreatec = new OrganizationCreated { Name = orgname };

                repo.WriteEvents(new AggregateKey(typeof(Organization), organizationId), new[] { orgCreatec });

                string newName = null;

                do
                {
                    var history = repo.ReadEvents(new AggregateKey(typeof(Organization), organizationId));

                    var organization = new Organization(organizationId, history);
                    System.Console.WriteLine("");
                    System.Console.WriteLine("Organization " + organization.Name);
                    System.Console.WriteLine("Rename: (q to exit)");
                    newName = Console.ReadLine();

                    if(!string.IsNullOrWhiteSpace(newName.Replace("q","")))
                    {
                        organization.Rename(newName);
                        organization.Save(repo);
                    }

                }
                while (newName != "q");
        }

        private static void WriteByEventType(EventStoreRepository repo)
        {

            System.Console.WriteLine("OrganizationCreated");
            var createdEvents = repo.ReadEventsByType<OrganizationCreated>();
            foreach(var evt in createdEvents)
            {
                System.Console.WriteLine(evt.Name);
            }
            
            System.Console.WriteLine("OrganizationRenamed");
            var renamedEvents = repo.ReadEventsByType<OrganizationRenamed>();
            foreach(var evt in renamedEvents)
            {
                System.Console.WriteLine(evt.NewName);
            }
        }
    }
}
