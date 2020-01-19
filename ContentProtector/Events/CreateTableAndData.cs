using ContentProtector.App_Plugins.ContentProtector.Models;
using System;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Scoping;

namespace ContentProtector.App_Plugins.ContentProtector.Events {
    public class CreateTableAndData:MigrationBase {

        private readonly IScopeProvider _scopeProvider;
        public CreateTableAndData(IMigrationContext context,IScopeProvider scopeProvider) : base(context) {
            _scopeProvider = scopeProvider;
        }
        public override void Migrate() {
            // Lots of methods available in the MigrationBase class - discover with this.
            if(TableExists("ContentProtector") == false) {
                Create.Table<ActionModel>().Do();
            }
            else {
                Logger.Info<CreateTableAndData>("The database table Content Protector already exists, skipping","Content Protector");
            }
            if(TableExists("ContentProtector")) {
                try {
                    ActionModel saveData = new ActionModel() {
                        id = 1,
                        name = "save",
                        disableAction = false,
                        lastEdited = null,
                        lastEditedBy = "",
                        nodes = "",
                        userExceptions = "",
                        expansion = null

                    };

                    ActionModel moveData = new ActionModel() {
                        id = 2,
                        name = "move",
                        disableAction = false,
                        lastEdited = null,
                        lastEditedBy = "",
                        nodes = "",
                        userExceptions = "",
                        expansion = null

                    };

                    ActionModel trashData = new ActionModel() {
                        id = 3,
                        name = "trash",
                        disableAction = false,
                        lastEdited = null,
                        lastEditedBy = "",
                        nodes = "",
                        userExceptions = "",
                        expansion = null

                    };

                    ActionModel deleteData = new ActionModel() {
                        id = 4,
                        name = "delete",
                        disableAction = false,
                        lastEdited = null,
                        lastEditedBy = "",
                        nodes = "",
                        userExceptions = "",
                        expansion = null

                    };

                    ActionModel copyData = new ActionModel() {
                        id = 5,
                        name = "copy",
                        disableAction = false,
                        lastEdited = null,
                        lastEditedBy = "",
                        nodes = "",
                        userExceptions = "",
                        expansion = null

                    };

                    ActionModel publishData = new ActionModel() {
                        id = 6,
                        name = "publish",
                        disableAction = false,
                        lastEdited = null,
                        lastEditedBy = "",
                        nodes = "",
                        userExceptions = "",
                        expansion = null

                    };

                    ActionModel unpublishData = new ActionModel() {
                        id = 7,
                        name = "unpublish",
                        disableAction = false,
                        lastEdited = null,
                        lastEditedBy = "",
                        nodes = "",
                        userExceptions = "",
                        expansion = null

                    };

                    ActionModel rollBackData = new ActionModel() {
                        id = 8,
                        name = "rollBack",
                        disableAction = false,
                        lastEdited = null,
                        lastEditedBy = "",
                        nodes = "",
                        userExceptions = "",
                        expansion = null

                    };

                    using(var scope = _scopeProvider.CreateScope(autoComplete: true)) {
                        scope.Database.Save<ActionModel>(saveData);
                        scope.Database.Save<ActionModel>(moveData);
                        scope.Database.Save<ActionModel>(trashData);
                        scope.Database.Save<ActionModel>(deleteData);
                        scope.Database.Save<ActionModel>(copyData);
                        scope.Database.Save<ActionModel>(publishData);
                        scope.Database.Save<ActionModel>(unpublishData);
                        scope.Database.Save<ActionModel>(rollBackData);
                    }
                }
                catch(Exception ex) {
                    Logger.Error<ActionModel>("Failed to save Content Protector initial settings",ex.Message);
                }

            }

        }
    }
}