using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Web;

namespace BasicServerHTTPlistener
{
    internal class Program
    {
        private static void Main(string[] args)

        {

            List<string> urls = new List<string>
{
    "https://www.google.com/",
    "https://www.youtube.com/",
    "https://www.facebook.com/",
    "https://www.wikipedia.org/",
    "https://www.amazon.com/",
    "https://www.instagram.com/",
    "https://www.twitter.com/",
    "https://www.linkedin.com/",
    "https://www.whatsapp.com/",
    "https://www.pinterest.com/",
    "https://www.reddit.com/",
    "https://www.ebay.com/",
    "https://www.bing.com/",
    "https://www.twitch.tv/",
    "https://www.apple.com/",
    "https://www.yahoo.com/",
    "https://www.spotify.com/",
    "https://www.quora.com/",
    "https://www.tumblr.com/",
    "https://www.etsy.com/",
    "https://www.lyft.com/",
    "https://www.soundcloud.com/",
     "https://www.discord.com/",
     "https://smee.io/",
     "https://www.cloudflare.com/fr-fr/",
     "http://www.tigli.fr/",



};



            static double CalculMoyenne(List<DateTime> creationDates)
            {
                TimeSpan totalAge = TimeSpan.Zero;

                foreach (DateTime creationDate in creationDates)
                {
                    totalAge += DateTime.UtcNow - creationDate;
                }

                return totalAge.TotalSeconds / creationDates.Count;
            }

            static double CalculEcartType(List<DateTime> creationDates, double averageAge)
            {
                double sumOfSquares = 0;

                foreach (DateTime creationDate in creationDates)
                {
                    TimeSpan age = DateTime.UtcNow - creationDate;
                    double ageDifference = age.TotalSeconds - averageAge;
                    sumOfSquares += ageDifference * ageDifference;
                }

                double variance = sumOfSquares / creationDates.Count;
                return  Math.Sqrt(variance);
            }


            static string FormatTime(double totalSeconds)
            {
                TimeSpan duration = TimeSpan.FromSeconds(totalSeconds);

                int days = duration.Days;
                int hours = duration.Hours;
                int minutes = duration.Minutes;
                int seconds = duration.Seconds;

                return $"{days} jours, {hours} heures, {minutes} minutes, {seconds} secondes";
            }




            //if HttpListener is not supported by the Framework
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("A more recent Windows version is required to use the HttpListener class.");
                return;
            }


            // Create a listener.

            HttpListener listener = new HttpListener();

            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();

            // get args
            foreach (string s in args)
            {
                Console.WriteLine("Listening for connections on " + s);
            }



