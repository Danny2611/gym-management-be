using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Repositories.User
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(string id);
        Task<Role?> GetByNameAsync(string name);
        Task<IEnumerable<Role>> GetAllAsync();
    }
}