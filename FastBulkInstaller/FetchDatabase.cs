using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FastBulkInstaller
{
    public class FetchDatabase
    {
        public static async Task<string> FetchDB()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("https://pastebin.com/raw/XkgNYRTL");

                    if (response.IsSuccessStatusCode)
                    {
                        string rawText = await response.Content.ReadAsStringAsync();
                        return rawText;
                    }
                    else
                    {
                        Console.WriteLine($"HTTP request failed with status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                return null;
            }
        }
    }
}
