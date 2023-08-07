using ContentProtector.Core.Constants;
using ContentProtector.Core.Models;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Scoping;

namespace ContentProtector.Core.Migrations
{
	public class CreateTableAndData : MigrationBase
	{
		private readonly ILogger<CreateTableAndData> _logger;
		private readonly IScopeProvider _scopeProvider;

		public CreateTableAndData(IMigrationContext context, ILogger<CreateTableAndData> logger, IScopeProvider scopeProvider) : base(context)
		{
			_logger = logger;
			_scopeProvider = scopeProvider;
		}

		protected override void Migrate()
		{
			if (TableExists("ContentProtector") == false)
			{
				Create.Table<ActionModel>().Do();
			}
			else
			{
				_logger.LogInformation("The database table Content Protector already exists, skipping");
			}

			if (!TableExists("ContentProtector"))
			{
				return;
			}

			try
			{
				var saveData = new ActionModel
				{
					Id = (int)ActionModelId.Save,
					Name = "save",
					DisableAction = false,
					LastEdited = null,
					LastEditedBy = "",
					Nodes = "",
					UserExceptions = "",
					Expansion = null
				};

				var moveData = new ActionModel
				{
					Id = (int)ActionModelId.Move,
					Name = "move",
					DisableAction = false,
					LastEdited = null,
					LastEditedBy = "",
					Nodes = "",
					UserExceptions = "",
					Expansion = null
				};

				//todo: do we need this one?
				var trashData = new ActionModel
				{
					Id = (int)ActionModelId.Trash,
					Name = "trash",
					DisableAction = false,
					LastEdited = null,
					LastEditedBy = "",
					Nodes = "",
					UserExceptions = "",
					Expansion = null
				};

				var deleteData = new ActionModel
				{
					Id = (int)ActionModelId.Delete,
					Name = "delete",
					DisableAction = false,
					LastEdited = null,
					LastEditedBy = "",
					Nodes = "",
					UserExceptions = "",
					Expansion = null
				};

				var copyData = new ActionModel
				{
					Id = (int)ActionModelId.Copy,
					Name = "copy",
					DisableAction = false,
					LastEdited = null,
					LastEditedBy = "",
					Nodes = "",
					UserExceptions = "",
					Expansion = null
				};

				var publishData = new ActionModel
				{
					Id = (int)ActionModelId.Publish,
					Name = "publish",
					DisableAction = false,
					LastEdited = null,
					LastEditedBy = "",
					Nodes = "",
					UserExceptions = "",
					Expansion = null
				};

				var unpublishData = new ActionModel
				{
					Id = (int)ActionModelId.Unpublish,
					Name = "unpublish",
					DisableAction = false,
					LastEdited = null,
					LastEditedBy = "",
					Nodes = "",
					UserExceptions = "",
					Expansion = null
				};

				var rollBackData = new ActionModel
				{
					Id = (int)ActionModelId.Rollback,
					Name = "rollBack",
					DisableAction = false,
					LastEdited = null,
					LastEditedBy = "",
					Nodes = "",
					UserExceptions = "",
					Expansion = null
				};

				var renameData = new ActionModel
				{
					Id = (int)ActionModelId.Rename,
					Name = "rename",
					DisableAction = false,
					LastEdited = null,
					LastEditedBy = "",
					Nodes = "",
					UserExceptions = "",
					Expansion = null
				};

				using var scope = _scopeProvider.CreateScope(autoComplete: true);

				scope.Database.Save(saveData);
				scope.Database.Save(moveData);
				scope.Database.Save(trashData);
				scope.Database.Save(deleteData);
				scope.Database.Save(copyData);
				scope.Database.Save(publishData);
				scope.Database.Save(unpublishData);
				scope.Database.Save(rollBackData);
				scope.Database.Save(renameData);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to save Content Protector initial settings");
			}
		}
	}
}