using CompanyFundingAPI.Entities;
using System.Collections.Generic;

namespace CompanyFundingAPI.Repository
{
    public class FundingService : IFundingService<Fund>
    {
        #region Private Members
        private IFundingRepository<EdgarCompanyInfo> _fundingRepository;
        #endregion

        #region Constructors
        public FundingService(IFundingRepository<EdgarCompanyInfo> fundingRepository) { 
         _fundingRepository = fundingRepository;
        }
        #endregion

        #region Public Interface Methods
        public List<Fund> GetAllFunds()
        {
           List < EdgarCompanyInfo > edgarCompanies = _fundingRepository.GetAll();
            var funds=CalculateFunds(edgarCompanies);
            return funds;
        }

        public Fund GetFundsByCompanyId(int id)
        {
            throw new NotImplementedException();
        }

        public Fund GetFundsByCompanyName(string companyName)
        {
            throw new NotImplementedException();
        }

        public List<Fund> GetFundsByCompanyStartsWith(string value)
        {
            List<EdgarCompanyInfo> edgarCompanies=_fundingRepository.Get(value);
            var funds= CalculateFunds(edgarCompanies);
            return funds;
        }
        #endregion

        #region Private Methods
        private List<Fund> CalculateFunds(List<EdgarCompanyInfo> edgarCompanies) {

            bool allYearsPresent = true;
            List<string> requiredYears = Utility.GetYears(_fundingRepository.GetConfiguration(ConfigurationKeys.YearRangeForFunding.ToString())).Select(year => "CY" + year).ToList();

            List<Fund> funds = new List<Fund>();
            //This will be passed to the Calculate Funds
        

            foreach(EdgarCompanyInfo edgarCompany in edgarCompanies)
            {
                Fund fund = new Fund() { id = edgarCompany.Cik, name = edgarCompany.EntityName };
                InfoFactUsGaapIncomeLossUnitsUsd[] incomeLoss = edgarCompany.Facts.UsGaap.NetIncomeLoss.Units.Usd.Where(x=>x.Frame!=null && x.Form!=null && !x.Frame.Contains("Q") && x.Form == "10-K").ToArray();
                List<InfoFactUsGaapIncomeLossUnitsUsd> finalValues=new List<InfoFactUsGaapIncomeLossUnitsUsd>();
                bool incomeYearComparison=true;
                decimal val_2022 = 0;
                decimal val_2021 = 0;
                if (incomeLoss != null && incomeLoss.Length > 0) {
                  
                    foreach (var year in requiredYears)
                    {
                        allYearsPresent = true;
                        InfoFactUsGaapIncomeLossUnitsUsd item=new InfoFactUsGaapIncomeLossUnitsUsd();
                        var items = incomeLoss.Where(x => x.Frame.Equals(year)).ToList();
                        item = items.FirstOrDefault();
                        if (items==null || items.Count==0)
                        {
                            allYearsPresent = false;
                            break;
                        }
                        else if(year.Equals("CY2021")|| year.Equals("CY2022"))
                        {
                            //Check positive income otherwise brrak
                           
                            if (item.Val <= 0)
                            {
                                allYearsPresent = false;
                                break;
                            }
                            if (year.Equals("CY2021"))
                            {
                                val_2021 = item.Val;
                            }
                            else
                            {
                                val_2022 += item.Val;
                            }
                           if(val_2022<val_2021)
                            {
                                incomeYearComparison = true;
                            }
                        }
             
                            finalValues.Add(item);
                           
                        
                    }

                    if (allYearsPresent && finalValues.Count==requiredYears.Count)
                    {
                        //Now we can calculate the Standard Funding amount 
                        decimal highestIncome = finalValues.Max(x => x.Val);
                       fund.standardFundableAmount= CalculateStandardFundingAmount(highestIncome);
                       fund.specialFundableAmount = CalculateSpecialFundingAmount(fund.standardFundableAmount,fund.name,incomeYearComparison);
                      
                    }
                }
             
                funds.Add(fund);
            }
        return funds;
        }

        private decimal CalculateSpecialFundingAmount(decimal standardIncomeAmount,string Name,bool isIncomeLessThan)
        {
            decimal percentage = 0;
            decimal specialFunding = standardIncomeAmount;
            if (Utility.ContainsVowel(Name[0].ToString()))
            {
               percentage= Convert.ToDecimal(_fundingRepository.GetConfiguration(ConfigurationKeys.AddSpecialPercentage.ToString()));

                specialFunding+= Utility.CalculatePercentage(standardIncomeAmount, percentage);
            }
            if (isIncomeLessThan)
            {
                percentage = Convert.ToDecimal(_fundingRepository.GetConfiguration(ConfigurationKeys.SubtractSpecialPercentage.ToString()));
                specialFunding += (standardIncomeAmount - Utility.CalculatePercentage(standardIncomeAmount, percentage));
            }
            return specialFunding;

        }
        private decimal CalculateStandardFundingAmount(decimal MaximumIncomeAmount)
        {
            decimal total = 0;
            decimal percentage=0;
            decimal incomeThreshold = Convert.ToDecimal(_fundingRepository.GetConfiguration(ConfigurationKeys.IncomeThreshold.ToString()));
            if(Utility.IsValueGreaterEqualThanAmount(MaximumIncomeAmount, incomeThreshold))
            {
                percentage = Convert.ToDecimal(_fundingRepository.GetConfiguration(ConfigurationKeys.LessThanPercentage.ToString()));
               
            }
            else
            {
                percentage = Convert.ToDecimal(_fundingRepository.GetConfiguration(ConfigurationKeys.GreaterThanPercentage.ToString()));
            }

            total = Utility.CalculatePercentage(MaximumIncomeAmount, percentage);
            return total;
        }
        #endregion
    }
}
