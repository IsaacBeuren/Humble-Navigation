var map = L.map;
var ConfigMarkers = L.layerGroup();
var device;
var width;
var height;

$(document).ready(function () {
    CheckScreenSize();
    map = L.map('map', {
        center: [-23.7142012, -46.5687234],
        zoom: 16,
        zoomSnap: 0.10,
        cursor: true,
        minZoom: 2,
        maxZoom: 20,
    });
    InitializeMap();
    GetRouteList();
    $('#map').css({ 'height': height*0.8 });

});

$(window).resize(function () {
    CheckScreenSize();
});

function CheckScreenSize() {
    width = parseInt($(document).width());
    height = parseInt($(document).height());
    console.log(width);
    device = 'mobile';
    if (width >= 768) {
        console.log(1);
        device = 'Ipad';
    }
    if (width >= 1024) {
        console.log(2);
        device = 'Ipad';
    }
    if (width >= 1280) {
        console.log(3);
        device = 'smartTv';
    }
    if (width >= 2133) {
        console.log(4);
        device = 'pc';
    }


    console.log(device);
}

function GetRouteList() {
    $.ajax({
        url: "http://agv-api.eu-west-1.elasticbeanstalk.com/api/Route/GetAll",
        type: "GET"
    }).done(function (data) {
        $("#encoded").html(data.encoded);
        insertRow(data);
        $('.listRoute').click(function (e) {
            var ID = this.id;
            GetRoutePoints(ID);
        })
        $('.listRoute').dblclick(function (e) {
            var ID = this.id;
            $.ajax({
                url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/ConfigPoints/TransferRouteToAGV?ID=' + ID,
                type: "GET"
            }).done(function (data) {
                $("#encoded").html(data.encoded);
                alert("Dados Gravados com Sucesso!");
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
        var row = '<tr class="listRoute" id="' + data[i].id + '">' +
            '<td id="' + data[i].id + '" hidden>' + data[i].id + '</td>' +
            '<td id="' + data[i].id + '">' + data[i].description + '</td>' +
            '</tr>';
        body.append(row);
    }
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
                velocity: data[i].velocity,
                leftlight: data[i].leftlight,
                rightlight: data[i].rightlight,
                icon: data[i].icon
            };
            listRoutePoints.push(data2);
        }
        insertConfigurededPointsRow(listRoutePoints);
        ConfigMarkers.clearLayers();
        for (i = 0; i < listRoutePoints.length; i++) {
            insertMarkers(listRoutePoints[i]);
        }
    }).fail(function (xhr, status, error) {
        $("#error").html("Could not reach the API: " + error);
    });
}

function insertConfigurededPointsRow(data) {
    var body = $("#bodyTabelaRoute").find("tbody");
    body.empty();
    for (i = 0; i < data.length; i++) {
        var row = '<tr class="tabelaRoute" id="' + data[i].id + '">' +
            '<td hidden>' + data[i].id + '</td>' +
            '<td>' + data[i].description + '</td>' +
            "</tr>";
        body.append(row);
    }
}

function InitializeMap() {
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: 'Autonomous Logistics Solutions'
    }).addTo(map);
    map.addLayer(ConfigMarkers);
}

function insertMarkers(data) {
    var icon = L.icon({
        iconUrl: '/ExternalAGV/img/map_marker-orange.png',
        iconSize: 18, // size of the icon
        popupAnchor: [0, 0], // point from which the popup should open relative to the iconAnchor
        number: (data.ID)
    });
    if (data.lat !== null && data.lng !== null) {
        ConfigMarkers.addLayer(
            L.marker([convertDM2DMS(data.lat), convertDM2DMS(data.lng)], { icon: icon }).addTo(map)
        );
    }
}

function convertDM2DMS(value) {
    var latArray = value.toString().split(".");
    var degree = parseFloat(latArray[0].substring(0, 3));
    var minute = parseFloat(latArray[0].substring(3, 5) + "." + latArray[1]) / 60;
    var ret;
    if (degree > 0) {
        ret = degree + minute;
    }
    else {
        ret = degree - minute;
    }
    return ret;
}