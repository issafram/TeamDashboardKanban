function taskClick() {
    $(".task").click(function () {
        var name = $(this).find(".person-thumbnail").attr("alt");
        var link = "/Tasks/DetailedPerson?project=" + testViewBagValue + "&name=" + name;
        
        $.ajax({
            url: link,
            type: 'GET',
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
        
    });
}

$(document).ready(taskClick);