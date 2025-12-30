using GymManagement.Application.Interfaces.Repositories;
using GymManagement.Application.Interfaces.Services;

namespace GymManagement.Application.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IPackageDetailRepository _packageDetailRepository;

        public MembershipService(
            IMembershipRepository membershipRepository,
            IPackageDetailRepository packageDetailRepository)
        {
            _membershipRepository = membershipRepository;
            _packageDetailRepository = packageDetailRepository;
        }

        public async Task<List<string>> GetMemberTrainingLocationsAsync(string memberId)
        {
            // Get all active memberships for the member
            var memberships = await _membershipRepository.GetActiveMembershipsByMemberIdAsync(memberId);

            if (memberships == null || memberships.Count == 0)
            {
                return new List<string>();
            }

            // Extract package IDs from memberships
            var packageIds = memberships.Select(m => m.PackageId).ToList();

            // Get all package details for these packages
            var packageDetails = await _packageDetailRepository.GetByPackageIdsAsync(packageIds);

            // Collect unique training areas
            var uniqueLocations = new HashSet<string>();

            foreach (var detail in packageDetails)
            {
                if (detail.TrainingAreas != null && detail.TrainingAreas.Count > 0)
                {
                    foreach (var location in detail.TrainingAreas)
                    {
                        uniqueLocations.Add(location);
                    }
                }
            }

            return uniqueLocations.ToList();
        }
    }
}
