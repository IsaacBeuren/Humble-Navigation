var globalID;

$(document).ready(function () {
    GetEquipamentList();
});

$('#insert-event').click(function (e) {
    $('#id').val("");
    $('#type').val("");
    $('#equipNumber').val("");
    $('#form-event').html("Insert");
    $('.modal').modal('show');
})

$('#form-event').click(function (e) {
    var command = $('#form-event').html();
    var data = {
        ID: $('#id').val(),
        Prefix: $('#type').val(),
        IDAGV: $('#equipNumber').val(),
    };
    if (command == 'Insert') {
        $.ajax({
            url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/Config/Insert',
            dataType: 'json',
            data: data,
            type: "POST"
        }).done(function (data) {
            GetEquipamentList();
            $("#encoded").html(data.encoded);
        }).fail(function (xhr, status, error) {
            $("#error").html("Could not reach the API: " + error);
        });
    }
    if (command == 'Update') {
        $.ajax({
            url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/Config/Update',
            dataType: 'json',
            data: data,
            type: "POST"
        }).done(function (data) {
            GetEquipamentList();
            $("#encoded").html(data.encoded);
        }).fail(function (xhr, status, error) {
            $("#error").html("Could not reach the API: " + error);
        });
    }
    $('.modal').modal('hide');
})

$('#remove-event').click(function (e) {
    var command = $('#form-event').html();
    if (command == 'Update') {
        var id = $('#id').val();
        $.ajax({
            url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/Config/Remove?ID=' + id,
            type: "GET"
        }).done(function (data) {
            $("#encoded").html(data.encoded);
            GetEquipamentList();
            $('.modal').modal('hide');
        }).fail(function (xhr, Modal, error) {
            $("#error").html("Could not reach the API: " + error);
            $('#loader').hide();
        });
    }
})

function GetEquipamentList() {
    $.ajax({
        url: "http://agv-api.eu-west-1.elasticbeanstalk.com/api/Config/GetAGVList",
        type: "GET"
    }).done(function (data) {
        $("#encoded").html(data.encoded);
        insertRow(data);
        $('.listEquipament').dblclick(function (e) {
            globalID = this.id;
            $.ajax({
                url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/Config/GetAGVByID?ID=' + globalID,
                type: "GET"
            }).done(function (data) {
                $("#encoded").html(data.encoded);
                console.log(data);
                $('#id').val(data.id);
                $('#type').val(data.prefix);
                $('#equipNumber').val(data.idagv);
                $('#form-event').html("Update");
                $('.modal').modal('show');
            }).fail(function (xhr, Modal, error) {
                $("#error").html("Could not reach the API: " + error);
                $('#loader').hide();
            });
        })
    }).fail(function (xhr, Modal, error) {
        $("#error").html("Could not reach the API: " + error);
        $('#loader').hide();
    });
}

function insertRow(data) {
    var body = $("#bodyTabela").find("tbody");
    body.empty();
    for (i = 0; i < data.length; i++) {
        var row = '<tr class="listEquipament" id="' + data[i].id + '">' +
            '<td id="' + data[i].id + '">' + data[i].id  + '</td>' +
            '<td id="' + data[i].id + '">' + data[i].prefix + formatNumber(data[i].idagv) +'</td>' +
            '</tr>';
        body.append(row);
    }
}

function formatNumber(val) {
    if (val.toString().length == 1) {
        var number = "0" + val;
        return number;
    }
    return val;
}

