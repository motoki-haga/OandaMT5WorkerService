

namespace OandaMT5WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Worker>();

            builder.Services.AddWindowsService(options =>
            {
                options.ServiceName = "Metatrader5 Import Service";
            });

            var host = builder.Build();
            host.Run();
        }
    }
}