using ContentProtector.App_Plugins.ContentProtector.Models;
using System;
using System.Linq;
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Web;


//Move, Copy,

namespace ContentProtector.App_Plugins.ContentProtector.Components {
    public class ActionBlocker:IComponent {
        private IScopeProvider _scopeProvider;
        private ILogger _logger;
        private IUmbracoContextFactory _context;
        public ActionBlocker(IUmbracoContextFactory context,IScopeProvider scopeProvider,ILogger logger) {
            _scopeProvider = scopeProvider;
            _logger = logger;
            _context = context;
        }

        public void Initialize() {
            ContentService.Saving += ContentService_Saving;
            //ContentService.Moving += ContentService_Moving;
            //ContentService.Copying += ContentService_Copying;
            ContentService.Publishing += ContentService_Publishing;
            ContentService.Unpublishing += ContentService_Unpublishing;
            ContentService.RollingBack += ContentService_RollingBack;
            ContentService.Deleting += ContentService_Deleting;
            ContentService.Trashing += ContentService_Trashing;
        }

        private void ContentService_Trashing(IContentService sender,MoveEventArgs<IContent> eventArgs) {
            ActionModel trash = null;
            int _currentUserId;

            using(var contextReference = _context.EnsureUmbracoContext()) {
                _currentUserId = contextReference.UmbracoContext.Security.IsAuthenticated() ? contextReference.UmbracoContext.Security.CurrentUser.Id : Umbraco.Core.Constants.Security.SuperUserId;
            }

            try {

                using(var scope = _scopeProvider.CreateScope(autoComplete: true)) {
                    var sql = scope.SqlContext.Sql().Select("*").From<ActionModel>()
                                    .Where<ActionModel>(x => x.id == 3);
                    trash = scope.Database.Fetch<ActionModel>(sql).FirstOrDefault();
                }
            }
            catch(Exception ex) {
                _logger.Error<ActionModel>("Failed to get Content Protector setting for trash action: " + ex.Message);
            }

            foreach(var node in eventArgs.MoveInfoCollection) {
                if(trash != null) {
                    if(trash.nodes.Contains(node.Entity.Id.ToString()) || trash.disableAction) {
                        if(!trash.userExceptions.Split(',').Contains(_currentUserId.ToString())) {
                            eventArgs.CancelOperation(new EventMessage("Action rejected. Contact website admin","You cannot trash " + node.Entity.Name,EventMessageType.Error));
                        }
                    }
                }
            }
        }

        private void ContentService_Deleting(IContentService sender,DeleteEventArgs<IContent> eventArgs) {
            ActionModel delete = null;
            int _currentUserId;

            using(var contextReference = _context.EnsureUmbracoContext()) {
                _currentUserId = contextReference.UmbracoContext.Security.IsAuthenticated() ? contextReference.UmbracoContext.Security.CurrentUser.Id : Umbraco.Core.Constants.Security.SuperUserId;
            }

            try {

                using(var scope = _scopeProvider.CreateScope(autoComplete: true)) {
                    var sql = scope.SqlContext.Sql().Select("*").From<ActionModel>()
                                    .Where<ActionModel>(x => x.id == 4);
                    delete = scope.Database.Fetch<ActionModel>(sql).FirstOrDefault();
                }
            }
            catch(Exception ex) {
                _logger.Error<ActionModel>("Failed to get Content Protector setting for delete action: " + ex.Message);
            }

            foreach(var node in eventArgs.DeletedEntities) {
                if(delete != null) {
                    if(delete.nodes.Split(',').Contains(node.Id.ToString()) || delete.disableAction) {
                        if(!delete.userExceptions.Split(',').Contains(_currentUserId.ToString())) {
                            eventArgs.CancelOperation(new EventMessage("Action rejected. Contact website admin","You cannot delete " + node.Name,EventMessageType.Error));
                        }
                    }
                }
            }
        }