            // Trap Ctrl-C on console to exit 
            Console.CancelKeyPress += delegate {
                // call methods to close socket and exit
                listener.Stop();
                listener.Close();
                Environment.Exit(0);
            };





            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                string documentContents;
                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();
                    }
                }

          
                // Varaibles utilisées pour stocker des infos, statistiques
                string responseString;
                List<DateTime> creationDates = new List<DateTime>();
                Dictionary<string, int> serverPopularity = new Dictionary<string, int>();
                List<int> listLength = new List<int>();
                int nbSiteCookies = 0;
                int totalField = 0;

                Dictionary<string, int> headerNameCounts = new Dictionary<string, int>();





                if (request.Url.LocalPath.StartsWith("/fetch-"))
                {
                    string action = request.Url.LocalPath.Substring(7);
                    StringBuilder headersInfo = new StringBuilder();
                    Console.WriteLine("Une demande de fetch pour des stats est recue \n");

                    Parallel.ForEach(urls, url =>
                    {
                        HttpWebRequest siteRequest = (HttpWebRequest)WebRequest.Create(url);
                        HttpWebResponse siteResponse = null;
                        try
                        {
                            siteRequest.Timeout = 3000; // Timeout après 3 secondes
                            siteResponse = (HttpWebResponse)siteRequest.GetResponse();

                            if (action == "headers")
                            {
                                foreach (string key in siteResponse.Headers.AllKeys)
                                {
                                    headersInfo.AppendLine($"{url} - {key}: {siteResponse.Headers[key]}");
                                }
                                headersInfo.AppendLine("\n");
                            }
                            else if (action == "server-type")
                            {



                                string serverType = siteResponse.Headers["Server"];
                                if (serverType!=null)
                                {

                                    if (serverPopularity.ContainsKey(serverType))
                                    {
                                        serverPopularity[serverType]++;
                                    }
                                    else
                                    {
                                        serverPopularity[serverType] = 1;
                                    }
                                

                                headersInfo.AppendLine($"{url} utilise un serveur {serverType}");
                                }
                            }
                            else if(action =="date")
                            {

                                string dateCreation = siteResponse.Headers["Last-Modified"];

                                if (dateCreation != null)
                                {
                                    headersInfo.AppendLine($"{url} a été modifié la dernière fois le {dateCreation}");
                                    DateTime creationDate = DateTime.Parse(dateCreation);
                                    lock (creationDates)
                                    {
                                        creationDates.Add(creationDate);
                                    }
                                }


                            }

                            else if (action == "length-data")
                            {
                                string length = siteResponse.Headers["Content-Length"];

                                if (length != null)
                                {
                                    listLength.Add(int.Parse(length));
                                    headersInfo.AppendLine($"{url} a un contenu de longueur {length}");
                                }

                            }

                            else if (action == "cookies")
                            {

                                string cookie = siteResponse.Headers["Set-Cookie"];

                                if (cookie != null)
                                {
                                    nbSiteCookies++;
                                    headersInfo.AppendLine($"{url} utilise les cookies = {cookie}");
                                }

                            }


                            else if (action == "field")
                            {

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
                            
                                headersInfo.AppendLine($"{url}: a {nb} champs");
                                
                            }




                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error fetching headers for {url}: {ex.Message}");
                        }
                        finally
                        {
                            siteResponse?.Close();
                        }
                    });

                    if (action == "headers")
                    {
                        headersInfo.Insert(0, $"Voici les headers complets provenant de {urls.Count()} sites \n\n");


                    }

                    if (action == "date" && creationDates.Count > 0)
                    {
                        double averageAge = CalculMoyenne(creationDates);
                        double ageStandardDeviation = CalculEcartType(creationDates, averageAge);

                        String resMoyenne = FormatTime(averageAge);
                        String resEcartType = FormatTime(ageStandardDeviation);

                        headersInfo.Insert(0, $"Moyenne d'âge des pages: {resMoyenne}\nEcart-type de l'âge des pages: {resEcartType} \n\n");
                    }

                    if (action == "length-data")
                    {
                        double total = listLength.Sum();
                        double average = total / listLength.Count();
                        double sumOfSquaresOfDifferences = listLength.Select(val => (val - average) * (val - average)).Sum();
                        double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / listLength.Count);

                        headersInfo.Insert(0, $"Valeur moyenne pour Content-length vaut {Math.Round(average, 2)} - Écart type pour Content-length vaut {Math.Round(standardDeviation, 2)}\n\n");
                    }


                    if (action == "server-type")
                    {
                        int totalCount = serverPopularity.Values.Sum();

                        StringBuilder responseBuilder = new StringBuilder();
                        responseBuilder.AppendLine("Statistiques sur les différents serveurs : \n");
                        foreach (KeyValuePair<string, int> serverType in serverPopularity.OrderByDescending(x => x.Value))
                        {
                            double percentage = (double)serverType.Value / totalCount * 100;
                            responseBuilder.AppendLine($"{serverType.Key}: {serverType.Value} ({percentage:0.##}%)");
                        }
                        responseString = responseBuilder.ToString();

                        headersInfo.Insert(0, $"Stats serveurs {responseString}\n");

                    }

                    if (action == "cookies")
                    {
                        int totalSite = urls.Count();
                        headersInfo.Insert(0, $"Sur {totalSite} sites, {nbSiteCookies} utilisent le champ Set-Cookies des headers \n\n"); 
                    }

                    if (action == "field")
                    {
                        // Trouver le header le plus et le moins présent
                        KeyValuePair<string, int> maxHeader = headerNameCounts.Aggregate((l, r) => l.Value > r.Value ? l : r);
                        KeyValuePair<string, int> minHeader = headerNameCounts.Aggregate((l, r) => l.Value < r.Value ? l : r);

                        double res = totalField / urls.Count();
                        string averageHeaderFields = $"En moyenne, un site a {res} champs dans son header.";
                        string mostCommonHeader = $"Header le plus présent : {maxHeader.Key} ({maxHeader.Value} occurrences)";
                        string leastCommonHeader = $"Header le moins présent : {minHeader.Key} ({minHeader.Value} occurrence)";

                        headersInfo.Insert(0, $"{averageHeaderFields}\n{mostCommonHeader}\n{leastCommonHeader}\n\n");
                    }



                    responseString = headersInfo.ToString();
                }






                else
                {
                    responseString = @"
                    <HTML>
                        <HEAD>
                            <style>
                                button {
                                    background-color: #4CAF50;
                                    border: none;
                                    color: white;
                                    padding: 10px 20px;
                                    text-align: center;
                                    text-decoration: none;
                                    display: inline-block;
                                    font-size: 16px;
                                    font-weight: 700;
                                    margin: 4px 2px;
                                    cursor: pointer;
                                    border-radius: 12px;
                                }
                                
                            </style>
                            <script>
                               async function fetchData(action) {
                                document.getElementById('dataDisplay').style.display = 'none';

                                document.getElementById('loadingText').style.display = 'block';

                                const response = await fetch('/fetch-' + action);
                                const data = await response.text();
                                document.getElementById('dataDisplay').innerText = data;

                                document.getElementById('loadingText').style.display = 'none';
                                document.getElementById('dataDisplay').style.display = 'block';

                            }

                            </script>
                        </HEAD>
                        <BODY>
                            <button onclick='fetchData(""headers"")'>Get all headers data</button>
                            <button onclick='fetchData(""server-type"")'>Stats on serveur type</button>
                            <button onclick='fetchData(""date"")'>Stats on date</button>
                            <button onclick='fetchData(""length-data"")'>Stats on headers length</button>
                            <button onclick='fetchData(""cookies"")'>Stats on cookies usage</button>
                            <button onclick='fetchData(""field"")'>Stats on the headers fields</button>

                            <p id=""loadingText"" style=""display: none; position: fixed; top: 50%; left: 50%; transform: translate(-50%, -50%); font-size: 28px; color: #21618C; font-weight: bold;"">Fetching data, please wait...</p>



                            <pre id='dataDisplay'></pre>
                        </BODY>
                    </HTML>";
                }





                Console.WriteLine(documentContents);

                HttpListenerResponse response = context.Response;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
 
        }
    }
}