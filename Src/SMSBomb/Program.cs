using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SMSBomb
{
    class Program
    {
        static async Task Main(string[] args)
        {
            args = new[] { "9120000000", "09121111111" };

            var bomber = new SMSBomber(GetData());
            foreach (var item in args)
            {
                await bomber.SendRequests(item);
                Console.WriteLine();
            }

            Console.WriteLine("Press Any Key To Exit ...");
            Console.ReadKey();

            List<ApiItem> GetData()
            {
                var datapath = Path.Combine(Directory.GetCurrentDirectory(), "AppData", "Data.Json");
                var text = File.ReadAllText(datapath);
                return JsonSerializer.Deserialize<List<ApiItem>>(text).Where(p => p.Enable).ToList();
            }
        }
    }
}