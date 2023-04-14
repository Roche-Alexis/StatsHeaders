using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Projet
{
    internal class Question2
    {


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
            return Math.Sqrt(variance);
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


        public static string GetDateStats(List<string> urls)
        {
            StringBuilder responseBuilder = new StringBuilder();

            List<DateTime> creationDates = new List<DateTime>();



            Parallel.ForEach(urls, url =>
            {
                HttpWebRequest siteRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse siteResponse = null;

                try
                {
                    siteRequest.Timeout = 3000; // Timeout after 3 seconds
                    siteResponse = (HttpWebResponse)siteRequest.GetResponse();


                    string dateCreation = siteResponse.Headers["Last-Modified"];

                    if (dateCreation != null)
                    {
                        responseBuilder.AppendLine($"{url} a été modifié la dernière fois le {dateCreation}");
                        DateTime creationDate = DateTime.Parse(dateCreation);
                        lock (creationDates)
                        {
                            creationDates.Add(creationDate);
                        }
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



            double averageAge = CalculMoyenne(creationDates);
            double ageStandardDeviation = CalculEcartType(creationDates, averageAge);

            String resMoyenne = FormatTime(averageAge);
            String resEcartType = FormatTime(ageStandardDeviation);

            responseBuilder.Insert(0, $"La moyenne d'âge des pages vaut {resMoyenne}\nL'ecart-type de l'âge des pages vaut {resEcartType} \n\n");




            return responseBuilder.ToString();
        }




    }
}

