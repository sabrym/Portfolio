using System;
using System.Reflection.PortableExecutable;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Portfolio.Data;
using Portfolio.Data.Configs;
using Portfolio.Services.Interfaces;
using Serilog;

namespace Portfolio.Services
{
	public class XMLTradeReaderService: ITradeReaderService
	{
        private readonly TradesConfig _configuration;

        public XMLTradeReaderService(TradesConfig configuration) => _configuration = configuration;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="yearEnding"></param>
        /// <returns></returns>
        public async Task<List<Trade>> RetrieveItemsForPandL(DateTime yearEnding)
        {
            int monthsInAYear = 12;
            var items = new List<Trade>();
            for (DateTime i = yearEnding; i >= yearEnding.AddMonths(-monthsInAYear); i = i.AddMonths(-1))
            {
                var itemsForMonth = await RetrieveItems(i);
                if(itemsForMonth.Any())
                    items.AddRange(itemsForMonth);
            }

            return items;
        }

        /// <summary>
        /// Reads the relevant xml file from the store location, parses it based on configuration
        /// </summary>
        /// <param name="month"></param>
        /// <returns>
        /// A processing result, total items processed and the payload
        /// </returns>
        public async Task<List<Trade>> RetrieveItems(DateTime month)
        {
            var identifier = month.ToString("MM-yyyy");
            // trades in month
            var tradeList = new List<Trade>();

            try
            {
                var fileName = string.Format(_configuration.LocationAndPrefix, identifier);
                if (!File.Exists(fileName))
                    throw new ApplicationException($"The file {fileName} does not exist");

                // Loads into memory, so no disposal required
                var doc = XDocument.Load(fileName);
                
                // tag name from config
                var tradeData = doc.Descendants(_configuration.DocumentTag);

                // maybe a try catch here too?
                foreach (var item in tradeData)
                {
                    var results = new Dictionary<string, object>();
                    foreach (var child in item.Descendants())
                    {
                        results.Add(child.Name.LocalName, child.Value);
                    }

                    var json = JsonConvert.SerializeObject(results);
                    tradeList.Add(JsonConvert.DeserializeObject<Trade>(json));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred reading data from: {identifier}", identifier);
            }

            // here we return a value even if there is no file, since we will need to perform this processing, for other existing files
            return tradeList;
        }
    }
}