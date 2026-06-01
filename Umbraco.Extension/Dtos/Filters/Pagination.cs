using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Extension.Enums;

namespace Umbraco.Extension.Dtos.Filters
{
    public record Pagination
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
