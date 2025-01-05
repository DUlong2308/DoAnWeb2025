using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Text;
using System.Threading.Tasks;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Models.Momo;

namespace WebApplicationlaptop.Services.Momo
{
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptionModel> _options;

        public MomoService(IOptions<MomoOptionModel> options)
        {
            _options = options;
        }

        public async Task<MomoCreatePaymentResponseModel> CreatePaymentMomo(OrderInfo model)
        {
            // Generate OrderId and OrderInfo
            model.OrderId = DateTime.UtcNow.Ticks.ToString();
            model.OrderInformation = "Khách hàng: " + model.FullName + ".<br />Nội dung: " + model.OrderInformation;

            // Prepare raw data for signature
            var rawData =
                $"partnerCode={_options.Value.PartnerCode}" +
                $"&accessKey={_options.Value.AccessKey}" +
                $"&requestId={model.OrderId}" +
                $"&amount={model.Amount}" +
                $"&orderId={model.OrderId}" +
                $"&orderInfo={model.OrderInformation}" +
                $"&returnUrl={_options.Value.ReturnUrl}" +
                $"&notifyUrl={_options.Value.NotifyUrl}" +
                $"&extraData=";

            // Generate signature
            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

            // Create RestClient and RestRequest
            var client = new RestClient(_options.Value.MomoApiUrl);
            var request = new RestRequest() { Method = Method.Post }; 
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");

            // Create the request data object
            var requestData = new
            {
                partnerCode = _options.Value.PartnerCode,
                accessKey = _options.Value.AccessKey,
                requestId = model.OrderId,
                amount = model.Amount.ToString(),
                orderId = model.OrderId,
                orderInfo = model.OrderInformation,
                returnUrl = _options.Value.ReturnUrl,
                notifyUrl = _options.Value.NotifyUrl,
                requestType = _options.Value.RequestType,
                signature = signature,
                extraData = ""
            };
            request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);
            return JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);
           
            
        }

        public async Task<MomoExecuteResponseModel> PaymentExecuteAsync(IQueryCollection collection)
        {
            // Giả lập xử lý bất đồng bộ
            await Task.Delay(100); // Giả lập thời gian chờ

            // Lấy dữ liệu từ collection
            if (!collection.TryGetValue("amount", out var amount) ||
                !collection.TryGetValue("orderInfo", out var orderInfo) ||
                !collection.TryGetValue("orderId", out var orderId))
            {
                throw new ArgumentException("Missing required parameters in the query collection.");
            }

            // Chuyển đổi amount sang decimal
            if (!decimal.TryParse(amount, out var parsedAmount))
            {
                throw new ArgumentException("Invalid amount format.");
            }

            return new MomoExecuteResponseModel
            {
                OrderId = orderId.ToString(), // Chuyển đổi sang string nếu cần
                Amount = parsedAmount, // Sử dụng giá trị đã chuyển đổi
                OrderInfo = orderInfo.ToString() // Chuyển đổi sang string nếu cần
            };
        }


        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] hashBytes;

            using (var hmac = new System.Security.Cryptography.HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower(); // Corrected ToLower()
            return hashString;
        }
    }
}