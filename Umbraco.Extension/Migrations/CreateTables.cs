using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Extension.Models;

namespace Umbraco.Extension.Migrations
{
    public class CreateTables : AsyncMigrationBase
    {
        private CreateTables(IMigrationContext context)
            : base(context)
        {
        }

        protected override Task MigrateAsync()
        {
            if (!TableExists("Albums"))
            {
                Create.Table<Albums>().Do();
            }

            if (!TableExists("Bookings"))
            {
                Create.Table<Bookings>().Do();
            }

            if (!TableExists("EventTypes"))
            {
                Create.Table<EventTypes>().Do();
            }

            if (!TableExists("Orders"))
            {
                Create.Table<Orders>().Do();
            }
            if (!TableExists("Payments"))
            {
                Create.Table<Payments>().Do();
            }
            if (!TableExists("PhotoPackages"))
            {
                Create.Table<PhotoPackages>().Do();
            }
            if (!TableExists("Photos"))
            {
                Create.Table<Photos>().Do();
            }

            return Task.CompletedTask;
        }
    }
}
