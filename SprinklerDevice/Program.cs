using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprinklerDevice
{
    class Program
    {
        static DeviceClient deviceClient;
        static void Main(string[] args)
        {
            Console.WriteLine("Sprinkler Control System:\n");
            deviceClient = DeviceClient.CreateFromConnectionString("[iothub-device-sas]");

            ReceiveIoTHub();
            Console.ReadLine();
        }

        private static async void ReceiveIoTHub()
        {
            while (true)
            {
                Message receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                Console.WriteLine("\nReceiving sprinkler command:");
                var message = Encoding.UTF8.GetString(receivedMessage.GetBytes());
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Message: {0}", message);
                Console.ResetColor();

                await deviceClient.CompleteAsync(receivedMessage);
            }
        }
    }
}

