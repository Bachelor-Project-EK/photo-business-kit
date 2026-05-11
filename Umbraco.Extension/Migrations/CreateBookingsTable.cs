using NPoco;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Extension.Migrations
{
    public class CreateBookingsTable : AsyncMigrationBase
    {
        public CreateBookingsTable(IMigrationContext context)
            : base(context)
        {
        }

        protected override Task MigrateAsync()
        {
            if (!TableExists("Bookings"))
            {
                Create.Table<Bookings>().Do();
            }

            return Task.CompletedTask;
        }
    }

    [TableName("Bookings")]
    [PrimaryKey("Id", AutoIncrement = false)]
    [ExplicitColumns]
    public class Bookings
    {
        [Column("Id")]
        public required Guid Id { get; set; }

        [Column("CustomerName")]
        [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
        public string CustomerName { get; set; } = string.Empty;

        [Column("BookingDate")]
        public DateTime BookingDate { get; set; }
    }
}
