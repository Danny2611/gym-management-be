// using GymManagement.Application.DTOs.Admin;
// using GymManagement.Application.Interfaces.Repositories.Admin;
// using GymManagement.Domain.Entities;
// using GymManagement.Infrastructure.Data;
// using MongoDB.Bson;
// using MongoDB.Driver;

// namespace GymManagement.Infrastructure.Repositories.Admin
// {
//     public class AdminRevenueReportRepository : IAdminRevenueReportRepository
//     {
//         private readonly MongoDbContext _context;
//         private readonly IMongoCollection<Payment> _payments;
//         private readonly IMongoCollection<Membership> _memberships;

//         public AdminRevenueReportRepository(MongoDbContext context)
//         {
//             _context = context;
//             _payments = _context.Payments;
//             _memberships = _context.Memberships;
//         }

//          private decimal SafeToDecimal(BsonValue value, decimal defaultValue = 0)
//         {
//             if (value == null || value.IsBsonNull)
//                 return defaultValue;

//             try
//             {
//                 return value.ToDecimal();
//             }
//             catch
//             {
//                 return defaultValue;
//             }
//         }

//         // Helper method to safely get int from BsonValue
//         private int SafeToInt32(BsonValue value, int defaultValue = 0)
//         {
//             if (value == null || value.IsBsonNull)
//                 return defaultValue;

//             try
//             {
//                 return value.ToInt32();
//             }
//             catch
//             {
//                 return defaultValue;
//             }
//         }

//         // Helper method to safely get string from BsonValue
//         private string SafeToString(BsonValue value, string defaultValue = "")
//         {
//             if (value == null || value.IsBsonNull)
//                 return defaultValue;

//             try
//             {
//                 return value.AsString;
//             }
//             catch
//             {
//                 return defaultValue;
//             }
//         }

//         public async Task<List<RevenueByPackageDto>> GetRevenueByPackagesAsync(
//             RevenueReportOptions options)
//         {
//             var pipeline = new List<BsonDocument>();

//             // Match stage
//             var matchFilter = new BsonDocument
//             {
//                 { "status", "completed" }
//             };

//             if (options.StartDate.HasValue || options.EndDate.HasValue)
//             {
//                 var dateFilter = new BsonDocument();
//                 if (options.StartDate.HasValue)
//                     dateFilter.Add("$gte", options.StartDate.Value);
//                 if (options.EndDate.HasValue)
//                     dateFilter.Add("$lte", options.EndDate.Value);
//                 matchFilter.Add("created_at", dateFilter);
//             }

//             if (!string.IsNullOrEmpty(options.PackageId))
//             {
//                 matchFilter.Add("package_id", new ObjectId(options.PackageId));
//             }

//             pipeline.Add(new BsonDocument("$match", matchFilter));

//             // Lookup packages
//             pipeline.Add(new BsonDocument("$lookup", new BsonDocument
//             {
//                 { "from", "packages" },
//                 { "localField", "package_id" },
//                 { "foreignField", "_id" },
//                 { "as", "package" }
//             }));

//             pipeline.Add(new BsonDocument("$unwind", "$package"));

//             // Category filter if provided
//             if (!string.IsNullOrEmpty(options.Category))
//             {
//                 pipeline.Add(new BsonDocument("$match", new BsonDocument
//                 {
//                     { "package.category", options.Category }
//                 }));
//             }

//             // Group by package
//             pipeline.Add(new BsonDocument("$group", new BsonDocument
//             {
//                 { "_id", "$package_id" },
//                 { "packageName", new BsonDocument("$first", "$package.name") },
//                 { "category", new BsonDocument("$first", "$package.category") },
//                 { "totalRevenue", new BsonDocument("$sum", "$amount") },
//                 { "totalSales", new BsonDocument("$sum", 1) }
//             }));

