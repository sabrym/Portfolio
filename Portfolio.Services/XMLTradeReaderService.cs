using System;
using System.Reflection.PortableExecutable;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Portfolio.Data;
using Portfolio.Data.Configs;
using Portfolio.Services.Interfaces;
using Portfolio.Utilities;
using Serilog;

namespace Portfolio.Services
{
	public class XMLTradeReaderService: ITradeReaderService
	{
        private readonly TradesConfig _configuration;
        private readonly IFileWrapper _fileWrapper;

        public XMLTradeReaderService(TradesConfig configuration, IFileWrapper fileWrapper) => (_configuration, _fileWrapper) = (configuration, fileWrapper);

        /// <summary>
        /// Iterate through a calendar year and process the relevant xml files
        /// </summary>
        /// <param name="yearEnding"></param>
        /// <returns>
        /// A list of trades that happened during the course of the reporting period
        /// </returns>
        public List<Trade> RetrieveItemsForPandL(DateTime yearEnding)
        {
            if (_fileWrapper.DirectoryIsEmpty(_configuration.Directory))
                throw new Utilities.ApplicationException("The directory is empty");

            int monthsInAYear = 12;
            var items = new List<Trade>();
            for (DateTime i = yearEnding; i >= yearEnding.AddMonths(-monthsInAYear); i = i.AddMonths(-1))
            {
                var itemsForMonth = RetrieveItems(i);
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
        public List<Trade> RetrieveItems(DateTime month)
        {
            var identifier = month.ToString(_configuration.MonthYearDelimiter);

            // trades in month
            var tradeList = new List<Trade>();

            try
            {
                var fileName = string.Format($"{_configuration.Directory}{_configuration.FileName}", identifier);

                if (!_fileWrapper.FileExists(fileName))
                    throw new Utilities.ApplicationException($"The file {fileName} does not exist");

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

            return tradeList;
        }
    }
}