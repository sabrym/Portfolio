using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Portfolio.Data
{
	public record DailyStock
    {
        [JsonProperty("4. close")]
        public decimal Price { get; set; }

        public decimal Close { get; set; }

        public DateTime Date { get; set; }
    }


	public record StockMetaData
	{
		[JsonProperty("1. Information")]
		public string Information { get; set; }
    }

	public record StockInfo
	{
		[JsonProperty("Meta Data")]
		public StockMetaData StockMetaData { get; set; }

        [JsonProperty("Time Series (Daily)")]
        [JsonConverter(typeof(StockConverter))]
        public List<DailyStock> DailyStocks { get; set; }
    }

    public class StockConverter : JsonConverter
    {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return string.Empty;
            }
            else if (reader.TokenType == JsonToken.String)
            {
                return serializer.Deserialize(reader, objectType);
            }
            else
            {
                JObject stockPrices = JObject.Load(reader);

                var dailyStocks = new List<DailyStock>();
                foreach (var x in stockPrices)
                {
                    string dateKey = x.Key;
                    JToken value = x.Value;

                    var parsedValue = value.ToObject<DailyStock>();
                    parsedValue.Date = DateTime.Parse(dateKey);
                    dailyStocks.Add(parsedValue);
                }

                return dailyStocks;
            }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }
    }
}

