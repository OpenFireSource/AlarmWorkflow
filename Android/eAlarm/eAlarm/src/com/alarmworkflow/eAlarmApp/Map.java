package com.alarmworkflow.eAlarmApp;

import mapviewballoons.example.simple.SimpleItemizedOverlay;

import com.alarmworkflow.eAlarmApp.R;
import com.alarmworkflow.eAlarmApp.general.CustomMapView;
import com.alarmworkflow.eAlarmApp.services.DataSource;
import com.alarmworkflow.eAlarmApp.services.MySQLiteHelper;
import com.google.android.maps.GeoPoint;
import com.google.android.maps.MapActivity;
import com.google.android.maps.MapController;
import com.google.android.maps.MapView;
import com.google.android.maps.OverlayItem;
import android.content.Intent;
import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.util.Log;
import android.view.MotionEvent;
import android.widget.ListView;

public class Map extends MapActivity {
	private MapView mapView;
	private MapController mapControl;

	SimpleItemizedOverlay operationOverlay;
	private ListView poiList;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.mapview);
		mapView = (CustomMapView) findViewById(R.id.map);
		poiList = (ListView) findViewById(R.id.poiList);
		if(poiList != null)
			Log.i("POI","POI Liste is da");
		// Vielleicht ganz interessant fuer Anfahrt und co.
		mapView.setTraffic(true);
		// Zoombuttons
		mapView.setBuiltInZoomControls(true);
		mapControl = mapView.getController();
		mapControl.setZoom(17);
		operationOverlay = new SimpleItemizedOverlay(getResources().getDrawable(
				R.drawable.brandmarker), mapView);
		gotoEinsatz();

	}

	private void gotoEinsatz() {
		Intent intent = getIntent();
		// ID des Einsatzes
		String id = intent.getExtras().getString(MySQLiteHelper.COLUMN_ID);
		DataSource data = DataSource.getInstance(this);
		java.util.Map<String, String> details = data.getDetails(id);
		// Latitude
		String lat = details.get(MySQLiteHelper.COLUMN_LAT);
		// Longitude
		String lon = details.get(MySQLiteHelper.COLUMN_LONG);

		Drawable drawable = this.getResources().getDrawable(
				R.drawable.brandmarker);

		try {
			// Geopoint-konform umwandeln (Koennte Exceptions schmeissen)
			int latitude = (int) (Float.parseFloat(lat) * 1E6);
			int longitude = (int) (Float.parseFloat(lon) * 1E6);
			GeoPoint alarm = new GeoPoint(latitude, longitude);
			mapControl.animateTo(alarm);
			createEinsatzMarker(alarm);

		} catch (NumberFormatException e) {
		}

	}

	private void createEinsatzMarker(GeoPoint p) {
		OverlayItem item = new OverlayItem(p, "Einsatzort", "");
		operationOverlay.addOverlay(item);
		mapView.getOverlays().add(operationOverlay);
		Log.i("MARKER", "Marker sollte hinzugefuegt worden sein");
		Log.i("MARKER", mapView.getOverlays().size() + "");
	}

	@Override
	protected boolean isRouteDisplayed() {
		// TODO Auto-generated method stub
		return false;
	}

}
