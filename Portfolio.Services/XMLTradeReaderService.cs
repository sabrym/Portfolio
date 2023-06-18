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
            var items = new List<ProcessingResult>();
            for (DateTime i = yearEnding; i >= yearEnding.AddMonths(-12); i = yearEnding.AddMonths(-1))
            {
                var foo = await RetrieveItems(i);
                if(foo.ProcessedCount > 0)
                    items.Add(foo);
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
                // Loads into memory, so no disposal required
                var doc = XDocument.Load(string.Format(_configuration["Trades:LocationAndPrefix"], identifier));
                
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
                Log.Error(ex, "An error occurred reading data from: {identifier}", Guid.NewGuid());
            }

            return new ProcessingResult(identifier, totalItems, tradeList.Count, failedItems, tradeList);
        }
    }
}

