/*
* JobSeekers area javascript
*/
var _table = null;
var _tableComments = null;
var _tableHistory = null;
var _filter = '';
var _citiesLookup = new Array();

$(document).ready(function () {

    //reset on page load
    //remove any localstorage uless its coming from the users area
    var referrer = document.referrer;

    //store in localstorage to keep track if we cleared the state or not
    //this is a fix for IE,Edge and FF
    var _wasCleared = localStorage.getItem('WorkBC_JobSeeker_WasLoaded');
    if (_wasCleared === null)
        _wasCleared = false;

    if (referrer === null || referrer.toLowerCase().indexOf('/jobseekers/') == -1 && !_wasCleared) {
        if (_table && _table.state) {
            _table.state.clear();
        }
        window.location.reload();
        localStorage.setItem('WorkBC_JobSeeker_WasLoaded', true);
    }
    else {
        localStorage.removeItem('WorkBC_JobSeeker_WasLoaded');
    }

    //implement tooltip
    $('[data-toggle="tooltip"]').tooltip();

    //datatables search textbox
    $('.search__input').on('keyup', function () {
        _table.search($(this).val()).draw();
    });

    //page size dropdown
    $('#inputResultsCount').on('change', function () {
        var pageSize = $(this).val();
        _table.page.len(pageSize).draw();
    });

    //reset filters button
    $('#btn-reset').on('click', function () {
        $('.search__input').val('');
        _filter = '';
        _table.state.clear();
        _table.search($(this).val()).draw();
        $('.btn-search-filter').removeClass('active');
        $('.btn-all').addClass('active');
        $(".datatable-container").removeClass("filtered-status");
    });

    //filter buttons
    $('.btn-all').on('click', function () {
        $(".datatable-container").removeClass("filtered-status");
        _filter = '';
        _table.ajax.reload();

        $('.btn-search-filter').removeClass('active');
        $(this).addClass('active');
    });

    $('.btn-awaiting').on('click', function () {
        $(".datatable-container").addClass("filtered-status");
        _filter = 'Pending';
        _table.ajax.reload();

        $('.btn-search-filter').removeClass('active');
        $(this).addClass('active');
    });

    $('.btn-active').on('click', function () {
        $(".datatable-container").addClass("filtered-status");
        _filter = 'Active';
        _table.ajax.reload();

        $('.btn-search-filter').removeClass('active');
        $(this).addClass('active');
    });

    $('.btn-deactivated').on('click', function () {
        $(".datatable-container").addClass("filtered-status");
        _filter = 'Deactivated';
        _table.ajax.reload();

        $('.btn-search-filter').removeClass('active');
        $(this).addClass('active');
    });

    $('.btn-deleted').on('click', function () {
        $(".datatable-container").addClass("filtered-status");
        _filter = 'Deleted';
        _table.ajax.reload();

        $('.btn-search-filter').removeClass('active');
        $(this).addClass('active');
    });

    if ($("#dtJobSeekers").length) {
        // for pagination scrolling
        var oldStart = 0;

        //Have to have this on the VIEW because we need to have access to >>Url.Action<< and we will not have access to that in the JS file
        _table = $('#dtJobSeekers').DataTable({
            "pagingType": "full_numbers_no_ellipses",
            "proccessing": true,
            "serverSide": true,
            "responsive": true,
            "bFilter": true,
            "stateSave": true, //keep the state 
            "pageLength": +$('#inputResultsCount').val(),
            // hide pager if there is only one page of records
            "fnDrawCallback": function (oSettings) {
                if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                    $(oSettings.nTableWrapper).find('.dataTables_paginate').hide();
                } else {
                    $(oSettings.nTableWrapper).find('.dataTables_paginate').show();
                }

                var api = this.api();
                var pageStart = api.page.info().start;
                var pageLength = api.page.info().length;
                var pageEnd = numberWithCommas(+pageStart + +pageLength);
                if (+api.page.info().end < (+pageStart + +pageLength)) {
                    pageEnd = numberWithCommas(api.page.info().end);
                }
                var records_displayed = numberWithCommas(api.page.info().recordsDisplay);

                if (records_displayed != '0') {
                    $('#lblShowing').html('Showing ' + numberWithCommas(pageStart + 1) + '-' + pageEnd + ' of ' + records_displayed + ' job seeker' + (records_displayed != 1 ? 's' : ''));
                } else {
                    $('#lblShowing').html('');
                }

                //once screen rendered, hook the click events for the delete button
                $('.delete-button').on('click', function () {
                    //read the user-id field from the data attribute and set the hidden field for the POST
                    var userId = $(this).data('userid');
                    $('#DeleteUserId').val(userId);
                });

                //edit locked job-seeker button click event
                $('.edit-locked-button').on('click', function () {

                    //read the data attributes and set the fields
                    var userId = $(this).data('userid');
                    var dateLocked = $(this).data('date-locked');
                    var timeLocked = $(this).data('time-locked');
                    var userLockedDisplayName = $(this).data('user-locked-displayname');
                    var href = $('#lnkViewAsReadonly').attr('href');
                    var lockedByCurrentAdmin = $(this).data('continue-editing');

                    $('#currentUserId').val(userId);
                    $('#currentUserDateLocked').html(dateLocked);
                    $('#currentUserTimeLocked').html(timeLocked);
                    $('#currentUserLockedDisplayName').html(userLockedDisplayName);
                    $('#lnkViewAsReadonly').attr('href', href.split('?')[0] + '?userid=' + userId);

                    if (lockedByCurrentAdmin)
                        $('#lnkViewAsReadonly').text('Continue editing');
                    else
                        $('#lnkViewAsReadonly').text('View as read-only');
                });

                // scroll to the top if pagination page has changed
                if (oSettings._iDisplayStart !== oldStart) {
                    $('html, body').animate({ scrollTop: $(".btn-all").offset().top - 50 }, 'fast');
                    oldStart = oSettings._iDisplayStart;
                }

                $(".dataTables_paginate").removeClass("wait");

                if (pageLength) {
                    $("#inputResultsCount").val(pageLength);
                }
            },
            "language": {
                "emptyTable": "No job seeker accounts match your search criteria",
                "info": "Showing _START_ to _END_ of _TOTAL_ job seekers",
                "infoFiltered": "",
                "paginate": {
                    "first": "<i class=\"fa fa-angle-left\"></i><i class=\"fa fa-angle-left\"></i>",
                    "last": "<i class=\"fa fa-angle-right\"></i><i class=\"fa fa-angle-right\"></i>",
                    "previous": "<i class=\"fa fa-angle-left\"></i>",
                    "next": "<i class=\"fa fa-angle-right\"></i>"
                }
            },
            "ajax": {
                url: "/jobseekers/UserSearch/JobSeekerSearch",
                type: 'GET',
                data: function (d) {
                    d.filter = _filter;
                },
                dataFilter: function (inData) {
                    //success
                    return inData;
                },
                error: function (err, status) {
                    // what error is seen(it could be either server side or client side.
                    console.log(err);
                }
            },
            "stateSaveCallback": function (settings, data) {
                //add status to localstorage
                data.status = _filter;
                localStorage.setItem('DataTables_' + settings.sInstance, JSON.stringify(data));
            },
            "stateLoadCallback": function (settings) {
                return JSON.parse(localStorage.getItem('DataTables_' + settings.sInstance));
            },
            "stateLoadParams": function (settings, data) {
                $('.search__input').val(data.search.search);
                _filter = data.status;

                $('.btn-search-filter').removeClass('active');
                $(".datatable-container").addClass("filtered-status");

                switch (_filter) {
                    case 'Deactivated':
                        $('.btn-deactivated').addClass('active');
                        break;
                    case 'Active':
                        $('.btn-active').addClass('active');
                        break;
                    case 'Pending':
                        $('.btn-awaiting').addClass('active');
                        break;
                    case 'Deleted':
                        $('.btn-deleted').addClass('active');
                        break;
                    case '':
                        $('.btn-all').addClass('active');
                        $(".datatable-container").removeClass("filtered-status");
                        break;
                }
            },
            "createdRow": function (row, data, dataIndex) {
                if (data['accountStatus'] === 'Deleted') {
                    $(row).addClass('deleted');
                }
                else if (data['lockedByAdminUserId'] > 0) {
                    $(row).addClass('locked');
                }
            },
            "columns": [
                { data: 'Id', render: function (data, type, item) { return item.id; }, visible: false },
                { data: 'AccountStatus', title: 'Status', render: function (data, type, item) { return '<span>' + item.accountStatus + '</span>'; }, className: 'status' },
                { data: 'FirstName', title: 'First Name', render: function (data, type, item) { return '<span>' + item.firstName + '</span>'; }, className: 'firstname' },
                { data: 'LastName', title: 'Last Name', render: function (data, type, item) { return item.lastName; } },
                { data: 'Email', title: 'Email Address', render: function (data, type, item) { return item.email; } },
                { data: 'City', title: 'Location', render: function (data, type, item) { return getCity(item); }, orderable: false },
                { data: 'LastModified', title: 'Last Updated', render: function (data, type, item) { return item.lastModified; } },
                { data: 'DateRegistered', title: 'Registered Date', render: function (data, type, item) { return item.dateRegistered; } },
                { data: 'Actions', title: 'Actions', orderable: false, render: function (data, type, item) { return getActions(item) } },
                { data: 'LockedByAdminUserId', render: function (data, type, item) { return item.lockedByAdminUserId; }, visible: false },
                { data: 'AdminDisplayName', render: function (data, type, item) { return item.adminDisplayName; }, visible: false },
                { data: 'DateLocked', render: function (data, type, item) { return item.dateLocked; }, visible: false },
                { data: 'TimeLocked', render: function (data, type, item) { return item.timeLocked; }, visible: false },
                { data: 'LockedByCurrentAdmin', render: function (data, type, item) { return item.lockedByCurrentAdmin; }, visible: false },
            ],
            "columnDefs": [
                { className: "status", "targets": [1] },
                { className: "emails", "targets": [4] },
                { className: "actions", "targets": [8] }
            ]
        })
            .on('xhr.dt', function (e, settings, json, xhr) {
                if (json) {
                    //update filter totals
                    $('#spAll').html(numberWithCommas(json.totalActive + json.totalDeleted + json.totalDeactivated + json.totalWaiting));
                    $('#spActive').html(numberWithCommas(json.totalActive));
                    $('#spDeleted').html(numberWithCommas(json.totalDeleted));
                    $('#spDeactivated').html(numberWithCommas(json.totalDeactivated));
                    $('#spPending').html(numberWithCommas(json.totalWaiting));

                    _table.columns(".status").visible($('.btn-all').hasClass('active'));
                }
            })
            .on('page.dt', function () {
                $(".dataTables_paginate").addClass("wait");
                $(":focus").blur();
            });

        var getCity = function (item) {

            if (item.country === 'Canada' && item.province && item.province.length && item.province !== 'BC') {
                return item.province + ", Canada";
            } else if (item.country && item.country.length && item.country !== 'Canada') {
                return item.country;
            } else if (item.country === 'Canada' && item.province === 'BC') {
                // if the country is Canada and the province is BC but there is no city then 
                // display blank (the user importer sets these defaults for all migrated accounts 
                // without location info)
                if (!item.city || !item.city.length) {
                    return '';
                } else {
                    return item.city + ', BC';
                }
            } else {
                // default
                return (item.city ? item.city + ', ' : '') + (item.province ? item.province + ', ' : '') + (item.country ? item.country : '');
            }
        };

        var getActions = function (item) {
            var editUrl = '/jobseekers/UserManagement/EditUser?userid=' + item.id;

            var viewButton = '<a href="' + editUrl + '" class="btn btn-inline"><i class="fa fa-pencil fa-left"></i><span>View</span></a>';
            var editButton = '<a href="' + editUrl + '" class="btn btn-inline"><i class="fa fa-pencil fa-left"></i><span>Edit</span></a>';
            var deleteButton = '<button type="button" class="btn btn-inline delete-button" data-toggle="modal" data-target="#deleteModal" data-userId="'
                + item.id + '"><i class="fa fa-trash fa-left"></i><span>Delete</span></button>';
            var lockedEditButton = '<button type="button" class="btn btn-inline edit-locked-button" data-toggle="modal" data-target="#lockedModal" data-userId="' + item.id + '" data-date-locked="' + item.dateLocked + '"  data-time-locked="' + item.timeLocked + '" data-user-locked-displayname="' + item.adminDisplayName + '" data-continue-editing="' + item.lockedByCurrentAdmin + '"> <i class="fa fa-pencil fa-left"></i>Edit</button>';

            return item.accountStatus.toLowerCase() !== 'deleted'
                ? '<div class="actions__btn-group">' + (item.lockedByAdminUserId > 0 ? lockedEditButton : editButton) + deleteButton
                : viewButton + '</div>';
        };

        var numberWithCommas = function (x) {
            return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        };

    }

    //only on edit/create screen
    if ($('.ddlCities').length) {

        //hide/show cities based on province selected
        checkForCities();

        //fetch all the cities
        getCities();

        $('.ddlCities').select2({
            placeholder: 'Please select'
        })
            .on('select2:select', function (e) {
                //Logic to show or hide the region area based on if there are more than one location with the same name
                var data = e.params.data;

                //update hidden fields for location
                $('#LocationId').val(data.id);
                $('#City').val(data.text);

                if (_citiesLookup.find(x => x.city === data.text).isDuplicate) {

                    //get the regions for this city
                    $.ajax({
                        type: "GET",
                        url: '/jobseekers/UserManagement/GetRegionsByLocationName?CityName=' + encodeURIComponent(data.text),
                        success: function (data) {

                            //clear previous values
                            $('.region').html('');

                            var found = false;

                            //load available regions for the selected location
                            for (var i = 0; i < data.length; i++) {
                                if ($('#RegionId').val() == data[i].id) {
                                    $('.region').append('<option value="' + data[i].id + '" selected="selected">' + data[i].name + '</option>');
                                    found = true;
                                }
                                else {
                                    $('.region').append('<option value="' + data[i].id + '">' + data[i].name + '</option>');
                                }
                            }

                            if (!found) {
                                $('.region').prepend('<option value="" selected="selected">-- Select a Region --</option>');
                            }

                            $('#IsDuplicateLocation').val(true);

                            $('#dvRegion').show();
                        },
                        fail: function (data) {
                            console.log('FAILED - REGION');
                        }
                    });
                }
                else {
                    $('#RegionId').val('-1');
                    $('#IsDuplicateLocation').val(false);
                    $('#dvRegion').hide();
                }
            });

        //check for locationID and city and pre-populate it (Edit screen)
        if ($('#LocationId').val() != null && $('#LocationId').val() != '' && $('#City').val() != null && $('#City').val() != '') {
            $('.ddlCities').val($('#LocationId').val());
        }

        //check for regionID pre-populate it (Edit screen)
        if ($('#IsDuplicateLocation').val() != null && $('#IsDuplicateLocation').val() == 'True') {

            $.ajax({
                type: "GET",
                url: '/jobseekers/UserManagement/GetRegionsByLocationId?LocationId=' + $('#LocationId').val(),
                success: function (data) {

                    //clear previous values
                    $('.region').html('');

                    var found = false;

                    //load available regions for the selected location
                    for (var i = 0; i < data.length; i++) {
                        if ($('#RegionId').val() == data[i].id) {
                            $('.region').append('<option value="' + data[i].id + '" selected="selected">' + data[i].name + '</option>');
                            found = true;
                        }
                        else {
                            $('.region').append('<option value="' + data[i].id + '">' + data[i].name + '</option>');
                        }
                    }

                    if (!found) {
                        $('.region').prepend('<option value="" selected="selected">-- Select a Region --</option>');
                    }

                    $('#dvRegion').show();
                },
                fail: function (data) {
                    console.log('FAILED - REGION');
                }
            });
        }

        $('.region').on('change', function () {
            $('#RegionId').val($(this).val());
        });

        //country dropdown list
        $('.ddlCountries').on('change', function () {
            $.ajax({
                type: "GET",
                url: '/jobseekers/UserManagement/GetProvinces?CountryId=' + $(this).val(),
                success: function (data) {

                    //clear list of current provinces
                    $('.ddlProvinces').empty();

                    //loop and populate provinces based on country ID
                    for (var i = 0; i < data.length; i++) {
                        $('.ddlProvinces').append(new Option(data[i].name, data[i].provinceId));
                    }

                    if (data.length === 0) {
                        $('.ddlProvinces').append(new Option('No province available', -1));
                        $('.dvCities').hide();
                        $('#SelectProvinces').hide();
                    } else {
                        $('#SelectProvinces').show();
                    }
                },
                fail: function (data) {
                    console.log('FAILED - PROVINCE');
                }
            });

        });

        $('.ddlProvinces').on('change', function (data) {
            checkForCities();
        });

        $('.region').on('change', function (data) {
            $('#RegionId').val($(this).val());
        });

        //check for locationID and city and pre-populate it (Edit screen)
        if ($('#LocationId').val() != null && $('#LocationId').val() != '' && $('#City').val() != null && $('#City').val() != '') {
            $('.ddlCities').append('<option value="' + $('#LocationId').val() + '" selected="selected">' + $('#City').val() + '</option>');

            console.log($('#IsDuplicateLocation').val());
            if ($('#IsDuplicateLocation').val() == 'true') {

                //get the regions for this city
                $.ajax({
                    type: "GET",
                    url: '/jobseekers/UserManagement/GetRegionsByLocationName?CityName=' + $('#City').val(),
                    success: function (data) {

                        //clear previous values
                        $('.region').html('');

                        var found = false;

                        //load available regions for the selected location
                        for (var i = 0; i < data.length; i++) {
                            if (data[i].id == $('#RegionId').val()) {
                                $('.region').append('<option value="' + data[i].id + '" selected="selected">' + data[i].name + '</option>');
                                found = true;
                            }
                            else {
                                $('.region').append('<option value="' + data[i].id + '">' + data[i].name + '</option>');
                            }
                        }

                        if (!found) {
                            $('.region').prepend('<option value="" selected="selected">-- Select a Region --</option>');
                        }

                        $('#dvRegion').show();
                    },
                    fail: function (data) {
                        console.log('FAILED - REGION');
                    }
                });
            }
        }

        //check, if country is not Canada do not show Canadian provinces
        if ($('#CountryId').val() != '37') {

            //clear list of current provinces
            $('.ddlProvinces').empty();
            $('.ddlProvinces').append(new Option('No province available', -1));
            $('.dvCities').hide();
        }
    }

    //job seeker comments listing page
    if ($('#dtComments').length > 0) {
        _tableComments = $('#dtComments').DataTable({
            "pagingType": "full_numbers_no_ellipses",
            "responsive": true,
            "bFilter": true,
            "ordering": false,
            "language": {
                "emptyTable": "No comments available",
                "info": "Showing _START_ to _END_ of _TOTAL_ comments",
                "infoFiltered": "",
                "paginate": {
                    "first": "<i class=\"fa fa-angle-left\"></i><i class=\"fa fa-angle-left\"></i>",
                    "last": "<i class=\"fa fa-angle-right\"></i><i class=\"fa fa-angle-right\"></i>",
                    "previous": "<i class=\"fa fa-angle-left\"></i>",
                    "next": "<i class=\"fa fa-angle-right\"></i>"
                }
            },
            "pageLength": 10,
            // hide pager if there is only one page of records
            "fnDrawCallback": function (oSettings) {
                if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                    $(oSettings.nTableWrapper).find('.dataTables_paginate').hide();
                } else {
                    $(oSettings.nTableWrapper).find('.dataTables_paginate').show();
                }

                var api = this.api();
                var start = api.page.info().start;
                var end = api.page.info().end;
                var records_displayed = api.page.info().recordsDisplay;

                $('#lblShowing').html('Showing ' + (start + 1) + '-' + end + ' of ' + records_displayed + ' comments');
            }
        }).on('page.dt', function () {
            $('html, body').animate({
                scrollTop: $(".action-sidebar").offset().top
            }, 'slow');
        });

        //update sticky status
        $('.sticky').on('click', function () {

            console.log();
            $.ajax({
                type: "POST",
                url: '/jobseekers/UserManagement/ToggleComment',
                data: { commentId: $(this).data('comment-id'), jobSeekerId: $(this).data('jobseeker-id') },
                success: function (data) {
                    //refresh table
                    window.location.href = window.location.href;
                },
                fail: function (data) {
                    console.log('FAILED - UPDATE COMMENT');
                }
            });
        });

        $('.print').on('click', function () {
            window.print();
        });
    }

    //job seeker history listing page
    if ($('#dtHistory').length > 0) {


        _tableHistory = $('#dtHistory').DataTable({
            "pagingType": "full_numbers_no_ellipses",
            "responsive": true,
            "bFilter": true,
            "ordering": false,
            "language": {
                "emptyTable": "No logs available",
                "info": "Showing _START_ to _END_ of _TOTAL_ logs",
                "infoFiltered": "",
                "paginate": {
                    "first": "<i class=\"fa fa-angle-left\"></i><i class=\"fa fa-angle-left\"></i>",
                    "last": "<i class=\"fa fa-angle-right\"></i><i class=\"fa fa-angle-right\"></i>",
                    "previous": "<i class=\"fa fa-angle-left\"></i>",
                    "next": "<i class=\"fa fa-angle-right\"></i>"
                }
            },
            "pageLength": 10,
            // hide pager if there is only one page of records
            "fnDrawCallback": function (oSettings) {
                if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                    $(oSettings.nTableWrapper).find('.dataTables_paginate').hide();
                } else {
                    $(oSettings.nTableWrapper).find('.dataTables_paginate').show();
                }

                var api = this.api();
                var start = api.page.info().start;
                var end = api.page.info().end;
                var records_displayed = api.page.info().recordsDisplay;

                $('#lblShowing').html('Showing ' + (start + 1) + '-' + end + ' of ' + records_displayed + ' logs');
            }
        }).on('page.dt', function () {
            $('html, body').animate({
                scrollTop: $(".page-title").offset().top
            }, 'slow');
        });
    }

    if ($("#page-add-comments").length) {
        $('#Comment').on('keyup', function () {
            if ($(this).val().length > 0) {
                console.log($('input[type="submit"]'));
                $('button[type="submit"]').prop("disabled", false);
            }
            else {
                $('button[type="submit"]').prop("disabled", true);
            }
        });
    }

});

function checkForCities() {
    var province = $(".ddlProvinces :selected").text();

    if (province === 'British Columbia') {
        //show cities
        $('.dvCities').show();
    }
    else {
        //hide cities
        $('.dvCities').hide();
    }
}

function getCities() {
    $.get('/jobseekers/UserManagement/GetCities',
        function (data) {

            $.map(data, function (obj) {

                var arrObj = { city: obj.city, isDuplicate: obj.isDuplicate };
                var item = _citiesLookup.find(x => x.city === obj.city);

                if (item == null) {
                    $('.ddlCities').append(`<option value="${obj.id}" data-isDuplicate="${obj.isDuplicate}">${obj.city}</option>`);
                    _citiesLookup.push(arrObj);
                }
            });

            $('.ddlCities').trigger('select2.change');
        });
}