using Projet;
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

            string projectRootPath = GetProjectRootPath();
            List<string> urls = ReadUrlsFromFile(System.IO.Path.Combine(projectRootPath, "urls.txt"));


            static string GetProjectRootPath()
            {
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string exeDirectory = System.IO.Path.GetDirectoryName(exePath);
                string projectRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(exeDirectory, @"..\..\..\"));
                return projectRoot;
            }

            static List<string> ReadUrlsFromFile(string filename)
            {
                List<string> urls = new List<string>();

                try
                {
                    using (StreamReader sr = new StreamReader(filename))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            urls.Add(line);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error reading the URLs file:");
                    Console.WriteLine(e.Message);
                }

                return urls;
            }


            static string ReadFileContent(string filePath)
            {
                string content;
                using (StreamReader reader = new StreamReader(filePath))
                {
                    content = reader.ReadToEnd();
                }
                return content;
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

            Console.WriteLine($"Listening on port 8080...");


            // get args
            foreach (string s in args)
            {
                Console.WriteLine("Listening for connections on " + s);
            }

            // Trap Ctrl-C on console to exit 
            Console.CancelKeyPress += delegate
            {
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


                    if (action == "q1")
                    {
                        responseString = Question1.GetServerTypeStats(urls);
                        Console.WriteLine(responseString);

                    }
                    else if (action== "q2")
                    {
                        responseString = Question2.GetDateStats(urls);
                        Console.WriteLine(responseString);
                    }
                    else if (action == "q3")
                    {
                        responseString = Question3.GetStats(urls);
                        Console.WriteLine(responseString);
                    }
                    else
                    {
                        responseString = "Bad parameters";
                    }



                }





                /**


                Parallel.ForEach(urls, url =>
                {
                    HttpWebRequest siteRequest = (HttpWebRequest)WebRequest.Create(url);
                    HttpWebResponse siteResponse = null;
                    try
                    {
                        siteRequest.Timeout = 3000; // Timeout après 3 secondes
                        siteResponse = (HttpWebResponse)siteRequest.GetResponse();

                 
                    
                        if (action == "date")
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

                    headersInfo.Insert(0, $"Valeur moyenne pour Content-length vaut {Math.Round(average, 2)} \n Écart type pour Content-length vaut {Math.Round(standardDeviation, 2)}\n\n");
                }

          

                if (action == "cookies")
                {
                    int totalSite = urls.Count();
                    double res = 100 * nbSiteCookies / totalSite;
                    headersInfo.Insert(0, $"Sur {totalSite} sites, {nbSiteCookies} utilisent le champ Set-Cookies des headers, soit {res} % \n\n");
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

                **/






                else
                {
                    string htmlFilePath = System.IO.Path.Combine(projectRootPath, "index.html");
                    if (File.Exists(htmlFilePath))
                    {
                        responseString = File.ReadAllText(htmlFilePath);
                    }
                    else
                    {
                        responseString = "Error: index.html file not found.";
                    }
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