var map = L.map;
var ConfigMarkers = L.layerGroup();
var clickMarker = 0;
var pointList = [];

var showPlaceTip = false;
var showMarkerTip = true;


$(document).ready(function () {
    map = L.map('map', {
        center: [-23.7142012, -46.5687234],
        zoom: 17,
        zoomSnap: 0.10,
        cursor: true,
        minZoom: 2,
        maxZoom: 20,
    });
    InitializeMap();
    setInterval(function () {
        GetLastPosition();
    }, 5000);
});


function GetLastPosition() {
    $.ajax({
        url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/LastPosition/GetAll',
        type: "GET",
        success: function (events) {
            ConfigMarkers.clearLayers();
        }
    }).done(function (data) {
        console.log(data);
        $("#encoded").html(data.encoded);
        for (i = 0; i < data.length; i++) {
            insertMarkers(data[i]);
        }
    }).fail(function (xhr, status, error) {
        $("#error").html("Could not reach the API: " + error);
    });
}

function InitializeMap() {
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: 'Autonomous Logistics Solutions'
    }).addTo(map);
    //map.addLayer(legMarkers);
    map.addLayer(ConfigMarkers);
}

function insertMarkers(data) {
    var icon = L.icon({
        iconUrl: '/ExternalAGV/img/marker-icon-blue.png',
        iconSize: 18, // size of the icon
        popupAnchor: [0, 0], // point from which the popup should open relative to the iconAnchor
        number: (data.ID)
    });
    if (data.latitude !== null && data.longitude !== null) {
        ConfigMarkers.addLayer(
            L.marker([convertDM2DMS(data.latitude), convertDM2DMS(data.longitude)], { icon: icon }).addTo(map)
                .bindPopup(data.idagv + " - " + data.gpsQuality)
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