// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
showInPopup = (url, title) => {
    $.ajax(
        {
            type: "GET",
            url: url,
            success: function(res) {
                $("#CreateNoteBlock .modal-body").html(res);
                $("#CreateNoteBlock .modal-title").html(title);
                $("#CreateNoteBlock").modal('show');
            }
        });
}

jQueryAjaxPost = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    $('#view-all-post').html(res.html);

                    $('#CreateNoteBlock .modal-body').html('');
                    $('#CreateNoteBlock .modal-title').html('');
                    $('#CreateNoteBlock').modal('hide');
                }
                else
                    $('#CreateNoteBlock .modal-body').html(res.html);
            },
            error: function (err) {
                console.log(err);
            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex);
    }
}


jQueryAjaxDelete = form => {
    if (confirm('Are you sure to delete this record ?')) {
        try {
            $.ajax({
                type: 'POST',
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function(res) {
                    $('#view-all-post').html(res.html);
                },
                error: function(err) {
                    console.log(err);
                }

            });
            return false;
        } catch (ex) {
            console.log(ex);
        }
    }
    
}



$(document).ready(function () {


    $("#Avatar").load("/Account/ShowAvatar");

});