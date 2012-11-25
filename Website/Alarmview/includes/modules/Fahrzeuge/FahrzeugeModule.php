<?php
/*************************************************************************
*    This peace of software was writte by Andreas Glunz <andreas.glunz@mytum.de>     *
* It is Beerware. So do what ever you want. And if you like it, you can buy me a beer.  *
**************************************************************************/
include "moduleconfig.php";

class FahrzeugeModule implements FahrzeughallenModule
{
        private $AlarmDB;

        public function postContent()
        {
                return $this->buildContent();
        }

        public function buildContent()
        {
                $zeile = $this->AlarmDB->getLastAlarm();
                $return = "";
                $return .= "<table id=\"FahrzeugeDetails\" height=\"100%\" width=\"100%\" border=\"true\">";
                $return .= "<tr>";
                $return .= "<td id=\"NoAlert_FZ\"> Fahrzeuge</td>";
                $return .= "</tr></table>";
                return $return;
        }

    public function configure($DB)
    {
                $this->AlarmDB = $DB;
    }



        public function update($objResponse, $arg)
        {
                $lastAlarm = $this->AlarmDB->getLastAlarmID();
                //$objResponse->script("var x = prompt(\"". $arg ."\");");
                if($lastAlarm != $arg)
                {
                        $zeile = $this->AlarmDB->getLastAlarm();

        $return .= "<table id=\"Fahrzeuge\" height=\"100%\" width=\"100%\" border=\"true\">";
                $return .= "<tr>";

                $return .= "<td width=\"16%\" id=\"Fahrzeuge\">";
                if($zeile[Fahrzeuge] == "")
                        {
                                $return .= "<br />";
                        }
                        else
                        {
                                $return .= $zeile[Fahrzeuge];
                        }
                        $return .= "</td>";



                $return .= "</tr></table>";
                $return .="</td>";

                        $objResponse->assign("FahrzeugeDetails", "innerHTML", $return);




                }

                return $objResponse;



        }
}

?>