using CompanyFundingAPI.Entities;
using CompanyFundingAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CompanyFundingAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FundController : ControllerBase
    {
     

        private readonly IFundingService<Fund> _fundingService;

        public FundController(IFundingService<Fund> fundingService)
        {
          _fundingService = fundingService;
        }

        [HttpGet(Name = "GetFunds")]
        public IEnumerable<Fund> Get()
        {
          return _fundingService.GetAllFunds();
        }

        [HttpGet("{companyStartsWith}", Name = "GetFundsByCompany")]
        public IEnumerable<Fund> Get(string companyStartsWith)
        {
           
            return _fundingService.GetFundsByCompanyStartsWith(companyStartsWith);
        }


    }
}
