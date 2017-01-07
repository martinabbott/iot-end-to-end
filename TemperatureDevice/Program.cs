using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;

namespace TemperatureDevice
{
    class Program
    {
        static DeviceClient deviceClient;
        static double temperature;
        static RoomData[] rooms = new RoomData[]
        {
            new RoomData { id = "lobby", connectionString = "[iothub-lobby-device]", colour = ConsoleColor.Cyan },
            new RoomData { id = "mainoffice", connectionString = "[iothub-mainoffice-deivce]", colour = ConsoleColor.Yellow },
            new RoomData { id = "sideoffice", connectionString = "[iothub-sideoffice-device", colour = ConsoleColor.Magenta },
            new RoomData { id = "kitchen", connectionString = "[iothub-kitchen-device]", colour = ConsoleColor.Green },
            new RoomData { id = "restroom", connectionString = "[iothub-restroom-device]", colour = ConsoleColor.Red }
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Building Management Demo");
            Console.WriteLine("What room am I?");
            Console.WriteLine("1: Lobby");
            Console.WriteLine("2: Main Office");
            Console.WriteLine("3: Side Office");
            Console.WriteLine("4: Kitchen");
            Console.WriteLine("5: Restroom");

            var id = int.Parse(Console.ReadLine());
            var room = rooms[id - 1]; ;

            Console.ForegroundColor = room.colour;

            deviceClient = DeviceClient.CreateFromConnectionString(room.connectionString);

            var rnd = new Random();
            var root = 23.0;

            do
            {
                while (!Console.KeyAvailable)
                {
                    SendToIoTHub(new DeviceData { id = room.id, date = DateTime.UtcNow, temperature = root + rnd.NextDouble() });
                    Thread.Sleep(1000);
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.UpArrow);

            do
            {
                var seed = 2.0;
                while (!Console.KeyAvailable)
                {
                    temperature = root + seed + rnd.NextDouble();
                    SendToIoTHub(new DeviceData { id = room.id, date = DateTime.UtcNow, temperature = temperature });
                    Thread.Sleep(1000);
                    if (seed < 400.0) seed *= 2.5;
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.DownArrow);

            do
            {
                var seed = 30.0;
                while (!Console.KeyAvailable)
                {
                    var rndVal = rnd.NextDouble(); 
                    temperature = temperature - seed + (rndVal > 0.5 ? rndVal : -rndVal);
                    SendToIoTHub(new DeviceData { id = room.id, date = DateTime.UtcNow, temperature = temperature });
                    Thread.Sleep(1000);
                    if (temperature < 50.0) seed = 0.0;
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.LeftArrow);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Finished");
            Console.ReadLine();
        }

        static async void SendToIoTHub(object data)
        {
            var messageString = JsonConvert.SerializeObject(data);
            var message = new Message(Encoding.UTF8.GetBytes(messageString));

            await deviceClient.SendEventAsync(message);
            Console.WriteLine("Sending: {1}", DateTime.Now, messageString);
        }
    }
}
