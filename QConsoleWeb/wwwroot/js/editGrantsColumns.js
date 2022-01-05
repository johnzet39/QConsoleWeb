
$(function () {
    var placeholderElement_col = $('#modal-placeholder-columns');

    $('button[data-toggle="ajax-edit-columns"]').click(function (event) {
        event.preventDefault();

        var url = $(this).data('url');
        var v_schema = $(this).data('schema');
        var v_table = $(this).data('table');
        var v_rolename = $(this).data('rolename');
        $.get(url, { schema: v_schema, table: v_table, rolename: v_rolename }).done(function (data) {
            $('#modal-placeholder-columns').html(data);
            $('#modal-placeholder-columns > .modal').modal('show');
        });
    });

    placeholderElement_col.on('click', '[data-save-col="modal"]', function (event) {
        console.log('aaa');
        event.preventDefault();

        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var model = form.serialize();

        //var model_data = JSON.stringify(model);

        $.ajax({
            type: "POST",
            url: actionUrl,
            dataType: 'json',
            data: model,
            crossDomain: true,
            success: function (returndata) {
                if (returndata.ok) {
                    if (returndata.mobile == true) {
                        window.location = returndata.newurl
                    }
                    else {
                        placeholderElement_col.find('.modal').modal('hide');
                    }
                    
                }
                else {
                    console.log('error');
                    console.log(returndata.message);
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