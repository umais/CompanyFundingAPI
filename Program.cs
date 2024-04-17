using CompanyFundingAPI.Entities;
using CompanyFundingAPI.Repository;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ISECEdgarRepository<EdgarCompanyInfo>, SECEdgarAPIRepository>();
builder.Services.AddSingleton<IFundingRepository<EdgarCompanyInfo>,FundingRepository>();
builder.Services.AddSingleton<IFundingService<Fund>, FundingService>();
var app = builder.Build();
var config = builder.Configuration;
var secAPI = app.Services.GetService<ISECEdgarRepository<EdgarCompanyInfo>>();

//If you want to reload the data from API and store it locally then change LoadData=true in the appettings file.
if (secAPI != null && Convert.ToBoolean(config["LoadData"]))
{
    secAPI.LoadData();
}






// Display API responses

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
