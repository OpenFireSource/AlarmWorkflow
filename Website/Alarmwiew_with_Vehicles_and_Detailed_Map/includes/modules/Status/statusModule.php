<?php
/*************************************************************************
*    This peace of software was writte by Andreas Glunz <andreas.glunz@mytum.de>     *
* It is Beerware. So do what ever you want. And if you like it, you can buy me a beer.  *
**************************************************************************/
include "moduleconfig.php";

class statusModule implements FahrzeughallenModule
{
	private $FMSDB;
	
	public function postContent()
	{
		return $this->buildContent();
	}
	
	public function buildContent()
	{
		$return = "";
		$return .= "<table width=\"100%\"><tr><td width=\"20px\"><img id=\"StatusImage\" src=\"icons/greendot.gif\" /></td><td id=\"StatusText\">Aktuell</td></tr></table>";
		return $return;
	}
    
    public function configure($DB)
    {
    }
	
	public function update($objResponse, $timestamp)
	{
		$objResponse->assign("StatusBereich", "innerHTML", $this->buildContent());
		return $objResponse;
	}
}
?>