using Microsoft.Azure.WebJobs;

namespace IoTHubWebJob
{
    public class Program
    {
        public static void Main()
        {
            var config = new JobHostConfiguration();
            config.UseServiceBus();

            var host = new JobHost(config);
            host.RunAndBlock();
        }
    }
}