//             // Add average revenue
//             pipeline.Add(new BsonDocument("$addFields", new BsonDocument
//             {
//                 { "averageRevenue", new BsonDocument("$divide", new BsonArray { "$totalRevenue", "$totalSales" }) }
//             }));

//             // Sort by total revenue descending
//             pipeline.Add(new BsonDocument("$sort", new BsonDocument("totalRevenue", -1)));

//             var results = await _payments.Aggregate<BsonDocument>(pipeline).ToListAsync();

//             return results.Select(r => new RevenueByPackageDto
//             {
//                 PackageId = r["_id"].AsObjectId.ToString(),
//                 PackageName = r["packageName"].AsString,
//                 Category = r.Contains("category") ? r["category"].AsString : "basic",
//                 TotalRevenue = r["totalRevenue"].ToDecimal(),
//                 TotalSales = r["totalSales"].ToInt32(),
//                 AverageRevenue = r["averageRevenue"].ToDecimal()
//             }).ToList();
//         }

//         public async Task<List<RevenueTimeSeriesDto>> GetRevenueTimeSeriesAsync(
//             RevenueReportOptions options)
//         {
//             var pipeline = new List<BsonDocument>();

//             // Match stage
//             var matchFilter = new BsonDocument
//             {
//                 { "status", "completed" }
//             };

//             if (options.StartDate.HasValue || options.EndDate.HasValue)
//             {
//                 var dateFilter = new BsonDocument();
//                 if (options.StartDate.HasValue)
//                     dateFilter.Add("$gte", options.StartDate.Value);
//                 if (options.EndDate.HasValue)
//                     dateFilter.Add("$lte", options.EndDate.Value);
//                 matchFilter.Add("created_at", dateFilter);
//             }

//             pipeline.Add(new BsonDocument("$match", matchFilter));

//             // Lookup packages
//             pipeline.Add(new BsonDocument("$lookup", new BsonDocument
//             {
//                 { "from", "packages" },
//                 { "localField", "package_id" },
//                 { "foreignField", "_id" },
//                 { "as", "package" }
//             }));

//             pipeline.Add(new BsonDocument("$unwind", "$package"));

//             // Get date grouping based on groupBy option
//             var dateGroup = GetDateGrouping(options.GroupBy);

//             // First group by period and package
//             pipeline.Add(new BsonDocument("$group", new BsonDocument
//             {
//                 { "_id", new BsonDocument
//                     {
//                         { "period", dateGroup },
//                         { "packageId", "$package_id" }
//                     }
//                 },
//                 { "packageName", new BsonDocument("$first", "$package.name") },
//                 { "revenue", new BsonDocument("$sum", "$amount") },
//                 { "sales", new BsonDocument("$sum", 1) }
//             }));

//             // Second group by period only
//             pipeline.Add(new BsonDocument("$group", new BsonDocument
//             {
//                 { "_id", "$_id.period" },
//                 { "totalRevenue", new BsonDocument("$sum", "$revenue") },
//                 { "totalSales", new BsonDocument("$sum", "$sales") },
//                 { "packages", new BsonDocument("$push", new BsonDocument
//                     {
//                         { "packageId", "$_id.packageId" },
//                         { "packageName", "$packageName" },
//                         { "revenue", "$revenue" },
//                         { "sales", "$sales" }
//                     })
//                 }
//             }));

//             // Sort by period
//             var sortDoc = new BsonDocument();
//             if (options.GroupBy == "day")
//             {
//                 sortDoc.Add("_id.year", 1);
//                 sortDoc.Add("_id.month", 1);
//                 sortDoc.Add("_id.day", 1);
//             }
//             else if (options.GroupBy == "week")
//             {
//                 sortDoc.Add("_id.year", 1);
//                 sortDoc.Add("_id.week", 1);
//             }
//             else if (options.GroupBy == "month")
//             {
//                 sortDoc.Add("_id.year", 1);
//                 sortDoc.Add("_id.month", 1);
//             }
//             else
//             {
//                 sortDoc.Add("_id.year", 1);
//             }

