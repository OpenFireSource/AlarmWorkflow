/// <reference path="..\Scripts\jquery-2.1.3.js" />
"use strict";

execute();

// Returns a location string from the given location.
function getLocationString(location) {
    return location.Street + " " + location.StreetNumber + ", " + location.ZipCode + " " + location.City;
}

// Log something to either the logging mechanism or the console, depending on whether the script is executed by AW or tested in IE.
function log(text) {
    if (typeof window.external.log != "undefined") {
        window.external.log(text);
    } else {
        console.log(text);
    }
}

// Notifies the hosting IE that the script has concluded. Hint: This is required so that the job can safely continue execution!
function setReady() {
    if (typeof window.external.setReady != "undefined") {
        window.external.setReady();
    }
}

// Runs the main logic.
function execute() {

    log("execute() has started...");

    /* The variables are null when the page is opened in a browser. In this case we fill in some test data so the user can test the script.
     */
    if (var_awOperation === null && var_awSource === null) {

        var_awSource = {
            Street: "Karlstraße",
            StreetNumber: "5",
            City: "München",
            ZipCode: "80335"
        };

        var_awOperation = {
            Id: 9999,
            OperationGuid: "D92D2CB0-C00E-46D7-AF70-147A5B8D14E7",
            TimestampIncome: "2015-01-04T10:53:30.1914442+01:00",
            Timestamp: "2013-05-08T17:28:00+02:00",
            OperationNumber: "A B C 1 2 3, 4 5 6",
            Messenger: "The AlarmWorkflow-Team",
            Priority: "3",
            Einsatzort: {
                Street: "Hirtenstraße",
                StreetNumber: "5",
                City: "München",
                ZipCode: "80335"
            },
            Comment: "Testkommentar",
            Picture: "Hier ist etwas zu tun, dort auch, und daneben ebenfalls.",
            OperationPlan: "Keiner weiß mehr",
            Keywords: {
                Keyword: "TEST",
                EmergencyKeyword: "TEST",
                B: "BMA",
                R: "",
                S: "",
                T: ""
            },
            Resources: [{
                FullName: "1.2.3 ABC FF Testhausen 30/1 (DLK 23/12)", Timestamp: "04.01.2015 11:10:00", RequestedEquipment: []
            }, {
                FullName: "1.2.3 ABC FF Testhausen 40/2 (LF)", Timestamp: "04.01.2015 11:10:00", RequestedEquipment: []
            }, {
                FullName: "1.2.3 ABC FF Testhausen 40/3 (LF)", Timestamp: "04.01.2015 11:10:00", RequestedEquipment: ["2x Pressluftatmer", "Dies", "und das"]
            }, {
                FullName: "1.2.3 ABC FF Testhausen 10/1 (LF) (KdoW)", Timestamp: "04.01.2015 11:10:00", RequestedEquipment: ["Wärmebildkamera", "Und noch etwas"]
            }],
            CustomData: {
                Absender: "ILS TESTHAUSEN",
                Termin: "",
                "Einsatzort Zusatz": "",
                "Einsatzort Plannummer": "TSH.A.12.345.",
                "Einsatzort Station": "",
                "Zielort Zusatz": "",
                "Zielort Station": ""
            },
            IsAcknowledged: false,
            Loops: ["123", "234", "345", "456"]
        };
    };

    var routeSrc = "";
    routeSrc += "http://maps.google.com/maps/api/directions/json?sensor=false";
    routeSrc += "&origin=" + encodeURIComponent(getLocationString(var_awSource));
    routeSrc += "&destination=" + encodeURIComponent(getLocationString(var_awOperation.Einsatzort));

    log("Requesting route polyline using URI \"" + routeSrc + "\".");

    /* HINT: This statement works only in IE (10+). In Chrome etc. you'll get a "same-origin" violation.
     * For our case it shouldn't matter too much since we use the IE, but that should probably be done properly.
     */
    try {
        $.getJSON(routeSrc, "", function (data) {

            try {

                var routePolylinePoints = data.routes[0].overview_polyline.points;

                var secondRequest = "";
                secondRequest += "http://maps.google.com/maps/api/staticmap?sensor=false";
                secondRequest += "&size=800x800";
                secondRequest += "&path=weight:3|color:red|enc:" + routePolylinePoints;

                log("Requesting route image using URI \"" + secondRequest + "\".");

                var $routeImg = $("<img src='" + secondRequest + "' />");
                $("#route").replaceWith($routeImg);

                log("execute() has finished successfully!");

            } catch (e) {
                log("Error retrieving the route image! Reason: " + e.message);
            }

            setReady();
        });

    } catch (e) {
        log("Error retrieving the route polyline! Reason: " + e.message);
        setReady();
    }

}