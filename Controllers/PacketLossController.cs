using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.DTOs;

namespace AuthService.Controllers;

[Authorize]
[ApiController]
[Route("api/packet-loss")]
public class PacketLossController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PacketLossController> _logger;

    public PacketLossController(ApplicationDbContext context, ILogger<PacketLossController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("ne-names")]
    public async Task<IActionResult> GetNeNames([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = _context.RefNePacketLosses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.ToLower();
                query = query.Where(x => x.NeName.ToLower().Contains(searchTerm));
            }

            var neNames = await query
                .Select(x => x.NeName)
                .OrderBy(x => x)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(neNames);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching NE names");
            return StatusCode(500, new { error = "Failed to fetch NE names" });
        }
    }

    [HttpPost("data")]
    public async Task<IActionResult> GetPacketLoss([FromBody] PacketLossFilterDto request)
    {
        try
        {
            // Ensure dates are in YYYY-MM-DD format for string comparison
            if (string.IsNullOrWhiteSpace(request.StartDate) || string.IsNullOrWhiteSpace(request.EndDate))
            {
                return BadRequest(new { error = "Start Date and End Date are required." });
            }
            
            var startDate = request.StartDate;
            var endDate = request.EndDate;

            var query = _context.PacketLosses.AsQueryable();

            query = query.Where(x => x.Date.CompareTo(startDate) >= 0 && x.Date.CompareTo(endDate) <= 0);

            if (request.NeNames != null && request.NeNames.Count > 0)
            {
                query = query.Where(x => request.NeNames.Contains(x.NeName));
            }

            if (request.Mode?.ToLower() == "hourly")
            {
                 var hourlyQuery = _context.PacketLossHourlies.AsQueryable();

                // complex "hourly" logic
                // StartDate/EndDate come as "YYYY-MM-DD HH:00" or just "YYYY-MM-DD" depending...
                // But FE sends "YYYY-MM-DD HH:00" for hourly.
                
                DateTime sDt, eDt;
                // Fallback parsing
                if (!DateTime.TryParse(request.StartDate, out sDt)) sDt = DateTime.Parse(request.StartDate.Substring(0, 10));
                if (!DateTime.TryParse(request.EndDate, out eDt)) eDt = DateTime.Parse(request.EndDate.Substring(0, 10));

                var sDateStr = sDt.ToString("yyyy-MM-dd");
                var sHour = sDt.Hour;
                
                var eDateStr = eDt.ToString("yyyy-MM-dd");
                var eHour = eDt.Hour;

                // Filter logic:
                // (Date > sDate OR (Date == sDate AND Hour >= sHour))
                // AND
                // (Date < eDate OR (Date == eDate AND Hour <= eHour))
                
                // Note: HourId is string in DB? Model says 'string'.
                // If it contains "9", "10", "0", etc. we need integer comparison.
                // EF Core translation for Convert.ToInt32(x.HourId) depends on provider.
                // Assuming standard numeric string.
                
                hourlyQuery = hourlyQuery.Where(x => 
                    (x.Date.CompareTo(sDateStr) > 0 || (x.Date == sDateStr && Convert.ToInt32(x.HourId) >= sHour)) &&
                    (x.Date.CompareTo(eDateStr) < 0 || (x.Date == eDateStr && Convert.ToInt32(x.HourId) <= eHour))
                );

                if (request.NeNames != null && request.NeNames.Count > 0)
                {
                    hourlyQuery = hourlyQuery.Where(x => request.NeNames.Contains(x.NeName));
                }

                var rawHourlyData = await hourlyQuery
                    .Select(x => new
                    {
                        x.Date,
                        x.HourId,
                        x.NeName,
                        x.TwampPlAvg,
                        x.TwampPlMax,
                        x.SctpPacketLoss
                    })
                    .ToListAsync();

                var processedData = rawHourlyData
                    .Select(x => new 
                    {
                        Date = $"{x.Date} {x.HourId.PadLeft(2, '0')}:00", 
                        x.NeName,
                        x.TwampPlAvg,
                        x.TwampPlMax,
                        x.SctpPacketLoss
                    })
                    .OrderBy(x => x.Date)
                    .ToList();
                
                 var mappedData = processedData.Select(x => new 
                 {
                     x.Date,
                     x.NeName,
                     x.TwampPlAvg,
                     x.TwampPlMax,
                     x.SctpPacketLoss
                 }).ToList();

                 return ProcessPacketLossResponse(mappedData);
            }
            else
            {
                var rawData = await query
                    .Select(x => new
                    {
                        x.Date,
                        x.NeName,
                        x.TwampPlAvg,
                        x.TwampPlMax,
                        x.SctpPacketLoss
                    })
                    .OrderBy(x => x.Date)
                    .ToListAsync();

                return ProcessPacketLossResponse(rawData);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Packet Loss data");
            return StatusCode(500, new { error = "Failed to fetch Packet Loss data" });
        }
    }

    private IActionResult ProcessPacketLossResponse<T>(List<T> rawData) where T : class
    {
            double? ParseDouble(string? val)
            {
                if (string.IsNullOrWhiteSpace(val)) return null;
                if (double.TryParse(val, out var d)) return d;
                return null;
            }
            
            var standardData = rawData.Select(x => {
                var type = x.GetType();
                return new 
                {
                    Date = (type.GetProperty("Date")?.GetValue(x) as string) ?? string.Empty,
                    NeName = (type.GetProperty("NeName")?.GetValue(x) as string) ?? string.Empty,
                    TwampPlAvg = type.GetProperty("TwampPlAvg")?.GetValue(x) as string,
                    TwampPlMax = type.GetProperty("TwampPlMax")?.GetValue(x) as string,
                    SctpPacketLoss = type.GetProperty("SctpPacketLoss")?.GetValue(x) as string
                };
            }).ToList();

            var response = new PacketLossResponseDto();
            var groupedByNe = standardData.GroupBy(x => x.NeName).ToList();

            // 1. TWAMP PL AVG
            var avgChart = new KpiChartDto { Name = "TWAMP PL AVG", Unit = "%" };
            foreach (var group in groupedByNe)
            {
                var points = group
                    .GroupBy(x => x.Date)
                    .Select(g => new KpiDataPointDto
                    {
                        Date = g.Key ?? string.Empty,
                        Value = g.Average(x => ParseDouble(x.TwampPlAvg))
                    })
                    .OrderBy(p => p.Date)
                    .ToList();

                avgChart.Series.Add(new CellSeriesDto { CellName = group.Key ?? string.Empty, Data = points });
            }
            response.TwampPlAvg = avgChart;

            // 2. TWAMP PL MAX
            var maxChart = new KpiChartDto { Name = "TWAMP PL MAX", Unit = "%" };
            foreach (var group in groupedByNe)
            {
                var points = group
                    .GroupBy(x => x.Date)
                    .Select(g => new KpiDataPointDto
                    {
                        Date = g.Key ?? string.Empty,
                        Value = g.Max(x => ParseDouble(x.TwampPlMax))
                    })
                    .OrderBy(p => p.Date)
                    .ToList();

                maxChart.Series.Add(new CellSeriesDto { CellName = group.Key ?? string.Empty, Data = points });
            }
            response.TwampPlMax = maxChart;

            // 3. SCTP PACKET LOSS
            var sctpChart = new KpiChartDto { Name = "SCTP PACKET LOSS", Unit = "%" };
            foreach (var group in groupedByNe)
            {
                var points = group
                    .GroupBy(x => x.Date)
                    .Select(g => new KpiDataPointDto
                    {
                        Date = g.Key ?? string.Empty,
                        Value = g.Average(x => ParseDouble(x.SctpPacketLoss)) 
                    })
                    .OrderBy(p => p.Date)
                    .ToList();

                sctpChart.Series.Add(new CellSeriesDto { CellName = group.Key ?? string.Empty, Data = points });
            }
            response.SctpPacketLoss = sctpChart;

            return Ok(response);
    }
}

public class PacketLossFilterDto
{
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public List<string> NeNames { get; set; } = new();
    public string Mode { get; set; } = "daily"; // "daily" or "hourly"
}

public class PacketLossResponseDto
{
    public KpiChartDto TwampPlAvg { get; set; } = new();
    public KpiChartDto TwampPlMax { get; set; } = new();
    public KpiChartDto SctpPacketLoss { get; set; } = new();
}
