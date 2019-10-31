
$(function () {
    var placeholderElement = $('#modal-placeholder');

    $('button[data-toggle="ajax-delete-modal"]').click(function (event) {
        event.preventDefault();

        var url = $(this).data('url');
        var userid = $(this).data('userid');
        var username = $(this).data('username');
        $.get(url, { id: userid, name: username }).done(function (data) {
            $('#modal-placeholder').html(data);
            $('#modal-placeholder > .modal').modal('show');
        });
    });

    placeholderElement.on('click', '[data-save="modal"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var userid = $('#userid').val();
        var dataToSend = form.serialize();

        $.ajax({
            type: "POST",
            url: actionUrl,
            dataType: 'json',
            data: dataToSend,
            crossDomain: true,
            success: function (returndata) {
                if (returndata.ok) {
                    var newBody = $('.modal-body', returndata);
                    placeholderElement.find('.modal-body').replaceWith(newBody);

                    placeholderElement.find('.modal').modal('hide');
                    window.location = returndata.newurl
                }
                else {
                    var modal_body = placeholderElement.find('.modal-body');
                    modal_body.addClass('modal-error');
                    modal_body.children().first().replaceWith(returndata.message);
                }
            }
        });

        //$.post(actionUrl, dataToSend).done(function (data) {
        //        var newBody = $('.modal-body', data);
        //        placeholderElement.find('.modal-body').replaceWith(newBody);

        //        placeholderElement.find('.modal').modal('hide');
        //        window.location.href = 'User/Index';
        //        //var isValid = newBody.find('[name="IsValid"]').val() == 'True';
        //        //if (isValid) {
        //        //    placeholderElement.find('.modal').modal('hide');
        //        //}
        //    })
        //    .fail((error) => {
        //        alert('error!');
        //    });
    });

});
