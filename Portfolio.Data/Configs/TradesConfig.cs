using System;
using System.ComponentModel.DataAnnotations;

namespace Portfolio.Data.Configs
{
	public record TradesConfig
	{
		public static string Trades = "Trades";
        [Required]
        public string Directory { get; init; }
        [Required]
        public string DocumentTag { get; set; }
        [Required]
        public string MonthYearDelimiter { get; set; }
        [Required]
        public string FileName { get; set; }        
    }
}

