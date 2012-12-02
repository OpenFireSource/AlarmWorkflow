package com.alarmworkflow.eAlarmApp.datastorage;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.List;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NodeList;

import com.alarmworkflow.eAlarmApp.general.GeoHelper;
import com.alarmworkflow.eAlarmApp.general.KMLParser;
import com.google.android.maps.GeoPoint;
import com.google.android.maps.OverlayItem;

import android.os.Environment;
import android.util.Log;

public class KMLStorage {

	static final String KEY_PLACEMARK = "Placemark";
	static final String KEY_NAME = "name";
	static final String KEY_COORDINATES = "coordinates";
	static final String KEY_DESC = "description";

	String kmlContent = "";

	private List<OverlayItem> hydranten = new ArrayList<OverlayItem>();

	private List<OverlayItem> bmzs = new ArrayList<OverlayItem>();
	private List<OverlayItem> otherPlaces = new ArrayList<OverlayItem>();
	private static KMLStorage instance;

	public static KMLStorage getInstance() {

		if (instance == null) {
			instance = new KMLStorage();
		}
		return instance;
	}

	public KMLStorage() {
		readFile();
		parse();
	}

	void readFile() {
		if (!Environment.MEDIA_MOUNTED.equals(Environment
				.getExternalStorageState())) {

		} else {
			try {
				File file = new File("/sdcard/eAlarmapp.kml");
				if (file.exists()) {
					FileInputStream fileIn = new FileInputStream(file);
					BufferedReader reader = new BufferedReader(
							new InputStreamReader(fileIn));
					String line = "";
					while ((line = reader.readLine()) != null) {
						kmlContent += line;
					}

					reader.close();
					Log.i("READER", "Read File " + kmlContent);
				} else {
					file.createNewFile();
				}
			} catch (Exception e) {
			}
		}
	}

	void parse() {
		KMLParser parser = new KMLParser();
		Document doc = parser.getDomElement(kmlContent);
		if (doc == null)
			return;
		NodeList nl = doc.getElementsByTagName(KEY_PLACEMARK);

		for (int i = 0; i < nl.getLength(); i++) {
			Element e = (Element) nl.item(i);
			String name = parser.getValue(e, KEY_NAME);
			String coordinates = parser.getValue(e, KEY_COORDINATES);
			String longitude = coordinates.split(",")[0];
			String latitude = coordinates.split(",")[1];
			String description = parser.getValue(e, KEY_DESC);
			try {
				if (name.toLowerCase().contains("ufh")
						|| name.toLowerCase().contains("uefh")) {
					GeoPoint point = new GeoPoint(
							(int) (Float.parseFloat(latitude) * 1E6),
							(int) (Float.parseFloat(longitude) * 1E6));
					getHydranten().add(
							new OverlayItem(point, name, description));
					GeoHelper.getInstance().addPoint(point);
				} else if (name.toLowerCase().contains("bmz")) {
					GeoPoint point = new GeoPoint(
							(int) (Float.parseFloat(latitude) * 1E6),
							(int) (Float.parseFloat(longitude) * 1E6));
					getBmzs().add(new OverlayItem(point, name, description));
					GeoHelper.getInstance().addPoint(point);
				} else {
					GeoPoint point = new GeoPoint(
							(int) (Float.parseFloat(latitude) * 1E6),
							(int) (Float.parseFloat(longitude) * 1E6));
					getOtherPlaces().add(
							new OverlayItem(point, name, description));
					GeoHelper.getInstance().addPoint(point);
				}
			} catch (NumberFormatException ex) {

			}
		}
	}

	/**
	 * @return the bmzs
	 */
	public List<OverlayItem> getBmzs() {
		return bmzs;
	}

	/**
	 * @param bmzs
	 *            the bmzs to set
	 */
	public void setBmzs(List<OverlayItem> bmzs) {
		this.bmzs = bmzs;
	}

	/**
	 * @return the otherPlaces
	 */
	public List<OverlayItem> getOtherPlaces() {
		return otherPlaces;
	}

	/**
	 * @param otherPlaces
	 *            the otherPlaces to set
	 */
	public void setOtherPlaces(List<OverlayItem> otherPlaces) {
		this.otherPlaces = otherPlaces;
	}

	/**
	 * @return the hydranten
	 */
	public List<OverlayItem> getHydranten() {
		return hydranten;
	}

	/**
	 * @param hydranten
	 *            the hydranten to set
	 */
	public void setHydranten(List<OverlayItem> hydranten) {
		this.hydranten = hydranten;
	}
}
