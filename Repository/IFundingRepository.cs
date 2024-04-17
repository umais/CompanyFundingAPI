namespace CompanyFundingAPI.Repository
{
    public interface IFundingRepository<T>
    {
       
        public List<T> GetAll();

        public List<T> Get(string startswith);

         void LoadConfiguration();
        public string GetConfiguration(string key);
    }
}