        private void ContentService_RollingBack(IContentService sender,RollbackEventArgs<IContent> eventArgs) {
            ActionModel rollBack = null;
            int _currentUserId;

            using(var contextReference = _context.EnsureUmbracoContext()) {
                _currentUserId = contextReference.UmbracoContext.Security.IsAuthenticated() ? contextReference.UmbracoContext.Security.CurrentUser.Id : Umbraco.Core.Constants.Security.SuperUserId;
            }

            try {

                using(var scope = _scopeProvider.CreateScope(autoComplete: true)) {
                    var sql = scope.SqlContext.Sql().Select("*").From<ActionModel>()
                                    .Where<ActionModel>(x => x.id == 8);
                    rollBack = scope.Database.Fetch<ActionModel>(sql).FirstOrDefault();
                }
            }
            catch(Exception ex) {
                _logger.Error<ActionModel>("Failed to get Content Protector setting for rollBack action: " + ex.Message);
            }

            if(rollBack != null) {
                if(rollBack.nodes.Contains(eventArgs.Entity.Id.ToString()) || rollBack.disableAction) {
                    if(!rollBack.userExceptions.Split(',').Contains(_currentUserId.ToString())) {
                        eventArgs.CancelOperation(new EventMessage("Action rejected. Contact website admin","You cannot rollBack " + eventArgs.Entity.Name,EventMessageType.Error));
                    }
                }
            }
        }

        private void ContentService_Unpublishing(IContentService sender,PublishEventArgs<IContent> eventArgs) {
            ActionModel unpublish = null;
            int _currentUserId;

            using(var contextReference = _context.EnsureUmbracoContext()) {
                _currentUserId = contextReference.UmbracoContext.Security.IsAuthenticated() ? contextReference.UmbracoContext.Security.CurrentUser.Id : Umbraco.Core.Constants.Security.SuperUserId;
            }

            try {

                using(var scope = _scopeProvider.CreateScope(autoComplete: true)) {
                    var sql = scope.SqlContext.Sql().Select("*").From<ActionModel>()
                                    .Where<ActionModel>(x => x.id == 7);
                    unpublish = scope.Database.Fetch<ActionModel>(sql).FirstOrDefault();
                }
            }
            catch(Exception ex) {
                _logger.Error<ActionModel>("Failed to get Content Protector setting for unpublish action: " + ex.Message);
            }

            foreach(var node in eventArgs.PublishedEntities) {
                if(unpublish != null) {
                    if(unpublish.nodes.Split(',').Contains(node.Id.ToString()) || unpublish.disableAction) {
                        if(!unpublish.userExceptions.Split(',').Contains(_currentUserId.ToString())) {
                            eventArgs.CancelOperation(new EventMessage("Action rejected. Contact website admin","You cannot unpublish " + node.Name,EventMessageType.Error));
                        }
                    }
                }
            }
        }

        private void ContentService_Publishing(IContentService sender,ContentPublishingEventArgs eventArgs) {
            ActionModel publish = null;
            int _currentUserId;

            using(var contextReference = _context.EnsureUmbracoContext()) {
                _currentUserId = contextReference.UmbracoContext.Security.IsAuthenticated() ? contextReference.UmbracoContext.Security.CurrentUser.Id : Umbraco.Core.Constants.Security.SuperUserId;
            }

            try {

                using(var scope = _scopeProvider.CreateScope(autoComplete: true)) {
                    var sql = scope.SqlContext.Sql().Select("*").From<ActionModel>()
                                    .Where<ActionModel>(x => x.id == 6);
                    publish = scope.Database.Fetch<ActionModel>(sql).FirstOrDefault();
                }
            }
            catch(Exception ex) {
                _logger.Error<ActionModel>("Failed to get Content Protector setting for publish action: " + ex.Message);
            }

            foreach(var node in eventArgs.PublishedEntities) {
                if(publish != null) {
                    if(publish.nodes.Split(',').Contains(node.Id.ToString()) || publish.disableAction) {
                        if(!publish.userExceptions.Split(',').Contains(_currentUserId.ToString())) {
                            eventArgs.CancelOperation(new EventMessage("Action rejected. Contact website admin","You cannot publish " + node.Name,EventMessageType.Error));
                        }
                    }
                }
            }
        }

        //private void ContentService_Copying(IContentService sender,CopyEventArgs<IContent> eventArgs) {
        //    ActionModel copy = null;
        //    int _currentUserId;

