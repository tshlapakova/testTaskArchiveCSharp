using System;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using System.Globalization;

namespace DataCollecting
{
    public class Program
    {
        public static void Main()
        {
            //TODO: Go through each page and collect data to JSON instead of CSV

            // the URL of the target page
            string url = "https://scrapeme.live/shop/";

            var web = new HtmlWeb();
            // downloading to the target page and parsing its HTML content
            var document = web.Load(url);
            var nodes = document.DocumentNode.SelectNodes("//*[@id='main']/ul/li[position()>0 and position()<17]");
            var counter = 0;
            // initializing the list of objects that will store the scraped data
            List<Episode> episodes = new List<Episode>();
            // looping over the nodes and extract data from them
            foreach (var node in nodes)
            {
                if(node != null)
                {
                    // add a new Episode instance to the list of scraped data
                    episodes.Add(new Episode()
                    {
                        Id = ++counter,
                        Name = HtmlEntity.DeEntitize(node.SelectSingleNode("a[1]/h2").InnerText),
                        Price = HtmlEntity.DeEntitize(node.SelectSingleNode("a[1]/span/span").InnerText),
                        Sku = HtmlEntity.DeEntitize(node.SelectSingleNode("a[2]").GetAttributeValue("data-product_sku", "")),
                        Image_url = HtmlEntity.DeEntitize(node.SelectSingleNode("a[1]/img").GetAttributeValue("src", ""))
                    });
                }
            }

            // initializing the CSV file
            using (var writer = new StreamWriter("output.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                // populating the CSV file
                csv.WriteRecords(episodes);
            }

        }
    }
}
