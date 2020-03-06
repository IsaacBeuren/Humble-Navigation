var arrPoints = [];
var marker;
var polyline;

$(document).ready(function () {
    //GetAll();
});

var map = L.map('map', {
    center: [-46.565928, -23.712321],
    zoom: 18,
    minZoom: 1,
    maxZoom: 30
});

L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
}).addTo(map);

map.on("contextmenu", function (event) {
    var startPointArray;
    var data = [
        event.latlng.lat,
        event.latlng.lng,
    ];
    arrPoints.push(data);
    marker = new L.marker([event.latlng.lat, event.latlng.lng], {
        draggable: true,
        autoPan: true,
        cursor: true,
    }).addTo(map)
        .bindPopup(event.latlng.lat + " || " + event.latlng.lng)
        .openPopup();

    createLine(arrPoints);

    //map.fitBounds(marker.getBounds());
    // create a red polyline from an array of LatLng points
    marker.on('drag', function (e) {
        //console.log(e.target._latlng);
        //console.log('marker drag event');
    });

    marker.on('dragstart', function (e) {
        for (i = 0; i < arrPoints.length; i++) {
            if (arrPoints[i][0] === e.target._latlng.lat || arrPoints[i][1] === e.target._latlng.lng) {
                startPointArray = i;
            }
        }
    });

    marker.on('dragend', function (e) {
        var data = {
            lat: e.target._latlng.lat,
            lng: e.target._latlng.lng,
        };
        var retData = VerifyNeastMarker(data);

        arrPoints[startPointArray][0] = retData.lat;
        arrPoints[startPointArray][1] = retData.lng;
        createLine(arrPoints);
    });
});

function createMarkers(data) {
    for (i = 0; i < data.length; i++) {
        marker = new L.marker([data[i][0], data[i][1]], {
            draggable: true,
            autoPan: true,
        }).addTo(map)
            .bindPopup(data[i][0] + " || " + data[i][1])
            .openPopup();
    }
}

function createLine(data) {
    $(".btnSave").attr("hidden", false);
    try {
        clearPolylines();
    }
    catch (e) {
        
    }
    insertRow(data);
    if (data.length > 1) {
        polyline = L.polyline(data, {
            color: 'red',
            weight: 3,
            opacity: 0.5,
            smoothFactor: 1
        });
        polyline.addTo(map);
        // zoom the map to the polyline
        map.fitBounds(polyline.getBounds());
    }
}

// clear markers   
function clearMarkers() {
    map.removeLayer(marker);
}

// clear polylines   
function clearPolylines() {
    map.removeLayer(polyline);
}

$('.btnSave').click(function (e) {
    $(".btnSave").attr("hidden", true);
})

$('.modalLatLng').find('#delete-event').click(function (e) {
    var id = $('.modalLatLng').find('#idItem').val();
    var removed = arrPoints.splice(id,1);
    createLine(arrPoints);
    console.log(removed);
    $('.modalLatLng').modal('hide');
})

function insertRow(data) {
    var body = $("#bodyTabela").find("tbody");
    body.empty();
    for (i = 0; i < data.length; i++) {
        var row = '<tr>' +
            '<td id="' + i + '" class="listlatlng">' + i + '</td>' +
            '<td id="' + i + '" class="listlatlng">' + data[i][0] + '</td>' +
            '<td id="' + i + '" class="listlatlng">' + data[i][1] + '</td>' +
            "</tr>";
        body.append(row);
    }
    $('.listlatlng').dblclick(function (e) {
        var listID = this.id;
        $('.modalLatLng').find('#idItem').val(listID);
        $('.modalLatLng').find('#valuesPoints').empty();
        $('.modalLatLng').find('#valuesPoints').append(arrPoints[listID][0] + " " + arrPoints[listID][1]);
        var id = $('.modalLatLng').find('#idItem').val();

        console.log(id);


        $('.modalLatLng').modal('show');
    })
}

function GetAll() {
    $('#loader').show();
    $.ajax({
        contentType: 'application/json',
        crossDomain: true,
        url: "http://localhost:56948/api/points/getall",
        type: "GET"
    }).done(function (data) {
        $("#encoded").html(data.encoded);
        console.log(data);
        $('#loader').hide();
    }).fail(function (xhr, Carrier, error) {
        $("#error").html("Could not reach the API: " + error);
        $('#loader').hide();
    });
}

function Insert(WriteList) {
    $('#loader').show();
    var externalUrl;
    if (WriteList.ID >= 1) {
        externalUrl = "/mesapi/api/Carrier/Update";
    }
    else {
        externalUrl = "/mesapi/api/Carrier/Insert";
    }
    console.log(WriteList);
    $.ajax({
        url: externalUrl,
        dataType: 'json',
        data: WriteList,
        type: "POST"
    }).done(function (data) {
        console.log(data);
        $("#encoded").html(data.encoded);
        $('#loader').hide();
        GetList();
    }).fail(function (xhr, Carrier, error) {
        $("#error").html("Could not reach the API: " + error);
        $('#loader').hide();
    });
}

function VerifyNeastMarker(data) {
    var retData = {
        lat: data.lat,
        lng: data.lng,
    };
    return retData;
}