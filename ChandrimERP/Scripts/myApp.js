var app = angular.module('myApp', ['angularTreeview']);
app.controller('myController', ['$scope', '$http', function ($scope, $http) {
    $http.get('/Accounts/GetFileStructure').then(function (response) {
        $scope.List = response.data.treeList;
    })
}])