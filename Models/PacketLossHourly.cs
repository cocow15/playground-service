using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models;

[Table("packet_loss_hourly", Schema = "dashboard")]
public class PacketLossHourly
{
    [Column("date_id")]
    public string Date { get; set; } = string.Empty;

    [Column("hour_id")]
    public string HourId { get; set; } = string.Empty;

    [Column("ne_name")]
    public string NeName { get; set; } = string.Empty;

    [Column("packetlossratiofwd")]
    public string? PacketLossRatioFwd { get; set; }

    [Column("packetlossratiorev")]
    public string? PacketLossRatioRev { get; set; }

    [Column("twamp_latency_avg")]
    public string? TwampLatencyAvg { get; set; }

    [Column("twamp_pl_avg")]
    public string? TwampPlAvg { get; set; }

    [Column("twamp_pl_max")]
    public string? TwampPlMax { get; set; }

    [Column("sctp_packet_loss")]
    public string? SctpPacketLoss { get; set; }

    [Column("source_file")]
    public string? SourceFile { get; set; }

    [Column("ingested_at")]
    public DateTime? IngestedAt { get; set; }
}
