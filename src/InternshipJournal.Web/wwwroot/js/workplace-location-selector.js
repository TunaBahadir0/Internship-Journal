(function () {
    function createLocationSelector(options) {
        var $country = $(options.countrySelect);
        var $province = $(options.provinceSelect);
        var $district = $(options.districtSelect);
        var locationService = internshipJournal.locations.location;

        function resetSelect($select, placeholder) {
            $select.empty().append($('<option>').val('').text(placeholder));
        }

        function fillSelect($select, items, placeholder) {
            resetSelect($select, placeholder);
            $.each(items, function (index, item) {
                $select.append($('<option>').val(item.id).text(item.name));
            });
        }

        function loadProvinces(countryId, selectedProvinceId) {
            resetSelect($province, options.provincePlaceholder);
            resetSelect($district, options.districtPlaceholder);
            $province.prop('disabled', true);
            $district.prop('disabled', true);

            if (!countryId) {
                return $.Deferred().resolve().promise();
            }

            return locationService.getProvinces(countryId).then(
                function (result) {
                    fillSelect($province, result, options.provincePlaceholder);
                    $province.prop('disabled', false);
                    if (selectedProvinceId) {
                        $province.val(selectedProvinceId);
                    }
                },
                function () {
                    abp.message.error(options.errorMessage);
                }
            );
        }

        function loadDistricts(provinceId, selectedDistrictId) {
            resetSelect($district, options.districtPlaceholder);
            $district.prop('disabled', true);

            if (!provinceId) {
                return $.Deferred().resolve().promise();
            }

            return locationService.getDistricts(provinceId).then(
                function (result) {
                    fillSelect($district, result, options.districtPlaceholder);
                    $district.prop('disabled', false);
                    if (selectedDistrictId) {
                        $district.val(selectedDistrictId);
                    }
                },
                function () {
                    abp.message.error(options.errorMessage);
                }
            );
        }

        $country.on('change', function () {
            loadProvinces($(this).val());
        });

        $province.on('change', function () {
            loadDistricts($(this).val());
        });

        locationService.getCountries().then(
            function (result) {
                fillSelect($country, result, options.countryPlaceholder);

                if (options.selectedCountryId) {
                    $country.val(options.selectedCountryId);
                    loadProvinces(options.selectedCountryId, options.selectedProvinceId).then(function () {
                        if (options.selectedProvinceId) {
                            loadDistricts(options.selectedProvinceId, options.selectedDistrictId);
                        }
                    });
                }
            },
            function () {
                abp.message.error(options.errorMessage);
            }
        );
    }

    window.workplaceLocationSelector = {
        create: createLocationSelector
    };
})();
