using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GymManagement.Infrastructure.Repositories.Admin
{
    public class AdminPaymentRepository : IAdminPaymentRepository
    {
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Payment> _payments;

        public AdminPaymentRepository(MongoDbContext context)
        {
            _context = context;
            _payments = _context.Payments;
        }

        public async Task<(List<Payment> payments, int totalCount)> GetAllAsync(
            PaymentQueryOptions options)
        {
            var filterBuilder = Builders<Payment>.Filter;
            var filters = new List<FilterDefinition<Payment>>();

            // Status filter
            if (!string.IsNullOrEmpty(options.Status))
            {
                filters.Add(filterBuilder.Eq(p => p.Status, options.Status));
            }

            // Payment method filter
            if (!string.IsNullOrEmpty(options.PaymentMethod))
            {
                filters.Add(filterBuilder.Eq(p => p.PaymentMethod, options.PaymentMethod));
            }

            // Date range filter
            if (options.DateFrom.HasValue || options.DateTo.HasValue)
            {
                if (options.DateFrom.HasValue && options.DateTo.HasValue)
                {
                    filters.Add(filterBuilder.Gte(p => p.CreatedAt, options.DateFrom.Value));
                    filters.Add(filterBuilder.Lte(p => p.CreatedAt, options.DateTo.Value));
                }
                else if (options.DateFrom.HasValue)
                {
                    filters.Add(filterBuilder.Gte(p => p.CreatedAt, options.DateFrom.Value));
                }
                else if (options.DateTo.HasValue)
                {
                    filters.Add(filterBuilder.Lte(p => p.CreatedAt, options.DateTo.Value));
                }
            }

            var finalFilter = filters.Count > 0
                ? filterBuilder.And(filters)
                : filterBuilder.Empty;

            // Get total count
            var totalCount = (int)await _payments.CountDocumentsAsync(finalFilter);

            // Build sort
            var sortBuilder = Builders<Payment>.Sort;
            SortDefinition<Payment> sort;

            if (!string.IsNullOrEmpty(options.SortBy))
            {
                sort = options.SortOrder?.ToLower() == "desc"
                    ? sortBuilder.Descending(options.SortBy)
                    : sortBuilder.Ascending(options.SortBy);
            }
            else
            {
                sort = sortBuilder.Descending(p => p.CreatedAt);
            }

            // Get payments with pagination
            var payments = await _payments
                .Find(finalFilter)
                .Sort(sort)
                .Skip((options.Page - 1) * options.Limit)
                .Limit(options.Limit)
                .ToListAsync();

            return (payments, totalCount);
        }

        public async Task<Payment?> GetByIdAsync(string id)
        {
            return await _payments
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Payment?> UpdateStatusAsync(string id, string status, string? transactionId)
        {
            var updateBuilder = Builders<Payment>.Update;
            var updates = new List<UpdateDefinition<Payment>>
            {
                updateBuilder.Set(p => p.Status, status),
                updateBuilder.Set(p => p.UpdatedAt, DateTime.UtcNow)
            };

            if (!string.IsNullOrEmpty(transactionId))
            {
                updates.Add(updateBuilder.Set(p => p.TransactionId, transactionId));
            }

            var combinedUpdate = updateBuilder.Combine(updates);

            var options = new FindOneAndUpdateOptions<Payment>
            {
                ReturnDocument = ReturnDocument.After
            };

            return await _payments.FindOneAndUpdateAsync(
                p => p.Id == id,
                combinedUpdate,
                options
            );
        }

        public async Task<(List<Payment> payments, int totalCount)> GetByMemberIdAsync(
            string memberId,
            PaymentQueryOptions options)
        {
            var filterBuilder = Builders<Payment>.Filter;
            var filters = new List<FilterDefinition<Payment>>
            {
                filterBuilder.Eq(p => p.MemberId, memberId)
            };

            // Status filter
            if (!string.IsNullOrEmpty(options.Status))
            {
                filters.Add(filterBuilder.Eq(p => p.Status, options.Status));
            }

            // Payment method filter
            if (!string.IsNullOrEmpty(options.PaymentMethod))
            {
                filters.Add(filterBuilder.Eq(p => p.PaymentMethod, options.PaymentMethod));
            }

            // Date range filter
            if (options.DateFrom.HasValue || options.DateTo.HasValue)
            {
                if (options.DateFrom.HasValue && options.DateTo.HasValue)
                {
                    filters.Add(filterBuilder.Gte(p => p.CreatedAt, options.DateFrom.Value));
                    filters.Add(filterBuilder.Lte(p => p.CreatedAt, options.DateTo.Value));
                }
                else if (options.DateFrom.HasValue)
                {
                    filters.Add(filterBuilder.Gte(p => p.CreatedAt, options.DateFrom.Value));
                }
                else if (options.DateTo.HasValue)
                {
                    filters.Add(filterBuilder.Lte(p => p.CreatedAt, options.DateTo.Value));
                }
            }

            var finalFilter = filterBuilder.And(filters);

            // Get total count
            var totalCount = (int)await _payments.CountDocumentsAsync(finalFilter);

            // Build sort
            var sortBuilder = Builders<Payment>.Sort;
            SortDefinition<Payment> sort;

            if (!string.IsNullOrEmpty(options.SortBy))
            {
                sort = options.SortOrder?.ToLower() == "desc"
                    ? sortBuilder.Descending(options.SortBy)
                    : sortBuilder.Ascending(options.SortBy);
            }
            else
            {
                sort = sortBuilder.Descending(p => p.CreatedAt);
            }

            // Get payments with pagination
            var payments = await _payments
                .Find(finalFilter)
                .Sort(sort)
                .Skip((options.Page - 1) * options.Limit)
                .Limit(options.Limit)
                .ToListAsync();

            return (payments, totalCount);
        }

        public async Task<PaymentStatisticsDto> GetStatisticsAsync()
        {
            var total = (int)await _payments.CountDocumentsAsync(FilterDefinition<Payment>.Empty);
            var pending = (int)await _payments.CountDocumentsAsync(p => p.Status == "pending");
            var completed = (int)await _payments.CountDocumentsAsync(p => p.Status == "completed");
            var failed = (int)await _payments.CountDocumentsAsync(p => p.Status == "failed");
            var cancelled = (int)await _payments.CountDocumentsAsync(p => p.Status == "cancelled");

            // Calculate total revenue and completed revenue using aggregation
            var revenueAggregation = await _payments.Aggregate()
                .Group(
                    p => p.Status,
                    g => new
                    {
                        Status = g.Key,
                        TotalAmount = g.Sum(p => p.Amount)
                    }
                )
                .ToListAsync();

            var totalRevenue = revenueAggregation.Sum(r => r.TotalAmount);
            var completedRevenue = revenueAggregation
                .Where(r => r.Status == "completed")
                .Sum(r => r.TotalAmount);

            // Payment methods statistics
            var paymentMethodAggregation = await _payments.Aggregate()
                .Group(
                    p => p.PaymentMethod,
                    g => new
                    {
                        PaymentMethod = g.Key,
                        Count = g.Count()
                    }
                )
                .ToListAsync();

            var paymentMethods = new PaymentMethodStatsDto
            {
                Qr = paymentMethodAggregation.FirstOrDefault(p => p.PaymentMethod == "qr")?.Count ?? 0,
                Credit = paymentMethodAggregation.FirstOrDefault(p => p.PaymentMethod == "credit")?.Count ?? 0,
                Napas = paymentMethodAggregation.FirstOrDefault(p => p.PaymentMethod == "napas")?.Count ?? 0,
                Undefined = paymentMethodAggregation.FirstOrDefault(p => p.PaymentMethod == "undefined")?.Count ?? 0
            };

            // Monthly revenue for the last 12 months
            var twelveMonthsAgo = DateTime.UtcNow.AddMonths(-12);

            var monthlyAggregation = await _payments.Aggregate()
                .Match(p => p.CreatedAt >= twelveMonthsAgo && p.Status == "completed")
                .Group(
                    p => new { Year = p.CreatedAt.Year, Month = p.CreatedAt.Month },
                    g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Revenue = g.Sum(p => p.Amount),
                        Count = g.Count()
                    }
                )
                .SortBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToListAsync();

            var monthlyRevenue = monthlyAggregation.Select(m => new MonthlyRevenueDto
            {
                Month = $"{m.Year}-{m.Month:D2}",
                Revenue = m.Revenue,
                Count = m.Count
            }).ToList();

            return new PaymentStatisticsDto
            {
                Total = total,
                Pending = pending,
                Completed = completed,
                Failed = failed,
                Cancelled = cancelled,
                TotalRevenue = totalRevenue,
                CompletedRevenue = completedRevenue,
                PaymentMethods = paymentMethods,
                MonthlyRevenue = monthlyRevenue
            };
        }
    }
}