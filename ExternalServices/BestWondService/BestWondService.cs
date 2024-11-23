using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Locker.ExternalServices.BestWondService.Request;
using Locker.ExternalServices.BestWondService.Response;
using Locker.Models;

namespace Kargomat.ExternalServices.CabinetService;

public class BestWondService : IBestWondService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    private readonly ILogger<BestWondService> _logger;

    public BestWondService(IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<BestWondService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }


    public async Task<CreateLockerOrderModel> SaveOrder(LockerCreateOrderRequest request)
    {
        long timestamps = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var requestBody = new 
            {
                app_id = _configuration.GetSection("BestWondServiceSettings:AppId").Value,
                box_size = request.BoxSize,    
                device_number = request.DeviceNumber,
                order_no = request.OrderId,
                timestamps = timestamps
            };


            var fields = new SortedDictionary<object,object>();
            fields.Add("app_id",requestBody.app_id ?? string.Empty);
            fields.Add("box_size",requestBody.box_size);
            fields.Add("device_number",requestBody.device_number ?? string.Empty);
            fields.Add("order_no",requestBody.order_no);
            fields.Add("timestamps",requestBody.timestamps);

            var queryString = string.Join("&", fields.Select(kv => $"{kv.Key}={kv.Value}"));
            var signString = queryString + _configuration.GetSection("BestWondServiceSettings:Secret").Value;

            var sign = ComputeSha512Hash(signString);

            var httpClient = _httpClientFactory.CreateClient("Cabinet");

            var response = await httpClient.PostAsJsonAsync<object>(string.Format(_configuration.GetSection("BestWondServiceSettings:SaveOrderEndpoint").Value,sign),requestBody);
            

            _logger.LogInformation($"BestWondApi Save Order Response - OrderId {request.OrderId}", JsonSerializer.Serialize(response));

            var baseResponse = new CreateLockerOrderModel{
                ErrorMessage = "External server error"
            };
            
            if(!response.IsSuccessStatusCode)
                return baseResponse;

            var result = response.Content.ReadAsStringAsync().Result;

            baseResponse.ErrorMessage = "Response body is unexpected";
            var apiResponse = JsonSerializer.Deserialize<LockerCreateOrderResponse>(result);
            if(apiResponse == null)
            return baseResponse;

            baseResponse.ErrorMessage = string.Empty;

            return new CreateLockerOrderModel{
                IsSuccess = apiResponse.code == 0,
                BoxName = apiResponse.data?.box_name,
                ErrorMessage = apiResponse.msg,
                SaveCode = apiResponse.data?.save_code,
                PickCode = apiResponse.data?.pick_code
            };
        
    }

     static string ComputeSha512Hash(string rawData)
        {
            using (SHA512 sha512Hash = SHA512.Create())
            {
                var bytes = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
}
