<?php
/*************************************************************************
*    This peace of software was writte by Andreas Glunz <andreas.glunz@mytum.de>     *
* It is Beerware. So do what ever you want. And if you like it, you can buy me a beer.  *
**************************************************************************/
interface FahrzeughallenModule
{
    public function postContent();
    public function configure($db);
	public function update($objResponse, $timestamp);
}

include "Status/statusModule.php";
include "alarmDetails/alarmDetailsModule.php";
include "fahrzeuge/Fahrzeugemodule.php";


class moduleHandler
{
	private $AlarmDB;
	private $modules = array();
	
	function __construct($db)
	{ 
		$this->AlarmDB = $db;
		$this->modules["status"] = new statusModule();
		$this->modules["status"]->configure($db);
		$this->modules["details"] = new alarmDetailsModule();
		$this->modules["details"]->configure($db);
		$this->modules["fahrzeuge"] = new FahrzeugeModule();
		$this->modules["fahrzeuge"]->configure($db);
	}
	
	function getModule($Name)
	{
		return $this->modules[$Name];
	}
}
?>