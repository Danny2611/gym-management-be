using GymManagement.Application.DTOs.Auth;

namespace GymManagement.Application.Services.User
{
    public interface ISocialAuthService
    {
        Task<SocialAuthCallbackResponse> HandleExternalLoginAsync(ExternalAuthInfo externalInfo);
    }

}