//             pipeline.Add(new BsonDocument("$sort", sortDoc));

//             var results = await _payments.Aggregate<BsonDocument>(pipeline).ToListAsync();

//             return results.Select(r => new RevenueTimeSeriesDto
//             {
//                 Period = FormatPeriod(r["_id"].AsBsonDocument, options.GroupBy),
//                 TotalRevenue = r["totalRevenue"].ToDecimal(),
//                 TotalSales = r["totalSales"].ToInt32(),
//                 Packages = r["packages"].AsBsonArray.Select(p => new TimeSeriesPackageDto
//                 {
//                     PackageId = p["packageId"].AsObjectId.ToString(),
//                     PackageName = p["packageName"].AsString,
//                     Revenue = p["revenue"].ToDecimal(),
//                     Sales = p["sales"].ToInt32()
//                 }).ToList()
//             }).ToList();
//         }

//         public async Task<AdvancedAnalyticsDto> GetAdvancedAnalyticsAsync(
//             DateTime? startDate,
//             DateTime? endDate)
//         {
//             // Member retention rate
//             var totalMemberships = await _memberships.CountDocumentsAsync(FilterDefinition<Membership>.Empty);
//             var activeMemberships = await _memberships.CountDocumentsAsync(m => m.Status == "active");
//             var memberRetentionRate = totalMemberships > 0
//                 ? (decimal)activeMemberships / totalMemberships * 100
//                 : 0;

//             // Average lifetime value
//             var lifetimeValuePipeline = new List<BsonDocument>
//             {
//                 new BsonDocument("$lookup", new BsonDocument
//                 {
//                     { "from", "payments" },
//                     { "localField", "member_id" },
//                     { "foreignField", "member_id" },
//                     { "as", "payments" }
//                 }),
//                 new BsonDocument("$addFields", new BsonDocument
//                 {
//                     { "totalSpent", new BsonDocument("$sum", new BsonDocument("$map", new BsonDocument
//                         {
//                             { "input", "$payments" },
//                             { "as", "payment" },
//                             { "in", new BsonDocument("$cond", new BsonArray
//                                 {
//                                     new BsonDocument("$eq", new BsonArray { "$$payment.status", "completed" }),
//                                     "$$payment.amount",
//                                     0
//                                 })
//                             }
//                         }))
//                     }
//                 }),
//                 new BsonDocument("$group", new BsonDocument
//                 {
//                     { "_id", BsonNull.Value },
//                     { "averageLifetimeValue", new BsonDocument("$avg", "$totalSpent") }
//                 })
//             };

//             var lifetimeValueResult = await _memberships.Aggregate<BsonDocument>(lifetimeValuePipeline).FirstOrDefaultAsync();
//             var averageLifetimeValue = lifetimeValueResult != null && lifetimeValueResult.Contains("averageLifetimeValue")
//                 ? lifetimeValueResult["averageLifetimeValue"].ToDecimal()
//                 : 0;

//             // Churn rate
//             var expiredMemberships = await _memberships.CountDocumentsAsync(m => m.Status == "expired");
//             var churnRate = totalMemberships > 0
//                 ? (decimal)expiredMemberships / totalMemberships * 100
//                 : 0;

//             // Package popularity
//             var packagePopularityPipeline = new List<BsonDocument>
//             {
//                 new BsonDocument("$lookup", new BsonDocument
//                 {
//                     { "from", "packages" },
//                     { "localField", "package_id" },
//                     { "foreignField", "_id" },
//                     { "as", "package" }
//                 }),
//                 new BsonDocument("$unwind", "$package"),
//                 new BsonDocument("$group", new BsonDocument
//                 {
//                     { "_id", "$package_id" },
//                     { "packageName", new BsonDocument("$first", "$package.name") },
//                     { "memberCount", new BsonDocument("$sum", 1) }
//                 }),
//                 new BsonDocument("$sort", new BsonDocument("memberCount", -1))
//             };

