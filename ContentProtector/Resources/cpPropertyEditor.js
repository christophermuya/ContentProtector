//angular.module("umbraco.resources").factory("cpPropertyEditor", function (editorService) {
//    return {
//        userPicker: function () {
//            var result = [];
//            var userPicker = {
//                title: "User Exceptions",
//                //selection: vm.currentSelectedUsers,
//                submit: function submit(model) {
//                    console.log(model.selection);
//                    result = model.selection;
//                    //vm.currentSelectedUsers = model.selection;
//                    editorService.close();
//                },
//                close: function close() {
//                    editorService.close();
//                }
//            };
//            editorService.userPicker(userPicker);
//            return result;
//        }
//    };
//});