
using GymManagement.Domain.Entities;
namespace GymManagement.Application.Interfaces.Repositories.User
{
    public interface IProgressRepository
    {
        Task<Progress> CreateAsync(Progress progress);
        Task<Progress> GetLatestAsync(string memberId);
        Task<Progress> GetInitialAsync(string memberId);
        Task<Progress> GetPreviousMonthAsync(string memberId);
        Task<List<Progress>> GetByDateRangeAsync(string memberId, DateTime startDate, DateTime endDate);
        Task<List<Progress>> GetMonthlyMetricsAsync(string memberId);
    }
}