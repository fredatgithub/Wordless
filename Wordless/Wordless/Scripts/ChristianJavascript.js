﻿$(document).ready(function () {
    $('#Search').on('input', function () {
    
        $.ajax({
            method:"GET",
            url: '/Book/BooksAPI',
            dataType: 'json',
            data: {
                
                searchstring: $("#Search").val()
            },
            success: function (data) {
                $('#BookDisplay').html(data.HtmlString);
            },
            error: function (jqXHR, statusText, errorThrown) {
                $('#BookDisplay').html('Ett fel inträffade: <br>'
                    + statusText);
            }
        });
    });
});
$(document).ready(function () {
    $(".book-divs").hover(function () {
        $(this).animate({
            opacity: '1'
        });
    }, function () {
        $(this).animate({
            opacity: '0.9'
        });
    });
});