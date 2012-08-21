function jQueryHover() {
    $(".task").hover(
        function () {
            $(this).addClass("hover");
        },
        function () {
            $(this).removeClass("hover");
        }
    );

    $(".taskName").hover(
        function () {
            $(this).addClass("hoverSpan");
        },
        function () {
            $(this).removeClass("hoverSpan");
        }
    );
}

$(document).ready(jQueryHover);