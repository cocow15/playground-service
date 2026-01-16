using Microsoft.EntityFrameworkCore;
using AuthService.Models;

namespace AuthService.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<RefLteDay> RefLteDays { get; set; }
    public DbSet<LteDay> LteDays { get; set; }
    public DbSet<PacketLoss> PacketLosses { get; set; }
    public DbSet<RefNePacketLoss> RefNePacketLosses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure schema
        modelBuilder.HasDefaultSchema("dashboard");

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users", "dashboard");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);
            
            entity.Property(e => e.PasswordHash)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // One-to-many relationship with RefreshTokens
            entity.HasMany(e => e.RefreshTokens)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens", "dashboard");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();
            
            entity.Property(e => e.Token)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Ignore computed properties
            entity.Ignore(e => e.IsRevoked);
            entity.Ignore(e => e.IsExpired);
            entity.Ignore(e => e.IsActive);
        });

        // RefLteDay configuration (materialized view)
        modelBuilder.Entity<RefLteDay>(entity =>
        {
            entity.ToTable("mv_ref_lte_day", "dashboard");
            entity.HasNoKey(); // Materialized view has no primary key
            
            entity.Property(e => e.SiteId)
                .HasColumnName("site_id");
            
            entity.Property(e => e.NeId)
                .HasColumnName("neid");
            
            entity.Property(e => e.Band)
                .HasColumnName("band");

            entity.Property(e => e.CellName)
                .HasColumnName("cellname");
        });

        // LteDay configuration (lte_day table)
        modelBuilder.Entity<LteDay>(entity =>
        {
            entity.ToTable("lte_day", "dashboard");
            entity.HasNoKey();

            entity.Property(e => e.DateTime).HasColumnName("DATETIME");
            entity.Property(e => e.SiteId).HasColumnName("SITE_ID");
            entity.Property(e => e.Ne).HasColumnName("NE");
            entity.Property(e => e.NeId).HasColumnName("NEID");
            entity.Property(e => e.Suffix).HasColumnName("SUFFIX");
            entity.Property(e => e.Band).HasColumnName("BAND");
            entity.Property(e => e.Sector).HasColumnName("SECTOR");
            entity.Property(e => e.SectorGroup).HasColumnName("SECTORGROUP");
            entity.Property(e => e.CellName).HasColumnName("CELLNAME");
            entity.Property(e => e.Avail).HasColumnName("AVAIL");
            entity.Property(e => e.Erab).HasColumnName("E-RAB");
            entity.Property(e => e.Rrc).HasColumnName("RRC");
            entity.Property(e => e.Sssr).HasColumnName("SSSR");
            entity.Property(e => e.Sar).HasColumnName("SAR");
            entity.Property(e => e.IntraFho).HasColumnName("INTRA-FHO");
            entity.Property(e => e.PmHoExeAttLteIntraF).HasColumnName("pmhoexeattlteintraf");
            entity.Property(e => e.InterFho).HasColumnName("INTER-FHO");
            entity.Property(e => e.PmHoExeAttLteInterF).HasColumnName("pmhoexeattlteinterf");
            entity.Property(e => e.DlUtil).HasColumnName("DL UTIL");
            entity.Property(e => e.UlUtil).HasColumnName("UL UTIL");
            entity.Property(e => e.PrbMaxDl).HasColumnName("PRB_MAX_DL");
            entity.Property(e => e.PrbMaxUl).HasColumnName("PRB_MAX_UL");
            entity.Property(e => e.PrbMax).HasColumnName("PRB_MAX");
            entity.Property(e => e.AvgCqi).HasColumnName("AVG CQI");
            entity.Property(e => e.Se).HasColumnName("SE");
            entity.Property(e => e.UserDlThp).HasColumnName("USER DL THP");
            entity.Property(e => e.UserUlThp).HasColumnName("USER UL THP");
            entity.Property(e => e.CellDlThp).HasColumnName("CELL DL THP");
            entity.Property(e => e.CellUlThp).HasColumnName("CELL UL THP");
            entity.Property(e => e.MaxDlThpMbps).HasColumnName("MAX DL THP MBPS");
            entity.Property(e => e.MaxUlThpMbps).HasColumnName("MAX UL THP MBPS");
            entity.Property(e => e.LatencyMs).HasColumnName("LATENCY MS");
            entity.Property(e => e.UlPucch).HasColumnName("UL PUCCH");
            entity.Property(e => e.UlRssiDbm).HasColumnName("UL RSSI DBM");
            entity.Property(e => e.UlPacketLoss).HasColumnName("UL PACKET LOSS");
            entity.Property(e => e.DlPacketLoss).HasColumnName("DL PACKET LOSS");
            entity.Property(e => e.MaxRrcUser).HasColumnName("MAX RRC USER");
            entity.Property(e => e.MaxActiveUser).HasColumnName("MAX ACTIVE USER");
            entity.Property(e => e.Csfb).HasColumnName("CSFB");
            entity.Property(e => e.DlVolMb).HasColumnName("DL VOL MB");
            entity.Property(e => e.UlVolMb).HasColumnName("UL VOL MB");
            entity.Property(e => e.PayloadMb).HasColumnName("PAYLOAD MB");
            entity.Property(e => e.VoltePayDl).HasColumnName("VOLTE PAY DL");
            entity.Property(e => e.VoltePayUl).HasColumnName("VOLTE PAY UL");
            entity.Property(e => e.PayloadGb).HasColumnName("PAYLOAD GB");
            entity.Property(e => e.TrafficErl).HasColumnName("TRAFFIC ERL");
        });

        // PacketLoss configuration
        modelBuilder.Entity<PacketLoss>(entity =>
        {
            entity.ToTable("packet_loss", "dashboard");
            entity.HasNoKey(); 

            entity.Property(e => e.Date).HasColumnName("date_id");
            entity.Property(e => e.NeName).HasColumnName("ne_name");
            entity.Property(e => e.PacketLossRatioFwd).HasColumnName("packetlossratiofwd");
            entity.Property(e => e.PacketLossRatioRev).HasColumnName("packetlossratiorev");
            entity.Property(e => e.TwampLatencyAvg).HasColumnName("twamp_latency_avg");
            entity.Property(e => e.TwampPlAvg).HasColumnName("twamp_pl_avg");
            entity.Property(e => e.TwampPlMax).HasColumnName("twamp_pl_max");
            entity.Property(e => e.SctpPacketLoss).HasColumnName("sctp_packet_loss");
            entity.Property(e => e.SourceFile).HasColumnName("source_file");
            entity.Property(e => e.IngestedAt).HasColumnName("ingested_at");
        });

        // RefNePacketLoss configuration (materialized view)
        modelBuilder.Entity<RefNePacketLoss>(entity =>
        {
            entity.ToTable("mv_ref_ne_packet_loss", "dashboard");
            entity.HasNoKey();
            
            entity.Property(e => e.NeName).HasColumnName("ne_name");
        });
    }
}
