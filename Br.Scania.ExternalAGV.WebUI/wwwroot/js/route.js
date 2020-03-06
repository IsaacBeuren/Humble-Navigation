var globalID;
var RouteID;
var listRoutePoints = [];
var sequence = "";
var pickUpPoint = "";
var dropPoint = "";

$(document).ready(function () {
    GetRouteList();
    GetConfigPoints();
});

$('#insert-event').click(function (e) {
    $('#id').val("");
    $('#type').val("");
    $('#form-event').html("Insert");
    var content = $(".modal").find("#modalContent");
    var row = "Descrição:";
    content.empty();
    content.append(row); 

    content = $(".modal").find("#modalContent2");
    content.hide();

    content = $(".modal").find("#modalTitle");
    row = "Rota";
    content.empty();
    content.append(row);

    content = $(".modal").find(".col-7");
    row = '<input id="equipNumber" type="text" class="form-control" placeholder="" aria-label="" aria-describedby="basic-addon1">';
    content.empty();
    content.append(row);

    $('#equipNumber').val("");
    $('.modal').modal('show');
})

$('#form-event').click(function (e) {
    var command = $('#form-event').html();
    var data = {
        ID: $('#id').val(),
        Prefix: $('#type').val(),
        description: $('#equipNumber').val(),
    };
    if (command == 'Insert') {
        $.ajax({
            url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/Route/Insert',
            dataType: 'json',
            data: data,
            type: "POST"
        }).done(function (data) {
            GetRouteList();
            $("#encoded").html(data.encoded);
        }).fail(function (xhr, status, error) {
            $("#error").html("Could not reach the API: " + error);
        });
    }
    if (command == 'Update') {
        $.ajax({
            url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/Route/Update',
            dataType: 'json',
            data: data,
            type: "POST"
        }).done(function (data) {
            GetRouteList();
            $("#encoded").html(data.encoded);
        }).fail(function (xhr, status, error) {
            $("#error").html("Could not reach the API: " + error);
        });
    }

    if (command == 'Ok') {
        var selected = $("#selectPoint").val();
        var selected2 = $("#selectPoint2").val();

        var data2 = {
            ID: data.ID,
            Description: '',
            Routes: sequence,
            PickUpPoint: selected,
            DropPoint: selected2
        };

        $.ajax({
            url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/Route/UpdateRoute',
            dataType: 'json',
            data: data2,
            type: "POST"
        }).done(function (data) {
            GetRouteList();
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
            url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/Route/Remove?ID=' + id,
            type: "GET"
        }).done(function (data) {
            $("#encoded").html(data.encoded);
            GetRouteList();
            $('.modal').modal('hide');
        }).fail(function (xhr, Modal, error) {
            $("#error").html("Could not reach the API: " + error);
            $('#loader').hide();
        });
    }
})

