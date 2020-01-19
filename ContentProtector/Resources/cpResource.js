angular.module("umbraco.resources").factory("cpResource", function ($http) {
    return {
        getById: function (id) {
            return $http.get("backoffice/api/ContentProtectorApi/GetById?id=" + id);
        },
        getAll: function () {
            return $http.get("backoffice/api/ContentProtectorApi/GetAll");
        },
        save: function (model) {
            return $http.post("backoffice/api/ContentProtectorApi/Save", angular.toJson(model));
        }  
    };
}); 