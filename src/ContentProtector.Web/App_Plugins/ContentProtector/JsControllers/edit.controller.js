angular.module("umbraco").controller("cp.edit.controller", function (navigationService, $routeParams, contentResource, editorService, $route, cpResource, usersResource, notificationsService) {
	var vm = this;
	vm.currentSelectedUsers = [];
	vm.selectedContentIds = [];
	vm.disableAction = undefined;
	vm.actionName = "";

	var model = {
		id: undefined,
		name: undefined,
		nodes: undefined,
		disableAction: undefined,
		userExceptions: undefined,
		editedDate: undefined,
		editedby: undefined
	};

	vm.showView = false;
	vm.failToRetrieve = false;

	cpResource.getById($routeParams.id).then(function (response) {
		if (response.data !== null) {
			//Highlight current node onclick
			navigationService.syncTree({ tree: "contentProtector", path: [$routeParams.id] });
			model.id = response.data.Id;
			model.name = response.data.Name;
			model.nodes = response.data.Nodes;
			model.disableAction = response.data.DisableAction;
			model.userExceptions = response.data.UserExceptions;
			model.editedDate = response.data.LastEdited;
			model.editedby = response.data.LastEditedBy;

			vm.disableAction = response.data.DisableAction;
			vm.actionName = response.data.Name.charAt(0).toUpperCase() + response.data.Name.slice(1);

			if (response.data.UserExceptions !== "") {
				getExceptionUsers(response.data.UserExceptions);
			}
			if (response.data.Nodes !== undefined) {
				if (response.data.Nodes !== "") {
					getNodes(response.data.Nodes);
				}
			}
		}

	}).then(function () {
		//Temporary
	}).then(function () {
		vm.editedDate = model.editedDate;
		vm.editedByUser = model.editedby;

		vm.openUserPicker = function () {
			var userPicker = {
				title: "User Exceptions",
				selection: vm.currentSelectedUsers,
				submit: function submit(model) {
					vm.currentSelectedUsers = model.selection;
					computeUserExceptionIds(model.selection);
					editorService.close();
				},
				close: function close() {
					editorService.close();
				}
			};
			editorService.userPicker(userPicker);
		};

		vm.openContentPicker = function openContentPicker() {
			var contentPicker = {
				title: "Select Content to Protect",
				section: 'content',
				treeAlias: 'content',
				multiPicker: true,
				hideSubmitButton: false,
				hideHeader: true,
				show: true,
				submit: function submit(model) {
					if (model.selection) {
						angular.forEach(model.selection, function (item) {
							multiSelectItem(item, vm.selectedContentIds);
						});
					}
					computeContentIds(model.selection);
					editorService.close();
				},
				close: function close() {
					editorService.close();
				}
			};
			editorService.treePicker(contentPicker);
		};
	}).then(function () {
		if (model.id === undefined || ($routeParams.id < 1 || $routeParams.id > 8)) {
			vm.showView = true;
			vm.failToRetrieve = true;
		}
		else {
			vm.showView = true;
		}
	});

	vm.cancel = function () {
		$route.reload();
	};

	vm.toggleDisableAction = function () {
		if (vm.disableAction === false) {
			vm.disableAction = true;
		}
		else if (vm.disableAction === true) {
			vm.disableAction = false;
		}
	};

	vm.save = function () {
		computeUserExceptionIds(vm.currentSelectedUsers);
		computeContentIds(vm.selectedContentIds);
		model.disableAction = vm.disableAction;
		vm.saveButtonState = 'busy';

		cpResource.save(model).then(function (response) {
			notificationsService.success('Saved ', " settings for " + model.name + " action.");
			vm.saveButtonState = 'success';

		}, function (error) {
			vm.saveButtonState = 'error';
			notificationsService.error('Error saving...', error.data.ExceptionMessage);
		});
	};

	vm.removeSelectedItem = function (index, selection) {
		selection.splice(index, 1);
	};

	function getExceptionUsers(users) {
		for (item of users.split(",")) {
			usersResource.getUser(item).then(function (data) {
				vm.currentSelectedUsers.push(data);
			});
		}
	}

	function getNodes(nodes) {
		contentResource.getByIds(nodes.split(",")).then(function (data) {
			vm.selectedContentIds = data;
		});
	}

	function multiSelectItem(item, selection) {
		var found = false;
		// check if item is already in the selected list
		if (selection.length > 0) {
			angular.forEach(selection, function (selectedItem) {
				if (selectedItem.udi === item.udi) {
					found = true;
				}
			});
		}
		// only add the selected item if it is not already selected
		if (!found) {
			selection.push(item);
		}
	}

	function computeContentIds(items) {
		model.nodes = "";
		for (let i = 0; i < items.length; i++) {
			if (i == (items.length - 1)) {
				model.nodes += items[i].id;
			}
			else {
				model.nodes += items[i].id + ",";
			}
		}
	}

	function computeUserExceptionIds(items) {
		model.userExceptions = "";
		for (let i = 0; i < items.length; i++) {
			if (i == (items.length - 1)) {
				model.userExceptions += items[i].id;
			}
			else {
				model.userExceptions += items[i].id + ",";
			}
		}
	}
});