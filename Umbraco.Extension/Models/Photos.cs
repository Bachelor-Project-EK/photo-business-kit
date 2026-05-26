using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Extension.Models;
[TableName(nameof(Photos))]
[PrimaryKey(nameof(Id), AutoIncrement = false)]
[ExplicitColumns]
public class Photos
{
    [Column(nameof(Id))]
    [PrimaryKeyColumn(AutoIncrement = false)]
    public required Guid Id { get; set; }

    [Column(nameof(AlbumId))]
    [ForeignKey(typeof(Albums), Column = nameof(Albums.Id))]
     public required Guid AlbumId { get; set; }

    [ResultColumn]
    [Reference(ReferenceType.OneToOne, ColumnName = nameof(AlbumId), ReferenceMemberName = nameof(Albums.Id))]
    public Albums? Album { get; set; }

    // Filename from the uploaded file, for example: wedding-final.jpg
    [Column(nameof(OriginalFileName))]
    [Length(255)]
    public required string OriginalFileName { get; set; }

    // Later contains the generated Blob Storage path.
    // Example: albums/{albumId}/{photoId}.jpg
    [Column(nameof(BlobPath))]
    [Length(500)]
    public required string BlobPath { get; set; }

    // Saved file type after validation, for example: image/jpeg
    [Column(nameof(ContentType))]
    [Length(100)]
    public required string ContentType { get; set; }

    // Saved file size after validation.
    [Column(nameof(FileSizeInBytes))]
    public required long FileSizeInBytes { get; set; }

    [Column(nameof(CreatedOn))]
     public required DateTimeOffset CreatedOn { get; set; }

    [Column(nameof(UpdatedOn))]
     public required DateTimeOffset UpdatedOn { get; set; }

    [Column(nameof(Link))]
    [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
    public string? Link { get; set; }

}