using System;
using System.ComponentModel.DataAnnotations;

namespace Portfolio.Data.Configs
{
	public record TradesConfig
	{
		public static string Trades = "Trades";
        [Required]
        public string LocationAndPrefix { get; init; }
        [Required]
        public string DocumentTag { get; set; }
	}
}

