using ContentProtector.Core.Constants;
using ContentProtector.Core.Models;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace ContentProtector.Core.NotificationHandlers
{
	public class PublishingNotificationHandler : BaseNotificationHandler, INotificationHandler<ContentPublishingNotification>
	{
		private readonly ILogger<PublishingNotificationHandler> _logger;
		private readonly IScopeProvider _scopeProvider;

		public PublishingNotificationHandler(IBackOfficeSecurityAccessor backOfficeSecurityAccessor, ILogger<PublishingNotificationHandler> logger, IScopeProvider scopeProvider) : base(backOfficeSecurityAccessor)
		{
			_logger = logger;
			_scopeProvider = scopeProvider;
		}

		public void Handle(ContentPublishingNotification notification)
		{
			ActionModel? publish = null;
			var currentUserId = GetCurrentUserId();

			try
			{
				using var scope = _scopeProvider.CreateScope(autoComplete: true);

				var sql = scope
					.SqlContext
					.Sql()
					.Select("*")
					.From<ActionModel>()
					.Where<ActionModel>(x => x.Id == (int)ActionModelId.Publish);

				publish = scope
					.Database
					.Fetch<ActionModel>(sql)
					.FirstOrDefault();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to get Content Protector setting for publish action");
				notification.CancelOperation(new EventMessage("Contact website admin", "Failed to get Content Protector setting for publish action", EventMessageType.Error));
			}

			foreach (var node in notification.PublishedEntities)
			{
				if (publish == null)
				{
					continue;
				}

				if (!publish.Nodes.Split(',').Contains(node.Id.ToString()) && !publish.DisableAction)
				{
					continue;
				}

				if (!publish.UserExceptions.Split(',').Contains(currentUserId.ToString()))
				{
					notification.CancelOperation(new EventMessage("Action rejected. Contact website admin", $"You cannot publish '{node.Name}'", EventMessageType.Error));
				}
			}
		}
	}
}