using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace GymManagement.Infrastructure.Repositories.Admin
{
    public class AdminDashboardReportRepository : IAdminDashboardReportRepository
    {
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Payment> _payments;
        private readonly IMongoCollection<Member> _members;
        private readonly IMongoCollection<Membership> _memberships;
        private readonly IMongoCollection<Package> _packages;

        public AdminDashboardReportRepository(MongoDbContext context)
        {
            _context = context;
            _payments = _context.Payments;
            _members = _context.Members;
            _memberships = _context.Memberships;
            _packages = _context.Packages;
        }

        public async Task<AdminDashboardStatsDto> GetDashboardStatsAsync(
      AdminReportDateRange dateRange)
        {
            var filterBuilder = Builders<Payment>.Filter;
            var paymentFilters = new List<FilterDefinition<Payment>>
    {
        filterBuilder.Eq(p => p.Status, "completed")
    };

            // ===== Date range filter =====
            if (dateRange.StartDate.HasValue)
            {
                paymentFilters.Add(filterBuilder.Gte(p => p.CreatedAt, dateRange.StartDate.Value));
            }

            if (dateRange.EndDate.HasValue)
            {
                paymentFilters.Add(filterBuilder.Lte(p => p.CreatedAt, dateRange.EndDate.Value));
            }

            var paymentFilter = filterBuilder.And(paymentFilters);

            // ===== FIX Render API =====
            var paymentSerializer = BsonSerializer.SerializerRegistry.GetSerializer<Payment>();
            var renderedPaymentFilter = paymentFilter.Render(
                new RenderArgs<Payment>(paymentSerializer, BsonSerializer.SerializerRegistry));

            // ==============================
            // TOTAL REVENUE
            // ==============================
            var revenuePipeline = new[]
            {
        new BsonDocument("$match", renderedPaymentFilter),
        new BsonDocument("$group", new BsonDocument
        {
            { "_id", BsonNull.Value },
            { "total", new BsonDocument("$sum", "$amount") }
        })
    };

            var revenueResult = await _payments
                .Aggregate<BsonDocument>(revenuePipeline)
                .ToListAsync();

            var currentRevenue = revenueResult.FirstOrDefault()?["total"].ToDecimal() ?? 0;

            // ==============================
            // MEMBER COUNTS
            // ==============================
            var totalMembers = await _members.CountDocumentsAsync(FilterDefinition<Member>.Empty);
            var activeMembers = await _members.CountDocumentsAsync(m => m.Status == "active");
            var expiredMemberships = await _memberships.CountDocumentsAsync(m => m.Status == "expired");

            // ==============================
            // GROWTH CALCULATIONS
            // ==============================
            decimal revenueGrowth = 0;
            decimal memberGrowth = 0;

            if (dateRange.StartDate.HasValue && dateRange.EndDate.HasValue)
            {
                var periodDuration = dateRange.EndDate.Value - dateRange.StartDate.Value;
                var previousPeriodStart = dateRange.StartDate.Value - periodDuration;
                var previousPeriodEnd = dateRange.StartDate.Value;

                // ===== Previous revenue =====
                var previousRevenueFilter = filterBuilder.And(
                    filterBuilder.Eq(p => p.Status, "completed"),
                    filterBuilder.Gte(p => p.CreatedAt, previousPeriodStart),
                    filterBuilder.Lt(p => p.CreatedAt, previousPeriodEnd)
                );

                var renderedPreviousRevenueFilter = previousRevenueFilter.Render(
                    new RenderArgs<Payment>(paymentSerializer, BsonSerializer.SerializerRegistry));

                var previousRevenuePipeline = new[]
                {
            new BsonDocument("$match", renderedPreviousRevenueFilter),
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", BsonNull.Value },
                { "total", new BsonDocument("$sum", "$amount") }
            })
        };

                var previousRevenueResult = await _payments
                    .Aggregate<BsonDocument>(previousRevenuePipeline)
                    .ToListAsync();

                var previousRevenue = previousRevenueResult.FirstOrDefault()?["total"].ToDecimal() ?? 0;

                if (previousRevenue > 0)
                {
                    revenueGrowth = ((currentRevenue - previousRevenue) / previousRevenue) * 100;
                }

                // ===== Member growth =====
                var currentMemberFilter = Builders<Member>.Filter.And(
                    Builders<Member>.Filter.Gte(m => m.CreatedAt, dateRange.StartDate.Value),
                    Builders<Member>.Filter.Lte(m => m.CreatedAt, dateRange.EndDate.Value)
                );

                var previousMemberFilter = Builders<Member>.Filter.And(
                    Builders<Member>.Filter.Gte(m => m.CreatedAt, previousPeriodStart),
                    Builders<Member>.Filter.Lt(m => m.CreatedAt, previousPeriodEnd)
                );

                var currentMemberCount = await _members.CountDocumentsAsync(currentMemberFilter);
                var previousMemberCount = await _members.CountDocumentsAsync(previousMemberFilter);

                if (previousMemberCount > 0)
                {
                    memberGrowth =
                        ((decimal)(currentMemberCount - previousMemberCount) / previousMemberCount) * 100;
                }
            }

            // ==============================
            // TOP PACKAGES
            // ==============================
            var topPackagesPipeline = new[]
            {
        new BsonDocument("$match", renderedPaymentFilter),
        new BsonDocument("$group", new BsonDocument
        {
            { "_id", "$package_id" },
            { "revenue", new BsonDocument("$sum", "$amount") },
            { "memberCount", new BsonDocument("$sum", 1) }
        }),
        new BsonDocument("$sort", new BsonDocument("revenue", -1)),
        new BsonDocument("$limit", 5)
    };

            var topPackagesResult = await _payments
                .Aggregate<BsonDocument>(topPackagesPipeline)
                .ToListAsync();

            var topPackages = new List<AdminTopPackageDto>();
            foreach (var result in topPackagesResult)
            {
                var packageId = result["_id"].AsString;
                var package = await _packages.Find(p => p.Id == packageId).FirstOrDefaultAsync();

                topPackages.Add(new AdminTopPackageDto
                {
                    PackageId = packageId,
                    PackageName = package?.Name ?? "N/A",
                    Revenue = result["revenue"].ToDecimal(),
                    MemberCount = result["memberCount"].ToInt32()
                });
            }

            // ==============================
            // RECENT PAYMENTS
            // ==============================
            var recentPaymentsQuery = await _payments
                .Find(paymentFilter)
                .SortByDescending(p => p.CreatedAt)
                .Limit(10)
                .ToListAsync();

            var recentPayments = new List<AdminRecentPaymentDto>();
            foreach (var payment in recentPaymentsQuery)
            {
                var member = await _members.Find(m => m.Id == payment.MemberId).FirstOrDefaultAsync();
                var package = await _packages.Find(p => p.Id == payment.PackageId).FirstOrDefaultAsync();

                recentPayments.Add(new AdminRecentPaymentDto
                {
                    PaymentId = payment.Id,
                    MemberName = member?.Name ?? "N/A",
                    PackageName = package?.Name ?? "N/A",
                    Amount = payment.Amount,
                    Status = payment.Status,
                    CreatedAt = payment.CreatedAt
                });
            }

            // ==============================
            // RESULT
            // ==============================
            return new AdminDashboardStatsDto
            {
                TotalRevenue = currentRevenue,
                TotalMembers = (int)totalMembers,
                ActiveMembers = (int)activeMembers,
                ExpiredMemberships = (int)expiredMemberships,
                RevenueGrowth = Math.Round(revenueGrowth, 2),
                MemberGrowth = Math.Round(memberGrowth, 2),
                TopPackages = topPackages,
                RecentPayments = recentPayments
            };
        }


        public async Task<AdminAdvancedAnalyticsDto> GetAdvancedAnalyticsAsync(
     AdminReportDateRange dateRange)
        {
            if (!dateRange.StartDate.HasValue || !dateRange.EndDate.HasValue)
                throw new ArgumentException("StartDate và EndDate là bắt buộc");

            var startDate = dateRange.StartDate.Value;
            var endDate = dateRange.EndDate.Value;

            // ==============================
            // 1. MEMBER RETENTION RATE
            // ==============================
            var membersAtStart = await _members.CountDocumentsAsync(
                Builders<Member>.Filter.Lte(m => m.CreatedAt, startDate)
            );

            var retainedMembers = await _members.CountDocumentsAsync(
                Builders<Member>.Filter.And(
                    Builders<Member>.Filter.Lte(m => m.CreatedAt, startDate),
                    Builders<Member>.Filter.Eq(m => m.Status, "active")
                )
            );

            var memberRetentionRate =
                membersAtStart > 0
                    ? ((decimal)retainedMembers / membersAtStart) * 100
                    : 0;

            // ==============================
            // 2. AVERAGE LIFETIME VALUE (LTV)
            // ==============================
            var lifetimeValuePipeline = new[]
            {
        new BsonDocument("$match", new BsonDocument
        {
            { "status", "completed" }
        }),
        new BsonDocument("$group", new BsonDocument
        {
            { "_id", "$member_id" },
            { "totalSpent", new BsonDocument("$sum", "$amount") }
        }),
        new BsonDocument("$group", new BsonDocument
        {
            { "_id", BsonNull.Value },
            { "averageLifetimeValue", new BsonDocument("$avg", "$totalSpent") }
        })
    };

            var lifetimeValueResult = await _payments
                .Aggregate<BsonDocument>(lifetimeValuePipeline)
                .ToListAsync();

            var averageLifetimeValue =
                lifetimeValueResult.FirstOrDefault()?["averageLifetimeValue"].ToDecimal() ?? 0;

            // ==============================
            // 3. CHURN RATE
            // ==============================
            var activeMembershipsAtStart = await _memberships.CountDocumentsAsync(
                Builders<Membership>.Filter.And(
                    Builders<Membership>.Filter.Lte(m => m.StartDate, startDate),
                    Builders<Membership>.Filter.Ne(m => m.Status, "expired")
                )
            );

            var expiredInPeriod = await _memberships.CountDocumentsAsync(
                Builders<Membership>.Filter.And(
                    Builders<Membership>.Filter.Eq(m => m.Status, "expired"),
                    Builders<Membership>.Filter.Gte(m => m.EndDate, startDate),
                    Builders<Membership>.Filter.Lte(m => m.EndDate, endDate)
                )
            );

            var churnRate =
                activeMembershipsAtStart > 0
                    ? ((decimal)expiredInPeriod / activeMembershipsAtStart) * 100
                    : 0;

            // ==============================
            // 4. PACKAGE POPULARITY (IN PERIOD)
            // ==============================
            var packagePopularityPipeline = new[]
            {
        new BsonDocument("$match", new BsonDocument
        {
            { "created_at", new BsonDocument
                {
                    { "$gte", startDate },
                    { "$lte", endDate }
                }
            }
        }),
        new BsonDocument("$group", new BsonDocument
        {
            { "_id", "$package_id" },
            { "count", new BsonDocument("$sum", 1) }
        }),
        new BsonDocument("$sort", new BsonDocument("count", -1))
    };

            var packagePopularityResult = await _memberships
                .Aggregate<BsonDocument>(packagePopularityPipeline)
                .ToListAsync();

            var totalSubscriptions = packagePopularityResult.Sum(p => p["count"].ToInt32());

            var packagePopularity = new List<AdminPackagePopularityDto>();

            foreach (var item in packagePopularityResult)
            {
                var packageId = item["_id"].AsString;
                var package = await _packages.Find(p => p.Id == packageId).FirstOrDefaultAsync();

                var count = item["count"].ToInt32();

                packagePopularity.Add(new AdminPackagePopularityDto
                {
                    PackageName = package?.Name ?? "N/A",
                    MemberCount = count,
                    Percentage =
                        totalSubscriptions > 0
                            ? Math.Round(((decimal)count / totalSubscriptions) * 100, 2)
                            : 0
                });
            }

            // ==============================
            // 5. REVENUE BY PAYMENT METHOD
            // ==============================
            var revenueByMethodPipeline = new[]
            {
        new BsonDocument("$match", new BsonDocument
        {
            { "status", "completed" },
            { "created_at", new BsonDocument
                {
                    { "$gte", startDate },
                    { "$lte", endDate }
                }
            }
        }),
        new BsonDocument("$group", new BsonDocument
        {
            { "_id", "$paymentMethod" },
            { "revenue", new BsonDocument("$sum", "$amount") }
        })
    };

            var revenueByMethodResult = await _payments
                .Aggregate<BsonDocument>(revenueByMethodPipeline)
                .ToListAsync();

            var totalRevenue = revenueByMethodResult.Sum(r => r["revenue"].ToDecimal());

            var revenueByPaymentMethod = revenueByMethodResult.Select(r => new AdminRevenueByPaymentMethodDto
            {
                Method = r["_id"].AsString,
                Revenue = r["revenue"].ToDecimal(),
                Percentage =
                    totalRevenue > 0
                        ? Math.Round((r["revenue"].ToDecimal() / totalRevenue) * 100, 2)
                        : 0
            }).ToList();

            // ==============================
            // RESULT
            // ==============================
            return new AdminAdvancedAnalyticsDto
            {
                MemberRetentionRate = Math.Round(memberRetentionRate, 2),
                AverageLifetimeValue = Math.Round(averageLifetimeValue, 2),
                ChurnRate = Math.Round(churnRate, 2),
                PackagePopularity = packagePopularity,
                RevenueByPaymentMethod = revenueByPaymentMethod
            };
        }

    }
}