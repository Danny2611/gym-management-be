using GymManagement.Application.DTOs.User.Transaction;

namespace GymManagement.Application.Interfaces.Services.User
{
    public interface ITransactionService
    {
        Task<List<TransactionItemDto>> GetAllMemberTransactionsAsync(string memberId, TransactionFilterDto filter);
        Task<TransactionDetailDto> GetTransactionByIdAsync(string transactionId);
        Task<List<RecentTransactionDto>> GetRecentSuccessfulTransactionsAsync(string memberId);
    }
}