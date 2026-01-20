using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

using GymManagement.Application.Interfaces.Services;
using GymManagement.Application.Interfaces.Services.User;
using GymManagement.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using GymManagement.Application.DTOs.User.Payment;
using System.Net.Http.Json;


namespace GymManagement.Infrastructure.Services
{
    public class MoMoPaymentService : IMoMoPaymentService
    {
        private readonly MoMoSettings _config;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public MoMoPaymentService(
            IOptions<MoMoSettings> config,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _config = config.Value;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        /// <summary>
        /// Tạo yêu cầu thanh toán MoMo
        /// </summary>
        public async Task<MoMoPaymentResponse> CreatePaymentRequestAsync(
            string packageId,
            string memberId,
            long amount,
            string orderInfo)
        {
            var requestId = Guid.NewGuid().ToString();
            var orderId = $"ORDER_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

            // Tạo extraData
            var extraDataObj = new MoMoExtraData
            {
                PackageId = packageId,
                MemberId = memberId
            };
            var extraDataJson = JsonSerializer.Serialize(extraDataObj);
            var extraData = Convert.ToBase64String(Encoding.UTF8.GetBytes(extraDataJson));

            // Tạo signature



            var rawSignature = $"accessKey={_config.AccessKey}" +
                             $"&amount={amount}" +
                             $"&extraData={extraData}" +
                             $"&ipnUrl={_config.IpnUrl}" +
                             $"&orderId={orderId}" +
                             $"&orderInfo={orderInfo}" +
                             $"&partnerCode={_config.PartnerCode}" +
                             $"&redirectUrl={_config.RedirectUrl}" +
                             $"&requestId={requestId}" +
                             $"&requestType={_config.RequestType}";


            var signature = ComputeHmacSha256(rawSignature, _config.SecretKey);

            // Tạo request body
            var requestBody = new MoMoPaymentRequest
            {
                PartnerCode = _config.PartnerCode,
                RequestId = requestId,
                Amount = amount,
                OrderId = orderId,
                OrderInfo = orderInfo,
                RedirectUrl = _config.RedirectUrl,
                IpnUrl = _config.IpnUrl,
                RequestType = _config.RequestType,
                ExtraData = extraData,
                Signature = signature
            };

            try
            {
                // Gửi request đến MoMo API
                var response = await _httpClient.PostAsJsonAsync(_config.EndpointUrl, requestBody);
                var result = await response.Content.ReadFromJsonAsync<MoMoPaymentResponse>();

                return result ?? throw new Exception("Empty response from MoMo");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MoMo payment request error: {ex.Message}");
                throw new Exception("Không thể kết nối với cổng thanh toán MoMo");
            }
        }

        /// <summary>
        /// Xác thực callback từ MoMo
        /// </summary>
        /// 
        public bool VerifyCallback(MoMoIpnCallbackDto data)
        {
            var rawSignature =
                $"accessKey={_config.AccessKey}" +
                $"&amount={data.Amount}" +
                $"&extraData={data.ExtraData}" +
                $"&message={data.Message}" +
                $"&orderId={data.OrderId}" +
                $"&orderInfo={data.OrderInfo}" +
                $"&orderType={data.OrderType}" +
                $"&partnerCode={data.PartnerCode}" +
                $"&payType={data.PayType}" +
                $"&requestId={data.RequestId}" +
                $"&responseTime={data.ResponseTime}" +
                $"&resultCode={data.ResultCode}" +
                $"&transId={data.TransId}";

            var expectedSignature = ComputeHmacSha256(rawSignature, _config.SecretKey);

            Console.WriteLine("🔐 MoMo IPN Verify");
            Console.WriteLine("RAW SIGNATURE:");
            Console.WriteLine(rawSignature);
            Console.WriteLine("EXPECTED:");
            Console.WriteLine(expectedSignature);
            Console.WriteLine("RECEIVED:");
            Console.WriteLine(data.Signature);

            return expectedSignature.Equals(
                data.Signature,
                StringComparison.OrdinalIgnoreCase
            );
        }



        /// <summary>
        /// Giải mã extraData từ MoMo
        /// </summary>
        public MoMoExtraData DecodeExtraData(string extraData)
        {
            try
            {
                Console.WriteLine($"Raw extraData received: {extraData}");

                if (string.IsNullOrEmpty(extraData))
                {
                    Console.WriteLine("extraData is missing or empty");
                    return new MoMoExtraData();
                }

                // Giải mã Base64
                var decodedBytes = Convert.FromBase64String(extraData);
                var decodedString = Encoding.UTF8.GetString(decodedBytes);
                Console.WriteLine($"Decoded string: {decodedString}");

                // Parse JSON
                var jsonData = JsonSerializer.Deserialize<MoMoExtraData>(decodedString);
                Console.WriteLine($"Parsed JSON data: {JsonSerializer.Serialize(jsonData)}");

                if (string.IsNullOrEmpty(jsonData?.PackageId) || string.IsNullOrEmpty(jsonData?.MemberId))
                {
                    Console.WriteLine($"Missing required fields in extraData: {JsonSerializer.Serialize(jsonData)}");
                }

                return jsonData ?? new MoMoExtraData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding extraData: {ex.Message}");
                return new MoMoExtraData();
            }
        }

        /// <summary>
        /// Compute HMAC SHA256
        /// </summary>
        private string ComputeHmacSha256(string message, string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(messageBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}