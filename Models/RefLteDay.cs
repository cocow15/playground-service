namespace AuthService.Models;

/// <summary>
/// Model for mv_ref_lte_day materialized view
/// </summary>
public class RefLteDay
{
    public string SiteId { get; set; } = string.Empty;
    public string NeId { get; set; } = string.Empty;
    public string Band { get; set; } = string.Empty;
    public string CellName { get; set; } = string.Empty;
}
