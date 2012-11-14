<?php
/*************************************************************************
*    This peace of software was writte by Andreas Glunz <andreas.glunz@mytum.de>     *
* It is Beerware. So do what ever you want. And if you like it, you can buy me a beer.  *
**************************************************************************/
?>
<html>
        <head>
                <meta http-equiv="content-type" content="text/html; charset=utf-8"/>
                <title>Alarmview</title>
                <link rel="stylesheet" type="text/css" href="templates/FFWPlanegg/styles.css">
                <?php $xajax->printJavascript(); ?>
                <script type='text/javascript' charset='UTF-8'>
                        /* <![CDATA[ */
                        var mytimer = window.setInterval ("test()",10000);
                        RedDot = new Image();
                        RedDot.src = "icons/reddot.gif";
                        function test()
                        {
                                weburl = window.location.href.substr(0, window.location.href.lastIndexOf('/') + 1);

                                if(document.getElementById("StatusImage").src == weburl+"icons/greendot.gif")
                                {
                                        document.getElementById("StatusImage").src = "icons/yellowdot.gif";
                                        document.getElementById("StatusText").innerHTML = "Synchronisiere...";
                                }
                                else
                                {
                                        document.getElementById("StatusImage").src = RedDot.src;
                                        document.getElementById("StatusText").innerHTML = "Server nicht gefunden!";
                                }
                                xajax_myFunction(<?php echo $db->getLastAlarmID(); /*echo "1";*/ ?>);

                        }
                        /* ]]> */
                </script>
                <script type="text/javascript"><!--
                        function taste(e) {
                                var zeichen = String.fromCharCode(e.which);
                                if (zeichen.toUpperCase() == "R") {
                                        xajax_ShutdownDisplay("<?php if($_GET["wache"] != null){ echo $_GET["wache"];} ?>");
                                        JavaScript:location.reload();
                                }
                        }
                        document.captureEvents(Event.KEYPRESS);
                        document.onkeypress = taste;
                //--></script>

                <script src="http://maps.google.com/maps?file=api&v=2.x&key=ABQIAAAAS0cvlevLva1mX6USDZifTBQYHCFQyehzCVeYkJhwOSy2qLH9bBTG6KDxbFiw9DYYn2ry_JY7bXwcMg"
      type="text/javascript"></script>

                <script type="text/javascript">
            var map;
    var gdir;
    var geocoder = null;
    var addressMarker;

    function initialize() {
      if (GBrowserIsCompatible()) {
        map = new GMap2(document.getElementById("map_canvas"));
        map.setCenter(new GLatLng(48.139688,11.359617), 15);
        gdir = new GDirections(map, document.getElementById("gdir"));
        GEvent.addListener(gdir, "addoverlay", onGDirectionsLoad);
        GEvent.addListener(gdir, "error", handleErrors);

      }
    }

    function setDirections(gdir) {
      gdir.load(gdir);
    }



  function onGDirectionsLoad(){
   var poly = gdir.getPolyline();
   if (poly.getVertexCount() > 700) {
     alert("This route has too many vertices");
     return;
   }
   var baseUrl = "http://maps.google.com/staticmap?";

   var params = [];
   var markersArray = [];
   markersArray.push(poly.getVertex(0).toUrlValue(5) + ",greena");
   markersArray.push(poly.getVertex(poly.getVertexCount()-1).toUrlValue(5) + ",greenb");
   params.push("markers=" + markersArray.join("|"));

   var polyParams = "rgba:0x0000FF80,weight:5|";
   var polyLatLngs = [];
   for (var j = 0; j < poly.getVertexCount(); j++) {
     polyLatLngs.push(poly.getVertex(j).lat().toFixed(5) + "," + poly.getVertex(j).lng().toFixed(5));
   }
   params.push("path=" + polyParams + polyLatLngs.join("|"));
   params.push("size=252x400");
   params.push("key=ABQIAAAAS0cvlevLva1mX6USDZifTBQYHCFQyehzCVeYkJhwOSy2qLH9bBTG6KDxbFiw9DYYn2ry_JY7bXwcMg");

   baseUrl += params.join("&");



   var extraParams = [];
   extraParams.push("center=" + poly.getVertex(poly.getVertexCount()-1).toUrlValue(5));
   extraParams.push("zoom=" + 16);
   addImg(baseUrl + "&" + extraParams.join("&"), "staticMapEndIMG");
}

function addImg(url, id) {
 var img = document.createElement("img");
 img.src = url;
 document.getElementById(id).innerHTML = "";
 document.getElementById(id).appendChild(img);
}

    </script>

  </head>

<body bgcolor="#83C7FA" onload="initialize()" onunload="GUnload()">

<form action="#" onsubmit="setDirections(this.from.value, this.to.value, this.locale.value); return false">

                <table width="100%" height="95%" border="0">
                        <tr height="50%">
                                <td>
                                        <?php echo $moduleHandler->getModule("details")->postContent(); ?>
                                </td>
                        </tr>
         <tr>
                                <td>
                <div id="map_canvas" style="width: 70%; height: 400px; float:left; border: 3px solid black;"></div>

                 <div id="staticMapEndIMG" style="width: 28%; height: 400px; float:left; border: 3px solid black;"></div>


                </td>

                        </tr>
                        <tr height="3%">
                            <td>
                <?php echo $moduleHandler->getModule("fahrzeuge")->postContent(); ?>
                </td>
            </tr>

            <tr>
                                <td id="StatusBereich">
                                        <?php echo $moduleHandler->getModule("status")->postContent(); ?>
                                </td>

                                <td>
       <!--         <a href="javascript:window.print()" >Drucken</a>-->
                </td>
                        </tr>
                </table>
        </body>

</html>