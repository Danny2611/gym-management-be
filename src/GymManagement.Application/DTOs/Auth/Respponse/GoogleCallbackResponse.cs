namespace GymManagement.Application.DTOs.Auth
{
    public class GoogleCallbackResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public UserInfo UserData { get; set; }
    }
}
