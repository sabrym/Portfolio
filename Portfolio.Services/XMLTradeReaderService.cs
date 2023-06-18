using System;
using System.Reflection.PortableExecutable;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Portfolio.Data;
using Portfolio.Services.Interfaces;
using Serilog;

namespace Portfolio.Services
{
	public class XMLTradeReaderService: ITradeReaderService
	{
        private IConfiguration _configuration;

        public XMLTradeReaderService(IConfiguration configuration) => _configuration = configuration;

        public async Task<List<Trade>> RetrieveItemsForPandL(DateTime yearEnding)
        {
            int monthsInAYear = 12;
            var items = new List<ProcessingResult>();
            for (DateTime i = yearEnding; i >= yearEnding.AddMonths(-monthsInAYear); i = i.AddMonths(-1))
            {
                var itemsForMonth = await RetrieveItems(i);
                if(itemsForMonth.ProcessedCount > 0)
                    items.Add(itemsForMonth);
            }

            return items.SelectMany(x => x.Trades).ToList();
        }
		
        public async Task<ProcessingResult> RetrieveItems(DateTime month)
        {
            var identifier = month.ToString("MM-yyyy");
            // trades in month
            var tradeList = new List<Trade>();
            var totalItems = 0;
            var failedItems = 0;
            try
            {
                var fileName = string.Format(_configuration["Trades:LocationAndPrefix"], identifier);
                if (!File.Exists(fileName))
                    throw new Exception("The file does not exist");

                // Loads into memory, so no disposal required
                var doc = XDocument.Load(fileName);
                
                // tag name from config
                var items = doc.Descendants(_configuration["Trades:DocumentTag"]);
                totalItems = items.Count();

                foreach (var item in items)
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
            return new ProcessingResult(identifier, totalItems, tradeList.Count, failedItems, tradeList);
        }
    }
}

