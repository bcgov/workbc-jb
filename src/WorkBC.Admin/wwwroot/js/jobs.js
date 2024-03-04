/*
* Jobs area javascript
*/
var _filter = '';
var _table = null;

$(document).ready(function () {

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
    });

    //filter buttons
    $('.btn-all').on('click', function () {
        _filter = '';
        _table.ajax.reload();

        $('.btn-search-filter').removeClass('active');
        $(this).addClass('active');
    });

    $('.btn-federal').on('click', function () {
        _filter = 'Federal';
        _table.ajax.reload();

        $('.btn-search-filter').removeClass('active');
        $(this).addClass('active');
    });

    $('.btn-external').on('click', function () {
        _filter = 'External';
        _table.ajax.reload();

        $('.btn-search-filter').removeClass('active');
        $(this).addClass('active');
    });

    // for pagination scrolling
    var oldStart = 0;

    //Have to have this on the VIEW because we need to have access to >>Url.Action<< and we will not have access to that in the JS file
    _table = $('#dtJobs').DataTable({
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
                $('#lblShowing').html('Showing ' + numberWithCommas(pageStart + 1) + '-' + pageEnd + ' of ' + records_displayed + ' job posting' + (records_displayed != 1 ? 's' : ''));
            } else {
                $('#lblShowing').html('');
            }

            //delete button
            $('.delete-button').on('click', function () {
                var jobId = $(this).data('job-id');
                $('#jobId').val(jobId);
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
            "emptyTable": "No job postings match your search criteria",
            "info": "Showing _START_ to _END_ of _TOTAL_ jobs",
            "infoFiltered": "",
            "paginate": {
                "first": "<i class=\"fa fa-angle-left\"></i><i class=\"fa fa-angle-left\"></i>",
                "last": "<i class=\"fa fa-angle-right\"></i><i class=\"fa fa-angle-right\"></i>",
                "previous": "<i class=\"fa fa-angle-left\"></i>",
                "next": "<i class=\"fa fa-angle-right\"></i>"
            }
        },
        "ajax":
        {
            url: "/jobs/JobSearch/JobsSearch",
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

            switch (_filter) {
                case 'External':
                    $('.btn-external').addClass('active');
                    break;
                case 'Federal':
                    $('.btn-federal').addClass('active');
                    break;
                case '':
                    $('.btn-all').addClass('active');
                    break;
            }
        },
        "createdRow": function (row, data, dataIndex) {
            //todo
            //use this to set css on row level for locking of rows
        },
        "columns": [
            { data: 'JobSource', title: 'Source', render: function (data, type, item) { return item.jobSource; } },
            { data: 'OriginalSource', title: 'Origin', render: function (data, type, item) { return item.originalSource; } },
            { data: 'JobId', title: 'Job ID', render: function (data, type, item) { return item.jobId; } },
            { data: 'Title', title: 'Job Title', render: function (data, type, item) { return item.title; }, className: 'job-title-col' },
            { data: 'Location', title: 'Job Location', render: function (data, type, item) { return item.location; } },
            { data: 'LastUpdated', title: 'Last Updated', render: function (data, type, item) { return item.lastUpdated.replace(' PST', '&nbsp;PST'); } },
            { data: 'DatePosted', title: 'Posted Date', render: function (data, type, item) { return item.datePosted.replace(' PST','&nbsp;PST'); } },
            { data: 'ExpireDate', title: 'Expiry Date', render: function (data, type, item) { return item.expireDate.replace(' PST', '&nbsp;PST'); } },
            { data: 'Url', title: 'URL', render: function (data, type, item) { return '<a target="_blank" class="btn btn-inline" href="' + item.url + '">View Job</a>'; } },
            { data: 'Actions', title: 'Actions', orderable: false, render: function (data, type, item) { return getActions(item) } }
        ],
        "columnDefs": [
            { className: "urls", "targets": [8] },
            { className: "actions", "targets": [9] }
        ]
    }).on('page.dt', function () {
        $(".dataTables_paginate").addClass("wait");
        $(":focus").blur();
    });

    var getActions = function (item) {
        var historyUrl = '/jobs/JobManagement/ViewJobPostingHistory?id=' + item.jobId;

        var historyButton = '<a href="' + historyUrl + '" class="btn btn-inline"><i class="fa fa-clock-o fa-left"></i><span>History</span></a>';
        var deleteButton = '<button type="button" class="btn btn-inline delete-button" data-toggle="modal" data-target="#deleteModal" data-job-id="'
            + item.jobId + '"><i class="fa fa-trash fa-left"></i><span>Delete</span></button>';

        return historyButton + (item.jobSourceId === 2 ? deleteButton : '');
    };

    var numberWithCommas = function (x) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    };

});
