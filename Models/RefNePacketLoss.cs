using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models;

[Table("mv_ref_ne_packet_loss", Schema = "dashboard")]
public class RefNePacketLoss
{
    [Column("ne_name")]
    public string NeName { get; set; } = string.Empty;
}
