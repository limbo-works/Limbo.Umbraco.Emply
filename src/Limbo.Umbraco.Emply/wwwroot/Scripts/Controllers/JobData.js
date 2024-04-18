angular.module("umbraco").controller("Limbo.Umbraco.Emply.JobData.Controller", function ($scope, editorService) {

    const vm = this;

    if (!$scope.model.value) return;

    function parseDate(value) {
        if (!value) return null;
        const date = new Date(value);
        return {
            date: date,
            comment: moment().to(date)
        };
    }

    function getLocalization(value) {
        if (!Array.isArray(value?.localization) || value.localization.length === 0) return null;
        return value.localization.find(x => x.locale === "da-DK")?.value ?? value.localization.length[0].value;
    }

    function init() {

        if (!$scope.model.value) return;
        if (typeof $scope.model.value !== "string") return;

        vm.data = JSON.parse($scope.model.value.substr(1));
        vm.dataSize = $scope.model.value.length / 1024;

        vm.job = {
            id: vm.data.jobId,
            title: getLocalization(vm.data.title),
            created: parseDate(vm.data.created),
            edited: parseDate(vm.data.created),
            deadline: parseDate(vm.data.deadlineUTC),
            location: vm.data.location?.address,
            categories: []
        };

        if (Array.isArray(vm.data.data)) {
            vm.data.data.forEach(function (data) {
                if (!Array.isArray(data.value)) return;
                if (!data.title.localization.find(x => x.value == "Stillingskategori")) return;
                data.value.forEach(function (v) {
                    if (v.title) vm.job.categories.push(getLocalization(v.title));
                });
            });
        }

    }

    vm.showJson = function () {
        editorService.open({
            title: "JSON",
            size: "large",
            view: "/App_plugins/Limbo.Umbraco.Emply/Views/JobDataOverlay.html",
            json: vm.data,
            close: function () {
                editorService.close();
            }
        });
    };

    init();

});