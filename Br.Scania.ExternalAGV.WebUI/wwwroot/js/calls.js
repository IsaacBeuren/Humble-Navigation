//var expiration = new Date();
//expiration.setTime((expiration.getTime() + 8 * 60 * 60 * 1000));

var calls = [];

$(document).ready(function () {

    $.ajax({
        url: 'http://humble-api.sa-east-1.elasticbeanstalk.com/api/Account/GenerateToken',
        type: "POST",
        data: { "username": "sbtegj", "password": "Q2FsaWZvcm5pYUAxMjM0" },  //ssbibv Q2FsaWZvcm5pYUAxMjM0
        success: function (data) {

            var expiration = new Date();
            expiration.setTime(data.Content.Expires);
            var issued = new Date();
            issued.setTime(data.Content.Issued);


            var user = {
                AuthenticationType: data.Content.token_type,
                Expires: expiration,
                GlobalUserName: data.Content.GlobalUserName,
                Issued: issued, //Utils.ConvertSecToDate(data.Content.Issued),
                Roles: data.Content.Roles,
                UserName: data.Content.UserName,
                Token: data.Content.access_token,
                refresh_token: data.Content.RefreshToken,
            }



            console.log("data.content: " + JSON.stringify(data.Content));
            amplify.store(("Humble" + "UserIdentity"), user)
            document.cookie = 'HumbleTokenExpAPI=TokenExp;path=/' + ';expires=' + expiration;


            $.ajax({
                crossDomain: true,
                headers: {
                    authorization: user.AuthenticationType + " " + user.Token,
                },
                contentType: "application/json; charset=utf-8",
                url: "http://humble-api.sa-east-1.elasticbeanstalk.com/api/Call/GetAllCallsOpenedByUser",
                dataType: 'json',
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log("Error: " + textStatus + ": " + errorThrown);

                    //if (jqXHR.status == 401) {
                    //    if (user.TimesToRefreshToken == 0)
                    //        self.RefreshToken();
                    //    else {
                    //        let expires = new Date(self.GetUser().Expires);
                    //        let now = new Date();
                    //        if (expires < now) {
                    //            self.ForceLogout();
                    //        }
                    //    }
                    //}

                    if (jqXHR.status == 404) {
                        console.log("Element not found (404).");
                    }

                    //if (errorFunction != null)
                    //    errorFunction(obj);
                },
                done: function (response) {
                    console.log("HUMBLE: " + JSON.stringify(response));
                },
                //beforeSend: function (obj) {
                //    // Refresh Token
                //    if (cacheUser.TimesToRefreshToken == 0) {
                //        let minBefore = 5;
                //        let expires = new Date(self.GetUser().Expires);
                //        let now = new Date();
                //        expires = new Date(expires.getTime() - (minBefore * 60 * 1000));
                //        if (expires <= now) {
                //            self.RefreshToken();
                //        }
                //    }

                //    if (beforeSendFunction != null)
                //        beforeSendFunction(obj);
                //},
                complete: function (response) {
                    console.log("Completed");
                    //console.log("HUMBLE: " + JSON.stringify(response));

                    for (i = 0; i < response.responseJSON.Content.length; i++) {
                        if (response.responseJSON.Content[i].OpenUser.toUpperCase() == "SBTEGJ" || response.responseJSON.Content[i].OpenUser.toUpperCase() == "SSBIBV") {
                            calls[i] = {
                                "id": response.responseJSON.Content[i].IdRoute,
                                "destination": response.responseJSON.Content[i].Box.DestinationSector.Factory.Code,
                                "description": response.responseJSON.Content[i].Box.Description
                            }
                        }
                    }

                    insertRow(calls);

                    //var pointUpd = {
                    //    id: 8184,
                    //    description: "BR158MOD", //\left(,\right)
                    //    lat: -2342.7285889489505,
                    //    lng: -4633.93309898998,
                    //    velocity: 4,
                    //    leftLight: false,
                    //    rightLight: false,
                    //    icon: "marker - icon.png",
                    //    onStraight: true
                    //}

                    //$.ajax({
                    //    url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/ConfigPoints/DirectUpdate',
                    //    dataType: 'json',
                    //    data: pointUpd,
                    //    type: "POST"
                    //}).done(function (data) {
                    //    console.log("UPDATE: " + JSON.stringify(data));
                    //}).fail(function (xhr, status, error) {
                    //    console.log("Could not reach the API: " + error);
                    //});


                    $('.startRoute').click(function () {


                        var call = {
                            ID: 1,
                            IDAGV: 1,
                            IDRoute: $(this).parent().parent().find("#routeID").val(),
                            InitTime: null,
                            EndTime: null,
                            CarriedItem: $('#listItem' + $(this).parent().parent().find("#callID").val()).find('.description').text(),
                            CUCode: $(this).parent().parent().find("#callID").val()
                        }

                        console.log("call: " + JSON.stringify(call));

                        //$.ajax({
                        //    url: 'http://10.251.13.80/agvAPI/api/Calls/Insert',
                        //    dataType: 'json',
                        //    data: call,
                        //    type: "POST"
                        //}).done(function (data) {
                        //    console.log("data encoded: " + JSON.stringify(data));
                        //}).fail(function (xhr, status, error) {
                        //    console.log("Could not reach the API: " + error);
                        //    alert("Falha");
                        //});

                        $.ajax({
                            url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/Config/UpdateStartById?ID=1&Start=true',
                            type: "POST"
                        }).done(function (data) {
                            console.log("resp: " + JSON.stringify(data));
                            //alert("Starting...");
                        }).fail(function (xhr, Modal, error) {
                            alert("Failed to connect");
                        });

                        //var ID = this.id;
                        //$.ajax({
                        //    url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/ConfigPoints/TransferRouteToAGV?ID=' + ID,
                        //    type: "GET"
                        //}).done(function (data) {
                        //    $("#encoded").html(data.encoded);
                        //    alert("Dados Gravados com Sucesso!");

                        //    $.ajax({
                        //        url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/Antenna/OpenReadAntenna',
                        //        type: "GET"
                        //    }).done(function () {
                        //        alert("Abrindo console...");
                        //    }).fail(function (xhr, Modal, error) {
                        //        alert("Falha em abrir o console");
                        //    });

                        //}).fail(function (xhr, Modal, error) {
                        //    $("#error").html("Could not reach the API: " + error);
                        //    $('#loader').hide();
                        //});
                    });


                    //if (completeFunction != null)
                    //    completeFunction(obj);
                },
                //always: function () {
                //    if (alwaysFunction != null)
                //        alwaysFunction(obj);
                //},
            });


        }
    });
});



