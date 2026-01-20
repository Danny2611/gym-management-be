using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace GymManagement.Infrastructure.Repositories.Admin
{
    public class AdminMemberReportRepository : IAdminMemberReportRepository
    {
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Member> _members;
        private readonly IMongoCollection<Membership> _memberships;

        public AdminMemberReportRepository(MongoDbContext context)
        {
            _context = context;
            _members = _context.Members;
            _memberships = _context.Memberships;
        }

        // Helper method to format period based on groupBy
        private string FormatPeriod(BsonDocument dateGroup, string groupBy)
        {
            var year = dateGroup.GetValue("year", 0).ToInt32();
            var month = dateGroup.Contains("month") ? dateGroup.GetValue("month", 0).ToInt32() : 0;
            var day = dateGroup.Contains("day") ? dateGroup.GetValue("day", 0).ToInt32() : 0;
            var week = dateGroup.Contains("week") ? dateGroup.GetValue("week", 0).ToInt32() : 0;

            return groupBy switch
            {
                "day" => $"{year}-{month:D2}-{day:D2}",
                "week" => $"{year}-W{week:D2}",
                "month" => $"{year}-{month:D2}",
                "year" => $"{year}",
                _ => $"{year}-{month:D2}"
            };
        }

        // Get date grouping expression
        private BsonDocument GetDateGroup(string groupBy)
        {
            return groupBy switch
            {
                "day" => new BsonDocument
                {
                    { "year", new BsonDocument("$year", "$created_at") },
                    { "month", new BsonDocument("$month", "$created_at") },
                    { "day", new BsonDocument("$dayOfMonth", "$created_at") }
                },
                "week" => new BsonDocument
                {
                    { "year", new BsonDocument("$year", "$created_at") },
                    { "week", new BsonDocument("$week", "$created_at") }
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

        public async Task<List<AdminMemberStatsDto>> GetMemberStatsAsync(
    AdminMemberStatsOptions options)
        {
            var filterBuilder = Builders<Member>.Filter;
            var filters = new List<FilterDefinition<Member>>();

            // ===== Date range filter =====
            if (options.StartDate.HasValue)
            {
                filters.Add(filterBuilder.Gte(m => m.CreatedAt, options.StartDate.Value));
            }

            if (options.EndDate.HasValue)
            {
                filters.Add(filterBuilder.Lte(m => m.CreatedAt, options.EndDate.Value));
            }

            // ===== Status filter =====
            if (!string.IsNullOrEmpty(options.Status))
            {
                filters.Add(filterBuilder.Eq(m => m.Status, options.Status));
            }

            var finalFilter = filters.Count > 0
                ? filterBuilder.And(filters)
                : filterBuilder.Empty;

            // ===== Date grouping =====
            var dateGroup = GetDateGroup(options.GroupBy);

            // ===== FIX: Render filter (MongoDB.Driver mới) =====
            var serializer = BsonSerializer.SerializerRegistry.GetSerializer<Member>();
            var renderedFilter = finalFilter.Render(
                new RenderArgs<Member>(serializer, BsonSerializer.SerializerRegistry));

            // ===== Aggregation pipeline =====
            var pipeline = new[]
            {
        new BsonDocument("$match", renderedFilter),

        new BsonDocument("$group", new BsonDocument
        {
            { "_id", dateGroup },
            { "newMembers", new BsonDocument("$sum", 1) },

            { "activeMembers", new BsonDocument("$sum",
                new BsonDocument("$cond", new BsonArray
                {
                    new BsonDocument("$eq", new BsonArray { "$status", "active" }),
                    1,
                    0
                }))
            },

            { "inactiveMembers", new BsonDocument("$sum",
                new BsonDocument("$cond", new BsonArray
                {
                    new BsonDocument("$eq", new BsonArray { "$status", "inactive" }),
                    1,
                    0
                }))
            },

            { "pendingMembers", new BsonDocument("$sum",
                new BsonDocument("$cond", new BsonArray
                {
                    new BsonDocument("$eq", new BsonArray { "$status", "pending" }),
                    1,
                    0
                }))
            },

            { "bannedMembers", new BsonDocument("$sum",
                new BsonDocument("$cond", new BsonArray
                {
                    new BsonDocument("$eq", new BsonArray { "$status", "banned" }),
                    1,
                    0
                }))
            }
        }),

        new BsonDocument("$sort", new BsonDocument
        {
            { "_id.year", 1 },
            { "_id.month", 1 },
            { "_id.day", 1 },
            { "_id.week", 1 }
        })
    };

            var memberResults = await _members
                .Aggregate<BsonDocument>(pipeline)
                .ToListAsync();

            // ===== Expired memberships =====
            var expiredMap = await GetExpiredMembershipsMapAsync(
                options.StartDate,
                options.EndDate,
                options.GroupBy);

            // ===== Calculate stats =====
            var cumulativeTotal = 0;
            var results = new List<AdminMemberStatsDto>();

            for (int i = 0; i < memberResults.Count; i++)
            {
                var result = memberResults[i];
                var dateGroupDoc = result["_id"].AsBsonDocument;
                var period = FormatPeriod(dateGroupDoc, options.GroupBy);

                var newMembers = result["newMembers"].ToInt32();
                var expiredMembers = expiredMap.TryGetValue(period, out var expired)
                    ? expired
                    : 0;

                cumulativeTotal += newMembers;

                var netGrowth = newMembers - expiredMembers;
                var previousTotal = i > 0 ? cumulativeTotal - newMembers : 0;
                var growthRate = previousTotal > 0
                    ? ((decimal)netGrowth / previousTotal) * 100
                    : 0;

                decimal? retentionRate = null;
                decimal? churnRate = null;

                if (options.IncludeRetention || options.IncludeChurn)
                {
                    var periodStart = GetPeriodStartDate(dateGroupDoc, options.GroupBy);
                    var periodEnd = GetPeriodEndDate(periodStart, options.GroupBy);

                    if (options.IncludeRetention)
                    {
                        retentionRate = await CalculateRetentionRateAsync(periodStart, periodEnd);
                    }

                    if (options.IncludeChurn && retentionRate.HasValue)
                    {
                        churnRate = 100 - retentionRate.Value;
                    }
                }

                results.Add(new AdminMemberStatsDto
                {
                    Period = period,
                    TotalMembers = cumulativeTotal,
                    NewMembers = newMembers,
                    ExpiredMembers = expiredMembers,
                    ActiveMembers = result["activeMembers"].ToInt32(),
                    InactiveMembers = result["inactiveMembers"].ToInt32(),
                    PendingMembers = result["pendingMembers"].ToInt32(),
                    BannedMembers = result["bannedMembers"].ToInt32(),
                    RetentionRate = retentionRate,
                    ChurnRate = churnRate,
                    GrowthRate = growthRate,
                    NetGrowth = netGrowth
                });
            }

            return results;
        }
        public async Task<decimal> CalculateRetentionRateAsync(
        DateTime startPeriod,
        DateTime endPeriod)
        {
            var filterBuilder = Builders<Member>.Filter;

            // ===== Members at start =====
            var membersAtStart = await _members.CountDocumentsAsync(
                filterBuilder.And(
                    filterBuilder.Lte(m => m.CreatedAt, startPeriod),
                    filterBuilder.In(m => m.Status, new[] { "active", "inactive" })
                )
            );

            if (membersAtStart == 0)
            {
                return 0;
            }

            // ===== Retained members =====
            var retainedMembers = await _members.CountDocumentsAsync(
                filterBuilder.And(
                    filterBuilder.Lte(m => m.CreatedAt, startPeriod),
                    filterBuilder.Or(
                        filterBuilder.Eq(m => m.Status, "active"),
                        filterBuilder.And(
                            filterBuilder.Eq(m => m.Status, "inactive"),
                            filterBuilder.Gte(m => m.UpdatedAt, endPeriod)
                        )
                    )
                )
            );

            return ((decimal)retainedMembers / membersAtStart) * 100;
        }


        private DateTime GetPeriodStartDate(BsonDocument dateGroup, string groupBy)
        {
            var year = dateGroup.GetValue("year", 0).ToInt32();
            var month = dateGroup.Contains("month") ? dateGroup.GetValue("month", 0).ToInt32() : 1;
            var day = dateGroup.Contains("day") ? dateGroup.GetValue("day", 0).ToInt32() : 1;

            return new DateTime(year, month, day);
        }

        private DateTime GetPeriodEndDate(DateTime startDate, string groupBy)
        {
            return groupBy switch
            {
                "day" => startDate.AddDays(1),
                "week" => startDate.AddDays(7),
                "month" => startDate.AddMonths(1),
                "year" => startDate.AddYears(1),
                _ => startDate.AddMonths(1)
            };
        }

        public async Task<Dictionary<string, int>> GetExpiredMembershipsMapAsync(
    DateTime? startDate,
    DateTime? endDate,
    string groupBy)
        {
            var filterBuilder = Builders<Membership>.Filter;
            var filters = new List<FilterDefinition<Membership>>
    {
        filterBuilder.Eq(m => m.Status, "expired")
    };

            if (startDate.HasValue)
            {
                filters.Add(filterBuilder.Gte(m => m.EndDate, startDate.Value));
            }

            if (endDate.HasValue)
            {
                filters.Add(filterBuilder.Lte(m => m.EndDate, endDate.Value));
            }

            var finalFilter = filterBuilder.And(filters);

            // ===== Date group =====
            var dateGroupDoc = new BsonDocument
    {
        { "year", new BsonDocument("$year", "$end_date") },
        { "month", new BsonDocument("$month", "$end_date") }
    };

            if (groupBy == "day")
            {
                dateGroupDoc.Add("day", new BsonDocument("$dayOfMonth", "$end_date"));
            }
            else if (groupBy == "week")
            {
                dateGroupDoc.Add("week", new BsonDocument("$week", "$end_date"));
            }

            // ===== FIX Render API =====
            var serializer = BsonSerializer.SerializerRegistry.GetSerializer<Membership>();
            var renderedFilter = finalFilter.Render(
                new RenderArgs<Membership>(serializer, BsonSerializer.SerializerRegistry));

            // ===== Aggregation pipeline =====
            var pipeline = new[]
            {
        new BsonDocument("$match", renderedFilter),
        new BsonDocument("$group", new BsonDocument
        {
            { "_id", dateGroupDoc },
            { "expiredMembers", new BsonDocument("$sum", 1) }
        })
    };

            var results = await _memberships
                .Aggregate<BsonDocument>(pipeline)
                .ToListAsync();

            return results.ToDictionary(
                r => FormatPeriod(r["_id"].AsBsonDocument, groupBy),
                r => r["expiredMembers"].ToInt32()
            );
        }


        public async Task<int> GetTotalMembersCountAsync(List<string>? statuses = null)
        {
            var filterBuilder = Builders<Member>.Filter;

            if (statuses != null && statuses.Any())
            {
                return (int)await _members.CountDocumentsAsync(
                    filterBuilder.In(m => m.Status, statuses));
            }

            return (int)await _members.CountDocumentsAsync(FilterDefinition<Member>.Empty);
        }

        public async Task<Dictionary<string, int>> GetStatusCountsAsync()
        {
            var pipeline = new[]
            {
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$status" },
                    { "count", new BsonDocument("$sum", 1) }
                })
            };

            var results = await _members.Aggregate<BsonDocument>(pipeline).ToListAsync();

            return results.ToDictionary(
                r => r["_id"].AsString,
                r => r["count"].ToInt32()
            );
        }

        public async Task<AdminRetentionFunnelDto> GetRetentionFunnelAsync()
        {
            var now = DateTime.UtcNow;
            var filterBuilder = Builders<Member>.Filter;

            var newMembers = await _members.CountDocumentsAsync(
                filterBuilder.Gte(m => m.CreatedAt, now.AddDays(-30))
            );

            var activeAfter30Days = await _members.CountDocumentsAsync(filterBuilder.And(
                filterBuilder.Gte(m => m.CreatedAt, now.AddDays(-60)),
                filterBuilder.Lte(m => m.CreatedAt, now.AddDays(-30)),
                filterBuilder.Eq(m => m.Status, "active")
            ));

            var activeAfter90Days = await _members.CountDocumentsAsync(filterBuilder.And(
                filterBuilder.Gte(m => m.CreatedAt, now.AddDays(-120)),
                filterBuilder.Lte(m => m.CreatedAt, now.AddDays(-90)),
                filterBuilder.Eq(m => m.Status, "active")
            ));

            var activeAfter180Days = await _members.CountDocumentsAsync(filterBuilder.And(
                filterBuilder.Gte(m => m.CreatedAt, now.AddDays(-210)),
                filterBuilder.Lte(m => m.CreatedAt, now.AddDays(-180)),
                filterBuilder.Eq(m => m.Status, "active")
            ));

            var activeAfter365Days = await _members.CountDocumentsAsync(filterBuilder.And(
                filterBuilder.Gte(m => m.CreatedAt, now.AddDays(-395)),
                filterBuilder.Lte(m => m.CreatedAt, now.AddDays(-365)),
                filterBuilder.Eq(m => m.Status, "active")
            ));

            return new AdminRetentionFunnelDto
            {
                NewMembers = (int)newMembers,
                ActiveAfter30Days = (int)activeAfter30Days,
                ActiveAfter90Days = (int)activeAfter90Days,
                ActiveAfter180Days = (int)activeAfter180Days,
                ActiveAfter365Days = (int)activeAfter365Days
            };
        }
    }
}