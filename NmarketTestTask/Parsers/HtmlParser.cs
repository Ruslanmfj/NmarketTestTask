using System;


using System.Collections.Generic;
using HtmlAgilityPack;
using NmarketTestTask.Models;

namespace NmarketTestTask.Parsers
{
    public class HtmlParser : IParser
    {
        public IList<House> GetHouses(string path)
        {
            List<House> houses = new List<House>();

            var doc = new HtmlDocument();
            doc.Load(path);
            var nodes = doc.DocumentNode.SelectNodes(".//td");
            foreach (var node in nodes)
            {
                var nodeClases = node.GetClasses();
                foreach (var nodeClass in nodeClases)
                {
                    if (nodeClass.ToString() == "house")
                    {
                        if (HouseDublicator(doc, node.InnerText, houses) == false)
                        {
                            houses.Add(new House()
                            {
                                Name = node.InnerText,
                                Flats = flatsOfHouse(doc, node.InnerText)
                            });
                        }
                    }
                }
            }
            return houses;
        }


        /// <summary>
        /// Поиск квартир по заданному номеру дома
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="house"></param>
        /// <returns></returns>
        public List<Flat> flatsOfHouse(HtmlDocument doc, string house)
        {
            var flats = new List<Flat>();
            var nodes = doc.DocumentNode.SelectNodes(".//td");
            foreach (var node in nodes)
            {
                var nodeClasses = node.GetClasses();
                foreach (var nodeClass in nodeClasses)
                {
                    if (nodeClass.ToString() == "house")
                        if (node.InnerText == house)
                        {
                            var flatNumber = nodes[nodes.IndexOf(node)+1];
                            var flatPrice = nodes[nodes.IndexOf(node)+2];
                            flats.Add(new Flat() { Number = flatNumber.InnerText, Price = flatPrice.InnerText });
                        }
                }
            }
            return flats;
        }


        /// <summary>
        /// Исключает повторное добавление дома в объект
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="houseName"></param>
        /// <param name="houses"></param>
        /// <returns></returns>
        public bool HouseDublicator(HtmlDocument doc, string houseName, List<House> houses)
        {
            bool result = false;    
            foreach (var house in houses)
            {
                if (house.Name == houseName) result = true;
            }
            return result;  
        }
    }
}
