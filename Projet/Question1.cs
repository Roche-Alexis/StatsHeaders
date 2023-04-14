using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Projet
{
    internal class Question1
    {


        public static string GetServerTypeStats(List<string> urls)
        {
            Dictionary<string, int> serverPopularity = new Dictionary<string, int>();
            StringBuilder responseBuilder = new StringBuilder();


            Parallel.ForEach(urls, url =>
            {
                HttpWebRequest siteRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse siteResponse = null;

                try
                {
                    siteRequest.Timeout = 3000; // Timeout after 3 seconds
                    siteResponse = (HttpWebResponse)siteRequest.GetResponse();


                    string serverType = siteResponse.Headers["Server"];

                    if (serverType != null)
                    {
                        if (serverPopularity.ContainsKey(serverType))
                        {
                            serverPopularity[serverType]++;
                        }
                        else
                        {
                            serverPopularity[serverType] = 1;
                        }
                        responseBuilder.AppendLine($"{url} utilise un serveur {serverType}");

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching server type for {url}: {ex.Message}");
                }
                finally
                {
                    siteResponse?.Close();
                }
            });




            int totalCount = serverPopularity.Values.Sum();

            StringBuilder statsBuilder = new StringBuilder();


            foreach (KeyValuePair<string, int> serverType in serverPopularity.OrderByDescending(x => x.Value))
            {
                double percentage = (double)serverType.Value / totalCount * 100;

                statsBuilder.AppendLine($"{serverType.Key}: {serverType.Value} ({percentage:0.##}%)");
            }

            statsBuilder.Insert(0, "Voici les statistiques sur les serveurs les plus utilisés : \n");



            StringBuilder combinedBuilder = new StringBuilder();
            combinedBuilder.Append(statsBuilder);
            combinedBuilder.AppendLine("\n");
            combinedBuilder.Append(responseBuilder);
            return combinedBuilder.ToString();
        }




    }
}

