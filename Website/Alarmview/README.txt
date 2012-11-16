Here are some informations to customize this website

Sample Coordinates 48.139688,11.359617

File ./content.php
Set your coordinates in the following line

map.setCenter(new GLatLng(48.139688,11.359617), 15);




File ./includes/modules/alarmDetails/alarmDetailsModule.php
Set your coordinates in the following lines at the 4 .load statements

                                $objResponse->script("directions1.load(\"from: 48.139688,11.359617 to: ". $array[0] .", ". $strasseArray[0] ."\", { \"preserveViewport\": \"false\" });");
                        else
                                $objResponse->script("directions1.load(\"from: 48.139688,11.359617 to: ". $array[0] .", ". $strasseArray[0] ."\");");


            $objResponse->assign("AlarmDetails", "innerHTML", $return);

                        $array = explode(" ", $zeile[Ort]);
                        $strasseArray = array();
                        $test = preg_match("*[\D]+\s[0-9]+*", $zeile["Strasse"], $strasseArray);
                        if($test == 0)
                                $strasseArray[0] = str_replace("?", "ss", $zeile["Strasse"]);
                        if($array[0] == "Hebertshausen")
                                $objResponse->script("gdir.load(\"from: 48.139688,11.359617 to: ". $array[0] .", ". $strasseArray[0] ."\", { \"preserveViewport\": \"false\" });");
                        else
                                $objResponse->script("gdir.load(\"from: 48.139688,11.359617 to: ". $array[0] .", ". $strasseArray[0] ."\");");




File ./config.config.php
in this file you have to set the correct MySQL Database Credentials

