using ContentProtector.Core.Constants;
using ContentProtector.Core.Models;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace ContentProtector.Core.NotificationHandlers
{
	public class SavingNotificationHandler : BaseNotificationHandler, INotificationHandler<ContentSavingNotification>
	{
		private readonly ILogger<SavingNotificationHandler> _logger;
		private readonly IScopeProvider _scopeProvider;
		private readonly IContentService _contentService;

		public SavingNotificationHandler(IBackOfficeSecurityAccessor backOfficeSecurityAccessor, ILogger<SavingNotificationHandler> logger, IScopeProvider scopeProvider, IContentService contentService) : base(backOfficeSecurityAccessor)
		{
			_logger = logger;
			_scopeProvider = scopeProvider;
			_contentService = contentService;
		}

		public void Handle(ContentSavingNotification notification)
		{
			ActionModel? save = null;
			ActionModel? rename = null;
			var currentUserId = GetCurrentUserId();

			try
			{
				using var scope = _scopeProvider.CreateScope(autoComplete: true);

				var sql = scope
					.SqlContext
					.Sql()
					.Select("*")
					.From<ActionModel>()
					.Where<ActionModel>(x => x.Id == (int)ActionModelId.Save || x.Id == (int)ActionModelId.Rename);

				var actions = scope
					.Database
					.Fetch<ActionModel>(sql);

				save = actions.FirstOrDefault(x => x.Id == (int)ActionModelId.Save);

				rename = actions.FirstOrDefault(x => x.Id == (int)ActionModelId.Rename);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to get Content Protector setting for save action");
				notification.CancelOperation(new EventMessage("Contact website admin", "Failed to get Content Protector setting for save action", EventMessageType.Error));
			}

			foreach (var node in notification.SavedEntities)
			{
				if (save != null)
				{
					if (save.Nodes.Split(',').Contains(node.Id.ToString()) || save.DisableAction)
					{
						if (!save.UserExceptions.Split(',').Contains(currentUserId.ToString()))
						{
							notification.CancelOperation(new EventMessage("Action rejected. Contact website admin", $"You cannot save '{node.Name}'", EventMessageType.Error));
						}
					}
				}

				if (rename == null)
				{
					continue;
				}

				if (!rename.Nodes.Split(',').Contains(node.Id.ToString()) && !rename.DisableAction)
				{
					continue;
				}

				if (rename.UserExceptions.Split(',').Contains(currentUserId.ToString()))
				{
					continue;
				}

				var content = _contentService.GetById(node.Id);

				if (!string.Equals(node.Name, content.Name, StringComparison.CurrentCultureIgnoreCase))
				{
					notification.CancelOperation(new EventMessage("Action rejected. Contact website admin", $"You cannot rename '{content.Name}'", EventMessageType.Error));
				}
			}
		}
	}
}