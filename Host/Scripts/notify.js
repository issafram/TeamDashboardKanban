(function ($) {
    $.fn.extend({
        notify: function (options) {
            var settings = $.extend({ type: 'sticky', speed: 500, onDemandButtonHeight: 35 }, options);
            return this.each(function () {
                var wrapper = $(this);
                var ondemandBtn = $('.ondemand-button');
                var dh = -35;
                var w = wrapper.outerWidth() - ondemandBtn.outerWidth();
                ondemandBtn.css('left', w).css('margin-top',  dh + "px" );
                var h = -wrapper.outerHeight();
                wrapper.addClass(settings.type).css('margin-top', h).addClass('visible').removeClass('hide');
                if (settings.type != 'ondemand') {
                    wrapper.stop(true, false).animate({ marginTop: 0 }, settings.speed);
                }
                else {
                    ondemandBtn.stop(true, false).animate({ marginTop: 0 }, settings.speed);
                }

                var closeBtn = $('.close', wrapper);
                closeBtn.click(function () {
                    if (settings.type == 'ondemand') {
                        wrapper.stop(true, false).animate({ marginTop: h }, settings.speed, function () {
                            wrapper.removeClass('visible').addClass('hide');
                            ondemandBtn.stop(true, false).animate({ marginTop: 0 }, settings.speed);
                        });
                    }
                    else {
                        wrapper.stop(true, false).animate({ marginTop: h }, settings.speed, function () {
                            wrapper.removeClass('visible').addClass('hide');
                        });
                    }
                });
                if (settings.type == 'floated') {
                    $(document).scroll(function (e) {
                        wrapper.stop(true, false).animate({ top: $(document).scrollTop() }, settings.speed);
                    }).resize(function (e) {
                        wrapper.stop(true, false).animate({ top: $(document).scrollTop() }, settings.speed);
                    });
                }
                else if (settings.type == 'ondemand') {
                    ondemandBtn.click(function () {
                        $(this).animate({ marginTop: dh }, settings.speed, function () {
                            wrapper.removeClass('hide').addClass('visible').animate({ marginTop: 0 }, settings.speed, function () {

                            });
                        })
                    });
                }

            });

        }
    });
})(jQuery);



function ajaxNotify(url) {
    $.ajax({
        url: url,
        type: 'POST',
        success: function (response) {
//            $(".notification").removeClass("sticky").removeClass("floated");
            $(".close").trigger("click");
            //$(".notification").removeClass("sticky").removeClass("floated").removeClass("visible").removeClass("hide").addClass("hide");

            // update html in #main div
            if (response > 0) {
                $(".notification").addClass("floated");
                if (response != 1) {
                    $(".notification.floated p").text(response + " data sets imported!");
                } else {
                    $(".notification.floated p").text(response + " data set imported!");
                }
                $('.notification.floated').notify({ type: '.floated' });
            }
        },
        error: function (error) {
            alert("Error!");
        }
    });
}


//function ajaxNotify(url) {
//    $.ajax({
//        url: url,
//        type: 'POST',
//        success: function (response) {
//            $(".notification").removeClass("sticky").removeClass("floated");
//            // update html in #main div
//            if (response == 0) {
//                $(".notification").addClass("sticky");
//                $(".notification.sticky p").text(response + " data sets imported!");
//                $('.notification.sticky').notify({ type: '.sticky' });
//            } else {
//                $(".notification").addClass("floated");
//                if (response != 1) {
//                    $(".notification.floated p").text(response + " data sets imported!");
//                } else {
//                    $(".notification.floated p").text(response + " data set imported!");
//                }
//                $('.notification.floated').notify({ type: '.floated' });
//            }
//        },
//        error: function (error) {
//            alert("Error!");
//        }
//    });
//}



$(document).ready(function () {
    ajaxNotify("/Tasks/ImportData?project=" + testViewBagValue);

    $('.button').click(function () {
        var link = $(this).attr("href"); // get href of cliked link

        ajaxNotify(link);

        return false; // return false so <a> click does not follow link

    });

});
   