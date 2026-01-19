using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.DTOs;

namespace AuthService.Controllers;

[ApiController]
[Route("api/gsm-filter")]
public class GsmFilterController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GsmFilterController> _logger;

    public GsmFilterController(ApplicationDbContext context, ILogger<GsmFilterController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all distinct site_ids for GSM
    /// </summary>
    [HttpGet("site-ids")]
    public async Task<IActionResult> GetSiteIds([FromQuery] string? keyword = null)
    {
        try
        {
            var query = _context.RefGsmDays
                .Where(x => x.SiteId != null)
                .Select(x => x.SiteId!)
                .Distinct();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.ToLower().Contains(keyword.ToLower()));
            }

            var siteIds = await query
                .OrderBy(x => x)
                .ToListAsync();

            return Ok(new FilterOptionsDto { Options = siteIds });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching GSM site_ids");
            return StatusCode(500, new { error = "Failed to fetch GSM site_ids" });
        }
    }

    /// <summary>
    /// Get GSM bands filtered by selected site_id
    /// </summary>
    [HttpPost("bands")]
    public async Task<IActionResult> GetBands([FromBody] BandFilterRequestDto request)
    {
        try
        {
            var query = _context.RefGsmDays.AsQueryable();

            if (request.SiteIds != null && request.SiteIds.Count > 0)
            {
                query = query.Where(x => x.SiteId != null && request.SiteIds.Contains(x.SiteId));
            }

            var bandQuery = query
                .Where(x => x.Band != null)
                .Select(x => x.Band!)
                .Distinct();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                bandQuery = bandQuery.Where(x => x.ToLower().Contains(request.Keyword.ToLower()));
            }

            var bands = await bandQuery
                .OrderBy(x => x)
                .ToListAsync();

            return Ok(new FilterOptionsDto { Options = bands });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching GSM bands");
            return StatusCode(500, new { error = "Failed to fetch GSM bands" });
        }
    }

    /// <summary>
    /// Get GSM cell names filtered by selected site_id and bands
    /// </summary>
    [HttpPost("cellnames")]
    public async Task<IActionResult> GetCellNames([FromBody] CellNameFilterRequestDto request)
    {
        try
        {
            var query = _context.RefGsmDays.AsQueryable();

            if (request.SiteIds != null && request.SiteIds.Count > 0)
            {
                query = query.Where(x => x.SiteId != null && request.SiteIds.Contains(x.SiteId));
            }

            if (request.Bands != null && request.Bands.Count > 0)
            {
                query = query.Where(x => x.Band != null && request.Bands.Contains(x.Band));
            }

            var cellNameQuery = query
                .Where(x => x.CellName != null)
                .Select(x => x.CellName!)
                .Distinct();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                cellNameQuery = cellNameQuery.Where(x => x.ToLower().Contains(request.Keyword.ToLower()));
            }

            var cellNames = await cellNameQuery
                .OrderBy(x => x)
                .ToListAsync();

            return Ok(new FilterOptionsDto { Options = cellNames });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching GSM cell names");
            return StatusCode(500, new { error = "Failed to fetch GSM cell names" });
        }
    }
}
