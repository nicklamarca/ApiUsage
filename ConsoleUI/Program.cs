using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ConsoleUI
{
    class Program
    {
        private static HttpClient apiClient;
        private static List<Person> peopleCache = new List<Person>();

        static async Task Main(string[] args)
        {
            InitializeClient();

            string lookupAnother = String.Empty;

            do
            {
                Console.Write("What ID would you like to use: ");
                string idText = Console.ReadLine();

                try
                {
                    Person person = await GetStarWarsCharacter(idText);
                    Console.WriteLine($"{person.FullName} is a {person.Gender}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.Write("Do you want to lookup another character (yes/no): ");
                lookupAnother = Console.ReadLine();

                Console.Clear();

            } while (lookupAnother.ToLower()=="yes");

            Console.ReadLine();
        }

        private static async Task<Person> GetStarWarsCharacter(string id)
        {
            string URL = $"https://swapi.py4e.com/api/people/{id}/";

            Person cached = peopleCache.Where(x => x.Id == id).FirstOrDefault();

            if(cached != null)
            {
                return cached;
            }

            using (HttpResponseMessage response = await apiClient.GetAsync(URL))
            {
                if (response.IsSuccessStatusCode)
                {
                    Person output = await response.Content.ReadAsAsync<Person>();
                    output.Id = id;
                    peopleCache.Add(output);
                    return output;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        private static void InitializeClient()
        {
            apiClient = new HttpClient();
            apiClient.DefaultRequestHeaders.Accept.Clear();
            apiClient.DefaultRequestHeaders.Accept.Add(
                                            new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }

}
