using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models;

[Table("2G_daily_kpi", Schema = "public")]
public class GsmDay
{
    [Column("DATETIME")]
    public DateTime? DateTime { get; set; }

    [Column("SITE_ID")]
    public string? SiteId { get; set; }

    [Column("CELL SUFFIX EID")]
    public string? CellSuffixEid { get; set; }

    [Column("BAND")]
    public string? Band { get; set; }

    [Column("NE")]
    public string? Ne { get; set; }

    [Column("SUFFIX")]
    public string? Suffix { get; set; }

    [Column("NEID")]
    public string? NeId { get; set; }

    [Column("CELL")]
    public string? Cell { get; set; }

    [Column("SECTOR")]
    public string? Sector { get; set; }

    [Column("SECTORGROUP")]
    public string? SectorGroup { get; set; } // Kept as string to match SQL definition varchar(50)

    [Column("cell_name")]
    public string? CellName { get; set; }

    [Column("AVAIL_NUM")]
    public double? AvailNum { get; set; }

    [Column("AVAIL_DENUM")]
    public double? AvailDenum { get; set; }

    [Column("SDSR_NUM")]
    public double? SdsrNum { get; set; }

    [Column("SDSR_DENUM")]
    public double? SdsrDenum { get; set; }

    [Column("S_CONG_NUM")]
    public double? SCongNum { get; set; }

    [Column("S_CONG_DENUM")]
    public double? SCongDenum { get; set; }

    [Column("T_CONG_NUM")]
    public double? TCongNum { get; set; }

    [Column("T_CONG_DENUM")]
    public double? TCongDenum { get; set; }

    [Column("TCH_BLOCK")]
    public double? TchBlock { get; set; }

    [Column("HOSR_NUM")]
    public double? HosrNum { get; set; }

    [Column("HOSR_DENUM")]
    public double? HosrDenum { get; set; }

    [Column("TCH_DROP")]
    public double? TchDrop { get; set; }

    [Column("SDCCH_TRAF")]
    public double? SdcchTraf { get; set; }

    [Column("TCH_TRAF")]
    public double? TchTraf { get; set; }

    [Column("PAYLOAD_GPRS_MB")]
    public double? PayloadGprsMb { get; set; }

    [Column("PAYLOAD_EGPRS_MB")]
    public double? PayloadEgprsMb { get; set; }

    [Column("PAYLOAD_MB")]
    public double? PayloadMb { get; set; }

    [Column("SDCCH_SR")]
    public double? SdcchSr { get; set; }

    [Column("TBF_COMP_NUM")]
    public double? TbfCompNum { get; set; }

    [Column("TBF_COMP_DENUM")]
    public double? TbfCompDenum { get; set; }

    [Column("TBF_DL_EST_NUM")]
    public double? TbfDlEstNum { get; set; }

    [Column("TBF_DL_EST_DENUM")]
    public double? TbfDlEstDenum { get; set; }

    [Column("TBF_DL_SR_NUM")]
    public double? TbfDlSrNum { get; set; }

    [Column("TBF_DL_SR_DENUM")]
    public double? TbfDlSrDenum { get; set; }

    [Column("ICH_1_NUM")]
    public double? Ich1Num { get; set; }

    [Column("ICH_1_DENUM")]
    public double? Ich1Denum { get; set; }

    [Column("ICH_2_NUM")]
    public double? Ich2Num { get; set; }

    [Column("ICH_2_DENUM")]
    public double? Ich2Denum { get; set; }

    [Column("ICH_3_NUM")]
    public double? Ich3Num { get; set; }

    [Column("ICH_3_DENUM")]
    public double? Ich3Denum { get; set; }

    [Column("ICH_4_NUM")]
    public double? Ich4Num { get; set; }

    [Column("ICH_4_DENUM")]
    public double? Ich4Denum { get; set; }

    [Column("ICH_5_NUM")]
    public double? Ich5Num { get; set; }

    [Column("ICH_5_DENUM")]
    public double? Ich5Denum { get; set; }

    [Column("TBF_UL_EST_NUM")]
    public double? TbfUlEstNum { get; set; }

    [Column("TBF_UL_EST_DENUM")]
    public double? TbfUlEstDenum { get; set; }

    [Column("ingested_at")]
    public DateTime? IngestedAt { get; set; }
}
