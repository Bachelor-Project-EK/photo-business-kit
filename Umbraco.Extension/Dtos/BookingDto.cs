using NPoco;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using Umbraco.Extension.Enum;
using Umbraco.Extension.Models;

namespace Umbraco.Extension.Dtos
{
    public class BookingDto
    {
        public required DateTimeOffset StartDate { get; set; }

        public required DateTimeOffset EndDate { get; set; }

        public string? Comment { get; set; }

        public required Guid PhotoPackageId { get; set; }
    }
}
