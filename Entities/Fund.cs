﻿namespace CompanyFundingAPI.Entities
{
    public class Fund
    {
        public int id {  get; set; }
        public string? name { get; set; }

        public decimal standardFundableAmount {  get; set; }

        public decimal specialFundableAmount { get; set; }


    }
}
