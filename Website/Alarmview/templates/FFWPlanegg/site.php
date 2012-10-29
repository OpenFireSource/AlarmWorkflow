<?php
/*************************************************************************
*    This peace of software was writte by Andreas Glunz <andreas.glunz@mytum.de>     *
* It is Beerware. So do what ever you want. And if you like it, you can buy me a beer.  *
**************************************************************************/
?>
<html>
	<head>
		<title><?php echo $ws_title; ?></title>
		<link rel="stylesheet" type="text/css" href="template/FFWPlanegg/styles.css">
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
				xajax_myFunction(<?php echo "'".date("Y-m-d H:i:s" ,time())."'" ?>);
			}
			/* ]]> */
		</script>
		<script type="text/javascript"><!--
			function taste(e) {
				var zeichen = String.fromCharCode(e.which);
				if (zeichen.toUpperCase() == "R") {
					JavaScript:location.reload();
				}
			}
			document.captureEvents(Event.KEYPRESS);
			document.onkeypress = taste;
		//--></script>
	</head>
	<body bgcolor="#83C7FA">
		<table>
			<tr>
				<td id="StatusBereich">
					<?php echo $moduleHandler->getModule("status")->postContent(); ?> 
				</td>
			</tr>
		</table>
	</body>
</html>