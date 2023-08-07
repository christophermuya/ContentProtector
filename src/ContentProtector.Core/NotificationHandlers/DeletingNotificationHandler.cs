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
	public class DeletingNotificationHandler : BaseNotificationHandler, INotificationHandler<ContentMovingToRecycleBinNotification>
	{
		private readonly ILogger<DeletingNotificationHandler> _logger;
		private readonly IScopeProvider _scopeProvider;

		public DeletingNotificationHandler(IBackOfficeSecurityAccessor backOfficeSecurityAccessor, ILogger<DeletingNotificationHandler> logger, IScopeProvider scopeProvider) : base(backOfficeSecurityAccessor)
		{
			_logger = logger;
			_scopeProvider = scopeProvider;
		}

		public void Handle(ContentMovingToRecycleBinNotification notification)
		{
			ActionModel? delete = null;
			var currentUserId = GetCurrentUserId();

			try
			{
				using var scope = _scopeProvider.CreateScope(autoComplete: true);

				var sql = scope
					.SqlContext
					.Sql()
					.Select("*")
					.From<ActionModel>()
					.Where<ActionModel>(x => x.Id == (int)ActionModelId.Delete);

				delete = scope
					.Database
					.Fetch<ActionModel>(sql)
					.FirstOrDefault();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to get Content Protector setting for delete action");
				notification.CancelOperation(new EventMessage("Contact website admin", "Failed to get Content Protector setting for delete action", EventMessageType.Error));
			}

			foreach (var node in notification.MoveInfoCollection)
			{
				if (delete == null)
				{
					continue;
				}

				if (!delete.Nodes.Split(',').Contains(node.Entity.Id.ToString()) && !delete.DisableAction)
				{
					continue;
				}

				if (!delete.UserExceptions.Split(',').Contains(currentUserId.ToString()))
				{
					notification.CancelOperation(new EventMessage("Action rejected. Contact website admin", $"You cannot delete '{node.Entity.Name}'", EventMessageType.Error));
				}
			}
		}
	}
}