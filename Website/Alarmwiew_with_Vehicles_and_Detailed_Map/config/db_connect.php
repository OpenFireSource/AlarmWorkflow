<?php
	//PEAR::DB
	require_once 'DB.php';

	$dsn = array(
		'phptype'  => 'mysql',
		'username' => 'root',			// Datenbank Username
		'password' => '',	    		// Datenbank Passwort
		'hostspec' => 'localhost',		// Datenbank host
		'database' => 'alarmworkflow',	// Datenbank Name
	);

	$options = array(
		'debug'       => 2,
	);

	$db =& DB::connect($dsn, $options);
	if (DB::isError($db)) {
		die($db->getMessage());
	}

	$tbl_fms_fz         = 'fms_fz';
	$tbl_fms_hist       = 'fms_hist';
	$tbl_zvei_schleifen = 'zvei_schleifen';
	$tbl_zvei_hist      = 'zvei_hist';
?>
