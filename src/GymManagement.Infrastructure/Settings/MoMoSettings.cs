namespace GymManagement.Infrastructure.Settings
{
    public class MoMoSettings
    {
        public string PartnerCode { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;
        public string IpnUrl { get; set; } = string.Empty;
        public string RequestType { get; set; } = "payWithMethod";
        public string EndpointUrl { get; set; } = "https://test-payment.momo.vn/v2/gateway/api/create";
    }
}