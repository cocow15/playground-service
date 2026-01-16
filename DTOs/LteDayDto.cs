namespace AuthService.DTOs;

/// <summary>
/// Request DTO for LTE Day KPI data
/// </summary>
public class LteDayKpiRequestDto
{
    public List<string> SiteIds { get; set; } = new();
    public List<string> Bands { get; set; } = new();
    public List<string> CellNames { get; set; } = new();
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for a single KPI data point
/// </summary>
public class KpiDataPointDto
{
    public string Date { get; set; } = string.Empty;
    public double? Value { get; set; }
}

/// <summary>
/// Response DTO for a cell series (one line in chart)
/// </summary>
public class CellSeriesDto
{
    public string CellName { get; set; } = string.Empty;
    public List<KpiDataPointDto> Data { get; set; } = new();
}

/// <summary>
/// Response DTO for a single KPI with multiple cell series
/// </summary>
public class KpiChartDto
{
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public List<CellSeriesDto> Series { get; set; } = new();
}

/// <summary>
/// Response DTO for a sector group containing all KPIs
/// </summary>
public class SectorKpiDto
{
    public int SectorGroup { get; set; }
    
    // Existing KPIs
    public KpiChartDto Availability { get; set; } = new();
    public KpiChartDto Erab { get; set; } = new();
    public KpiChartDto Rrc { get; set; } = new();
    public KpiChartDto Sssr { get; set; } = new();
    public KpiChartDto Sar { get; set; } = new();
    public KpiChartDto IntraFho { get; set; } = new();
    public KpiChartDto InterFho { get; set; } = new();
    public KpiChartDto DlUtil { get; set; } = new();
    public KpiChartDto UlUtil { get; set; } = new();
    
    // New KPIs - below UL Utilization
    public KpiChartDto PrbMaxDl { get; set; } = new();
    public KpiChartDto PrbMaxUl { get; set; } = new();
    public KpiChartDto Cqi { get; set; } = new();
    public KpiChartDto Se { get; set; } = new();
    
    // Existing KPIs - User Throughput
    public KpiChartDto UserDlThp { get; set; } = new();
    public KpiChartDto UserUlThp { get; set; } = new();
    
    // New KPIs - below User UL THP
    public KpiChartDto CellDlThp { get; set; } = new();
    public KpiChartDto CellUlThp { get; set; } = new();
    public KpiChartDto Csfb { get; set; } = new();
    // New KPIs - below CSFB
    public KpiChartDto UlPucch { get; set; } = new();
    public KpiChartDto UlRssi { get; set; } = new();
    public KpiChartDto MaxActiveUser { get; set; } = new();
    public KpiChartDto MaxRrcUser { get; set; } = new();
    
    // Bottom KPIs - Traffic first, then Payload
    public KpiChartDto TrafficErl { get; set; } = new();
    public KpiChartDto PayloadMb { get; set; } = new();
}

/// <summary>
/// Main response DTO for LTE Day KPI data - grouped by sector
/// </summary>
public class LteDayKpiResponseDto
{
    public List<SectorKpiDto> Sectors { get; set; } = new();
}

/// <summary>
/// Response DTO for Traffic and Payload aggregated data
/// </summary>
public class TrafficPayloadResponseDto
{
    // Payload
    public AggregatedSectorKpiDto PayloadBySite { get; set; } = new();
    public AggregatedSectorKpiDto PayloadByCell { get; set; } = new();
    public AggregatedSectorKpiDto PayloadByBand { get; set; } = new();
    public AggregatedSectorKpiDto PayloadByNe { get; set; } = new();
    
    // Traffic
    public AggregatedSectorKpiDto TrafficBySite { get; set; } = new();
    public AggregatedSectorKpiDto TrafficByCell { get; set; } = new();
    public AggregatedSectorKpiDto TrafficByBand { get; set; } = new();
    public AggregatedSectorKpiDto TrafficByNe { get; set; } = new();
}

public class AggregatedSectorKpiDto
{
    public KpiChartDto Total { get; set; } = new();
    public KpiChartDto Sector1 { get; set; } = new();
    public KpiChartDto Sector2 { get; set; } = new();
    public KpiChartDto Sector3 { get; set; } = new();
}
