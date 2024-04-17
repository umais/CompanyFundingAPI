using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CompanyFundingAPI.Entities
{
    public class InfoFactUsGaapIncomeLossUnitsUsd
    {
        /// Possibilities include 10-Q, 10-K,8-K, 20-F, 40-F, 6-K, and
      //  their variants.YOU ARE INTERESTED ONLY IN 10-K DATA!
 /// </summary>
 public string? Form { get; set; }
        /// <summary>
        /// For yearly information, the format is CY followed by the year
      //  number.For example: CY2021.YOU ARE INTERESTED ONLY IN YEARLY INFORMATION
     //   WHICH FOLLOWS THIS FORMAT!
 /// </summary>
 public string? Frame { get; set; }
        /// <summary>
        /// The income/loss amount.
        /// </summary>
        public decimal Val { get; set; }
    }
}
