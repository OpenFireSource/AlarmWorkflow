package com.alarmworkflow.eAlarmApp;

import com.alarmworkflow.eAlarmApp.R;
import com.alarmworkflow.eAlarmApp.services.DataSource;
import com.alarmworkflow.eAlarmApp.services.MySQLiteHelper;
import com.google.android.maps.GeoPoint;
import com.google.android.maps.ItemizedOverlay;
import com.google.android.maps.MapActivity;
import com.google.android.maps.MapController;
import com.google.android.maps.MapView;
import com.google.android.maps.OverlayItem;


import android.content.Context;
import android.content.Intent;
import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.util.Log;

public class Map extends MapActivity {
	private MapView map;
	private MapController mapControl;
	private OperationOverlay itemizedoverlay;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.mapview);
		map = (MapView) findViewById(R.id.map);
		// Vielleicht ganz interessant für Anfahrt und co.
		map.setTraffic(true);
		// Zoombuttons
		map.setBuiltInZoomControls(true);
		mapControl = map.getController();
		mapControl.setZoom(17);
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
		itemizedoverlay = new OperationOverlay(this, drawable);
		try {
			// Geopoint-konform umwandeln (könnte Exceptions schmeißen)
			int latitude = (int) (Float.parseFloat(lat) * 1E6);
			int longitude = (int) (Float.parseFloat(lon) * 1E6);
			GeoPoint alarm = new GeoPoint(latitude, longitude);
			mapControl.animateTo(alarm);
			createEinsatzMarker(alarm);

		} catch (Exception e) {
		}

	}

	private void createEinsatzMarker(GeoPoint p) {
		OverlayItem overlayitem = new OverlayItem(p, "", "");
		itemizedoverlay.addOverlay(overlayitem);
		Log.i("MARKER", "Marker hinzufügen " + itemizedoverlay.size() + " "
				+ overlayitem);

		if (itemizedoverlay.size() > 0) {
			map.getOverlays().add(itemizedoverlay);
			Log.i("MARKER", "Marker sollte hinzugefügt worden sein");
		}
	}

	@Override
	protected boolean isRouteDisplayed() {
		// TODO Auto-generated method stub
		return false;
	}

	public class OperationOverlay extends ItemizedOverlay<OverlayItem> {

		private static final int maxNum = 5;
		private OverlayItem overlays[] = new OverlayItem[maxNum];
		private int index = 0;
		private boolean full = false;
		private Context context;
		private OverlayItem previousoverlay;

		public OperationOverlay(Context context, Drawable defaultMarker) {
			super(boundCenterBottom(defaultMarker));
			this.setContext(context);
		}

		@Override
		protected OverlayItem createItem(int i) {
			return overlays[i];
		}

		@Override
		public int size() {
			if (full) {
				return overlays.length;
			} else {
				return index;
			}

		}

		public void addOverlay(OverlayItem overlay) {
			if (previousoverlay != null) {
				if (index < maxNum) {
					overlays[index] = previousoverlay;
				} else {
					index = 0;
					full = true;
					overlays[index] = previousoverlay;
				}
				index++;
				populate();
			} else {
				this.previousoverlay = overlay;
				overlays[index] = previousoverlay;
				index++;
				populate();
			}
		}

		protected boolean onTap(int index) {
			return true;
		}

		public Context getContext() {
			return context;
		}

		public void setContext(Context context) {
			this.context = context;
		};
	}
}
