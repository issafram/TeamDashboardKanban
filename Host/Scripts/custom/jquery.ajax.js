$(document).ready(function () {
    $(".ajaxMenu a").click(function () {
        ajaxNotify("/Tasks/ImportData?project=" + testViewBagValue);
        
        var link = $(this).attr("href"); // get href of cliked link

        $.ajax({
            url: link,
            type: 'POST',
            success: function (response) {
                // update html in #main div
                $("#main").html(response);
                jQueryHover();
                taskClick();
            },
            error: function (error) {
                alert("Error!");
            }
        });

        return false; // return false so <a> click does not follow link
    });
})