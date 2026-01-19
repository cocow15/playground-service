using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models;

[Table("mv_ref_2g_day", Schema = "dashboard")]
public class RefGsmDay
{
    [Column("site_id")]
    public string? SiteId { get; set; }

    [Column("neid")]
    public string? NeId { get; set; }

    [Column("band")]
    public string? Band { get; set; }

    [Column("cellname")]
    public string? CellName { get; set; }
}
