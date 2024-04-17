using System.Text.Json.Serialization;

namespace CompanyFundingAPI.Entities
{
    public class InfoFact
    {
        [JsonPropertyName("us-gaap")]
        public InfoFactUsGaap? UsGaap { get; set; }
    }
}
