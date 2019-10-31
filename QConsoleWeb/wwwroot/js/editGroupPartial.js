$(function () {
    const first = $("#splitterGroups");
    const second = $("#splitterDetails");

    $('tr[data-toggle="ajax-edit-group"]').click(function (event) {
        event.preventDefault();

        addClassToSplitters();

        if ($(this).hasClass('active')) {
            $(this).removeClass('active');
        } else {
            $(this).addClass('active').siblings().removeClass('active');
        }

        var url = $(this).data('url');
        var userid = $(this).data('userid');
        var rolename = $(this).data('rolename');

        $.ajax(
            {
                type: "GET",
                url: url,
                data: { userid: userid, rolename: rolename },
                success: function (returndata) {
                    $('#detailsContainer').html(returndata);
                }
            });
    });

    function addClassToSplitters() {
        
        //if (!first.classList.contains("firstSplitter") && !second.classList.contains("secondSplitter")) {
        //    console.log("addClass");
        //    first.classList.add('firstSplitter');
        //    second.classList.add('secondSplitter');
        if (!first.hasClass("animated") && !second.hasClass("animated")) {
            console.log("addClass");
            first.addClass('animated');
            second.addClass('aniamted');

            $('#splitterGroups').animate({
                width: '20%'
            }, 500);
        }
    }
});