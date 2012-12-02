package com.alarmworkflow.eAlarmApp.general;

import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;

import com.google.android.maps.GeoPoint;

public class GeoHelper {
	private static GeoHelper instance;
	private static List<GeoPoint> allPoints = new ArrayList<GeoPoint>();

	public static GeoHelper getInstance() {

		if (instance == null) {
			instance = new GeoHelper();
		}
		return instance;
	}

	public static List<GeoPoint> findMarker(GeoPoint topleft,
			GeoPoint bottomright, int zoomlevel) {
		List<GeoPoint> marker = new LinkedList<GeoPoint>();
		if (zoomlevel == 0 || zoomlevel == 1)
			marker.addAll(allPoints);
		else {
			if (topleft.getLongitudeE6() > bottomright.getLongitudeE6()) {
				for (int i = 0; i < allPoints.size(); i++) {
					GeoPoint p = allPoints.get(i);
					if ((p.getLongitudeE6() > topleft.getLongitudeE6() || p
							.getLongitudeE6() < bottomright.getLongitudeE6())
							&& p.getLatitudeE6() < topleft.getLatitudeE6()
							&& p.getLatitudeE6() > bottomright.getLatitudeE6()) {
						marker.add(p);
					}
				}
			} else {
				for (int i = 0; i < allPoints.size(); i++) {
					GeoPoint p = allPoints.get(i);
					if (p.getLongitudeE6() > topleft.getLongitudeE6()
							&& p.getLatitudeE6() < topleft.getLatitudeE6()
							&& p.getLongitudeE6() < bottomright
									.getLongitudeE6()
							&& p.getLatitudeE6() > bottomright.getLatitudeE6()) {
						marker.add(p);
					}
				}
			}
		}
		return marker;
	}

	/**
	 * @return the allPoints
	 */
	public List<GeoPoint> getAllPoints() {
		return allPoints;
	}

	/**
	 * 
	 */
	public void addPoints(List<GeoPoint> points) {
		allPoints.addAll(points);
	}

	/**
	 * @return the allPoints
	 */
	public void addPoint(GeoPoint point) {
		allPoints.add(point);
	}
	
}