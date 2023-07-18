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
	public class UnpublishingNotificationHandler : BaseNotificationHandler, INotificationHandler<ContentUnpublishingNotification>
	{
		private readonly ILogger<UnpublishingNotificationHandler> _logger;
		private readonly IScopeProvider _scopeProvider;

		public UnpublishingNotificationHandler(IBackOfficeSecurityAccessor backOfficeSecurityAccessor, ILogger<UnpublishingNotificationHandler> logger, IScopeProvider scopeProvider) : base(backOfficeSecurityAccessor)
		{
			_logger = logger;
			_scopeProvider = scopeProvider;
		}

		public void Handle(ContentUnpublishingNotification notification)
		{
			ActionModel? unpublish = null;
			var currentUserId = GetCurrentUserId();

			try
			{
				using var scope = _scopeProvider.CreateScope(autoComplete: true);

				var sql = scope
					.SqlContext
					.Sql()
					.Select("*")
					.From<ActionModel>()
					.Where<ActionModel>(x => x.Id == (int)ActionModelId.Unpublish);

				unpublish = scope.Database.Fetch<ActionModel>(sql).FirstOrDefault();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to get Content Protector setting for unpublish action");
				notification.CancelOperation(new EventMessage("Contact website admin", "Failed to get Content Protector setting for unpublish action", EventMessageType.Error));
			}

			foreach (var node in notification.UnpublishedEntities)
			{
				if (unpublish == null)
				{
					continue;
				}

				if (!unpublish.Nodes.Split(',').Contains(node.Id.ToString()) && !unpublish.DisableAction)
				{
					continue;
				}

				if (!unpublish.UserExceptions.Split(',').Contains(currentUserId.ToString()))
				{
					notification.CancelOperation(new EventMessage("Action rejected. Contact website admin", $"You cannot unpublish '{node.Name}'", EventMessageType.Error));
				}
			}
		}
	}
}