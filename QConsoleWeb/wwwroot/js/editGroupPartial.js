$(function () {

    $('tr[data-toggle="ajax-edit-group"]').click(function (event) {
        event.preventDefault();

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

});