using GymManagement.Application.DTOs.Admin;

namespace GymManagement.Application.Interfaces.Repositories.Admin
{
    public interface IAdminMemberReportRepository
    {
        Task<List<AdminMemberStatsDto>> GetMemberStatsAsync(AdminMemberStatsOptions options);
        Task<int> GetTotalMembersCountAsync(List<string>? statuses = null);
        Task<Dictionary<string, int>> GetStatusCountsAsync();
        Task<AdminRetentionFunnelDto> GetRetentionFunnelAsync();
        Task<Dictionary<string, int>> GetExpiredMembershipsMapAsync(
            DateTime? startDate,
            DateTime? endDate,
            string groupBy);
        Task<decimal> CalculateRetentionRateAsync(DateTime startPeriod, DateTime endPeriod);
    }
}