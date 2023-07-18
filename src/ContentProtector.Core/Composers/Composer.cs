using ContentProtector.Core.Components;
using ContentProtector.Core.NotificationHandlers;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace ContentProtector.Core.Composers
{
	public class Composer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			builder.Components().Append<InitializeMigrationPlan>();

			builder.AddNotificationHandler<ContentDeletingNotification, DeletingNotificationHandler>();
			builder.AddNotificationHandler<ContentPublishingNotification, PublishingNotificationHandler>();
			builder.AddNotificationHandler<ContentRollingBackNotification, RollingBackNotificationHandler>();
			builder.AddNotificationHandler<ContentSavingNotification, SavingNotificationHandler>();
			builder.AddNotificationHandler<ContentUnpublishingNotification, UnpublishingNotificationHandler>();
		}
	}
}