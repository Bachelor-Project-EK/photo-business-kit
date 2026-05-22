namespace Umbraco.Extension.Dtos;

public class PhotoPackageDto
{
    public Guid EventTypeId { get; set; }
    public string PhotoPackageName { get; set; }
    public int PhotoCount { get; set; }
    public decimal? PhotoPrice { get; set; }
    public decimal? HourlyPrice { get; set; }
}
