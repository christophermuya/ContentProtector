using ContentProtector.Core.Constants;
using ContentProtector.Core.Models;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Extensions;

namespace ContentProtector.Core.Controllers
{
	public class ContentProtectorApiController : UmbracoAuthorizedJsonController
	{
		private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
		private readonly ILogger<ContentProtectorApiController> _logger;
		private readonly IScopeProvider _scopeProvider;

		public ContentProtectorApiController(IBackOfficeSecurityAccessor backOfficeSecurityAccessor, ILogger<ContentProtectorApiController> logger, IScopeProvider scopeProvider)
		{
			_backOfficeSecurityAccessor = backOfficeSecurityAccessor;
			_logger = logger;
			_scopeProvider = scopeProvider;
		}

		public IEnumerable<ActionModel>? GetAll()
		{
			try
			{
				using var scope = _scopeProvider.CreateScope(autoComplete: true);

				var sql = scope
					.SqlContext
					.Sql()
					.Select("*")
					.From<ActionModel>();

				var value = scope.Database.Fetch<ActionModel>(sql);

				return value;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to get Content Protector settings");
			}

			return null;
		}

		public ActionModel? GetById(int id)
		{
			try
			{
				using var scope = _scopeProvider.CreateScope(autoComplete: true);

				var sql = scope
					.SqlContext
					.Sql()
					.Select("*")
					.From<ActionModel>()
					.Where<ActionModel>(x => x.Id == id);

				var value = scope.Database.Fetch<ActionModel>(sql).FirstOrDefault();

				return value;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to get Content Protector settings using Id {id}", id);
			}

			return null;
		}

		public ActionModel Save(ActionModel model)
		{
			try
			{
				using var scope = _scopeProvider.CreateScope(autoComplete: true);

				var sql = scope
					.SqlContext
					.Sql()
					.Select("*")
					.From<ActionModel>()
					.Where<ActionModel>(x => x.Id == model.Id);

				var value = scope.Database.Fetch<ActionModel>(sql).FirstOrDefault() ?? throw new Exception("Failed to get Content Protector settings");

				value.DisableAction = model.DisableAction;
				value.LastEdited = DateTime.Now;
				value.LastEditedBy = _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser?.Name ?? "Root";
				value.Nodes = model.Nodes;
				value.UserExceptions = model.UserExceptions;
				scope.Database.Update(value);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to save Content Protector settings for the" + model.Name + " action");
			}

			return model;
		}
	}
}