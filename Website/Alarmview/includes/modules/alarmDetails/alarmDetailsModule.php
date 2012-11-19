<?php
/*************************************************************************
*    This peace of software was writte by Andreas Glunz <andreas.glunz@mytum.de>     *
* It is Beerware. So do what ever you want. And if you like it, you can buy me a beer.  *
**************************************************************************/
include "moduleconfig.php";

class alarmDetailsModule implements FahrzeughallenModule
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
                $return .= "<table id=\"AlarmDetails\" height=\"100%\" width=\"100%\" border=\"true\">";
                $return .= "<tr>";
                $return .= "<td id=\"NoAlert\"> Kein Alarm! </td>";
                $return .= "</tr></table>";
                return $return;
        }

    public function configure($DB)
    {
                $this->AlarmDB = $DB;
    }

        private function getFarbe($stichwort)
        {
                switch($stichwort)
                {
                        case "Brand":
                                return "red";
                        case "Rettungsdienst":
                                return "white";
                        case "THL":
                                return "blue";
                        case "Sonstiges":
                                return "green";
                        default:
                                return "";
                }
        }

        public function update($objResponse, $arg)
        {
                $lastAlarm = $this->AlarmDB->getLastAlarmID();
                //$objResponse->script("var x = prompt(\"". $arg ."\");");
                if($lastAlarm != $arg)
                {
                        $zeile = $this->AlarmDB->getLastAlarm();
                        $return = "";
                        //$farbe = getFarbe("Brand");
                        $return .= "<tr>";
                        $return .= "<td width=\"50%\" id=\"AlarmMeldebild\">" . $zeile[Meldebild] . "</td>";
                        if($zeile[Hinweis] != "" && $zeile["Einsatzstichwort"] != "")
                                $return .= "<td id=\"AlarmHinweis\" >" . $zeile[Hinweis] . "<br />" . $zeile["Einsatzstichwort"] . "</td>";
                        else if($zeile[Hinweis] != "" || $zeile["Einsatzstichwort"] != "")
                                $return .= "<td id=\"AlarmHinweis\">" . $zeile[Hinweis] . $zeile["Einsatzstichwort"] . "</td>";
                        else
                                $return .= "<td style=\"text-align:center\"> <br /> </td>";


        $return .= "</tr><tr>";

                        $return .= "<td id=\"AlarmOrt\">" . $zeile[Ort] . "<br />" . $zeile["Strasse"] . "<br />" .$zeile["Kreuzung"] .  "</td>";

                        $return .= "<td id=\"AlarmObjekt\">" . $zeile[Alarmtime] . "<br />" . $zeile[Mitteiler] . "<br />" . $zeile["Objekt"] . "<br />" .$zeile["Einsatzplan"] .  "</td>";

                        // $return .= "<td id=\"AlarmObjekt\">" ;
                        // if($zeile[Objekt] == "")
                        // {
                                // $return .= "<br />";
                        // }
                        // else
                        // {
                                // $return .= $zeile[Objekt];
                        // }
                        // $return .= "</td>";


                        $objResponse->assign("AlarmDetails", "innerHTML", $return);

                        $array = explode(" ", $zeile[Ort]);
                        $strasseArray = array();
                        $test = preg_match("*[\D]+\s[0-9]+*", $zeile["Strasse"], $strasseArray);
                        if($test == 0)
                                $strasseArray[0] = str_replace("ß", "ss", $zeile["Strasse"]);
                        if($array[0] == "Hebertshausen")
                                $objResponse->script("directions1.load(\"from: 48.06618,11.38507 to: ". $array[0] .", ". $strasseArray[0] ."\", { \"preserveViewport\": \"false\" });");
                        else
                                $objResponse->script("directions1.load(\"from: 48.06618,11.38507 to: ". $array[0] .", ". $strasseArray[0] ."\");");


            $objResponse->assign("AlarmDetails", "innerHTML", $return);

                        $array = explode(" ", $zeile[Ort]);
                        $strasseArray = array();
                        $test = preg_match("*[\D]+\s[0-9]+*", $zeile["Strasse"], $strasseArray);
                        if($test == 0)
                                $strasseArray[0] = str_replace("ß", "ss", $zeile["Strasse"]);
                        if($array[0] == "Hebertshausen")
                                $objResponse->script("gdir.load(\"from: 48.06618,11.38507 to: ". $array[0] .", ". $strasseArray[0] ."\", { \"preserveViewport\": \"false\" });");
                        else
                                $objResponse->script("gdir.load(\"from: 48.06618,11.38507 to: ". $array[0] .", ". $strasseArray[0] ."\");");



                }

                return $objResponse;



        }
}

?>