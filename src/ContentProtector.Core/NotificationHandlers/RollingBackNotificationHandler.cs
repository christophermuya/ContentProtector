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
	public class RollingBackNotificationHandler : BaseNotificationHandler, INotificationHandler<ContentRollingBackNotification>
	{
		private readonly ILogger<RollingBackNotificationHandler> _logger;
		private readonly IScopeProvider _scopeProvider;

		public RollingBackNotificationHandler(IBackOfficeSecurityAccessor backOfficeSecurityAccessor, ILogger<RollingBackNotificationHandler> logger, IScopeProvider scopeProvider) : base(backOfficeSecurityAccessor)
		{
			_logger = logger;
			_scopeProvider = scopeProvider;
		}

		public void Handle(ContentRollingBackNotification notification)
		{
			ActionModel? rollBack = null;
			var currentUserId = GetCurrentUserId();

			try
			{
				using var scope = _scopeProvider.CreateScope(autoComplete: true);

				var sql = scope
					.SqlContext
					.Sql()
					.Select("*")
					.From<ActionModel>()
					.Where<ActionModel>(x => x.Id == (int)ActionModelId.Rollback);

				rollBack = scope
					.Database
					.Fetch<ActionModel>(sql)
					.FirstOrDefault();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to get Content Protector setting for rollback action");
				notification.CancelOperation(new EventMessage("Contact website admin", "Failed to get Content Protector setting for rollback action", EventMessageType.Error));
			}

			if (rollBack == null)
			{
				return;
			}

			if (!rollBack.Nodes.Contains(notification.Entity.Id.ToString()) && !rollBack.DisableAction)
			{
				return;
			}

			if (!rollBack.UserExceptions.Split(',').Contains(currentUserId.ToString()))
			{
				notification.CancelOperation(new EventMessage("Action rejected. Contact website admin", $"You cannot rollBack {notification.Entity.Name}", EventMessageType.Error));
			}
		}
	}
}