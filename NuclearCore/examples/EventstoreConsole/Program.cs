using System;

using EventStore.ClientAPI;

namespace EventstoreConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // docker run --name eventstore-node -it --rm -p 2113:2113 -p 1113:1113 eventstore/eventstore

            Console.WriteLine("Hello World!");

            // ConnectionSettings.Default.g

            var conn = ConnectionSettings.Default;

            var uri = new Uri("tcp://localhost:1113");

            try
            {
                var connection = EventStoreConnection.Create(uri);

                System.Console.WriteLine("Connect to Eventstore...");
                connection.ConnectAsync().Wait();
                System.Console.WriteLine("Connected!");

                string name = null;

                do
                {
                    System.Console.WriteLine("Enter a task: (q to exit)");
                    name = Console.ReadLine();

                    var evt = Events.EventDataBuilder.BuildTaskAddedEvent(name);

                    System.Console.WriteLine("Write to Eventstore...");
                    connection.AppendToStreamAsync("todos-jonas", ExpectedVersion.Any, evt).Wait();
                    System.Console.WriteLine("Written!!");

                }
                while (name != "q");


                connection.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
