using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Extension.Migrations;

namespace Umbraco.Extension.MigrationPlans
{
    public class BookingsMigrationPlan : MigrationPlan
    {
        public BookingsMigrationPlan()
            : base("BookingsMigrationPlan")
        {
            // Define the steps for the migration plan
            From(string.Empty)
                .To<CreateTables>("Initial migration to create tables");
            
            

            // You can add more steps here as needed for future migrations
        }
    }
}
