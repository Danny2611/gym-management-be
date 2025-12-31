using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment> GetByIdAsync(string id);
        Task<List<Payment>> GetByIdsAsync(List<string> ids);
    }
}
