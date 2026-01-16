namespace AuthService.DTOs;

/// <summary>
/// Response DTO for filter options
/// </summary>
public class FilterOptionsDto
{
    public List<string> Options { get; set; } = new();
}

/// <summary>
/// Request DTO for site_id filter with optional search
/// </summary>
public class SiteIdFilterRequestDto
{
    public string? Keyword { get; set; }
}

/// <summary>
/// Request DTO for band filter (accepts single site_id with optional search)
/// </summary>
public class BandFilterRequestDto
{
    public string? SiteId { get; set; }
    public string? Keyword { get; set; }
}

/// <summary>
/// Request DTO for cellname filter (accepts single site_id and multiple bands with optional search)
/// </summary>
public class CellNameFilterRequestDto
{
    public string? SiteId { get; set; }
    public List<string> Bands { get; set; } = new();
    public string? Keyword { get; set; }
}

