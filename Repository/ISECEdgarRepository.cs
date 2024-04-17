using CompanyFundingAPI.Entities;
using Microsoft.AspNetCore.SignalR;

namespace CompanyFundingAPI.Repository
{
    public interface ISECEdgarRepository<T>
    {
        public T GetById(string id);
        List<T> GetAll();
        public void LoadData();
    }
}
