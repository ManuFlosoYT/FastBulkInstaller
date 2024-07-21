using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace FastBulkInstaller
{
    public class FetchDatabase
    {
        public static async Task<string> FetchDB()
        {
            IEnumerable<string> lines = File.ReadLines(FBI.path);

            List<string> list = new List<string>(lines);

            //detect if a string is a valid URL or a local file address, store the result in a boolean variable
            bool isURL = Uri.TryCreate(list[3], UriKind.Absolute, out Uri uriResult);
            if (isURL)
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
                        return null;
                    }
                }
            }
            else
            {
                //read all lines of a file and return it as single string
                string filePath = Path.Combine(list[3].Replace("\"", ""));
                string rawText = File.ReadAllText(filePath);
                return rawText;
            }
            return null;
        }
    }
}
