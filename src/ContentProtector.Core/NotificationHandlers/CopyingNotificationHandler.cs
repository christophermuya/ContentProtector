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
	public class CopyingNotificationHandler : BaseNotificationHandler, INotificationHandler<ContentCopyingNotification>
	{
		private readonly ILogger<CopyingNotificationHandler> _logger;
		private readonly IScopeProvider _scopeProvider;

		public CopyingNotificationHandler(IBackOfficeSecurityAccessor backOfficeSecurityAccessor, ILogger<CopyingNotificationHandler> logger, IScopeProvider scopeProvider) : base(backOfficeSecurityAccessor)
		{
			_logger = logger;
			_scopeProvider = scopeProvider;
		}

		public void Handle(ContentCopyingNotification notification)
		{
			ActionModel? copy = null;
			var currentUserId = GetCurrentUserId();

			try
			{
				using var scope = _scopeProvider.CreateScope(autoComplete: true);

				var sql = scope
					.SqlContext
					.Sql()
					.Select("*")
					.From<ActionModel>()
					.Where<ActionModel>(x => x.Id == (int)ActionModelId.Copy);

				copy = scope
					.Database
					.Fetch<ActionModel>(sql)
					.FirstOrDefault();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to get Content Protector setting for copy action");
				notification.CancelOperation(new EventMessage("Contact website admin", "Failed to get Content Protector setting for copy action", EventMessageType.Error));
			}

			if (copy == null)
			{
				return;
			}

			if (!copy.Nodes.Split(',').Contains(notification.Original.Id.ToString()) && !copy.DisableAction)
			{
				return;
			}

			if (copy.UserExceptions.Split(',').Contains(currentUserId.ToString()))
			{
				return;
			}

			notification.CancelOperation(new EventMessage("Action rejected. Contact website admin", $"You cannot copy '{notification.Original.Name}'", EventMessageType.Error));
		}
	}
}