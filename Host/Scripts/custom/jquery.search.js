$(document).ready(function () {
    $(function () {
        // Register for the change event of the drop down
        $('#TeamDashboardProjectSearch').change(function () {
            // When the value changes, get send an AJAX request to the
            // Filter action passing the selected value and update the
            // contents of some result div with the partial html returned
            // by the controller action
            //            $('#result').load('<%: Url.Action("filter") %>',
            //                { selectedValue: $(this).val() }
            //            );
            $('#searchForm').change(function () {
                $('#searchForm').submit();
            }
            );
        });
    });

    $(function () {
        //        $('#btnFillList').click(function () {
        //var project = eval('@ViewBag.Project');
        $.getJSON("/Tasks/GetSearchList?project=" + testViewBagValue, null, function (result) {
            var options = $("#SearchNew");
            $.each(result, function () {
                if (this.Selected == true) {
                    options.append($("<option selected=selected />").val(this.Value).text(this.Text));
                }
                else {
                    options.append($("<option />").val(this.Value).text(this.Text));
                }
            });
        });
        //        });
    });

    $("#SearchNew").change(function() {
        this.form.submit();
    });


});