function insertRow(data) {
    var bodyCode = $("#tableCode").find("tbody");
    var bodyRoute = $("#tableRoute").find("tbody");
    var bodyItem = $("#tableItem").find("tbody");
    var bodyStart = $("#tableStart").find("tbody");
    bodyCode.empty();
    bodyRoute.empty();
    bodyItem.empty();
    bodyStart.empty();

    console.log("length: " + data.length);

    data.forEach(function (item, index) {
        console.log("data: " + JSON.stringify(item));
        var row = '<tr class="listCode" id="listCode' + item.id + '">' +
            '<td>' + item.id + '</td>' +
            '</tr>';
        bodyCode.append(row);

        var route = "";
        var routeID = 0;
        switch (item.destination) {
            case "P26":
                route = "EIXO_NEW";
                routeID = 1020;
                break;
            default:
                route = "Rota inexistente"
                routeID = 0;
        }

        //$.ajax({
        //    url: 'http://agv-api.eu-west-1.elasticbeanstalk.com/api/Route/GetAGVByID?ID='+,
        //    type: "POST"
        //}).done(function (data) {
        //    console.log("resp: " + JSON.stringify(data));
        //    //alert("Starting...");
        //}).fail(function (xhr, Modal, error) {
        //    alert("Failed to connect");
        //});

        row = '<tr class="listRoute" id="listRoute' + item.id + '">' +
            '<td>' + route + '</td>' +
            '</tr>';
        bodyRoute.append(row);

        row = '<tr class="listItem" id="listItem' + item.id + '">' +
            '<td class="description">' + item.description + '</td>' +
            '</tr>';
        bodyItem.append(row);

        row = '<tr class="listStart" id="listStart' + item.id + '">' +
            '<input type="hidden" id="callID" value="' + item.id + '"></input>' +
            '<input type="hidden" id="routeID" value="' + routeID + '"></input>' +
            '<td> <input class="startRoute" type="submit" value="Start"> </td>' +
            '</tr>';
        bodyStart.append(row);
    });
}