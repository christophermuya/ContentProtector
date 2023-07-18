angular.module("umbraco").controller("cp.content.controller", function (navigationService) {
	var vm = this;
	var backofficeView = Umbraco.Sys.ServerVariables.umbracoSettings.appPluginsPath + '/ContentProtector/backoffice/contentProtector/';

	navigationService.syncTree({ tree: "contentProtector", path: ['-1'] });

	vm.page = {
		title: 'Content Protector',
		description: 'Protect your content nodes by preventing certain actions from succeeding',
		navigation: [
			{
				name: 'Protection',
				alias: 'protection',
				icon: 'icon-lock',
				view: backofficeView + 'protection.html',
				active: true
			},
			{
				name: 'Info',
				alias: 'info',
				icon: 'icon-info',
				view: backofficeView + 'info.html'
			}
		]
	};
});