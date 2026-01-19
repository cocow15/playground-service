namespace AuthService.DTOs;

/// <summary>
/// Response DTO for a sector group containing all GSM KPIs
/// </summary>
public class GsmSectorKpiDto
{
    public int SectorGroup { get; set; }
    
    // Standard KPIs (Line Charts)
    public KpiChartDto Availability { get; set; } = new();
    public KpiChartDto SCong { get; set; } = new();
    public KpiChartDto TCong { get; set; } = new();
    public KpiChartDto Hosr { get; set; } = new();
    public KpiChartDto TchDrop { get; set; } = new();
    public KpiChartDto TchBlock { get; set; } = new();
    public KpiChartDto TbfComp { get; set; } = new();
    public KpiChartDto TbfDlEstSr { get; set; } = new();
    public KpiChartDto TbfUlEstSr { get; set; } = new();
    public KpiChartDto TbfDlSr { get; set; } = new();
    public KpiChartDto SdcchSr { get; set; } = new();
    public KpiChartDto Ich { get; set; } = new();
    
    // Payload KPIs (Line Charts)
    public KpiChartDto PayloadEdgeGb { get; set; } = new();
    public KpiChartDto PayloadEdgeGbStacked { get; set; } = new();
    public KpiChartDto PayloadGprsGb { get; set; } = new();
    public KpiChartDto PayloadGprsGbStacked { get; set; } = new();
    public KpiChartDto PayloadTotalGb { get; set; } = new();
    public KpiChartDto PayloadTotalGbStacked { get; set; } = new();
    
    // Traffic KPIs (Line Charts)
    public KpiChartDto TrafficSdcchErl { get; set; } = new();
    public KpiChartDto TrafficSdcchErlStacked { get; set; } = new();
    public KpiChartDto TrafficTchErl { get; set; } = new();
    public KpiChartDto TrafficTchErlStacked { get; set; } = new();
    public KpiChartDto TrafficTotalErl { get; set; } = new();
    public KpiChartDto TrafficTotalErlStacked { get; set; } = new();
}

/// <summary>
/// Main response DTO for GSM Day KPI data - grouped by sector
/// </summary>
public class GsmDayKpiResponseDto
{
    public List<GsmSectorKpiDto> Sectors { get; set; } = new();
}
