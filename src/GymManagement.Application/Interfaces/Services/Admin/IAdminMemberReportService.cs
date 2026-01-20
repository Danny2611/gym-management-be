using GymManagement.Application.DTOs.Admin;

namespace GymManagement.Application.Interfaces.Services.Admin
{
    public interface IAdminMemberReportService
    {
        Task<List<AdminMemberStatsDto>> GetMemberStatsAsync(AdminMemberStatsOptions options);
        Task<AdminComprehensiveMemberReportDto> GetComprehensiveMemberReportAsync(AdminMemberStatsOptions options);
        Task<byte[]> ExportMemberReportToExcelAsync(AdminMemberStatsOptions options);
        Task<byte[]> ExportMemberReportToPdfAsync(AdminMemberStatsOptions options);
    }
}