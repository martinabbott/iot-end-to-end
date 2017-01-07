using GrovePi;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace SprinklerControl
{
    public sealed partial class MainPage : Page
    {
        static readonly IBuildGroveDevices deviceFactory = DeviceFactory.Build;
        static DeviceClient deviceClient;

        public MainPage()
        {
            this.InitializeComponent();
            deviceClient = DeviceClient.CreateFromConnectionString("[iothub-device-sas]");

            ReceiveIoTHub();
        }

        private static async void ReceiveIoTHub()
        {
            var buzzer = deviceFactory.Buzzer(Pin.DigitalPin2);
            var button = deviceFactory.ButtonSensor(Pin.DigitalPin4);
            var rgb = deviceFactory.RgbLcdDisplay();
            rgb.SetBacklightRgb(200, 125, 0);
            rgb.SetText("Sprinklers\nonline");

            while (true)
            {
                try
                {
                    if (button.CurrentState == GrovePi.Sensors.SensorStatus.On)
                    {
                        rgb.SetBacklightRgb(200, 125, 0);
                        rgb.SetText("Sprinklers\nonline");
                        buzzer.ChangeState(GrovePi.Sensors.SensorStatus.Off);
                    }

                    Message receivedMessage = await deviceClient.ReceiveAsync();
                    if (receivedMessage == null) continue;

                    var message = JsonConvert.DeserializeObject<SprinklerCommand>(Encoding.UTF8.GetString(receivedMessage.GetBytes()));

                    if (message.activate)
                    {
                        rgb.SetBacklightRgb(250, 0, 0);
                        rgb.SetText("Zone: " + message.deviceName + "\nTemp: " + message.temperature.ToString());
                        buzzer.ChangeState(GrovePi.Sensors.SensorStatus.On);
                    }
                    else
                    {
                        rgb.SetBacklightRgb(0, 75, 200);
                        rgb.SetText("Zone: " + message.deviceName + "\nFire: Out");
                        buzzer.ChangeState(GrovePi.Sensors.SensorStatus.Off);
                    }

                    await deviceClient.CompleteAsync(receivedMessage);
                }
                catch (Exception e)
                {
                    var msg = e.Message;
                }
            }
        }
    }

    public sealed class SprinklerCommand
    {
        public string deviceName;
        public double temperature;
        public bool activate;
    }
}
