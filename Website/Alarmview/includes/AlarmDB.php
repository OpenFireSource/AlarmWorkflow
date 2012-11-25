<?php
/*************************************************************************
*    This peace of software was writte by Andreas Glunz <andreas.glunz@mytum.de>     *
* It is Beerware. So do what ever you want. And if you like it, you can buy me a beer.  *
**************************************************************************/
class AlarmDB
{
        private $db;

        function __construct($server, $user, $pwd, $database)
        {
                $this->db = NewADOConnection('mysql');
                 $this->db->Connect($server, $user, $pwd, $database);
        }

        function getLastAlarmID()
        {
                $result = $this->db->Execute("SELECT max( id ) FROM tb_einsatz ");
                if ($result === false) die("Failed loading last Alarm ID");
                return $result->fields[0];
        }

        function getLastAlarm()
        {
                $val = $this->getLastAlarmID();
                $this->db->Execute("SET NAMES 'utf8'");
                $result = $this->db->Execute("SELECT Meldebild, Hinweis, Objekt, Einsatzplan, Ort, Strasse, Kreuzung, Einsatzort, Einsatznr, Einsatzstichwort, Stichwort, Fahrzeuge, Mitteiler, Alarmtime FROM tb_einsatz WHERE ID = " . $val);
                 if ($result === false) die("Failed loading last Alarm - " . $val);
                 $zeile = array();
                         $zeile["Meldebild"] = $result->fields[0];
                         $zeile["Hinweis"] = $result->fields[1];
                        $zeile["Objekt"] = $result->fields[2];
                        $zeile["Einsatzplan"] = $result->fields[3];
                        $zeile["Ort"] = $result->fields[4];
                        $zeile["Strasse"] = $result->fields[5];
                        $zeile["Kreuzung"] = $result->fields[6];
                        $zeile["Einsatzort"] = $result->fields[7];
                        $zeile["Einsatznr"] = $result->fields[8];
                        $zeile["Einsatzstichwort"] = $result->fields[9];
                        $zeile["Stichwort"] = $result->fields[10];
                        $zeile["Fahrzeuge"] = $result->fields[11];
                        $zeile["Mitteiler"] = $result->fields[12];
                        $zeile["Alarmtime"] = $result->fields[13];
                        return $zeile;
        }

        function getFMSHistory($count)
        {
                if(!is_numeric($count)) die("$count is not numeric");
                $result = $this->db->Execute("SELECT timestamp, kennung, status, richtung FROM fms_hist WHERE richtung = 'FZ->LST' ORDER BY timestamp DESC");
                 if ($result === false) die("Failed loading Fahrzeug History");
                 $i = 0;
                 $Funcresult = array();
                 while (!$result->EOF) {
                         $zeile = array();
                         $zeile["time"] = $result->fields[0];
                         $zeile["kennung"] = $result->fields[1];
                         $zeile["status"] = $result->fields[2];
                         $zeile["richtung"] = $result->fields[3];
                        if(array_key_exists($zeile["kennung"], $this->fahrzeuge))
                        {
                                $car = $this->fahrzeuge[$zeile["kennung"]];
                                $zeile["rufname"] = str_replace('–', '-', htmlentities($car["rufname"]));
                        }
                    $result->MoveNext();
                    $Funcresult[$i] = $zeile;
                    $i++;
                    if($i == $count) break;
                 }
                 return $Funcresult;
        }

        function getAlarmSchleifen($timestamp)
        {
                $result = $this->db->Execute("SELECT schleife , time FROM zvei_hist WHERE time >= '".$timestamp."'");
                if ($result === false) die("Failed loading Alarm Schleifen");
                $return = array();
                $i = 0;
                while (!$result->EOF)
                {
                        $zeile = array();
                        $zeile["schleife"] = $result->fields[0];
                        $zeile["time"] = $result->fields[1];
                        $return[$i] = $zeile;
                        $i++;
                        $result->MoveNext();
                }
                return $return;
        }

        function getSchleife($schleife)
        {
                return $this->schleifen[$schleife];
        }

        function getFahrzeug($kennung)
        {
                return $this->fahrzeuge[$kennung];
        }

        function getLstStatusBuchstaben($Number)
        {
                return $this->buchstaben[$Number];
        }
}
?>