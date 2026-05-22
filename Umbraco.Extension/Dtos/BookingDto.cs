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
        public required DateTimeOffset CreatedOn { get; set; }

        public required DateTimeOffset UpdatedOn { get; set; }

        public required DateTimeOffset StartDate { get; set; }

        public required DateTimeOffset EndDate { get; set; }

        public required BookingStatus Status { get; set; }

        public string? Comment { get; set; }

        //public PhotoPackages? PhotoPackage { get; set; }
    }
}
