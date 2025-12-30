namespace GymManagement.Application.Interfaces.Services
{
    public interface IMembershipService
    {
        Task<List<string>> GetMemberTrainingLocationsAsync(string memberId);
    }
}
