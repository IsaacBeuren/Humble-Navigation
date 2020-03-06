var map = L.map;
var ConfigMarkers = L.layerGroup();
var device;
var width;
var height;

$(document).ready(function () {
    CheckScreenSize();
    map = L.map('map', {
        center: [-23.7142012, -46.5687234],
        zoom: 17,
        zoomSnap: 0.10,
        cursor: true,
        minZoom: 2,
        maxZoom: 20,
    });
    InitializeMap();
    $('#map').css({ 'height': height * 0.8 });
    GetRoutePoints();
    setInterval(function () {
        GetRoutePoints();
    }, 2000);
});

$(window).resize(function () {
    CheckScreenSize();
});

function CheckScreenSize() {
    width = parseInt($(document).width());
    height = parseInt($(document).height());
    device = 'mobile';
    if (width >= 768) {
        device = 'Ipad';
    }
    if (width >= 1024) {
        device = 'Ipad';
    }
    if (width >= 1280) {
        device = 'smartTv';
    }
    if (width >= 2133) {
        device = 'pc';
    }
}


function GetRoutePoints() {
    $("#bodyTabelaRoute").find("tbody").empty();
    listRoutePoints = [];
    $.ajax({
        url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/Points/GetAll',
        type: "GET",
    }).done(function (data) {
        $("#encoded").html(data.encoded);
        for (i = 0; i < data.length; i++) {
            var data2 = {
                description: data[i].description,
                done: data[i].done,
                lat: data[i].lat,
                lng: data[i].lng,
                velocity: data[i].velocity,
                leftlight: data[i].leftlight,
                rightlight: data[i].rightlight,
                sequence: data[i].sequence,
            };
            listRoutePoints.push(data2);
        }
        insertConfigurededPointsRow(listRoutePoints);
        ConfigMarkers.clearLayers();
        for (i = 0; i < listRoutePoints.length; i++) {
            insertMarkers(listRoutePoints[i]);
        }
        $('.listRoute').click(function (e) {
            

            if ($(this).find('.color').text().toString() == "blue") {
                var body = $("#bodyTabelaRoute").find("tbody");

                for (i = 0; i <= Number($(this).find(".counter").attr("id")); i++) {

                    console.log("Color: " + body.find('#' + i).parent().find('.color').text().toString());

                    if (body.find('#' + i).parent().find('.color').text().toString() == "blue") {
                        var arr = body.find('#' + i).parent().attr("id").toString().split(";");
                        var data = {
                            description: arr[0],
                            done: false,
                            lat: arr[1],
                            lng: arr[2],
                            velocity: 0,
                            leftlight: 0,
                            rightlight: 0,
                            sequence: 0,
                        };


                        console.log(data);
                        $.ajax({
                            url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/Points/TogglePoint',
                            dataType: 'json',
                            data: data,
                            type: "POST"
                        }).done(function (data) {
                            $("#encoded").html(data.encoded);
                            GetRoutePoints()
                        }).fail(function (xhr, status, error) {
                            $("#error").html("Could not reach the API: " + error);
                        });

                    }

                }
            } else {
                var arr = $(this).attr("id").toString().split(";");
                var data = {
                    description: arr[0],
                    done: false,
                    lat: arr[1],
                    lng: arr[2],
                    velocity: 0,
                    leftlight: 0,
                    rightlight: 0,
                    sequence: 0,
                };


                console.log(data);
                $.ajax({
                    url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/Points/TogglePoint',
                    dataType: 'json',
                    data: data,
                    type: "POST"
                }).done(function (data) {
                    $("#encoded").html(data.encoded);
                    GetRoutePoints()
                }).fail(function (xhr, status, error) {
                    $("#error").html("Could not reach the API: " + error);
                });
            }

        })
    }).fail(function (xhr, status, error) {
        $("#error").html("Could not reach the API: " + error);
    });
}

function insertConfigurededPointsRow(data) {
    var body = $("#bodyTabelaRoute").find("tbody");
    body.empty();
    for (i = 0; i < data.length; i++) {
        var color;
        if (data[i].done === true) {
            color = "red"
        }
        else {
            color = "blue"
        }
        var row = '<tr class="listRoute" id="' + data[i].description + ";" + data[i].lat + ";" + data[i].lng + '">' +
            '<td hidden>' + data[i].id + '</td>' +
            '<td hidden class="color">' + color + '</td>' +
            '<td class="counter" id="' + i +'">' + data[i].description + '</td>' +
            "</tr>";
        body.append(row);
        $("#" + i).css("background-color", color);
    }
}

function InitializeMap() {
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: 'Autonomous Logistics Solutions'
    }).addTo(map);
    map.addLayer(ConfigMarkers);
}

function insertMarkers(data) {
    var marker = "";
    if (data.done === true) {
        marker = '/ExternalAGV/img/map_marker-red.png';
    }
    else {
        marker = '/ExternalAGV/img/marker-icon-blue.png';
    }
    var icon = L.icon({
        iconUrl: marker,
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