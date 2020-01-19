angular.module("umbraco").controller("cp.protection.controller", function (cpResource) {
    var vm = this;
    vm.Name = "cp.protection";
    vm.model = [];

    cpResource.getAll().then(function (response) {
        if (response.data !== null) {
            vm.model = response.data;
        }        

    }).then(function () {
       
    });   
});
