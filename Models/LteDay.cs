namespace AuthService.Models;

/// <summary>
/// Model for lte_day table
/// </summary>
public class LteDay
{
    public DateTime? DateTime { get; set; }
    public string? SiteId { get; set; }
    public string? Ne { get; set; }
    public string? NeId { get; set; }
    public string? Suffix { get; set; }
    public string? Band { get; set; }
    public int? Sector { get; set; }
    public int? SectorGroup { get; set; }
    public string? CellName { get; set; }
    public double? Avail { get; set; }
    public double? Erab { get; set; }
    public double? Rrc { get; set; }
    public double? Sssr { get; set; }
    public double? Sar { get; set; }
    public double? IntraFho { get; set; }
    public double? PmHoExeAttLteIntraF { get; set; }
    public double? InterFho { get; set; }
    public double? PmHoExeAttLteInterF { get; set; }
    public double? DlUtil { get; set; }
    public double? UlUtil { get; set; }
    public double? PrbMaxDl { get; set; }
    public double? PrbMaxUl { get; set; }
    public double? PrbMax { get; set; }
    public double? AvgCqi { get; set; }
    public double? Se { get; set; }
    public double? UserDlThp { get; set; }
    public double? UserUlThp { get; set; }
    public double? CellDlThp { get; set; }
    public double? CellUlThp { get; set; }
    public double? MaxDlThpMbps { get; set; }
    public double? MaxUlThpMbps { get; set; }
    public double? LatencyMs { get; set; }
    public double? UlPucch { get; set; }
    public double? UlRssiDbm { get; set; }
    public double? UlPacketLoss { get; set; }
    public double? DlPacketLoss { get; set; }
    public double? MaxRrcUser { get; set; }
    public double? MaxActiveUser { get; set; }
    public double? Csfb { get; set; }
    public double? DlVolMb { get; set; }
    public double? UlVolMb { get; set; }
    public double? PayloadMb { get; set; }
    public double? VoltePayDl { get; set; }
    public double? VoltePayUl { get; set; }
    public double? PayloadGb { get; set; }
    public double? TrafficErl { get; set; }
}
