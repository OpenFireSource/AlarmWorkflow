<?php
/*************************************************************************
*    This peace of software was writte by Andreas Glunz <andreas.glunz@mytum.de>     *
* It is Beerware. So do what ever you want. And if you like it, you can buy me a beer.  *
**************************************************************************/
?>
<html>
	<head>
		<meta http-equiv="content-type" content="text/html; charset=utf-8"/>
		<title><?php echo $ws_title; ?></title>
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
			
		<script src="http://maps.google.com/maps?file=api&v=2.x&key=ABQIAAAA99vtIh_YB992KkU5phoZfhSuRcb93pVnRhGk57Q2-2NOKpAD5RRiYYrPd5uktL4JFxYwzWaZ0ivmnw"
      type="text/javascript"></script>
			
		<script type="text/javascript">
	    var map;
	    var directions;


	    function initialize() {
	      if (GBrowserIsCompatible())
	        map = new GMap2(document.getElementById("map_canvas"));
			map.setCenter(new GLatLng(48.1059833,11.4237051), 14);
			directions = new GDirections(map);
			//directions.load("from: Planegg, Pasinger Strasse 24 to: Martinsried, Keplerweg 9");
			//directions.load("from:Planegg, Pasinger Straﬂe 24 to: Martinsried, Keplerweg 9");
	        //GEvent.addListener(directions, "load", onGDirectionsLoad);
	        //GEvent.addListener(directions, "error", handleErrors);
			//http://maps.google.de/maps/mm?ie=UTF8&hl=de&ll=48.107718,11.43548&spn=0.043099,0.076904&z=14
			//directions.load("from:Pasingerstr. 24, planegg to:Ruffiniallee, planegg", { "preserveViewport": "true" });
			//directions.load("from:Planegg, Pasinger Str. 24 to:Planegg, Gumstrasse 19", { "preserveViewport": "true" });
			//83C7FA

	      
	    }
	   </script>
	</head>
	<body bgcolor="#83C7FA" onload="initialize()" onunload="GUnload()">
		<table width="100%" height="100%" border="0">
			<tr height="50%">
				<td>
					<?php echo $moduleHandler->getModule("details")->postContent(); ?> 
				</td>
			</tr>
			<tr>
				<td>
					<div id="map_canvas" style="width: 1000px; height: 365px; float:left; border: 1px solid black;"></div>
				</td>
				<!--<td>
					Asr&uuml;cke Ordnung
				</td>-->
			</tr>
			<tr>
				<td id="StatusBereich">
					<?php echo $moduleHandler->getModule("status")->postContent(); ?> 
				</td>
			</tr>
		</table>
	</body>
</html>