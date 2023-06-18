using System;
using System.Reflection.PortableExecutable;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Portfolio.Data;
using Serilog;

namespace Portfolio.Services
{
	public class XMLTradeReaderService: ITradeReaderService
	{
		public XMLTradeReaderService()
		{
		}

        public async Task<ProcessingResult> RetrieveItems()
        {
            var tradeList = new List<Trade>();
            var totalItems = 0;
            var failedItems = 0;
            try
            {
                // support for multiple files
                // assume for 1 xml file in the location

                // Loads into memory, so no disposal required
                var doc = XDocument.Load(@"/Users/sabrym/Projects/Farallon/Trades/Trade.xml");
                
                // tag name from config
                var items = doc.Descendants("trade");
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

            return new ProcessingResult(Guid.NewGuid().ToString(), totalItems, tradeList.Count, failedItems, tradeList);
        }

        public Task SaveItems()
        {
            throw new NotImplementedException();
        }
    }
}

