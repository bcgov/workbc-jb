/*
* Reports area javascript
*/
$(document).ready(function () {

    /*
    * DateRangePickerPartial scripts
    */
    if ($("#SimpleDateRangePartial").length || $('#JobSeekerDateRangePartial').length) {

        $.fn.datepicker.dates["en"]["months"] = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

        var startYear = window.datePickerMinYear || 2019;
        var startDate = new Date(startYear, 0, 1);

        // date pickers
        $("input.date-input").datepicker({
            format: "yyyy-mm-dd",
            startDate: startDate,
            endDate: new Date(),
            maxViewMode: 2, // decade
            templates: {
                leftArrow: '<i class="fa fa-caret-left fa-lg"></i>',
                rightArrow: '<i class="fa fa-caret-right fa-lg"></i>'
            }
        }).on('changeDate', function (e) {
            $("#DateRangeType").val("custom");
            $(this).datepicker('hide');
        });

        // show the datepicker when the input-group-addon is clicked
        $("#StartDateButtton").click(function () {
                $("#StartDate").datepicker().focus();
            }
        );

        // show the datepicker when the input-group-addon is clicked
        $("#EndDateButtton").click(function () {
                $("#EndDate").datepicker().focus();
            }
        );

        $("#DateRangeType").change(function () {
            var value = $(this).find(':selected').data('val');
            if (value === undefined) {
                $("#StartDate").datepicker().focus();
            } else {
                $("#StartDate").val(value.split(',')[0]);
                $("#EndDate").val(value.split(',')[1]);
            }
        });

    }

    /*
     * NocCategoryPartial scripts
     */
    if ($("#NocCategoryPartial").length) {

        $('#BroadCategory').select2();
        $('#MajorGroup').select2();
        $('#MinorGroup').select2();
        $('#UnitGroup').select2();

        var setNocCodeCategory = function () {
            var selectedRadio = $("input[name='NocCategoryLevel']:checked").val();
            $("#BroadOccupationalCategoryPanel").slideUp(400);
            $("#MajorGroupPanel").slideUp(400);
            $("#MinorGroupPanel").slideUp(400);
            $("#UnitGroupPanel").slideUp(400);
            if ($(`#${selectedRadio}Panel`).length) {
                $(`#${selectedRadio}Panel`).slideDown(400);
            }

            if (selectedRadio === "UnitGroup") {
                $("#showZeroesPanel").hide();
            } else {
                $("#showZeroesPanel").show();
            }
        };

        $("input[name='NocCategoryLevel']").change(function () {
            setNocCodeCategory();
        });
    }

    /*
     * MatrixDateRangePartial scripts
     */
    if ($("#MatrixDateRangePartial").length) {

        var setMatrixDateRangeType = function () {
            var selectedRadio = $("input[name='DateTypeToggle']:checked").val();
            $("#yearlyPanel").slideUp(200);
            $("#monthlyPanel").slideUp(200);
            $("#weeklyPanel").slideUp(200);

            if ($(`#${selectedRadio}Panel`).length) {
                $(`#${selectedRadio}Panel`).slideDown(200);
            }
        };

        $("input[name='DateTypeToggle']").change(function () {
            setMatrixDateRangeType();
        });

        var setMonthlyRangeType = function (el) {

            var values = el.children("option:selected").data("val");

            if (values !== undefined) {
                var split = values.split(',');
                $("#MonthlyStartMonth").val(split[0]);
                $("#MonthlyStartYear").val(split[1]);
                $("#MonthlyEndMonth").val(split[2]);
                $("#MonthlyEndYear").val(split[3]);
            }
        };

        $("select[name='MonthlyRangeType']").change(function () {
            $("span.field-validation-error").html("");
            setMonthlyRangeType($(this));
        });

        $("#MonthlyStartMonth,#MonthlyStartYear,#MonthlyEndMonth,#MonthlyEndYear").change(function () {
            $("select[name='MonthlyRangeType']").val("custom");
        });
    }

    /*
     * CityReportTypePartial scripts
     */
    if ($("#CityReportTypePartial").length) {

        var setReportType = function () {
            var selectedRadio = $("input[name='ReportType']:checked").val();
            $("#regionPanel").slideUp(400);
            if ($(`#${selectedRadio}Panel`).length) {
                $(`#${selectedRadio}Panel`).slideDown(400);
            }
        };

        $("input[name='ReportType']").change(function () {
            setReportType();
        });
    }

    /*
     * JobSeekerDateRangePartial scripts
     */
    if ($("#JobSeekerDateRangePartial").length) {

        var setDateTypeToggle = function () {
            var selectedRadio = $("input[name='DateTypeToggle']:checked").val();
            $("#periodPanel").slideUp(400);
            if ($(`#${selectedRadio}Panel`).length) {
                $(`#${selectedRadio}Panel`).slideDown(400);
            }
        };

        $("input[name='DateTypeToggle']").change(function () {
            setDateTypeToggle();
        });
    }

    /*
     * Report results javascript
     */
    if (!$("#page-Job-Seeker-Detail").length) {
        if ($(".report-results").length) {

            var btnPrint = {
                text: 'Print',
                action: function (e, dt, node, config) {
                    window.print();
                },
                className: 'buttons-print-dialog'
            };

            $("#ResultsTable").DataTable({
                dom: 'Bfrtip',
                order: [],
                ordering: !(window.datatablesDisableSorting || false),
                paging: false,
                info: false,
                searching: false,
                language: {
                    emptyTable: "No records match your query"
                },
                buttons: ['copy', btnPrint, {
                    extend: 'excelHtml5',
                    title: window.datatablesExportTitleExcel,
                    messageTop: window.datatablesExportMessageExcel,
                    footer: true,
                    text: 'Excel',
                    customize: function (xlsx) {
                        var sheet = xlsx.xl.worksheets['sheet1.xml'];
                        $('row c[r^="A"]', sheet).attr('s', '50');// column A: left aligned text 
                        $('row c[r="A1"]', sheet).attr('s', '7'); // cell A1: bold, grey background
                        $('row c[r="A2"]', sheet).attr('s', '5'); // cell A2: normal text, grey background
                        $('row c[r="A3"]', sheet).attr('s', '2'); // cell A3: bold

                        // extra report-specific formatting
                        if (window.datatableExcelExtraFormats !== undefined) {
                            window.datatableExcelExtraFormats(sheet);
                        }
                    }
                }]
            });
        }
    }

    /*
     * Job Seeker Details Report javascript
     */
    if ($("#page-Job-Seeker-Detail").length) {
        $(".buttons-epplus").click(function () {
            document.cookie = "show-download-spinner=true";
            var cookieVal = new Date().getTime() - new Date(1970, 0, 1).getTime();
            var wbc_spinner_interval = setInterval(function () {
                if (document.cookie.indexOf(cookieVal) !== -1) {
                    $('div#progress-overlay').hide();
                    clearInterval(wbc_spinner_interval);
                }
            }, 1000);
            window.location = window.excelUrl + "&Cookie=" + cookieVal;
        });

        $(".buttons-print-dialog").click(function () {
            window.print();
        });
    }
});