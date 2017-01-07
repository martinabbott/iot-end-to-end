using System;
using System.IO;
using System.Text;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using System.Collections.Generic;

namespace IoTHubWebJob
{
    public class Functions
    {
        //public static bool activated = false;
        static Dictionary<string, double> zonetemps;
        static Dictionary<string, bool> zoneactive;

        public static void ProcessQueueMessage([ServiceBusTrigger("maxtemp")] string message, TextWriter log)
        {
            if (zonetemps == null) zonetemps = new Dictionary<string, double>();
            if (zoneactive == null) zoneactive = new Dictionary<string, bool>();

            var command = JsonConvert.DeserializeObject<CommandMessage>(message);
            var zone = command.id;
            var temp = command.max;

            if (!zonetemps.ContainsKey(zone))
            {
                zonetemps.Add(zone, temp);
            }
            else
            {
                zonetemps[zone] = temp;
            }

            if (!zoneactive.ContainsKey(zone)) zoneactive.Add(zone, false);

            if (zonetemps[zone] > 400.0 && !zoneactive[zone])
            {
                zoneactive[zone] = true;
                var sprinkler = new SprinklerCommand { deviceName = command.id, temperature = command.max, activate = true };
                SendToIoTHub(sprinkler);
            }
            else if (zonetemps[zone] < 60.0 && zoneactive[zone])
            {
                zoneactive[zone] = false;
                var sprinkler = new SprinklerCommand { deviceName = command.id, temperature = command.max, activate = false };
                SendToIoTHub(sprinkler);
            }
        }

        static async void SendToIoTHub(object data)
        {
            var messageString = JsonConvert.SerializeObject(data);
            var message = new Message(Encoding.UTF8.GetBytes(messageString));
            message.Ack = DeliveryAcknowledgement.Full;
            message.MessageId = Guid.NewGuid().ToString();

            var serviceClient = ServiceClient.CreateFromConnectionString("[iot-hub-service-connection]");
            await serviceClient.SendAsync("sprinkler", message);

            await serviceClient.CloseAsync();
        }
    }

    public class CommandMessage
    {
        public string id;
        public double max;
        public string date;
    }

    public class SprinklerCommand
    {
        public string deviceName;
        public double temperature;
        public bool activate;
    }
}
