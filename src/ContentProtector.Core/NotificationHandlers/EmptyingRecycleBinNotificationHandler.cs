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
	public class EmptyingRecycleBinNotificationHandler : BaseNotificationHandler, INotificationHandler<ContentEmptyingRecycleBinNotification>
	{
		private readonly ILogger<EmptyingRecycleBinNotificationHandler> _logger;
		private readonly IScopeProvider _scopeProvider;

		public EmptyingRecycleBinNotificationHandler(IBackOfficeSecurityAccessor backOfficeSecurityAccessor, ILogger<EmptyingRecycleBinNotificationHandler> logger, IScopeProvider scopeProvider) : base(backOfficeSecurityAccessor)
		{
			_logger = logger;
			_scopeProvider = scopeProvider;
		}

		public void Handle(ContentEmptyingRecycleBinNotification notification)
		{
			ActionModel? tash = null;
			var currentUserId = GetCurrentUserId();

			try
			{
				using var scope = _scopeProvider.CreateScope(autoComplete: true);

				var sql = scope
					.SqlContext
					.Sql()
					.Select("*")
					.From<ActionModel>()
					.Where<ActionModel>(x => x.Id == (int)ActionModelId.Trash);

				tash = scope
					.Database
					.Fetch<ActionModel>(sql)
					.FirstOrDefault();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to get Content Protector setting for delete action");
				notification.Cancel = true;
				notification.Messages.Add(new EventMessage("Contact website admin", "Failed to get Content Protector setting for trash action", EventMessageType.Error));
			}

			if (notification.DeletedEntities == null)
			{
				return;
			}

			foreach (var node in notification.DeletedEntities)
			{
				if (tash == null)
				{
					continue;
				}

				if (!tash.Nodes.Split(',').Contains(node.Id.ToString()) && !tash.DisableAction)
				{
					continue;
				}

				if (tash.UserExceptions.Split(',').Contains(currentUserId.ToString()))
				{
					continue;
				}

				notification.Cancel = true;
				notification.Messages.Add(new EventMessage("Action rejected. Contact website admin", $"You cannot trash '{node.Name}'", EventMessageType.Error));
			}
		}
	}
}