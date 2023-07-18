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
	public class MovingNotificationHandler : BaseNotificationHandler, INotificationHandler<ContentMovingNotification>
	{
		private readonly ILogger<MovingNotificationHandler> _logger;
		private readonly IScopeProvider _scopeProvider;

		public MovingNotificationHandler(IBackOfficeSecurityAccessor backOfficeSecurityAccessor, ILogger<MovingNotificationHandler> logger, IScopeProvider scopeProvider) : base(backOfficeSecurityAccessor)
		{
			_logger = logger;
			_scopeProvider = scopeProvider;
		}

		public void Handle(ContentMovingNotification notification)
		{
			ActionModel? move = null;
			var currentUserId = GetCurrentUserId();

			try
			{
				using var scope = _scopeProvider.CreateScope(autoComplete: true);

				var sql = scope
					.SqlContext
					.Sql()
					.Select("*")
					.From<ActionModel>()
					.Where<ActionModel>(x => x.Id == (int)ActionModelId.Move);

				move = scope
					.Database
					.Fetch<ActionModel>(sql)
					.FirstOrDefault();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to get Content Protector setting for move action");
				notification.Cancel = true;
				notification.Messages.Add(new EventMessage("Contact website admin", "Failed to get Content Protector setting for move action", EventMessageType.Error));
			}

			foreach (var node in notification.MoveInfoCollection)
			{
				if (move == null)
				{
					continue;
				}

				if (!move.Nodes.Split(',').Contains(node.Entity.Id.ToString()) && !move.DisableAction)
				{
					continue;
				}

				if (move.UserExceptions.Split(',').Contains(currentUserId.ToString()))
				{
					continue;
				}

				notification.Cancel = true;
				notification.CancelOperation(new EventMessage("Action rejected. Contact website admin", $"You cannot move '{node.Entity.Name}'", EventMessageType.Error));
			}
		}
	}
}