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
            if (!string.IsNullOrWhiteSpace(request.SiteId))
            {
                query = query.Where(x => x.SiteId == request.SiteId);
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

    private static KpiChartDto BuildKpiChart<T>(
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

        // Get DateTime and CellName using reflection (simplified approach)
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
