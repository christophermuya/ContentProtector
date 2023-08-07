using Umbraco.Cms.Core.Security;

namespace ContentProtector.Core.NotificationHandlers
{
	public class BaseNotificationHandler
	{
		private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;

		public BaseNotificationHandler(IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
		{
			_backOfficeSecurityAccessor = backOfficeSecurityAccessor;
		}

		public int GetCurrentUserId()
		{
			//if the event is triggered by scheduled publishing, there is no current user - so we'll default to the root install user
			var currentUserId = _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser?.Id ?? -1;

			return currentUserId;
		}
	}
}