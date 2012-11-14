<?php
/*************************************************************************
*    This peace of software was writte by Andreas Glunz <andreas.glunz@mytum.de>     *
* It is Beerware. So do what ever you want. And if you like it, you can buy me a beer.  *
**************************************************************************/
include "config/config.php";
include "includes/adodb/adodb.inc.php";
include "includes/AlarmDB.php";
include "includes/modules/module.php";
require ('xajax_core/xajax.inc.php');

$xajax = new xajax();
$xajax->configure('javascript URI', '');

$db = new AlarmDB($db_server, $db_user, $db_pwd, $db_database);

$xajax->registerFunction("myFunction");
$xajax->registerFunction("ShutdownDisplay");
$xajax->processRequest();

function ShutdownDisplay($arg)
{
	//This is for debuging and development issues
	if(true)
	{
		//Test if Display is On!!
		$ip = "";
		if($arg == "mrd")
		{
			$ip = "192.168.1.253"; //Martinsried
		}
		else
		{
			$ip = "192.168.0.243"; // Planegg
		}
		$url = "http://". $ip ."/";
		$url = parse_url($url);
	    do {
	        $fp = @fsockopen($url[host], 80);
	    } while(!$fp);
	    fputs($fp, "GET {$url[path]}?{$url[query]} HTTP/1.1\r\n");
	    fputs($fp, "Host: {$url[host]}\r\n");
	    fputs($fp, "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; de; rv:1.8.1.4) Gecko/20070515 Firefox/2.0.0.4\r\n");
	    fputs($fp, "Accept: text/html\r\n");
	    fputs($fp, "Keep-Alive: 300\r\n");
	    fputs($fp, "Connection: keep-alive\r\n");
	    fputs($fp, "Cookie: TEST\r\n");
	    fputs($fp, "\r\n");

	    while(!feof($fp))
	      $res .= fgets($fp, 128);
	      
	    fclose($fp);
		
		//If display is off turn it on -- if display is on turn it off
		if(ereg( "TFTPowerControl: ON", $res ) != false)
		{
			$url = "http://" . $ip . "/SWITCH.CGI?s1=0";
			$url = parse_url($url);
			do {
				$fp = @fsockopen($url[host], 80);
			} while(!$fp);
			fputs($fp, "GET {$url[path]}?{$url[query]} HTTP/1.1\r\n");
			fputs($fp, "Host: {$url[host]}\r\n");
			fputs($fp, "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; de; rv:1.8.1.4) Gecko/20070515 Firefox/2.0.0.4\r\n");
			fputs($fp, "Accept: text/html\r\n");
		    #fputs($fp, "Referer: $referer\r\n");
		    fputs($fp, "Keep-Alive: 300\r\n");
		    fputs($fp, "Connection: keep-alive\r\n");
		    fputs($fp, "Cookie: TEST\r\n");
		    fputs($fp, "\r\n");

		    while(!feof($fp))
		      $res .= fgets($fp, 128);
		      
		    fclose($fp);
		}
		else if(ereg( "TFTPowerControl: OFF", $res ) != false)
		{
			$url = "http://" . $ip . "/SWITCH.CGI?s1=1";
			$url = parse_url($url);
		    do {
		        $fp = @fsockopen($url[host], 80);
		    } while(!$fp);
		    fputs($fp, "GET {$url[path]}?{$url[query]} HTTP/1.1\r\n");
		    fputs($fp, "Host: {$url[host]}\r\n");
		    fputs($fp, "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; de; rv:1.8.1.4) Gecko/20070515 Firefox/2.0.0.4\r\n");
		    fputs($fp, "Accept: text/html\r\n");
		    #fputs($fp, "Referer: $referer\r\n");
		    fputs($fp, "Keep-Alive: 300\r\n");
		    fputs($fp, "Connection: keep-alive\r\n");
		    fputs($fp, "Cookie: TEST\r\n");
		    fputs($fp, "\r\n");

		    while(!feof($fp))
		      $res .= fgets($fp, 128);
		      
		    fclose($fp);
		}
	}
}

function myFunction($arg)
{
	global $db_user;
	global $db_server;
	global $db_pwd;
	global $db_database;
		
	$db2 = new AlarmDB($db_server, $db_user, $db_pwd, $db_database);
	$moduleHandler2 = new moduleHandler($db2);
	
	// Instantiate the xajaxResponse object
	$objResponse = new xajaxResponse();
	$status = $moduleHandler2->getModule("status");
	$fahrzeuge = $moduleHandler2->getModule("fahrzeuge");
    $details = $moduleHandler2->getModule("details");
	
	
	//$objResponse->assign("AlarmHistoryBereich","innerHTML", $alarmHistory->postContent());
	//$objResponse->assign("FMSHistoryBereich","innerHTML", $fmsHistory->postContent());
	$objResponse = $status->update($objResponse, $arg);
	$objResponse = $details->update($objResponse, $arg);
	$objResponse = $fahrzeuge->update ($objResponse, $arg);

	
	//return the  xajaxResponse object
	return $objResponse;
}




$moduleHandler = new moduleHandler($db);

include "content.php";

?>