function GetRouteList() {
    $.ajax({
        url: "http://agv-api.eu-west-1.elasticbeanstalk.com/api/Route/GetAll",
        type: "GET"
    }).done(function (data) {
        $("#encoded").html(data.encoded);
        insertRow(data);
        $('.listRoute').click(function (e) {
            var ID = this.id;
            RouteID = ID;
            $.ajax({
                url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/Route/GetAGVByID?ID=' + ID,
                type: "GET"
            }).done(function (data) {
                $("#encoded").html(data.encoded);
                pickUpPoint = data.pickUpPoint;
                dropPoint = data.dropPoint;
                $('#routeName').html(data.description);
            }).fail(function (xhr, Modal, error) {
                $("#error").html("Could not reach the API: " + error);
                $('#loader').hide();
            });
            GetRoutePoints(ID);
        })
        $('.listRoute').dblclick(function (e) {
            globalID = this.id;
            $.ajax({
                url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/Route/GetAGVByID?ID=' + globalID,
                type: "GET"
            }).done(function (data) {
                $("#encoded").html(data.encoded);
                $('#id').val(data.id);
                $('#type').val(data.prefix);
                $('#form-event').html("Update");

                var content = $(".modal").find("#modalTitle");
                var row = "Rota";
                content.empty();
                content.append(row);
                
                content = $(".modal").find("#modalContent");
                row = "Descrição:";
                content.empty();
                content.append(row);
                
                content = $(".modal").find("#modalContent2");
                content.hide();

                content = $(".modal").find(".col-7");
                row = '<input id="equipNumber" type="text" class="form-control" placeholder="" aria-label="" aria-describedby="basic-addon1">';
                content.empty();
                content.append(row);
                $('#equipNumber').val(data.description);

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

function GetConfigPoints() {
    $.ajax({
        url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/ConfigPoints/GetAll',
        type: "GET",
    }).done(function (data) {
        $("#encoded").html(data.encoded);
        insertConfigRow(data);
    }).fail(function (xhr, status, error) {
        $("#error").html("Could not reach the API: " + error);
    });
}

function GetRoutePoints(ID) {
    $("#bodyTabelaRoute").find("tbody").empty();
    listRoutePoints = [];
    $.ajax({
        url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/ConfigPoints/GetCompleteRoutesByID?ID=' + ID,
        type: "GET",
    }).done(function (data) {
        $("#encoded").html(data.encoded);
        for (i = 0; i < data.length; i++) {
            var data2 = {
                id: data[i].id,
                description: data[i].description,
                lat: data[i].lat,
                lng: data[i].lng,
                idRoute: data[i].idRoute,
                velocity: data[i].velocity,
                leftlight: data[i].leftlight,
                rightlight: data[i].rightlight,
                icon: data[i].icon
            };
            listRoutePoints.push(data2);
        }
        insertConfigurededPointsRow(listRoutePoints);
    }).fail(function (xhr, status, error) {
        $("#error").html("Could not reach the API: " + error);
    });
}

function insertRow(data) {
    var body = $("#bodyTabela").find("tbody");
    body.empty();
    for (i = 0; i < data.length; i++) {
        

        var row = '<tr class="listRoute" id="' + data[i].id + '">' +
            '<td id="' + data[i].id + '" hidden>' + data[i].id + '</td>' +
            '<td id="' + data[i].id + '">' + data[i].description + '</td>' +
            '</tr>';
        body.append(row);
    }
}

function insertConfigRow(data) {
    var body = $("#bodyTabelaConfig").find("tbody");
    body.empty();
    for (i = 0; i < data.length; i++) {
        var row = '<tr class="tabelaConfig" id="' + data[i].id + '">' +
            '<td>' + data[i].description + '</td>' +
            //'<td>' + data[i].lat + '</td>' +
            //'<td>' + data[i].lng + '</td>' +
            "</tr>";
        body.append(row);
    }
    $('.tabelaConfig').dblclick(function (e) {
        var ID = this.id;
        $.ajax({
            url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/ConfigPoints/GetByID?ID=' + ID,
            type: "GET"
        }).done(function (data) {
            $("#encoded").html(data.encoded);
            var insertEnable = true;
            for (i = 0; i < listRoutePoints.length; i++) {
                if (listRoutePoints[i].id == ID) {
                    insertEnable = false;
                    break;
                }
            }
            if (insertEnable) {
                var data2 = {
                    id: data.id,
                    description: data.description,
                    lat: data.lat,
                    lng: data.lng,
                    idRoute: data.idRoute,
                    velocity: data.velocity,
                    leftlight: data.leftlight,
                    rightlight: data.rightlight,
                    icon: data.icon,
                    sequence: listRoutePoints.length,
                };
                listRoutePoints.push(data2);
            }
            insertConfigurededPointsRow(listRoutePoints);
        }).fail(function (xhr, Modal, error) {
            $("#error").html("Could not reach the API: " + error);
            $('#loader').hide();
        });
    })
}

function insertConfigurededPointsRow(data) {
    if (RouteID != undefined) {
        $('#id').val(RouteID);
        var body = $("#bodyTabelaRoute").find("tbody");
        body.empty();
        for (i = 0; i < data.length; i++) {
            var row = '<tr class="tabelaRoute" id="' + data[i].id + '">' +
                '<td hidden>' + RouteID + '</td>' +
                '<td id="description' + data[i].id + '">' + data[i].description + '</td>' +
                //'<td>' + data[i].lat + '</td>' +
                //'<td>' + data[i].lng + '</td>' +
                "</tr>";
            body.append(row);
        }
        $('.tabelaRoute').dblclick(function (e) {
            var ID = this.id;
            listRoutePoints = remove(listRoutePoints, ID);
            insertConfigurededPointsRow(listRoutePoints);
        })

        $('#button-update').html('<button type="button" class="btn btn-success" id="insert-route">Update Route</button>');

        $('#insert-route').click(function (e) {
            sequence = "";   
            $('#bodyTabelaRoute tr').each(function () {
                sequence = sequence + this.id + ";";
            });
            sequence = sequence.substring(1, sequence.length - 1);
            
            $('#form-event').html("Ok");

            var content = $(".modal").find("#modalTitle");
            var row = "Pontos de Parada";
            content.empty();
            content.append(row);

            content = $(".modal").find("#modalContent");
            row = "Coleta:";
            content.empty();
            content.append(row);

            content = $(".modal").find("#modalContent2");
            row = "Descarga:";
            content.empty();
            content.append(row);

            content = $(".modal").find(".col-7");
            row = '<select id="selectPoint">';
            var row2 = '<select id="selectPoint2">';

            if (pickUpPoint != null) {
                var aux = "";
                for (i = 0; i < pickUpPoint.length; i++) {
                    if (pickUpPoint.charAt(i) != " ") { aux += pickUpPoint.charAt(i); }
                    else { break; }
                }
                pickUpPoint = aux;
                aux = "";
            }

            if (dropPoint != null) {
                for (i = 0; i < dropPoint.length; i++) {
                    if (dropPoint.charAt(i) != " ") { aux += dropPoint.charAt(i); }
                    else { break; }
                }
                dropPoint = aux;
            }

            for (i = 0; i < data.length; i++) {
                var pointNames = $(".tabelaRoute").find("#description" + data[i].id);
                if (pointNames.text() == pickUpPoint) {
                    row += '<option value="' + pointNames.text() + '" selected>' + pointNames.text() + '</option>';
                }
                else { row += '<option value="' + pointNames.text() + '">' + pointNames.text() + '</option>'; }

                if (pointNames.text() == dropPoint) { row2 += '<option value="' + pointNames.text() + '" selected>' + pointNames.text() + '</option>'; }
                else { row2 += '<option value="' + pointNames.text() + '">' + pointNames.text() + '</option>'; }
            }
            row += '</select>';
            row2 += '</select>';
            content.empty();
            content.append(row + "<br><br>");
            content.append(row2);

            content = $(".modal").find("#remove-event");
            content.hide();
            $('.modal').modal('show');
        })
    }
    else {
        alert("Selecionar uma rota!");
    }
}

function remove(array, ID) {
    var newlist = [];
    for (i = 0; i < array.length; i++) {
        if (array[i].id != ID) {
            newlist.push(array[i]);
        }
    }
    return newlist;
};

$('#bodyTabelaRoute tbody').sortable();
$('#bodyTabelaRoute tbody').disableSelection();