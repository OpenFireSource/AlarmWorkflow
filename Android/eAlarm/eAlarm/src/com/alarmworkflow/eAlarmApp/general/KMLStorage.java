package com.alarmworkflow.eAlarmApp.general;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.List;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NodeList;

import com.google.android.maps.GeoPoint;
import com.google.android.maps.OverlayItem;

import android.os.Environment;

public class KMLStorage {

	static final String KEY_PLACEMARK = "Placemark";
	static final String KEY_NAME = "name";
	static final String KEY_COORDINATES = "coordinates";
	static final String KEY_DESC = "description";

	String kmlContent = "";

	List<OverlayItem> places = new ArrayList<OverlayItem>();

	private static KMLStorage instance;

	public static KMLStorage getInstance() {

		if (instance == null) {
			instance = new KMLStorage();
		}
		return instance;
	}

	public KMLStorage() {
		// TODO Auto-generated constructor stub
	}

	void readFile() {
		if (!Environment.MEDIA_MOUNTED.equals(Environment
				.getExternalStorageState())) {

		} else {
			try {
				File file = new File(
						"/sdcard/com.alarmworkflow.eAlarmapp.geopoints.kml");
				if (file.exists()) {
					FileInputStream fileIn = new FileInputStream(file);
					BufferedReader reader = new BufferedReader(
							new InputStreamReader(fileIn));
					String line = "";
					while ((line = reader.readLine()) != null) {
						kmlContent += line + "\n";
					}

					reader.close();
				} else {
					file.createNewFile();
				}
			} catch (Exception e) {
			}
		}
	}

	void parse() {
		KMLParser parser = new KMLParser();
		String xml = "";
		Document doc = parser.getDomElement(xml);
		NodeList nl = doc.getElementsByTagName(KEY_PLACEMARK);

		for (int i = 0; i < nl.getLength(); i++) {
			Element e = (Element) nl.item(i);
			String name = parser.getValue(e, KEY_NAME);
			String coordinates = parser.getValue(e, KEY_COORDINATES);
			String longitude = coordinates.split(",")[0];
			String latitude = coordinates.split(",")[1];
			String description = parser.getValue(e, KEY_DESC);
			try {
				places.add(new OverlayItem(new GeoPoint((int) (Float
						.parseFloat(latitude) * 1E6), (int) (Float
						.parseFloat(longitude) * 1E6)), name, description));
			} catch (NumberFormatException ex) {

			}
		}
	}
}