        //    using(var contextReference = _context.EnsureUmbracoContext()) {
        //        _currentUserId = contextReference.UmbracoContext.Security.IsAuthenticated() ? contextReference.UmbracoContext.Security.CurrentUser.Id : Umbraco.Core.Constants.Security.SuperUserId;
        //    }

        //    try {

        //        using(var scope = _scopeProvider.CreateScope(autoComplete: true)) {
        //            var sql = scope.SqlContext.Sql().Select("*").From<ActionModel>()
        //                            .Where<ActionModel>(x => x.id == 5);
        //            copy = scope.Database.Fetch<ActionModel>(sql).FirstOrDefault();
        //        }
        //    }
        //    catch(Exception ex) {
        //        _logger.Error<ActionModel>("Failed to get Content Protector setting for copy action: " + ex.Message);
        //    }

        //    if(copy != null) {
        //        if(copy.nodes.Contains(eventArgs.Copy.Id.ToString())) {
        //            if(!copy.userExceptions.Split(',').Contains(_currentUserId.ToString())) {
        //                eventArgs.Cancel = true;
        //                eventArgs.CanCancel = true;
        //                eventArgs.Messages.Add(new EventMessage("Action rejected. Contact website admin","You cannot copy " + eventArgs.Copy.Name,EventMessageType.Error));
        //            }
        //        }
        //    }

        //}

        //private void ContentService_Moving(IContentService sender,MoveEventArgs<IContent> eventArgs) {
        //    ActionModel move = null;
        //    int _currentUserId;

        //    using(var contextReference = _context.EnsureUmbracoContext()) {
        //        _currentUserId = contextReference.UmbracoContext.Security.IsAuthenticated() ? contextReference.UmbracoContext.Security.CurrentUser.Id : Umbraco.Core.Constants.Security.SuperUserId;
        //    }

        //    try {

        //        using(var scope = _scopeProvider.CreateScope(autoComplete: true)) {
        //            var sql = scope.SqlContext.Sql().Select("*").From<ActionModel>()
        //                            .Where<ActionModel>(x => x.id == 2);
        //            move = scope.Database.Fetch<ActionModel>(sql).FirstOrDefault();
        //        }
        //    }
        //    catch(Exception ex) {
        //        _logger.Error<ActionModel>("Failed to get Content Protector setting for move action: " + ex.Message);
        //    }

        //    foreach(var node in eventArgs.MoveInfoCollection) {
        //        if(move != null) {
        //            if(move.nodes.Contains(node.Entity.Id.ToString()) || move.disableAction) {
        //                if(!move.userExceptions.Split(',').Contains(_currentUserId.ToString())) {
        //                    eventArgs.CancelOperation(new EventMessage("Action rejected. Contact website admin","You cannot move " + node.Entity.Name,EventMessageType.Error));
        //                }
        //            }
        //        }
        //    }
        //}

        private void ContentService_Saving(IContentService sender,ContentSavingEventArgs eventArgs) {
            ActionModel save = null;
            int _currentUserId;

            using(var contextReference = _context.EnsureUmbracoContext()) {
                _currentUserId = contextReference.UmbracoContext.Security.IsAuthenticated() ? contextReference.UmbracoContext.Security.CurrentUser.Id : Umbraco.Core.Constants.Security.SuperUserId;
            }

            try {

                using(var scope = _scopeProvider.CreateScope(autoComplete: true)) {
                    var sql = scope.SqlContext.Sql().Select("*").From<ActionModel>()
                                    .Where<ActionModel>(x => x.id == 1);
                    save = scope.Database.Fetch<ActionModel>(sql).FirstOrDefault();
                }
            }
            catch(Exception ex) {
                _logger.Error<ActionModel>("Failed to get Content Protector setting for save action: " + ex.Message);
            }

            foreach(var node in eventArgs.SavedEntities) {
                if(save != null) {
                    if(save.nodes.Split(',').Contains(node.Id.ToString()) || save.disableAction) {
                        if(!save.userExceptions.Split(',').Contains(_currentUserId.ToString())) {
                            eventArgs.CancelOperation(new EventMessage("Action rejected. Contact website admin","You cannot save " + node.Name,EventMessageType.Error));
                        }
                    }
                }
            }
        }

        public void Terminate() {
        }
    }
}