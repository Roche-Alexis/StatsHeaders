using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Projet
{
    internal class Question3
    {


        public static string GetStats(List<string> urls)
        {

            List<int> listLength = new List<int>();
            int nbSiteCookies = 0;
            int totalField = 0;

            Dictionary<string, int> headerNameCounts = new Dictionary<string, int>();


            StringBuilder lenBuilder = new StringBuilder();
            StringBuilder cookiesBuilder = new StringBuilder();
            StringBuilder fieldsBuilder = new StringBuilder();




            Parallel.ForEach(urls, url =>
            {
                HttpWebRequest siteRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse siteResponse = null;

                try
                {
                    siteRequest.Timeout = 3000; // Timeout after 3 seconds
                    siteResponse = (HttpWebResponse)siteRequest.GetResponse();


                    string length = siteResponse.Headers["Content-Length"];

                    if (length != null)
                    {
                        listLength.Add(int.Parse(length));
                        lenBuilder.AppendLine($"{url} a un contenu de longueur {length}");
                    }

                    string cookie = siteResponse.Headers["Set-Cookie"];

                    if (cookie != null)
                    {
                        nbSiteCookies++;
                        cookiesBuilder.AppendLine($"{url} utilise les cookies = {cookie}");
                    }




                    int nb = siteResponse.Headers.AllKeys.Count();
                    totalField += nb;

                    foreach (string headerName in siteResponse.Headers.AllKeys)
                    {
                        if (headerNameCounts.ContainsKey(headerName))
                        {
                            headerNameCounts[headerName]++;
                        }
                        else
                        {
                            headerNameCounts[headerName] = 1;
                        }
                    }

                    fieldsBuilder.AppendLine($"{url}: a {nb} champs");

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




            double total = listLength.Sum();
            double average = total / listLength.Count();
            double sumOfSquaresOfDifferences = listLength.Select(val => (val - average) * (val - average)).Sum();
            double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / listLength.Count);

            lenBuilder.Insert(0, $"La valeur moyenne pour Content-length vaut {Math.Round(average, 2)} \nL'ecart type pour Content-length vaut {Math.Round(standardDeviation, 2)}\n\n");






            int totalSite = urls.Count();
            double res = 100 * nbSiteCookies / totalSite;
            cookiesBuilder.Insert(0, $"Sur {totalSite} sites, {nbSiteCookies} utilisent le champ Set-Cookies des headers, soit {res} % \n\n");






            KeyValuePair<string, int> maxHeader = headerNameCounts.Aggregate((l, r) => l.Value > r.Value ? l : r);
            KeyValuePair<string, int> minHeader = headerNameCounts.Aggregate((l, r) => l.Value < r.Value ? l : r);

            double resp = totalField / urls.Count();
            string averageHeaderFields = $"En moyenne, un site a {resp} champs dans son header.";
            string mostCommonHeader = $"Champ le plus présent : {maxHeader.Key} ({maxHeader.Value} occurrences)";
            string leastCommonHeader = $"Champ le moins présent : {minHeader.Key} ({minHeader.Value} occurrence)";

            fieldsBuilder.Insert(0, $"{averageHeaderFields}\n{mostCommonHeader}\n{leastCommonHeader}\n\n");


            StringBuilder combinedBuilder = new StringBuilder();
            combinedBuilder.Append(fieldsBuilder);

            combinedBuilder.AppendLine("\n\n");
            combinedBuilder.Append(lenBuilder);
            combinedBuilder.AppendLine("\n\n");
            combinedBuilder.Append(cookiesBuilder);



            return combinedBuilder.ToString();
        }




    }
}