//             var packagePopularityResults = await _memberships.Aggregate<BsonDocument>(packagePopularityPipeline).ToListAsync();
//             var totalPackageSubscriptions = packagePopularityResults.Sum(p => p["memberCount"].ToInt32());

//             var packagePopularity = packagePopularityResults.Select(p => new PackagePopularityDto
//             {
//                 PackageName = p["packageName"].AsString,
//                 MemberCount = p["memberCount"].ToInt32(),
//                 Percentage = totalPackageSubscriptions > 0
//                     ? (decimal)p["memberCount"].ToInt32() / totalPackageSubscriptions * 100
//                     : 0
//             }).ToList();

//             // Revenue by payment method
//             var revenueByMethodPipeline = new List<BsonDocument>
//             {
//                 new BsonDocument("$match", new BsonDocument("status", "completed")),
//                 new BsonDocument("$group", new BsonDocument
//                 {
//                     { "_id", "$paymentMethod" },
//                     { "revenue", new BsonDocument("$sum", "$amount") }
//                 })
//             };

//             var revenueByMethodResults = await _payments.Aggregate<BsonDocument>(revenueByMethodPipeline).ToListAsync();
//             var totalRevenueByMethod = revenueByMethodResults.Sum(r => r["revenue"].ToDecimal());

//             var revenueByPaymentMethod = revenueByMethodResults.Select(r => new RevenueByPaymentMethodDto
//             {
//                 Method = r["_id"].AsString,
//                 Revenue = r["revenue"].ToDecimal(),
//                 Percentage = totalRevenueByMethod > 0
//                     ? r["revenue"].ToDecimal() / totalRevenueByMethod * 100
//                     : 0
//             }).ToList();

//             return new AdvancedAnalyticsDto
//             {
//                 MemberRetentionRate = Math.Round(memberRetentionRate, 2),
//                 AverageLifetimeValue = Math.Round(averageLifetimeValue, 2),
//                 ChurnRate = Math.Round(churnRate, 2),
//                 PackagePopularity = packagePopularity,
//                 RevenueByPaymentMethod = revenueByPaymentMethod
//             };
//         }

//         private BsonDocument GetDateGrouping(string groupBy)
//         {
//             return groupBy.ToLower() switch
//             {
//                 "day" => new BsonDocument
//                 {
//                     { "year", new BsonDocument("$year", "$created_at") },
//                     { "month", new BsonDocument("$month", "$created_at") },
//                     { "day", new BsonDocument("$dayOfMonth", "$created_at") }
//                 },
//                 "week" => new BsonDocument
//                 {
//                     { "year", new BsonDocument("$year", "$created_at") },
//                     { "week", new BsonDocument("$week", "$created_at") }
//                 },
//                 "year" => new BsonDocument
//                 {
//                     { "year", new BsonDocument("$year", "$created_at") }
//                 },
//                 _ => new BsonDocument
//                 {
//                     { "year", new BsonDocument("$year", "$created_at") },
//                     { "month", new BsonDocument("$month", "$created_at") }
//                 }
//             };
//         }

//         private string FormatPeriod(BsonDocument dateGroup, string groupBy)
//         {
//             var year = dateGroup.Contains("year") ? dateGroup["year"].ToInt32() : 0;
//             var month = dateGroup.Contains("month") ? dateGroup["month"].ToInt32() : 0;
//             var day = dateGroup.Contains("day") ? dateGroup["day"].ToInt32() : 0;
//             var week = dateGroup.Contains("week") ? dateGroup["week"].ToInt32() : 0;

//             return groupBy.ToLower() switch
//             {
//                 "day" => $"{year}-{month:D2}-{day:D2}",
//                 "week" => $"{year}-W{week:D2}",
//                 "month" => $"{year}-{month:D2}",
//                 "year" => $"{year}",
//                 _ => $"{year}-{month:D2}"
//             };
//         }
//     }
// }