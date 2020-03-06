var map = L.map;
var legMarkers = L.layerGroup();
var PreConfigMarkers = L.layerGroup();
var zoomLevel = 18;
var clickMarker = 0;

var showPlaceTip = false;
var showMarkerTip = true;

var zoomIcon = {
    Width: zoomLevel * 4,
    Height: zoomLevel * 4
};

$(document).ready(function () {
    map = L.map('map', {
        center: [-23.7142012, -46.5687234],
        zoom: zoomLevel,
        zoomDelta: 1,
        zoomSnap: 0,
        cursor: true,
        minZoom: 2,
        maxZoom: 20,
    });
    InitializeMap();
    GetPreConfigPoints();
    mapControl();
    setInterval(function () {
        //Colocar a posição atual do AGV
        GetActualPoints();
    }, 2000);
});

function insertRow(transportOrder, popID) {
    var row = '<tr class="editItem">' +
        '<td id="' + popID + '">' + transportOrder + '</td>' +
        '<td id="' + popID + '">' + popID + '</td>' +
        "</tr>";
    $("#listPopID").find("tbody").append(row);
}

function GetPreConfigPoints() {
    var placePoints = [];
    $.ajax({
        url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/ConfigPoints/?GetAll',
        type: "GET",
        success: function (events) {
            PreConfigMarkers.clearLayers();
        }
    }).done(function (data) {
        $("#encoded").html(data.encoded);
        dLen = data.length;
        for (i = 0; i < dLen; i++) {
            var points = {
                id:data[i].id,
                lat: SplitDegree2Minute(data[i].lat),
                lng: SplitDegree2Minute(data[i].lng),
                icon: data[i].icon.trim(),
                Description: data[i].description.trim()
            };
            placePoints.push(points);
        }
        insertPreConfigMarkers(placePoints);
        insertRow(placePoints);
    }).fail(function (xhr, status, error) {
        $("#error").html("Could not reach the API: " + error);
    });
}

function GetActualPoints() {
    $.ajax({
        url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/ConfigPoints/?GetActualPoints',
        type: "GET",
        success: function (events) {
            PreConfigMarkers.clearLayers();
        }
    }).done(function (data) {
        $("#encoded").html(data.encoded);
        dLen = data.length;
        console.log(data);
    }).fail(function (xhr, status, error) {
        $("#error").html("Could not reach the API: " + error);
    });
}

function InitializeMap() {
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: 'Autonomous Logistics Solutions'
    }).addTo(map);
    map.addLayer(legMarkers);
    map.addLayer(PreConfigMarkers);
}

function insertPreConfigMarkers(data) {
    if (data.length > 0 && map.getZoom() > 4) {
        for (i = 0; i < dLen; i++) {
            var icon = L.icon({
                iconUrl: '/ExternalAGV/img/' + data[i].icon,
                iconSize: [zoomIcon.Width, zoomIcon.Height], // size of the icon
                popupAnchor: [-3, -76] // point from which the popup should open relative to the iconAnchor
            });
            if (data[i].lat !== null && data[i].lng !== null) {
                console.log("Create Marker");
                PreConfigMarkers.addLayer(
                    L.marker([data[i].lat, data[i].lng], { icon: icon }).addTo(map)
                );
            }
        }
    }
}

L.icon = function (options) {
    return new L.Icon(options);
};

function mapControl() {
    var zoom = map.getZoom();
    map.on('zoomend', function () {
        if (zoom < 3.99) {
            map.removeLayer(PreConfigMarkers);
        }
        else {
            map.addLayer(PreConfigMarkers);
        }
        if (zoom < 8) {
            zoomLevel = map.getZoom();
        }
        else {
            zoomLevel = 8;
        }
        zoomIcon.Width = zoomLevel * 4;
        zoomIcon.Height = zoomLevel * 4;
        GetPreConfigPoints();
    });
    map.on('click', function (e) {
        var attributes = e;
        //GetAllTransport(attributes.latlng);
        console.log('Click on Map');
    });
    map.on('dblclick', function (e) {
        var attributes = e;
        //GetAllTransport(attributes.latlng);
        console.log('Double Click on Map');
    });
    map.on("contextmenu", function (event) {
        var startPointArray;
        var data = [
            event.latlng.lat,
            event.latlng.lng,
        ];
        marker = new L.marker([event.latlng.lat, event.latlng.lng], {
            draggable: true,
            autoPan: true,
            cursor: true,
        }).addTo(map)
            .bindPopup(event.latlng.lat + " || " + event.latlng.lng)
            .openPopup();
    });
}




function insertRow(data) {
    var body = $("#bodyTabela").find("tbody");
    body.empty();
    for (i = 0; i < data.length; i++) {
        var row = '<tr>' +
            '<td id="' + data[i].id + '" class="listlatlng">' + data[i].id + '</td>' +
            '<td id="' + data[i].id + '" class="listlatlng">' + data[i].lat + '</td>' +
            '<td id="' + data[i].id + '" class="listlatlng">' + data[i].lng + '</td>' +
            "</tr>";
        body.append(row);
    }
    $('.listlatlng').dblclick(function (e) {
        var listID = this.id;
        console.log(listID);
        //$('.modalLatLng').modal('show');
    })
}

//$("#chkTransport").click(function () {
//    showMarkerTip = this.checked;
//    GetPreConfigPoints();
//});

function SplitDegree2Minute(coordinate) {
    var sCoordinate = coordinate.toString();
    var array = sCoordinate.split(".");
    var signal;
    var i = 0;
    if (sCoordinate.substring(0, 1) == '-') {
        i = 1;
        signal = '-';
    }
    var output = signal + sCoordinate.substring(i, i+2) + "." + sCoordinate.substring(i+2, i+4) + array[1];
    return output;
}

function showCoordenadasMinutos(gDec, x) {
    var graus;
    var minutos;
    var aux;
    var segundos;
    var milisegundos;
    var direcao;

    // Separa os graus
    graus = parseInt(gDec);

    // Pega a fração dos graus e converte em minutos
    aux = (graus - gDec) * 60;
    minutos = parseInt(aux);

    // Pega a fração dos minutos e converte em segundos
    aux = (aux - minutos) * 60;
    segundos = parseInt(aux);

    // Pega a fração dos segundos e converte em milisegundos
    milisegundos = parseInt((aux - segundos) * 60);

    // Essa parte eu verifico se é o eixo X ou Y para substituir o simbolo de negativo  pelas iniciais de norte ou sul para o eixo Y, leste ou oeste para o eixo X
    if (x) {
        // Eixo X
        if (graus < 0)
            direcao = "O";
        else
            direcao = "L";
    } else {
        // Eixo Y
        if (graus < 0)
            direcao = "S";
        else
            direcao = "N";
    }
    // Devolvo a string formatada, a função Math.abs é para retornar o valor absoluto // (retirar o valor negativo) já que estou usando a notação norte, sul, leste ou oeste
    return Math.abs(graus) + "° " + minutos + "' " + segundos + "." + milisegundos + "'' " + direcao;
}