using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Extension.Models;

namespace Umbraco.Extension.Migrations
{
    public class CreateTables : AsyncMigrationBase
    {
        public CreateTables(IMigrationContext context)
            : base(context)
        {
        }

        protected override Task MigrateAsync()
        {
            if (!TableExists(nameof(EventTypes)))
            {
                Create.Table<EventTypes>().Do();
            }

            if (!TableExists(nameof(PhotoPackages)))
            {
                Create.Table<PhotoPackages>().Do();

                Alter.Table(nameof(PhotoPackages))
                    .AlterColumn(nameof(PhotoPackages.PhotoPrice))
                    .AsCustom("MONEY")
                    .Nullable()
                    .Do();

                Alter.Table(nameof(PhotoPackages))
                    .AlterColumn(nameof(PhotoPackages.HourlyPrice))
                    .AsCustom("MONEY")
                    .Nullable()
                    .Do();
            }

            if (!TableExists(nameof(Bookings)))
            {
                Create.Table<Bookings>().Do();
                Create.ForeignKey("FK_Bookings_cmsMember_nodeId")
                    .FromTable(nameof(Bookings))
                    .ForeignColumn(nameof(Bookings.NodeId))
                    .ToTable("cmsMember").PrimaryColumns("nodeId").Do();
            }

            if (!TableExists(nameof(Albums)))
            {
                Create.Table<Albums>().Do();
            }

            if (!TableExists(nameof(Payments)))
            {
                Create.Table<Payments>().Do();
                Alter.Table(nameof(Payments))
                    .AlterColumn(nameof(Payments.TotalPrice))
                    .AsCustom("MONEY")
                    .Do();
            }

            if (!TableExists(nameof(Photos)))
            {
                Create.Table<Photos>().Do();
            }

            return Task.CompletedTask;
        }
    }
}

