using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.DTOs;

namespace AuthService.Controllers;

[ApiController]
[Route("api/lte-day")]
public class LteDayController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LteDayController> _logger;

    public LteDayController(ApplicationDbContext context, ILogger<LteDayController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get LTE Day KPI data grouped by sector with multiple cell series
    /// </summary>
    [HttpPost("kpi")]
    public async Task<IActionResult> GetKpiData([FromBody] LteDayKpiRequestDto request)
    {
        try
        {
            // Parse dates
            if (!DateOnly.TryParse(request.StartDate, out var startDate) ||
                !DateOnly.TryParse(request.EndDate, out var endDate))
            {
                return BadRequest(new { error = "Invalid date format. Use YYYY-MM-DD" });
            }

            var startDateTime = startDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

            var query = _context.LteDays.AsQueryable();

            // Apply date filter
            query = query.Where(x => x.DateTime >= startDateTime && x.DateTime <= endDateTime);

            // Apply site_id filter if provided
            if (request.SiteIds != null && request.SiteIds.Count > 0)
            {
                query = query.Where(x => x.SiteId != null && request.SiteIds.Contains(x.SiteId));
            }

            // Apply band filter if provided
            if (request.Bands != null && request.Bands.Count > 0)
            {
                query = query.Where(x => request.Bands.Contains(x.Band!));
            }

            // Apply cellname filter if provided
            if (request.CellNames != null && request.CellNames.Count > 0)
            {
                query = query.Where(x => request.CellNames.Contains(x.CellName!));
            }

            // Get raw data
            var rawData = await query
                .Select(x => new
                {
                    x.DateTime,
                    x.SectorGroup,
                    x.CellName,
                    x.Avail,
                    x.Erab,
                    x.Rrc,
                    x.Sssr,
                    x.Sar,
                    x.IntraFho,
                    x.InterFho,
                    x.DlUtil,
                    x.UlUtil,
                    x.PrbMaxDl,
                    x.PrbMaxUl,
                    x.AvgCqi,
                    x.Se,
                    x.UserDlThp,
                    x.UserUlThp,
                    x.CellDlThp,
                    x.CellUlThp,
                    x.Csfb,
                    x.UlPucch,
                    x.UlRssiDbm,
                    x.MaxActiveUser,
                    x.MaxRrcUser,
                    x.TrafficErl,
                    x.PayloadMb
                })
                .OrderBy(x => x.DateTime)
                .ToListAsync();

            // Group by SectorGroup
            var sectorGroups = rawData
                .Where(x => x.SectorGroup.HasValue)
                .GroupBy(x => x.SectorGroup!.Value)
                .OrderBy(g => g.Key);

            var response = new LteDayKpiResponseDto();

            foreach (var sectorGroup in sectorGroups)
            {
                var sectorData = sectorGroup.ToList();
                var cellNames = sectorData
                    .Where(x => !string.IsNullOrEmpty(x.CellName))
                    .Select(x => x.CellName!)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                var sectorKpi = new SectorKpiDto
                {
                    SectorGroup = sectorGroup.Key,
                    // Existing KPIs
                    Availability = BuildKpiChart("Availability", "%", sectorData, cellNames, x => x.Avail),
                    Erab = BuildKpiChart("E-RAB SR", "%", sectorData, cellNames, x => x.Erab),
                    Rrc = BuildKpiChart("RRC SR", "%", sectorData, cellNames, x => x.Rrc),
                    Sssr = BuildKpiChart("SSSR", "%", sectorData, cellNames, x => x.Sssr),
                    Sar = BuildKpiChart("SAR", "%", sectorData, cellNames, x => x.Sar),
                    IntraFho = BuildKpiChart("Intra-F HO", "%", sectorData, cellNames, x => x.IntraFho),
                    InterFho = BuildKpiChart("Inter-F HO", "%", sectorData, cellNames, x => x.InterFho),
                    DlUtil = BuildKpiChart("DL Utilization", "%", sectorData, cellNames, x => x.DlUtil),
                    UlUtil = BuildKpiChart("UL Utilization", "%", sectorData, cellNames, x => x.UlUtil),
                    // New KPIs - below UL Utilization
                    PrbMaxDl = BuildKpiChart("PRB MAX DL", "%", sectorData, cellNames, x => x.PrbMaxDl),
                    PrbMaxUl = BuildKpiChart("PRB MAX UL", "%", sectorData, cellNames, x => x.PrbMaxUl),
                    Cqi = BuildKpiChart("CQI", "", sectorData, cellNames, x => x.AvgCqi),
                    Se = BuildKpiChart("SE", "bps/Hz", sectorData, cellNames, x => x.Se),
                    // Existing KPIs - User Throughput
                    UserDlThp = BuildKpiChart("User DL THP", "Mbps", sectorData, cellNames, x => x.UserDlThp),
                    UserUlThp = BuildKpiChart("User UL THP", "Mbps", sectorData, cellNames, x => x.UserUlThp),
                    // New KPIs - below User UL THP
                    CellDlThp = BuildKpiChart("Cell DL THP", "Mbps", sectorData, cellNames, x => x.CellDlThp),
                    CellUlThp = BuildKpiChart("Cell UL THP", "Mbps", sectorData, cellNames, x => x.CellUlThp),
                    Csfb = BuildKpiChart("CSFB", "", sectorData, cellNames, x => x.Csfb),
                    // New KPIs - below CSFB
                    UlPucch = BuildKpiChart("UL PUCCH", "dBm", sectorData, cellNames, x => x.UlPucch),
                    UlRssi = BuildKpiChart("UL RSSI", "dBm", sectorData, cellNames, x => x.UlRssiDbm),
                    MaxActiveUser = BuildKpiChart("Max Active User", "", sectorData, cellNames, x => x.MaxActiveUser),
                    MaxRrcUser = BuildKpiChart("Max RRC User", "", sectorData, cellNames, x => x.MaxRrcUser),
                    // Bottom KPIs - Traffic first, then Payload
                    TrafficErl = BuildKpiChart("Traffic", "Erl", sectorData, cellNames, x => x.TrafficErl),
                    PayloadMb = BuildKpiChart("Payload", "MB", sectorData, cellNames, x => x.PayloadMb)
                };

                response.Sectors.Add(sectorKpi);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching LTE Day KPI data");
            return StatusCode(500, new { error = "Failed to fetch KPI data" });
        }
    }

    /// <summary>
    /// Get Traffic and Payload KPI data (Aggregated by Site, Band, Cell, NE)
    /// </summary>
    [HttpPost("traffic-payload")]
    public async Task<IActionResult> GetTrafficPayload([FromBody] LteDayKpiRequestDto request)
    {
        try
        {
            if (!DateOnly.TryParse(request.StartDate, out var startDate) ||
                !DateOnly.TryParse(request.EndDate, out var endDate))
            {
                return BadRequest(new { error = "Invalid date format. Use YYYY-MM-DD" });
            }

            var startDateTime = startDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

            var query = _context.LteDays.AsQueryable();
            query = query.Where(x => x.DateTime >= startDateTime && x.DateTime <= endDateTime);

            if (request.SiteIds != null && request.SiteIds.Count > 0)
                query = query.Where(x => x.SiteId != null && request.SiteIds.Contains(x.SiteId));

            if (request.Bands != null && request.Bands.Count > 0)
                query = query.Where(x => request.Bands.Contains(x.Band!));

            if (request.CellNames != null && request.CellNames.Count > 0)
                query = query.Where(x => request.CellNames.Contains(x.CellName!));

            // Fetch Data including SectorGroup
            var rawData = await query
                .Select(x => new { x.DateTime, x.SiteId, x.Band, x.CellName, x.NeId, x.SectorGroup, x.TrafficErl, x.PayloadMb })
                .ToListAsync();

            var response = new TrafficPayloadResponseDto();

            // Local Helper to build series
            KpiChartDto BuildSeries(string chartName, string unit, IEnumerable<dynamic> groupedData, Func<dynamic, string> keySelector, Func<dynamic, double?> valueSelector)
            {
                var chart = new KpiChartDto { Name = chartName, Unit = unit };
                foreach (var group in groupedData)
                {
                    string seriesName = keySelector(group);
                    var points = new List<KpiDataPointDto>();
                    var groupList = (IEnumerable<dynamic>)group;
                    
                    // Aggregate by Date
                    var dateGroups = groupList.GroupBy(x => ((DateTime)x.DateTime).ToString("yyyy-MM-dd"));
                    
                    foreach(var dateGroup in dateGroups)
                    {
                        double? sum = dateGroup.Sum(x => (double?)valueSelector(x));
                        points.Add(new KpiDataPointDto { 
                            Date = dateGroup.Key, 
                            Value = sum.HasValue ? Math.Round(sum.Value, 2) : null
                        });
                    }
                    // Sort points by date
                    points = points.OrderBy(p => p.Date).ToList();
                    chart.Series.Add(new CellSeriesDto { CellName = seriesName, Data = points });
                }
                // Sort series by name
                chart.Series = chart.Series.OrderBy(s => s.CellName).ToList();
                return chart;
            }

            // Helper for Aggregated Set
            AggregatedSectorKpiDto BuildAggregatedSet(string categoryName, string unit, IEnumerable<dynamic> sourceData, Func<dynamic, string> keySelector, Func<dynamic, double?> valueSelector)
            {
                var dto = new AggregatedSectorKpiDto();
                
                // Total (Summary)
                var totalGrouped = sourceData.GroupBy(keySelector);
                dto.Total = BuildSeries($"{categoryName} - Total", unit, totalGrouped, g => g.Key ?? "Unknown", valueSelector);

                // Sector 1
                var sec1Data = sourceData.Where(x => x.SectorGroup == 1).GroupBy(keySelector);
                dto.Sector1 = BuildSeries($"{categoryName} - Sector 1", unit, sec1Data, g => g.Key ?? "Unknown", valueSelector);

                // Sector 2
                var sec2Data = sourceData.Where(x => x.SectorGroup == 2).GroupBy(keySelector);
                dto.Sector2 = BuildSeries($"{categoryName} - Sector 2", unit, sec2Data, g => g.Key ?? "Unknown", valueSelector);

                // Sector 3
                var sec3Data = sourceData.Where(x => x.SectorGroup == 3).GroupBy(keySelector);
                dto.Sector3 = BuildSeries($"{categoryName} - Sector 3", unit, sec3Data, g => g.Key ?? "Unknown", valueSelector);

                return dto;
            }

            // --- Payload Construction ---
            response.PayloadBySite = BuildAggregatedSet("Site", "MB", rawData, x => x.SiteId, x => x.PayloadMb);
            response.PayloadByBand = BuildAggregatedSet("Band", "MB", rawData, x => x.Band, x => x.PayloadMb);
            response.PayloadByCell = BuildAggregatedSet("Cell", "MB", rawData, x => x.CellName, x => x.PayloadMb);
            response.PayloadByNe   = BuildAggregatedSet("NE",   "MB", rawData, x => x.NeId, x => x.PayloadMb);

            // --- Traffic Construction ---
            response.TrafficBySite = BuildAggregatedSet("Site", "Erl", rawData, x => x.SiteId, x => x.TrafficErl);
            response.TrafficByBand = BuildAggregatedSet("Band", "Erl", rawData, x => x.Band,   x => x.TrafficErl);
            response.TrafficByCell = BuildAggregatedSet("Cell", "Erl", rawData, x => x.CellName, x => x.TrafficErl);
            response.TrafficByNe   = BuildAggregatedSet("NE",   "Erl", rawData, x => x.NeId,   x => x.TrafficErl);


            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Traffic Payload data");
            return StatusCode(500, new { error = "Failed to fetch Traffic Payload data" });
        }
    }

    private KpiChartDto BuildKpiChart<T>(
        string name,
        string unit,
        List<T> data,
        List<string> cellNames,
        Func<T, double?> valueSelector) where T : class
    {
        var chart = new KpiChartDto
        {
            Name = name,
            Unit = unit,
            Series = new List<CellSeriesDto>()
        };

        // Get DateTime and CellName using reflection (simplified approach given our anonymous type earlier matches)
        // Note: The rawData in GetKpiData is List<AnonymousType>, so T is AnonymousType.
        // Reflection works on AnonymousTypes.
        var dateTimeProp = typeof(T).GetProperty("DateTime");
        var cellNameProp = typeof(T).GetProperty("CellName");

        if (dateTimeProp == null || cellNameProp == null) return chart;

        foreach (var cellName in cellNames)
        {
            var cellData = data
                .Where(x => (string?)cellNameProp.GetValue(x) == cellName)
                .Select(x => new KpiDataPointDto
                {
                    Date = ((DateTime?)dateTimeProp.GetValue(x))?.ToString("yyyy-MM-dd") ?? "",
                    Value = valueSelector(x).HasValue ? Math.Round(valueSelector(x)!.Value, 2) : null
                })
                .ToList();

            chart.Series.Add(new CellSeriesDto
            {
                CellName = cellName,
                Data = cellData
            });
        }

        return chart;
    }
}
