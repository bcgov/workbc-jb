/*
* AdminAccounts area javascript
*/
var _filter = '';
var _table = null;
var _shadowTable = null;

$(document).ready(function () {

    // only run this code on the search page
    if ($("#page-admin-accounts").length) {
        //delete button click event
        $('.delete-button').on('click', function () {

            //read the user-id field from the data attribute and set the hidden field for the POST
            var userId = $(this).data('userid');
            $('#userId').val(userId);
        });

        //edit locked admin user button click event
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
            $('#lnkViewAsReadonly').attr('href', href + '/' + userId);

            if (lockedByCurrentAdmin == 'True')
                $('#lnkViewAsReadonly').text('Continue editing');
            else
                $('#lnkViewAsReadonly').text('View as read-only');
        });

        //reset filters
        $('#btn-reset').on('click', function () {
            $('.search__input').val('');
            $('.search__input').keyup();
            _table.column(3).search('').draw();
            $('.btn-search-filter').removeClass('active');
            $('.btn-all').addClass('active');
            _filter = '';
            setFilterButtons();
        });

        //filter buttons
        $('.btn-reporting').on('click', function () {
            _filter = 'Reporting';
            _table.column(3).search('Reporting').draw();

            $('.btn-search-filter').removeClass('active');
            $(this).addClass('active');
        });

        $('.btn-admin').on('click', function () {
            _filter = 'Regular administrator';
            _table.column(3).search('Regular administrator').draw();

            $('.btn-search-filter').removeClass('active');
            $(this).addClass('active');
        });

        $('.btn-super-user').on('click', function () {
            _filter = 'Super administrator';
            _table.column(3).search('Super administrator').draw();

            $('.btn-search-filter').removeClass('active');
            $(this).addClass('active');
        });

        $('.btn-all').on('click', function () {
            _filter = '';
            _table.column(3).search('').draw();

            $('.btn-search-filter').removeClass('active');
            $(this).addClass('active');
        });
        
        //Init the datatabes object
        _table = $('#dtAdminUsers').DataTable({
            "pagingType": "full_numbers_no_ellipses",
            "responsive": true,
            "bFilter": true,
            "stateSave": true, //keep the state
            "language": {
                "emptyTable": "No accounts match your search criteria",
                "zeroRecords": "No accounts match your search criteria",
                "info": "Showing _START_ to _END_ of _TOTAL_ users",
                "infoFiltered": "",
                "paginate": {
                    "first": "<i class=\"fa fa-angle-left\"></i><i class=\"fa fa-angle-left\"></i>",
                    "last": "<i class=\"fa fa-angle-right\"></i><i class=\"fa fa-angle-right\"></i>",
                    "previous": "<i class=\"fa fa-angle-left\"></i>",
                    "next": "<i class=\"fa fa-angle-right\"></i>"
                }
            },
            "pageLength": $('#inputResultsCount').val(),
            // hide pager if there is only one page of records
            "fnDrawCallback": function (oSettings) {
                if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                    $(oSettings.nTableWrapper).find('.dataTables_paginate').hide();
                } else {
                    $(oSettings.nTableWrapper).find('.dataTables_paginate').show();
                }

                var api = this.api();
                var start = api.page.info().start;
                var length = api.page.info().length;
                var end = +start + +length;
                if (+api.page.info().end < (+start + +length)) {
                    end = api.page.info().end;
                }
                var records_displayed = api.page.info().recordsDisplay;

                if (records_displayed != '0') {
                    $('#lblShowing').html('Showing ' + (start + 1) + '-' + end + ' of ' + records_displayed + ' user' + (records_displayed != 1 ? 's' : ''));
                } else {
                    $('#lblShowing').html('');
                }
            },
            "stateSaveCallback": function (settings, data) {
                //add status to localstorage
                data.status = _filter;
                data.search.search = $('.search__input').val();
                localStorage.setItem('DataTables_' + settings.sInstance, JSON.stringify(data));
            },
            "stateLoadCallback": function (settings) {
                return JSON.parse(localStorage.getItem('DataTables_' + settings.sInstance));
            },
            "stateLoadParams": function (settings, data) {
                $('.search__input').val(data.search.search);
                _filter = data.status;

                $('.btn-search-filter').removeClass('active');

                switch (_filter) {
                    case 'Reporting':
                        $('.btn-reporting').addClass('active');
                        break;
                    case 'Regular administrator':
                        $('.btn-admin').addClass('active');
                        break;
                    case 'Super administrator':
                        $('.btn-super-user').addClass('active');
                        break;
                    case 'Deleted':
                        $('.btn-deleted').addClass('active');
                        break;
                    case '':
                        $('.btn-all').addClass('active');
                        break;
                }

                // send some keyup events to apply the search
                setTimeout(() => { $('.search__input').keyup(); }, 300);
                setTimeout(() => { $('.search__input').keyup(); }, 700);
            },
            "columnDefs": [{
                "targets": 'no-sort',
                "orderable": false
            }]
        }).on('page.dt', function () {
            $('html, body').animate({
                scrollTop: $(".btn-all").offset().top
            }, 'slow');
        });

        //shadow table for retrieving counts without extra filters
        _shadowTable = $('#dtAdminUsersShadow').DataTable({
            "paging": false,
            "bFilter": true,
            "stateSave": true,
            // hide pager if there is only one page of records
            "stateSaveCallback": function (settings, data) {
                //add status to localstorage
                data.status = _filter;
                data.search.search = $('.search__input').val();
                localStorage.setItem('DataTables_' + settings.sInstance, JSON.stringify(data));
            },
            "stateLoadCallback": function (settings) {
                return JSON.parse(localStorage.getItem('DataTables_' + settings.sInstance));
            },
        });

        //Count different user types
        setFilterButtons();

        //Update table result size
        $('#inputResultsCount').on('change', function () {
            var pageSize = $(this).val();
            _table.page.len(pageSize).draw();
        });

        $('.search__input').on('keyup', function () {
            _table.search($(this).val()).draw();
            _shadowTable.search($(this).val()).draw();
            setFilterButtons();
        });

        //This function override the databtable search function
        //We need to do this because we need to search across multiple columns with an OR
        //default behavior will search with an AND and will not return any results.
        //Using "toLowerCase()" to have the search not be case sensitive
        $.fn.dataTable.ext.search.push(
            function (settings, data, dataIndex) {
                //displayname
                if (data[1].toLowerCase().indexOf($('.search__input').val().toLowerCase()) > -1) {
                    return true;
                }
                //idir username
                else if (data[2].toLowerCase().indexOf($('.search__input').val().toLowerCase()) > -1) {
                    return true;
                }
                else {
                    return false;
                }
            }
        );
    }

    // only run this code on the add/edit page
    if ($("#page-edit-admin-user").length) {

        $("button.ad-lookup").click(function (e) {

            var adLookupError = function (message) {
                $("button.ad-lookup").css("background-color", "red").css("border-color", "red");
                $("button.ad-lookup i").removeClass("fa-spin fa-spinner fa-user-plus fa-check").addClass("fa-times");
                $(':input[type="submit"]').prop('disabled', true);
                $('.admin-fields').addClass('d-none');
                $('#SamAccountNameAjaxError').text(message);
            };

            // Assign handlers immediately after making the request,
            // and remember the jqxhr object for this request
            var samName = $("#Username").val();

            if (samName.trim() === "") {
                adLookupError();
                return;
            }

            $("button.ad-lookup i").removeClass("fa-user-plus fa-check fa-times").addClass("fa-spin fa-spinner");

            $.post("UserInfo/" + encodeURI(samName), function (data) {
                $("#DisplayName").val(data.displayName);
                $('#lblDisplayName').text(data.displayName);
                $("#SamAccountName").val(data.samAccountName);
                $("button.ad-lookup").css("background-color", "green").css("border-color", "green");
                $("button.ad-lookup i").removeClass("fa-spin fa-spinner fa-user-plus fa-times").addClass("fa-check");
                $(':input[type="submit"]').prop('disabled', false);
                $('.admin-fields').removeClass('d-none');
                $('#SamAccountNameAjaxError').text('');
            })
                .fail(function (result) {
                    adLookupError(result.responseText);
                });
        });

        $("#Username").keydown(function () {
            $("button.ad-lookup").css("background-color", "").css("border-color", "");
            $("button.ad-lookup i").removeClass("fa-spin fa-spinner fa-check fa-times").addClass("fa-user-plus");
            $("#DisplayName").val("");
            $('#lblDisplayName').text("");
            $("#SamAccountName").val("");
            $('.admin-fields').addClass('d-none');
            $(':input[type="submit"]').prop('disabled', true);
            $('span[data-valmsg-for=SamAccountName]').html('');
        });

        $('#Username').keypress(function (e) {
            var key = e.which;
            if (key === 13)  // the enter key code
            {
                $("button.ad-lookup").click();
                return false;
            }
        }); 

        $('#Username').on('paste', function () {
            setTimeout(() => {
                $("button.ad-lookup").click();
            }, 50);
        }); 

        $('#Username').blur(function () {
            $("button.ad-lookup").click();
        }); 
    }

    function setFilterButtons() {
        //Reporting
        var filteredDataReporting = _shadowTable
            .column(3, { search: 'applied' })
            .data()
            .filter(function (value, index) {
                return value === 'Reporting' ? true : false;
            });
        $('#spReporting').html(filteredDataReporting.count());

        //Admin
        var filteredDataAdmin = _shadowTable
            .column(3, { search: 'applied' })
            .data()
            .filter(function (value, index) {
                return value === 'Admin' ? true : false;
            });
        $('#spAdmin').html(filteredDataAdmin.count());

        //Super User
        var filteredDataSuperUser = _shadowTable
            .column(3, { search: 'applied' })
            .data()
            .filter(function (value, index) {
                return value === 'SuperAdmin' ? true : false;
            });
        $('#spSuperUser').html(filteredDataSuperUser.count());

        //Total
        $('#spTotal').html(filteredDataReporting.count() + filteredDataAdmin.count() + filteredDataSuperUser.count());
    }

});
