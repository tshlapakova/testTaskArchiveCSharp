using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataCollecting
{
    public class Program
    {
        public static void Main()
        {
            // the URL of the target page
            string url = "https://scrapeme.live/shop/";

            var web = new HtmlWeb();
            // downloading to the target page and parsing its HTML content
            var document = web.Load(url);
            var nodes = document.DocumentNode.SelectNodes("//*[@id='main']/ul/li[position()>0 and position()<17]");
            var counter = 0;
            var pages = Convert.ToInt32(document.DocumentNode.SelectSingleNode("//*[@id='main']/div[1]/nav/ul/li[8]/a").InnerText);
            // initializing the list of objects that will store the scraped data
            List<Episode> episodes = new List<Episode>();

            for (int pageNumber = 0; pageNumber < pages; pageNumber++)
            {
                url = pageNumber == 0 ? "https://scrapeme.live/shop/" : "https://scrapeme.live/shop/page/" + pageNumber + "/";

                // looping over the nodes and extract data from them
                foreach (var node in nodes)
                {
                    //TODO: Prevent recording array if duplicates

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

            //write data into json
            using (StreamWriter file = File.CreateText("result.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, episodes);
            }
        }
    }
}
