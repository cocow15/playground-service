using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.DTOs;

namespace AuthService.Controllers;

[ApiController]
[Route("api/gsm-day")]
public class GsmDayController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GsmDayController> _logger;

    public GsmDayController(ApplicationDbContext context, ILogger<GsmDayController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get GSM Day KPI data grouped by sector with multiple cell series
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

            var query = _context.GsmDays.AsQueryable();

            // Apply date filter
            query = query.Where(x => x.DateTime >= startDateTime && x.DateTime <= endDateTime);

            // Apply filters
            if (request.SiteIds != null && request.SiteIds.Count > 0)
                query = query.Where(x => x.SiteId != null && request.SiteIds.Contains(x.SiteId));

            if (request.Bands != null && request.Bands.Count > 0)
                query = query.Where(x => x.Band != null && request.Bands.Contains(x.Band));

            if (request.CellNames != null && request.CellNames.Count > 0)
                query = query.Where(x => x.CellName != null && request.CellNames.Contains(x.CellName));

            // Get raw data
            // We need to fetch enough columns to calculate all KPIs
            var rawData = await query
                .Select(x => new
                {
                    x.DateTime,
                    x.SectorGroup,
                    x.CellName,
                    x.SiteId,
                    x.Band,
                    x.NeId,
                    // KPI Columns
                    x.AvailNum, x.AvailDenum,
                    x.SCongNum, x.SCongDenum,
                    x.TCongNum, x.TCongDenum,
                    x.HosrNum, x.HosrDenum,
                    x.TchDrop, x.TchTraf,
                    x.SdcchSr, x.SdcchTraf,
                    x.TbfCompNum, x.TbfCompDenum,
                    x.TbfDlEstNum, x.TbfDlEstDenum,
                    x.TbfDlSrNum, x.TbfDlSrDenum,
                    x.TbfUlEstNum, x.TbfUlEstDenum,
                    x.PayloadMb,
                    x.TchBlock,
                    x.PayloadGprsMb,
                    x.PayloadEgprsMb,
                    // ICH Columns
                    x.Ich1Num, x.Ich1Denum,
                    x.Ich2Num, x.Ich2Denum,
                    x.Ich3Num, x.Ich3Denum,
                    x.Ich4Num, x.Ich4Denum,
                    x.Ich5Num, x.Ich5Denum
                })
                .OrderBy(x => x.DateTime)
                .ToListAsync();

            // Group by SectorGroup
            var sectorGroups = rawData
                .Where(x => x.SectorGroup != null)
                .GroupBy(x => x.SectorGroup!.ToString()) // SectorGroup is string in 2G Model but logic might assume it maps to 1,2,3...
                .OrderBy(g => g.Key);

            var response = new GsmDayKpiResponseDto();

            foreach (var sectorGroup in sectorGroups)
            {
                var sectorData = sectorGroup.ToList();
                var cellNames = sectorData
                    .Where(x => !string.IsNullOrEmpty(x.CellName))
                    .Select(x => x.CellName!)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
                
                // Try parse key to int if possible for DTO consistency, otherwise 0
                int.TryParse(sectorGroup.Key, out int sgInt);

                var sectorKpi = new GsmSectorKpiDto
                {
                    SectorGroup = sgInt,
                    
                    // Standard KPIs (Line Charts)
                    Availability = BuildKpiChart("Availability", "%", sectorData, cellNames, x => SafeDiv(x.AvailNum, x.AvailDenum) * 100),
                    SCong = BuildKpiChart("S_CONG", "%", sectorData, cellNames, x => SafeDiv(x.SCongNum, x.SCongDenum) * 100),
                    TCong = BuildKpiChart("T_CONG", "%", sectorData, cellNames, x => SafeDiv(x.TCongNum, x.TCongDenum) * 100),
                    Hosr = BuildKpiChart("HOSR", "%", sectorData, cellNames, x => SafeDiv(x.HosrNum, x.HosrDenum) * 100),
                    TchDrop = BuildKpiChart("TCH DROP", "", sectorData, cellNames, x => x.TchDrop),
                    TchBlock = BuildKpiChart("TCH BLOCK", "", sectorData, cellNames, x => x.TchBlock),
                    TbfComp = BuildKpiChart("TBF COMP", "%", sectorData, cellNames, x => SafeDiv(x.TbfCompNum, x.TbfCompDenum) * 100),
                    TbfDlEstSr = BuildKpiChart("TBF DL EST SR", "%", sectorData, cellNames, x => SafeDiv(x.TbfDlEstNum, x.TbfDlEstDenum) * 100),
                    TbfUlEstSr = BuildKpiChart("TBF UL EST SR", "%", sectorData, cellNames, x => SafeDiv(x.TbfUlEstNum, x.TbfUlEstDenum) * 100),
                    TbfDlSr = BuildKpiChart("TBF DL SR", "%", sectorData, cellNames, x => SafeDiv(x.TbfDlSrNum, x.TbfDlSrDenum) * 100),
                    SdcchSr = BuildKpiChart("SDCCH SR", "%", sectorData, cellNames, x => x.SdcchSr),
                    // ICH = Sum of ICH 1-5 (num/denum calculation)
                    Ich = BuildKpiChart("ICH", "%", sectorData, cellNames, x => 
                        SafeDiv(
                            (x.Ich1Num ?? 0) + (x.Ich2Num ?? 0) + (x.Ich3Num ?? 0) + (x.Ich4Num ?? 0) + (x.Ich5Num ?? 0),
                            (x.Ich1Denum ?? 0) + (x.Ich2Denum ?? 0) + (x.Ich3Denum ?? 0) + (x.Ich4Denum ?? 0) + (x.Ich5Denum ?? 0)
                        ) * 100),

                    // Payload KPIs - Each has Line Chart + Stacked Area below
                    PayloadEdgeGb = BuildKpiChart("PAYLOAD EDGE GB", "GB", sectorData, cellNames, x => x.PayloadEgprsMb / 1024),
                    PayloadEdgeGbStacked = BuildStackedSingleChart("PAYLOAD EDGE GB", "GB", sectorData, cellNames, x => x.PayloadEgprsMb / 1024),
                    PayloadGprsGb = BuildKpiChart("PAYLOAD GPRS GB", "GB", sectorData, cellNames, x => x.PayloadGprsMb / 1024),
                    PayloadGprsGbStacked = BuildStackedSingleChart("PAYLOAD GPRS GB", "GB", sectorData, cellNames, x => x.PayloadGprsMb / 1024),
                    PayloadTotalGb = BuildKpiChart("PAYLOAD TOTAL GB", "GB", sectorData, cellNames, x => 
                        ((x.PayloadEgprsMb ?? 0) + (x.PayloadGprsMb ?? 0)) / 1024),
                    PayloadTotalGbStacked = BuildStackedSingleChart("PAYLOAD TOTAL GB", "GB", sectorData, cellNames, x => 
                        ((x.PayloadEgprsMb ?? 0) + (x.PayloadGprsMb ?? 0)) / 1024),

                    // Traffic KPIs - Each has Line Chart + Stacked Area below
                    TrafficSdcchErl = BuildKpiChart("TRAFFIC SDCCH ERL", "Erl", sectorData, cellNames, x => x.SdcchTraf),
                    TrafficSdcchErlStacked = BuildStackedSingleChart("TRAFFIC SDCCH ERL", "Erl", sectorData, cellNames, x => x.SdcchTraf),
                    TrafficTchErl = BuildKpiChart("TRAFFIC TCH ERL", "Erl", sectorData, cellNames, x => x.TchTraf),
                    TrafficTchErlStacked = BuildStackedSingleChart("TRAFFIC TCH ERL", "Erl", sectorData, cellNames, x => x.TchTraf),
                    TrafficTotalErl = BuildKpiChart("TRAFFIC TOTAL ERL", "Erl", sectorData, cellNames, x => 
                        (x.TchTraf ?? 0) + (x.SdcchTraf ?? 0)),
                    TrafficTotalErlStacked = BuildStackedSingleChart("TRAFFIC TOTAL ERL", "Erl", sectorData, cellNames, x => 
                        (x.TchTraf ?? 0) + (x.SdcchTraf ?? 0))
                };

                response.Sectors.Add(sectorKpi);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching GSM Day KPI data");
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

            var query = _context.GsmDays.AsQueryable();
            query = query.Where(x => x.DateTime >= startDateTime && x.DateTime <= endDateTime);

            if (request.SiteIds != null && request.SiteIds.Count > 0)
                query = query.Where(x => x.SiteId != null && request.SiteIds.Contains(x.SiteId));

            if (request.Bands != null && request.Bands.Count > 0)
                query = query.Where(x => x.Band != null && request.Bands.Contains(x.Band));

            if (request.CellNames != null && request.CellNames.Count > 0)
                query = query.Where(x => x.CellName != null && request.CellNames.Contains(x.CellName));

            var rawData = await query
                .Select(x => new { x.DateTime, x.SiteId, x.Band, x.CellName, x.NeId, x.SectorGroup, x.TchTraf, x.PayloadMb })
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
                    
                    var dateGroups = groupList.GroupBy(x => ((DateTime?)x.DateTime)?.ToString("yyyy-MM-dd") ?? "");
                    
                    foreach(var dateGroup in dateGroups)
                    {
                        double? sum = dateGroup.Sum(x => (double?)valueSelector(x));
                        points.Add(new KpiDataPointDto { 
                            Date = dateGroup.Key, 
                            Value = sum.HasValue ? Math.Round(sum.Value, 2) : null
                        });
                    }
                    points = points.OrderBy(p => p.Date).ToList();
                    chart.Series.Add(new CellSeriesDto { CellName = seriesName, Data = points });
                }
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
                var sec1Data = sourceData.Where(x => x.SectorGroup == "1").GroupBy(keySelector);
                dto.Sector1 = BuildSeries($"{categoryName} - Sector 1", unit, sec1Data, g => g.Key ?? "Unknown", valueSelector);

                // Sector 2
                var sec2Data = sourceData.Where(x => x.SectorGroup == "2").GroupBy(keySelector);
                dto.Sector2 = BuildSeries($"{categoryName} - Sector 2", unit, sec2Data, g => g.Key ?? "Unknown", valueSelector);

                // Sector 3
                var sec3Data = sourceData.Where(x => x.SectorGroup == "3").GroupBy(keySelector);
                dto.Sector3 = BuildSeries($"{categoryName} - Sector 3", unit, sec3Data, g => g.Key ?? "Unknown", valueSelector);

                return dto;
            }

            // --- Payload Construction ---
            response.PayloadBySite = BuildAggregatedSet("Site", "MB", rawData, x => x.SiteId, x => x.PayloadMb);
            response.PayloadByBand = BuildAggregatedSet("Band", "MB", rawData, x => x.Band, x => x.PayloadMb);
            response.PayloadByCell = BuildAggregatedSet("Cell", "MB", rawData, x => x.CellName, x => x.PayloadMb);
            response.PayloadByNe   = BuildAggregatedSet("NE",   "MB", rawData, x => x.NeId, x => x.PayloadMb);

            // --- Traffic Construction ---
            response.TrafficBySite = BuildAggregatedSet("Site", "Erl", rawData, x => x.SiteId, x => x.TchTraf);
            response.TrafficByBand = BuildAggregatedSet("Band", "Erl", rawData, x => x.Band,   x => x.TchTraf);
            response.TrafficByCell = BuildAggregatedSet("Cell", "Erl", rawData, x => x.CellName, x => x.TchTraf);
            response.TrafficByNe   = BuildAggregatedSet("NE",   "Erl", rawData, x => x.NeId,   x => x.TchTraf);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching GSM Traffic Payload data");
            return StatusCode(500, new { error = "Failed to fetch Traffic Payload data" });
        }
    }

    /// <summary>
    /// Get filter options for GSM Day
    /// </summary>
    [HttpPost("filters")]
    public async Task<IActionResult> GetFilters([FromBody] LteDayKpiRequestDto request)
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

            // We use RefGsmDay for faster option retrieval
            var query = _context.RefGsmDays.AsQueryable();

            // Logic: 
            // 1. If SiteId is provided, filter Bands and Cells by that Site
            // 2. If no SiteId, maybe return top X sites? (Usually SiteId is typed by user)
            // The FE usually sends a partial site ID or empty.
            
            // For now, let's implement basic filtering similar to LTE approach IF it existed, 
            // or just simple retrieval.
            
            // Actually, in LteDayController we don't have a specific Filter endpoint shown in the provided snippet.
            // But the FE likely needs one. I will provide one.
            
            var siteIds = await query                      
                .Where(x => x.SiteId != null)
                .Select(x => x.SiteId!)
                .Distinct()
                .OrderBy(x => x)
                .Take(100)
                .ToListAsync();

            // ... more complex filter logic if needed (cascading filters)
            
            return Ok(new { siteIds }); 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching GSM filters");
            return StatusCode(500, new { error = "Failed to fetch filters" });
        }
    }


    private double? SafeDiv(double? num, double? denum)
    {
        if (!num.HasValue || !denum.HasValue || denum.Value == 0) return null;
        return num.Value / denum.Value;
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

    /// <summary>
    /// Build stacked payload chart with GPRS, EDGE, and Total series
    /// </summary>
    private KpiChartDto BuildStackedPayloadChart<T>(
        string name,
        List<T> data,
        List<string> cellNames) where T : class
    {
        var chart = new KpiChartDto
        {
            Name = name,
            Unit = "GB",
            Series = new List<CellSeriesDto>()
        };

        var dateTimeProp = typeof(T).GetProperty("DateTime");
        var payloadMbProp = typeof(T).GetProperty("PayloadMb");
        var payloadGprsProp = typeof(T).GetProperty("PayloadGprsMb");
        var payloadEgprsProp = typeof(T).GetProperty("PayloadEgprsMb");

        if (dateTimeProp == null) return chart;

        // Group by date and aggregate
        var dateGroups = data
            .GroupBy(x => ((DateTime?)dateTimeProp.GetValue(x))?.ToString("yyyy-MM-dd") ?? "")
            .OrderBy(g => g.Key);

        // GPRS Series
        var gprsData = dateGroups.Select(g => new KpiDataPointDto
        {
            Date = g.Key,
            Value = payloadGprsProp != null 
                ? Math.Round(g.Sum(x => ((double?)payloadGprsProp.GetValue(x) ?? 0) / 1024), 2) 
                : null
        }).ToList();
        chart.Series.Add(new CellSeriesDto { CellName = "GPRS", Data = gprsData });

        // EDGE Series
        var edgeData = dateGroups.Select(g => new KpiDataPointDto
        {
            Date = g.Key,
            Value = payloadEgprsProp != null 
                ? Math.Round(g.Sum(x => ((double?)payloadEgprsProp.GetValue(x) ?? 0) / 1024), 2) 
                : null
        }).ToList();
        chart.Series.Add(new CellSeriesDto { CellName = "EDGE", Data = edgeData });

        // Total Series
        var totalData = dateGroups.Select(g => new KpiDataPointDto
        {
            Date = g.Key,
            Value = Math.Round(g.Sum(x => 
                (((double?)payloadMbProp?.GetValue(x) ?? 0) + 
                 ((double?)payloadGprsProp?.GetValue(x) ?? 0) + 
                 ((double?)payloadEgprsProp?.GetValue(x) ?? 0)) / 1024), 2)
        }).ToList();
        chart.Series.Add(new CellSeriesDto { CellName = "Total", Data = totalData });

        return chart;
    }

    /// <summary>
    /// Build stacked traffic chart with TCH and SDCCH series
    /// </summary>
    private KpiChartDto BuildStackedTrafficChart<T>(
        string name,
        List<T> data,
        List<string> cellNames) where T : class
    {
        var chart = new KpiChartDto
        {
            Name = name,
            Unit = "Erl",
            Series = new List<CellSeriesDto>()
        };

        var dateTimeProp = typeof(T).GetProperty("DateTime");
        var tchTrafProp = typeof(T).GetProperty("TchTraf");
        var sdcchTrafProp = typeof(T).GetProperty("SdcchTraf");

        if (dateTimeProp == null) return chart;

        // Group by date and aggregate
        var dateGroups = data
            .GroupBy(x => ((DateTime?)dateTimeProp.GetValue(x))?.ToString("yyyy-MM-dd") ?? "")
            .OrderBy(g => g.Key);

        // TCH Series
        var tchData = dateGroups.Select(g => new KpiDataPointDto
        {
            Date = g.Key,
            Value = tchTrafProp != null 
                ? Math.Round(g.Sum(x => (double?)tchTrafProp.GetValue(x) ?? 0), 2) 
                : null
        }).ToList();
        chart.Series.Add(new CellSeriesDto { CellName = "TCH", Data = tchData });

        // SDCCH Series
        var sdcchData = dateGroups.Select(g => new KpiDataPointDto
        {
            Date = g.Key,
            Value = sdcchTrafProp != null 
                ? Math.Round(g.Sum(x => (double?)sdcchTrafProp.GetValue(x) ?? 0), 2) 
                : null
        }).ToList();
        chart.Series.Add(new CellSeriesDto { CellName = "SDCCH", Data = sdcchData });

        // Total Series
        var totalData = dateGroups.Select(g => new KpiDataPointDto
        {
            Date = g.Key,
            Value = Math.Round(g.Sum(x => 
                ((double?)tchTrafProp?.GetValue(x) ?? 0) + 
                ((double?)sdcchTrafProp?.GetValue(x) ?? 0)), 2)
        }).ToList();
        chart.Series.Add(new CellSeriesDto { CellName = "Total", Data = totalData });

        return chart;
    }

    /// <summary>
    /// Build a stacked area chart for a single KPI (per-cell series for stacking)
    /// </summary>
    private KpiChartDto BuildStackedSingleChart<T>(
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

        var dateTimeProp = typeof(T).GetProperty("DateTime");
        var cellNameProp = typeof(T).GetProperty("CellName");

        if (dateTimeProp == null || cellNameProp == null) return chart;

        // Create per-cell series for stacked area chart
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
