using CompanyFundingAPI.Entities;
using Microsoft.Extensions.Configuration;

namespace CompanyFundingAPI.Repository
{
    public class FundingRepository : IFundingRepository<EdgarCompanyInfo>
    {
        #region Private Properties
        private ISECEdgarRepository<EdgarCompanyInfo> _edgarRepository;
        private Dictionary<string, string> keyValue = new Dictionary<string, string>();
        private IConfiguration config;
        #endregion

        #region Constructors
        public FundingRepository(ISECEdgarRepository<EdgarCompanyInfo> edgarRepository, IConfiguration configuration) {
            this._edgarRepository = edgarRepository;
            this.config = configuration;
            ((IFundingRepository<EdgarCompanyInfo>) this).LoadConfiguration();
      
        }
        #endregion

        #region Private Interface Callable Method 
        void IFundingRepository<EdgarCompanyInfo>.LoadConfiguration()
        {
            string[] keys = Enum.GetNames(typeof(ConfigurationKeys));
            if(this.keyValue == null)
            {
                keyValue = new Dictionary<string, string>();
            }
            foreach (string key in keys)
            {
                if (!keyValue.ContainsKey(key))
                {
                    keyValue.Add(key, config[key].ToString());
                }
            }
           
        }
        #endregion

        #region Public Interface Implemented Methods
        public string GetConfiguration(string key)
        {
            return keyValue[key].ToString();
        }
        public List<EdgarCompanyInfo> Get(string startswith)
        {
            List<EdgarCompanyInfo> edgarCompanyInfos = _edgarRepository.GetAll()
           .Where(x => x.EntityName != null && x.EntityName.ToLower().StartsWith(startswith.ToLower()) && x.Facts != null &&
                       x.Facts.UsGaap != null &&
                       x.Facts.UsGaap.NetIncomeLoss != null &&
                       x.Facts.UsGaap.NetIncomeLoss.Units != null &&
                       x.Facts.UsGaap.NetIncomeLoss.Units.Usd != null //&&
                    //   x.Facts.UsGaap.NetIncomeLoss.Units.Usd.Any(y => y.Frame != null && y.Form == "10-K")
                       )

           .ToList();
            return edgarCompanyInfos;
        }


        public List<EdgarCompanyInfo> GetAll()
        {
            //Get All the IDS
            List<string> cikNumbers = Utility.ReadCIKNumbersFromFile(keyValue[ConfigurationKeys.SeedDataFile.ToString()]).Result;
        //Get the years
     
            List<EdgarCompanyInfo> edgarCompanyInfos = _edgarRepository.GetAll()
           .Where(x => x.Facts != null &&
                       x.Facts.UsGaap != null &&
                       x.Facts.UsGaap.NetIncomeLoss != null &&
                       x.Facts.UsGaap.NetIncomeLoss.Units != null &&
                       x.Facts.UsGaap.NetIncomeLoss.Units.Usd != null 
         
                     )

           .ToList();

            return edgarCompanyInfos;
        }

        #endregion





    }
}
