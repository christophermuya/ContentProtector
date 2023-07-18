using ContentProtector.Core.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.Trees;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;

namespace ContentProtector.Core.Controllers
{
	[PluginController(ContentProtectorSettings.PluginAreaName)]
	[Tree(Umbraco.Cms.Core.Constants.Applications.Settings,
		treeAlias: ContentProtectorSettings.Alias, 
		TreeTitle = ContentProtectorSettings.TreeTitle, 
		TreeGroup = Umbraco.Cms.Core.Constants.Trees.Groups.ThirdParty, SortOrder = 5)]
	public class ContentProtectorTreeController : TreeController
	{
		private readonly IMenuItemCollectionFactory _menuItemCollectionFactory;

		public ContentProtectorTreeController(ILocalizedTextService localizedTextService, UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection, IEventAggregator eventAggregator, IMenuItemCollectionFactory menuItemCollectionFactory) : base(localizedTextService, umbracoApiControllerTypeCollection, eventAggregator)
		{
			_menuItemCollectionFactory = menuItemCollectionFactory;
		}

		protected override ActionResult<TreeNode?> CreateRootNode(FormCollection queryStrings)
		{
			var rootResult = base.CreateRootNode(queryStrings);
			if (rootResult.Result is not null)
			{
				return rootResult;
			}

			var root = rootResult.Value;

			root.RoutePath = $"{Umbraco.Cms.Core.Constants.Applications.Settings}/{ContentProtectorSettings.Alias}/{"content"}";
			root.Icon = "icon-lock";
			root.HasChildren = true;
			root.MenuUrl = null;

			return root;
		}

		protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, FormCollection queryStrings)
		{
			if (id != Umbraco.Cms.Core.Constants.System.Root.ToInvariantString())
			{
				throw new NotSupportedException();
			}
			
			var nodes = new TreeNodeCollection();

			var save = CreateTreeNode("1", "-1", queryStrings, "Save", "icon-disk-image", false);
			save.MenuUrl = null;

			var publish = CreateTreeNode("6", "-1", queryStrings, "Publish", "icon-navigation-top", false);
			publish.MenuUrl = null;

			var unpublished = CreateTreeNode("7", "-1", queryStrings, "Unpublish", "icon-navigation-bottom", false);
			unpublished.MenuUrl = null;

			var trash = CreateTreeNode("3", "-1", queryStrings, "Trash", "icon-trash", false);
			trash.MenuUrl = null;

			var delete = CreateTreeNode("4", "-1", queryStrings, "Delete", "icon-trash-alt", false);
			delete.MenuUrl = null;

			var rollback = CreateTreeNode("8", "-1", queryStrings, "RollBack", "icon-alarm-clock", false);
			rollback.MenuUrl = null;

			var rename = CreateTreeNode("9", "-1", queryStrings, "Rename", "icon-edit", false);
			rollback.MenuUrl = null;

			nodes.Add(save);
			nodes.Add(publish);
			nodes.Add(unpublished);
			nodes.Add(trash);
			nodes.Add(delete);
			nodes.Add(rollback);
			nodes.Add(rename);

			return nodes;
		}

		protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, FormCollection queryStrings)
		{
			var menu = _menuItemCollectionFactory.Create();

			if (id != Umbraco.Cms.Core.Constants.System.Root.ToInvariantString())
			{
				return menu;
			}

			menu.Items.Add(new CreateChildEntity(LocalizedTextService));
			menu.Items.Add(new RefreshNode(LocalizedTextService, true));

			return menu;
		}
	}
}