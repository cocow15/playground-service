using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.DTOs;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilterController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FilterController> _logger;

    public FilterController(ApplicationDbContext context, ILogger<FilterController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all distinct site_ids (first level filter) with optional keyword search
    /// </summary>
    [HttpGet("site-ids")]
    public async Task<IActionResult> GetSiteIds([FromQuery] string? keyword = null)
    {
        try
        {
            var query = _context.RefLteDays
                .Select(x => x.SiteId)
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
            _logger.LogError(ex, "Error fetching site_ids");
            return StatusCode(500, new { error = "Failed to fetch site_ids" });
        }
    }

    /// <summary>
    /// Get bands filtered by selected site_id (second level filter) with optional keyword search
    /// </summary>
    [HttpPost("bands")]
    public async Task<IActionResult> GetBands([FromBody] BandFilterRequestDto request)
    {
        try
        {
            var query = _context.RefLteDays.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SiteId))
            {
                query = query.Where(x => x.SiteId == request.SiteId);
            }

            var bandQuery = query.Select(x => x.Band).Distinct();

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
            _logger.LogError(ex, "Error fetching bands");
            return StatusCode(500, new { error = "Failed to fetch bands" });
        }
    }

    /// <summary>
    /// Get cell names filtered by selected site_id and bands (third level filter) with optional keyword search
    /// </summary>
    [HttpPost("cellnames")]
    public async Task<IActionResult> GetCellNames([FromBody] CellNameFilterRequestDto request)
    {
        try
        {
            var query = _context.RefLteDays.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SiteId))
            {
                query = query.Where(x => x.SiteId == request.SiteId);
            }

            if (request.Bands != null && request.Bands.Count > 0)
            {
                query = query.Where(x => request.Bands.Contains(x.Band));
            }

            var cellNameQuery = query.Select(x => x.CellName).Distinct();

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
            _logger.LogError(ex, "Error fetching cell names");
            return StatusCode(500, new { error = "Failed to fetch cell names" });
        }
    }
}
