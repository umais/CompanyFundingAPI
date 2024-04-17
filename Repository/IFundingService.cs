namespace CompanyFundingAPI.Repository
{
    public interface IFundingService<T>
    {
        public T GetFundsByCompanyId(int id);

        public T GetFundsByCompanyName(string companyName);

        public List<T> GetAllFunds();

        public List<T> GetFundsByCompanyStartsWith(string letter);


    }
}
