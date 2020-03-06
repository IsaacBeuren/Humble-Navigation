var map = L.map;
var ConfigMarkers = L.layerGroup();
var clickMarker = 0;
var pointList = [];

var showPlaceTip = false;
var showMarkerTip = true;

var IDAGV = 1;




$(document).ready(function () {


    window.setInterval(function () {

        $.ajax({
            url: 'http://localhost/agvAPI/api/LastPosition/GetAll',
            type: "GET",
            success: function (response) {
                var obj = response[0];
                //console.log(JSON.stringify(response));

                $.ajax({
                    url: 'http://humble-als.eu-west-1.elasticbeanstalk.com/api/LastPosition/Update',
                    dataType: 'json',
                    data: obj,
                    type: "POST"
                }).done(function (data) {
                    console.log(data);
                });
            }
        });

        $.ajax({
            url: 'http://localhost/agvAPI/api/Route/GetAGVByID?ID=' + 7035,
            type: "GET",
            success: function (response) {
                var obj = response;
                //console.log(JSON.stringify(response));

                obj.id = 4031;

                $.ajax({
                    url: 'http://humble-als.eu-west-1.elasticbeanstalk.com/api/Route/UpdateRoute',
                    dataType: 'json',
                    data: obj,
                    type: "POST"
                }).done(function (data) {
                    console.log(data);
                });
            }
        });

    }, 1000);

    window.setInterval(function () {

        $.ajax({
            url: 'http://localhost/agvAPI/api/Points/GetAll',
            type: "GET",
            success: function (response) {
                //var points = [];
                //for (var i = 0; i < response.length; i++) {
                //    points[i] = response[i].description.toString() + ";" + response[i].done.toString();
                //}
                //console.log(JSON.stringify(points));

                response = JSON.stringify(response);
                //console.log(response);

                $.ajax({
                    url: 'http://humble-als.eu-west-1.elasticbeanstalk.com/api/Points/UpdateDone',
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    data: response,
                    type: "POST"
                }).done(function (boolResp) {
                    console.log("resp: " + boolResp);
                    console.log("success");
                }).fail(function (error) {
                    console.log("Could not reach the API: " + JSON.stringify(error));
                    console.log("fail");
                });
            }
        });

    }, 2000);


});