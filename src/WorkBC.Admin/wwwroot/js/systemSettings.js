/*
* SystemSettings javascript
*/
$(document).ready(function () {

    _table = null;
    $(document).ready(function () {

        //Init the datatabes object
        if ($('#dtSettings').length) {

            _table = $('#dtSettings').DataTable({
                "responsive": true,
                "bFilter": true,
                "language": {
                    "emptyTable": "No records match your search criteria",
                    "zeroRecords": "No records match your search criteria",
                    "info": "Showing _START_ to _END_ of _TOTAL_ settings",
                    "infoFiltered": "",
                    "paginate": {
                        "previous": "<i class=\"fa fa-angle-left\"></i>",
                        "next": "<i class=\"fa fa-angle-right\"></i>"
                    }
                },
                "pageLength": 1000,
                "stateSave": true, //keep the state
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

                    if (records_displayed != '0') {
                        $('#lblShowing').html('Showing ' + (start + 1) + '-' + end + ' of ' + records_displayed + ' settings');
                    } else {
                        $('#lblShowing').html('');
                    }
                },
                "stateSaveCallback": function (settings, data) {
                    //add status to localstorage
                    data.search.search = $('.search__input').val();
                    localStorage.setItem('DataTables_' + settings.sInstance, JSON.stringify(data));
                },
                "stateLoadCallback": function (settings) {
                    return JSON.parse(localStorage.getItem('DataTables_' + settings.sInstance));
                },
                "stateLoadParams": function (settings, data) {
                    $('.search__input').val(data.search.search);
                },
                "columnDefs": [{
                    "targets": 'no-sort',
                    "orderable": false
                }]
            });

            //This function override the databtable search function
            //We need to do this because we need to search across multiple columns with an OR
            //default behavior will search with an AND and will not return any results.
            //Using "toLowerCase()" to have the search not be case sensitive
            $.fn.dataTable.ext.search.push(
                function (settings, data, dataIndex) {
                    var keywords = $('.search__input').val().toLowerCase();

                    //firstname
                    if (data[0].toLowerCase().indexOf(keywords) > -1) {
                        return true;
                    }
                    //lastname
                    else if (data[1].toLowerCase().indexOf(keywords) > -1) {
                        return true;
                    }
                    //email
                    else if (data[2].toLowerCase().indexOf(keywords) > -1) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            );

            $('.search__input').on('keyup', function () {
                _table.search($(this).val()).draw();
            });
        }

        if ($("#ValueHtml").length) {
            // add syntax hilighting to html textarea inputs
            var cm = CodeMirror.fromTextArea(document.getElementById("ValueHtml"), {
                lineNumbers: true,
                mode: "htmlmixed",
                lineWrapping: true
            });
        }
    });

});
