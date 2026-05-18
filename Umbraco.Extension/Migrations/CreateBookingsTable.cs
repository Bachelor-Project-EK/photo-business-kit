using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Extension.Models;
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
}
