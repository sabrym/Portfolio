using System;
using System.ComponentModel.DataAnnotations;

namespace Portfolio.Data.Configs
{
	public record StockConfig
	{
        public static string Stock = "Stock";
        [Required, Url]
        public string Url { get; init; }

        [Required]
        public string ApiKey { get; init; }
	}
}

