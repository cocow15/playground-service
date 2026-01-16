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

            // Helper to parse strings safely (e.g. "", "0.23", null)
            double? ParseDouble(string? val)
            {
                if (string.IsNullOrWhiteSpace(val)) return null;
                if (double.TryParse(val, out var d)) return d;
                return null;
            }

            var response = new PacketLossResponseDto();

            // Group by NE for series
            var groupedByNe = rawData.GroupBy(x => x.NeName).ToList();

            // 1. TWAMP PL AVG
            var avgChart = new KpiChartDto { Name = "TWAMP PL AVG", Unit = "%" };
            foreach (var group in groupedByNe)
            {
                var points = group
                    .GroupBy(x => x.Date)
                    .Select(g => new KpiDataPointDto
                    {
                        Date = g.Key,
                        Value = g.Average(x => ParseDouble(x.TwampPlAvg))
                    })
                    .OrderBy(p => p.Date)
                    .ToList();

                avgChart.Series.Add(new CellSeriesDto { CellName = group.Key, Data = points });
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
                        Date = g.Key,
                        Value = g.Max(x => ParseDouble(x.TwampPlMax))
                    })
                    .OrderBy(p => p.Date)
                    .ToList();

                maxChart.Series.Add(new CellSeriesDto { CellName = group.Key, Data = points });
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
                        Date = g.Key,
                        Value = g.Average(x => ParseDouble(x.SctpPacketLoss)) // Assuming average is okay for multiple entries per day?
                    })
                    .OrderBy(p => p.Date)
                    .ToList();

                sctpChart.Series.Add(new CellSeriesDto { CellName = group.Key, Data = points });
            }
            response.SctpPacketLoss = sctpChart;

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Packet Loss data");
            return StatusCode(500, new { error = "Failed to fetch Packet Loss data" });
        }
    }
}

public class PacketLossFilterDto
{
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public List<string> NeNames { get; set; } = new();
}

public class PacketLossResponseDto
{
    public KpiChartDto TwampPlAvg { get; set; } = new();
    public KpiChartDto TwampPlMax { get; set; } = new();
    public KpiChartDto SctpPacketLoss { get; set; } = new();
}
