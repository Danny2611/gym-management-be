using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace GymManagement.Infrastructure.Repositories.Admin
{
    public class AdminRevenueReportRepository : IAdminRevenueReportRepository
    {
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Payment> _payments;
        private readonly IMongoCollection<Package> _packages;
        private readonly IMongoCollection<Membership> _memberships;
        private readonly IMongoCollection<Member> _members;

        public AdminRevenueReportRepository(MongoDbContext context)
        {
            _context = context;
            _payments = _context.Payments;
            _packages = _context.Packages;
            _memberships = _context.Memberships;
            _members = _context.Members;
        }

        // Helper method to safely get decimal from BsonValue
        private decimal SafeToDecimal(BsonValue value, decimal defaultValue = 0)
        {
            if (value == null || value.IsBsonNull)
                return defaultValue;

            try
            {
                return value.ToDecimal();
            }
            catch
            {
                return defaultValue;
            }
        }

        // Helper method to safely get int from BsonValue
        private int SafeToInt32(BsonValue value, int defaultValue = 0)
        {
            if (value == null || value.IsBsonNull)
                return defaultValue;

            try
            {
                return value.ToInt32();
            }
            catch
            {
                return defaultValue;
            }
        }

        // Helper method to safely get string from BsonValue
        private string SafeToString(BsonValue value, string defaultValue = "")
        {
            if (value == null || value.IsBsonNull)
                return defaultValue;

            try
            {
                return value.AsString;
            }
            catch
            {
                return defaultValue;
            }
        }

        public async Task<List<RevenueByPackageDto>> GetRevenueByPackagesAsync(
    RevenueReportOptions options)
        {
            var filterBuilder = Builders<Payment>.Filter;
            var filters = new List<FilterDefinition<Payment>>
    {
        filterBuilder.Eq(p => p.Status, "completed")
    };

            // Date range filter
            if (options.StartDate.HasValue)
                filters.Add(filterBuilder.Gte(p => p.CreatedAt, options.StartDate.Value));
            if (options.EndDate.HasValue)
                filters.Add(filterBuilder.Lte(p => p.CreatedAt, options.EndDate.Value));

            // Package filter
            if (!string.IsNullOrEmpty(options.PackageId))
                filters.Add(filterBuilder.Eq(p => p.PackageId, options.PackageId));

            var finalFilter = filterBuilder.And(filters);

            // ✅ FIX: Sử dụng Render với RenderArgs
            var renderArgs = new RenderArgs<Payment>(
                BsonSerializer.SerializerRegistry.GetSerializer<Payment>(),
                BsonSerializer.SerializerRegistry);

            var pipeline = new BsonDocument[]
            {
        new BsonDocument("$match", finalFilter.Render(renderArgs)),
        new BsonDocument("$group", new BsonDocument
        {
            { "_id", "$package_id" },
            { "totalRevenue", new BsonDocument("$sum", "$amount") },
            { "totalSales", new BsonDocument("$sum", 1) }
        }),
        new BsonDocument("$sort", new BsonDocument("totalRevenue", -1))
            };

            var results = await _payments.Aggregate<BsonDocument>(pipeline).ToListAsync();

            var revenueByPackages = new List<RevenueByPackageDto>();

            foreach (var result in results)
            {
                var packageId = SafeToString(result["_id"]);

                // Skip if packageId is empty
                if (string.IsNullOrEmpty(packageId))
                    continue;

                var package = await _packages.Find(p => p.Id == packageId).FirstOrDefaultAsync();

                // Skip if package not found
                if (package == null)
                    continue;

                // Apply category filter if specified
                if (!string.IsNullOrEmpty(options.Category) &&
                    package.Category?.ToLower() != options.Category.ToLower())
                    continue;

                var totalRevenue = SafeToDecimal(result["totalRevenue"]);
                var totalSales = SafeToInt32(result["totalSales"]);

                revenueByPackages.Add(new RevenueByPackageDto
                {
                    PackageId = packageId,
                    PackageName = package.Name ?? "Unknown",
                    Category = package.Category ?? "Unknown",
                    TotalRevenue = totalRevenue,
                    TotalSales = totalSales,
                    AverageRevenue = totalSales > 0 ? Math.Round(totalRevenue / totalSales, 2) : 0
                });
            }

            return revenueByPackages;
        }

        public async Task<List<RevenueTimeSeriesDto>> GetRevenueTimeSeriesAsync(
            RevenueReportOptions options)
        {
            var filterBuilder = Builders<Payment>.Filter;
            var filters = new List<FilterDefinition<Payment>>
    {
        filterBuilder.Eq(p => p.Status, "completed")
    };

            // Date range filter
            if (options.StartDate.HasValue)
                filters.Add(filterBuilder.Gte(p => p.CreatedAt, options.StartDate.Value));
            if (options.EndDate.HasValue)
                filters.Add(filterBuilder.Lte(p => p.CreatedAt, options.EndDate.Value));

            // Package filter
            if (!string.IsNullOrEmpty(options.PackageId))
                filters.Add(filterBuilder.Eq(p => p.PackageId, options.PackageId));

            var finalFilter = filterBuilder.And(filters);

            // Build date grouping based on groupBy parameter
            var dateGroup = GetDateGroupExpression(options.GroupBy);

            // ✅ FIX: Sử dụng Render với RenderArgs
            var renderArgs = new RenderArgs<Payment>(
                BsonSerializer.SerializerRegistry.GetSerializer<Payment>(),
                BsonSerializer.SerializerRegistry);

            var pipeline = new BsonDocument[]
            {
        new BsonDocument("$match", finalFilter.Render(renderArgs)),
        new BsonDocument("$group", new BsonDocument
        {
            { "_id", dateGroup },
            { "totalRevenue", new BsonDocument("$sum", "$amount") },
            { "totalSales", new BsonDocument("$sum", 1) },
            { "payments", new BsonDocument("$push", new BsonDocument
                {
                    { "package_id", "$package_id" },
                    { "amount", "$amount" }
                })
            }
        }),
        new BsonDocument("$sort", new BsonDocument("_id", 1))
            };

            var results = await _payments.Aggregate<BsonDocument>(pipeline).ToListAsync();

            var timeSeries = new List<RevenueTimeSeriesDto>();

            foreach (var result in results)
            {
                var period = FormatPeriod(result["_id"].AsBsonDocument, options.GroupBy);
                var totalRevenue = SafeToDecimal(result["totalRevenue"]);
                var totalSales = SafeToInt32(result["totalSales"]);

                // Get package breakdown
                var packages = new List<TimeSeriesPackageDto>();

                if (result.Contains("payments") && !result["payments"].IsBsonNull)
                {
                    var paymentsArray = result["payments"].AsBsonArray;

                    var packageGroups = paymentsArray
                        .GroupBy(p => SafeToString(p["package_id"]))
                        .Where(g => !string.IsNullOrEmpty(g.Key));

                    foreach (var group in packageGroups)
                    {
                        var packageId = group.Key;
                        var package = await _packages.Find(p => p.Id == packageId).FirstOrDefaultAsync();

                        if (package == null)
                            continue;

                        // Apply category filter if specified
                        if (!string.IsNullOrEmpty(options.Category) &&
                            package.Category?.ToLower() != options.Category.ToLower())
                            continue;

                        var packageRevenue = group.Sum(p => SafeToDecimal(p["amount"]));
                        var packageSales = group.Count();

                        packages.Add(new TimeSeriesPackageDto
                        {
                            PackageId = packageId,
                            PackageName = package.Name ?? "Unknown",
                            Revenue = packageRevenue,
                            Sales = packageSales
                        });
                    }
                }

                timeSeries.Add(new RevenueTimeSeriesDto
                {
                    Period = period,
                    TotalRevenue = totalRevenue,
                    TotalSales = totalSales,
                    Packages = packages
                });
            }

            return timeSeries;
        }
        public async Task<AdvancedAnalyticsDto> GetAdvancedAnalyticsAsync(
            DateTime? startDate,
            DateTime? endDate)
        {
            // Member retention rate
            var totalMemberships = await _memberships.CountDocumentsAsync(FilterDefinition<Membership>.Empty);
            var activeMemberships = await _memberships.CountDocumentsAsync(m => m.Status == "active");
            var memberRetentionRate = totalMemberships > 0
                ? Math.Round(((decimal)activeMemberships / totalMemberships) * 100, 2)
                : 0;

            // Average lifetime value
            var lifetimeValuePipeline = new BsonDocument[]
            {
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "payments" },
                    { "localField", "member_id" },
                    { "foreignField", "member_id" },
                    { "as", "payments" }
                }),
                new BsonDocument("$addFields", new BsonDocument
                {
                    { "totalSpent", new BsonDocument("$sum", new BsonDocument
                        {
                            { "$map", new BsonDocument
                                {
                                    { "input", "$payments" },
                                    { "as", "payment" },
                                    { "in", new BsonDocument("$cond", new BsonArray
                                        {
                                            new BsonDocument("$eq", new BsonArray { "$$payment.status", "completed" }),
                                            "$$payment.amount",
                                            0
                                        })
                                    }
                                }
                            }
                        })
                    }
                }),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", BsonNull.Value },
                    { "averageLifetimeValue", new BsonDocument("$avg", "$totalSpent") }
                })
            };

            var lifetimeValueResult = await _memberships.Aggregate<BsonDocument>(lifetimeValuePipeline).ToListAsync();
            var averageLifetimeValue = lifetimeValueResult.FirstOrDefault() != null
                ? Math.Round(SafeToDecimal(lifetimeValueResult[0]["averageLifetimeValue"]), 2)
                : 0;

            // Churn rate
            var expiredMemberships = await _memberships.CountDocumentsAsync(m => m.Status == "expired");
            var churnRate = totalMemberships > 0
                ? Math.Round(((decimal)expiredMemberships / totalMemberships) * 100, 2)
                : 0;

            // Package popularity
            var packagePopularityPipeline = new BsonDocument[]
            {
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$package_id" },
                    { "memberCount", new BsonDocument("$sum", 1) }
                }),
                new BsonDocument("$sort", new BsonDocument("memberCount", -1))
            };

            var packagePopularityResult = await _memberships.Aggregate<BsonDocument>(packagePopularityPipeline).ToListAsync();
            var totalPackageSubscriptions = packagePopularityResult.Sum(p => SafeToInt32(p["memberCount"]));

            var packagePopularity = new List<PackagePopularityDto>();
            foreach (var result in packagePopularityResult)
            {
                var packageId = SafeToString(result["_id"]);

                if (string.IsNullOrEmpty(packageId))
                    continue;

                var package = await _packages.Find(p => p.Id == packageId).FirstOrDefaultAsync();

                if (package == null)
                    continue;

                var memberCount = SafeToInt32(result["memberCount"]);

                packagePopularity.Add(new PackagePopularityDto
                {
                    PackageName = package.Name ?? "Unknown",
                    Percentage = totalPackageSubscriptions > 0
                        ? Math.Round(((decimal)memberCount / totalPackageSubscriptions) * 100, 2)
                        : 0,
                    MemberCount = memberCount
                });
            }

            // Revenue by payment method
            var revenueByMethodPipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("status", "completed")),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$paymentMethod" },
                    { "revenue", new BsonDocument("$sum", "$amount") }
                })
            };

            var revenueByMethodResult = await _payments.Aggregate<BsonDocument>(revenueByMethodPipeline).ToListAsync();
            var totalRevenueByMethod = revenueByMethodResult.Sum(r => SafeToDecimal(r["revenue"]));

            var revenueByPaymentMethod = new List<RevenueByPaymentMethodDto>();
            foreach (var result in revenueByMethodResult)
            {
                var method = SafeToString(result["_id"], "Unknown");
                var revenue = SafeToDecimal(result["revenue"]);

                revenueByPaymentMethod.Add(new RevenueByPaymentMethodDto
                {
                    Method = method,
                    Revenue = revenue,
                    Percentage = totalRevenueByMethod > 0
                        ? Math.Round((revenue / totalRevenueByMethod) * 100, 2)
                        : 0
                });
            }

            return new AdvancedAnalyticsDto
            {
                MemberRetentionRate = memberRetentionRate,
                AverageLifetimeValue = averageLifetimeValue,
                ChurnRate = churnRate,
                PackagePopularity = packagePopularity,
                RevenueByPaymentMethod = revenueByPaymentMethod
            };
        }

        private BsonDocument GetDateGroupExpression(string groupBy)
        {
            return groupBy?.ToLower() switch
            {
                "day" => new BsonDocument
                {
                    { "year", new BsonDocument("$year", "$created_at") },
                    { "month", new BsonDocument("$month", "$created_at") },
                    { "day", new BsonDocument("$dayOfMonth", "$created_at") }
                },
                "week" => new BsonDocument
                {
                    { "year", new BsonDocument("$isoWeekYear", "$created_at") },
                    { "week", new BsonDocument("$isoWeek", "$created_at") }
                },
                "month" => new BsonDocument
                {
                    { "year", new BsonDocument("$year", "$created_at") },
                    { "month", new BsonDocument("$month", "$created_at") }
                },
                "year" => new BsonDocument
                {
                    { "year", new BsonDocument("$year", "$created_at") }
                },
                _ => new BsonDocument
                {
                    { "year", new BsonDocument("$year", "$created_at") },
                    { "month", new BsonDocument("$month", "$created_at") }
                }
            };
        }

        private string FormatPeriod(BsonDocument dateGroup, string groupBy)
        {
            var year = SafeToInt32(dateGroup.GetValue("year", 0));
            var month = dateGroup.Contains("month") ? SafeToInt32(dateGroup.GetValue("month", 0)) : 0;
            var day = dateGroup.Contains("day") ? SafeToInt32(dateGroup.GetValue("day", 0)) : 0;
            var week = dateGroup.Contains("week") ? SafeToInt32(dateGroup.GetValue("week", 0)) : 0;

            return groupBy?.ToLower() switch
            {
                "day" => $"{year}-{month:D2}-{day:D2}",
                "week" => $"{year}-W{week:D2}",
                "month" => $"{year}-{month:D2}",
                "year" => $"{year}",
                _ => $"{year}-{month:D2}"
            };
        }
    }
}