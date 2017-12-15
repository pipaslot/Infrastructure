using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Pipaslot.Demo
{
    /// <summary>
    /// Web Application
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Run App
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        /// <summary>
        /// Build WebHost
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
