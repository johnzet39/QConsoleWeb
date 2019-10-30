$(function () {
    $('button[data-toggle="ajax-delete-modal"]').click(function (event) {

        var url = $(this).data('url');
        var userid = $(this).data('userid');
        var rolename = $(this).data('rolename');
        $.get(url, { userid: userid, rolename: rolename }).done(function (data) {
            $('#modal-placeholder').html(data);
            $('#modal-placeholder > .modal').modal('show');
        });
    });
});


$(function () {
    $('button[data-toggle="ajax-delete-modal"]').click(function (event) {

        var url = $(this).data('url');
        var userid = $(this).data('userid');
        $.get(url, { userid: userid }).done(function (data) {
            $('#modal-placeholder').html(data);
            $('#modal-placeholder > .modal').modal('show');
        });
    });
});
