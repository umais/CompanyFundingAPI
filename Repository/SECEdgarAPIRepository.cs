using CompanyFundingAPI.Entities;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CompanyFundingAPI.Repository
{
    public class SECEdgarAPIRepository:ISECEdgarRepository<EdgarCompanyInfo>
    {
        #region Private Properties
        private string ApiURL;
        private string JsonStoragePath;
        private string CIKSSeedDataFile;
        #endregion

        #region Constructor
        public SECEdgarAPIRepository(IConfiguration config) {
            this.ApiURL = config["SECAPI"].ToString();
            this.JsonStoragePath = config["LocalStorage"].ToString();
            this.CIKSSeedDataFile = config["SeedDataFile"].ToString();
        
        }
        #endregion

        #region Public Interface Methods
        public EdgarCompanyInfo GetById(string id) {

            EdgarCompanyInfo edgarCompanyInfo = GetEdgarCompanyInfoFromStorage(id);
            //Some CIKS IDS are not returning any data so put in an additional check to refresh if available at a later point
            if (edgarCompanyInfo == null || edgarCompanyInfo.EntityName==null)
            {
                edgarCompanyInfo=GetAndStoreEdgarCompanyInfoFromAPI(id);
            }
           
            return edgarCompanyInfo;
        }

        public List<EdgarCompanyInfo> GetAll()
        {
            List<EdgarCompanyInfo> edgarCompanyInfos = new List<EdgarCompanyInfo>();
            List<string> cikNumbers = Utility.ReadCIKNumbersFromFile(CIKSSeedDataFile).Result;
            foreach (string cikNumber in cikNumbers)
            {
                edgarCompanyInfos.Add(GetById(cikNumber));
            }

            return edgarCompanyInfos;
        }
        public void LoadData()
        {
            List<string> cikNumbers = Utility.ReadCIKNumbersFromFile(CIKSSeedDataFile).Result;
            foreach (string cikNumber in cikNumbers)
            {
                    GetAndStoreEdgarCompanyInfoFromAPI(cikNumber);
            }
        }
        #endregion

        #region Private Class Methods
        private EdgarCompanyInfo GetAndStoreEdgarCompanyInfoFromAPI(string cikNumber)
        {
            EdgarCompanyInfo edgarCompanyInfo = MakeAPIRequests(cikNumber);
            if (edgarCompanyInfo != null)
            {
                // Serialize the EdgarCompanyInfo object to JSON
                string jsonContent = JsonSerializer.Serialize(edgarCompanyInfo);

                // Create or refresh the JSON file for the CIK number
                string filePath = Path.Combine(JsonStoragePath, $"{cikNumber}.json");
                try
                {
                    File.WriteAllText(filePath, jsonContent);
                    Console.WriteLine($"JSON file created or updated for CIK number: {cikNumber}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating or updating JSON file for CIK number {cikNumber}: {ex.Message}");
                }
                return edgarCompanyInfo;
            }
            else
            {
                Console.WriteLine($"No data retrieved for CIK number: {cikNumber}");
            }
            return null;
        }
        private EdgarCompanyInfo GetEdgarCompanyInfoFromStorage(string cikNumber)
        {
            try
            {
                // Construct the file path for the JSON file corresponding to the CIK number
                string filePath = Path.Combine(JsonStoragePath, $"{cikNumber}.json");

                // Check if the file exists
                if (File.Exists(filePath))
                {
                    // Read the JSON content from the file
                    string jsonContent = File.ReadAllText(filePath);

                    // Deserialize the JSON content to an EdgarCompanyInfo object
                    JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<EdgarCompanyInfo>(jsonContent, options);
                }
                else
                {
                    Console.WriteLine($"JSON file not found for CIK number: {cikNumber}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading JSON file for CIK number {cikNumber}: {ex.Message}");
            }

            return null;
        }

    
        private EdgarCompanyInfo MakeAPIRequests(string cikNumber)
        {
            EdgarCompanyInfo edgarCompanyInfo = new();

            using (HttpClient client = new())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.34.0");
                client.DefaultRequestHeaders.Add("Accept", "*/*");

                string apiUrl = $"{ApiURL}{cikNumber}.json";

                try
                {
                    HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;

                        // Parse the JSON into a JsonDocument
                        using (JsonDocument document = JsonDocument.Parse(jsonResponse))
                        {
                            // Update the cik value to an integer
                            if (document.RootElement.TryGetProperty("cik", out JsonElement cikElement))
                            {
                                if (cikElement.ValueKind == JsonValueKind.String)
                                {
                                    int cikInt;
                                    if (int.TryParse(cikElement.GetString(), out cikInt))
                                    {
                                        // Replace the string value with the integer value
                                        // Update the cik value to an integer

                                        jsonResponse = jsonResponse.Replace($"\"cik\":\"{cikElement.GetString()}\"", $"\"cik\": {cikInt}");


                                    }
                                }
                            }

                       
                         
                        }

                        JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

                        // Deserialize the modified JSON to EdgarCompanyInfo
                        edgarCompanyInfo = JsonSerializer.Deserialize<EdgarCompanyInfo>(jsonResponse, options);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to retrieve data for CIK number: {cikNumber}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error making API request: {ex.Message}");
                }
            }

            return edgarCompanyInfo;
        }
        #endregion

    }
}
