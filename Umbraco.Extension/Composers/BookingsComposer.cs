using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extension.Notifications;

namespace Umbraco.Extension.Composers
{
    public class BookingsComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, RunBookingsMigration>();
        }
    }
}
