using System.Text.Json.Serialization;

namespace CompanyFundingAPI.Entities
{
    
    public class EdgarCompanyInfo
    {
        public int Cik { get; set; }
        public string? EntityName { get; set; }
        public InfoFact? Facts { get; set; }

    }
}
