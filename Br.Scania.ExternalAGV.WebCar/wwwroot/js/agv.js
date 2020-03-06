
$(document).ready(function () {
    $('#btnOn').click(function (e) {
        $("#btnOn").prop("disabled", true);
        $('#btnOff').removeAttr("disabled");
    })

    $('#btnOff').click(function (e) {
        $('#btnOn').removeAttr("disabled");
        $("#btnOff").prop("disabled", true);
    })

    // ***********************  Foward Button  ***********************************
    var btnFoward = false;
    $('#btnFoward').each(function () {
        $(this).mousedown(function () {
            btnFoward = true;
        });
        $(this).mouseup(function (e) {
            if (btnFoward) {
                btnFoward = false;
            }
        })
        $(this).mouseleave(function (e) {
            if (btnFoward) {
                btnFoward = false;
            }
        })
        $(this).keypress(function () {
            btnFoward = true;
        });
        $(this).keyup(function () {
            if (btnFoward) {
                btnFoward = false;
            }
        });
    });
    // ****************************************************************************

    // ***********************  Foward Button  ***********************************
    var btnReverse = false;
    $('#btnReverse').each(function () {
        $(this).mousedown(function () {
            btnReverse = true;
        });
        $(this).mouseup(function (e) {
            if (btnReverse) {
                btnReverse = false;
            }
        })
        $(this).mouseleave(function (e) {
            if (btnReverse) {
                btnReverse = false;
            }
        })
        $(this).keypress(function () {
            btnReverse = true;
        });
        $(this).keyup(function () {
            if (btnReverse) {
                btnReverse = false;
            }
        });
    });
    // ****************************************************************************

    // ***********************  Foward Button  ***********************************
    var btnRight = false;
    $('#btnRight').each(function () {
        $(this).mousedown(function () {
            btnRight = true;
        });
        $(this).mouseup(function (e) {
            if (btnRight) {
                btnRight = false;
            }
        })
        $(this).mouseleave(function (e) {
            if (btnRight) {
                btnRight = false;
            }
        })
        $(this).keypress(function () {
            btnRight = true;
        });
        $(this).keyup(function () {
            if (btnRight) {
                btnRight = false;
            }
        });
    });
    // ****************************************************************************

    // ***********************  Foward Button  ***********************************
    var btnLeft = false;
    $('#btnLeft').each(function () {
        $(this).mousedown(function () {
            btnLeft = true;
        });
        $(this).mouseup(function (e) {
            if (btnLeft) {
                btnLeft = false;
            }
        })
        $(this).mouseleave(function (e) {
            if (btnLeft) {
                btnLeft = false;
            }
        })
        $(this).keypress(function () {
            btnLeft = true;
        });
        $(this).keyup(function () {
            if (btnLeft) {
                btnLeft = false;
            }
        });
    });
    // ****************************************************************************

    //// *****************************  Keyboard  ***********************************

    //$(document).keydown(function (e) {
    //    console.log(e.which);
    //    switch (e.which) {
    //        case 37: // left
    //            if (btnLeft) {
    //                btnLeft = false;
    //            }
    //            break;

    //        case 38: // up
    //            if (btnFoward) {
    //                btnFoward = false;
    //            }
    //            break;

    //        case 39: // right
    //            if (btnRight) {
    //                btnRight = false;
    //            }
    //            break;

    //        case 40: // down
    //            if (btnReverse) {
    //                btnReverse = false;
    //            }
    //            break;

    //        default: return; // exit this handler for other keys
    //    }
    //});

    //// ****************************************************************************

    function verifyButtons() {
        var data = {
            Foward: btnFoward,
            Reverse: btnReverse,
            Right: btnRight,
            Left: btnLeft,
            Velocity:0
        };
        sendManualCommands(data);

    }
    setInterval(verifyButtons, 300);
});

$('.automan').click(function (e) {
    if (this.value == true) {
        //$('#btnOn').removeAttr("disabled");
        $('#btnCmdAuto').removeAttr("disabled");
    }
    else {
        //$("#btnOn").prop("disabled", true);
        $("#btnCmdAuto").prop("disabled", true);
    }
})


function sendManualCommands(dataSend) {
    console.log(dataSend);
    $.ajax({
        url: '/agvAPI/api/Commands/Moviments',
        dataType: 'json',
        data: dataSend,
        type: "POST"
    }).done(function (data) {
        console.log(data);
        $("#encoded").html(data.encoded);
        GetList();
    }).fail(function (xhr, Carrier, error) {
        $("#error").html("Could not reach the API: " + error);
    });
}





