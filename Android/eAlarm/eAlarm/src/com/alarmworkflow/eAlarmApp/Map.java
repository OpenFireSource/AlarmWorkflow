package com.alarmworkflow.eAlarmApp;

import java.util.LinkedList;
import java.util.List;
import mapviewballoons.example.simple.SimpleItemizedOverlay;

import com.alarmworkflow.eAlarmApp.R;
import com.alarmworkflow.eAlarmApp.datastorage.DataSource;
import com.alarmworkflow.eAlarmApp.datastorage.MySQLiteHelper;
import com.alarmworkflow.eAlarmApp.general.CustomMapView;
import com.alarmworkflow.eAlarmApp.general.GeoHelper;
import com.google.android.maps.GeoPoint;
import com.google.android.maps.MapActivity;
import com.google.android.maps.MapController;
import com.google.android.maps.MapView;
import com.google.android.maps.OverlayItem;

import de.android.overlaymanager.ManagedOverlay;
import de.android.overlaymanager.ManagedOverlayItem;
import de.android.overlaymanager.OverlayManager;
import de.android.overlaymanager.lazyload.LazyLoadCallback;
import de.android.overlaymanager.lazyload.LazyLoadException;
import de.android.overlaymanager.lazyload.LazyLoadListener;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.widget.ImageView;
import android.widget.ListView;

public class Map extends MapActivity {
	private MapView mapView;
	private MapController mapControl;

	private SimpleItemizedOverlay operationOverlay;
	private ListView poiList;
	private OverlayManager overlayManager;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.mapview);
		mapView = (CustomMapView) findViewById(R.id.map);
		poiList = (ListView) findViewById(R.id.poiList);
		if (poiList != null)
			Log.i("POI", "POI Liste is da");
		operationOverlay = new SimpleItemizedOverlay(getResources()
				.getDrawable(R.drawable.brandmarker), mapView);
		// Vielleicht ganz interessant fuer Anfahrt und co.
		mapView.setTraffic(true);
		// Zoombuttons
		mapView.setBuiltInZoomControls(true);
		mapControl = mapView.getController();
		mapControl.setZoom(16);
		overlayManager = new OverlayManager(getApplication(), mapView);
		createOverlayWithLazyLoading();
		GeoPoint alarm = generateAlarmMarker();	
		if (savedInstanceState == null) {
			if(alarm != null)
			mapControl.animateTo(alarm);
		}
		mapView.invalidate();
	}

	@Override
	public void onWindowFocusChanged(boolean b) {
		createOverlayWithLazyLoading();
	}

	public void createOverlayWithLazyLoading() {
		// animation will be rendered to this ImageView
		ImageView loaderanim = (ImageView) findViewById(R.id.loader);

		ManagedOverlay managedOverlay = overlayManager.createOverlay(
				"lazyloadOverlay",
				getResources().getDrawable(R.drawable.neutraloverlay));

		// default built-in animation
		managedOverlay.enableLazyLoadAnimation(loaderanim);

		managedOverlay.setLazyLoadCallback(new LazyLoadCallback() {
			@Override
			public List<ManagedOverlayItem> lazyload(GeoPoint topLeft,
					GeoPoint bottomRight, ManagedOverlay overlay)
					throws LazyLoadException {
				List<ManagedOverlayItem> items = new LinkedList<ManagedOverlayItem>();
				try {
					List<GeoPoint> marker = GeoHelper.findMarker(topLeft,
							bottomRight, overlay.getZoomlevel());
					for (int i = 0; i < marker.size(); i++) {
						GeoPoint point = marker.get(i);
						ManagedOverlayItem item = new ManagedOverlayItem(point,
								"Item" + i, "");
						items.add(item);
					}
				} catch (Exception e) {
					throw new LazyLoadException(e.getMessage());
				}
				return items;
			}
		});

		// A LazyLoadListener is optional!
		managedOverlay.setLazyLoadListener(new LazyLoadListener() {

			@Override
			public void onBegin(ManagedOverlay overlay) {

			}

			@Override
			public void onSuccess(ManagedOverlay overlay) {

			}

			@Override
			public void onError(LazyLoadException exception,
					ManagedOverlay overlay) {

			}
		});
		overlayManager.populate();

	}

	private GeoPoint generateAlarmMarker() {
		Intent intent = getIntent();
		// ID des Einsatzes
		String id = intent.getExtras().getString(MySQLiteHelper.COLUMN_ID);
		DataSource data = DataSource.getInstance(this);
		java.util.Map<String, String> details = data.getDetails(id);
		// Latitude
		String lat = details.get(MySQLiteHelper.COLUMN_LAT);
		// Longitude
		String lon = details.get(MySQLiteHelper.COLUMN_LONG);

		this.getResources().getDrawable(R.drawable.brandmarker);

		try {
			// Geopoint-konform umwandeln (Koennte Exceptions schmeissen)
			int latitude = (int) (Float.parseFloat(lat) * 1E6);
			int longitude = (int) (Float.parseFloat(lon) * 1E6);
			GeoPoint alarm = new GeoPoint(latitude, longitude);
			OverlayItem item = new OverlayItem(alarm, "Einsatzort", "");
			operationOverlay.addOverlay(item);
			mapView.getOverlays().add(operationOverlay);
			return alarm;
		} catch (NumberFormatException e) {
		}
		return null;

	}

	@Override
	protected boolean isRouteDisplayed() {
		// TODO Auto-generated method stub
		return false;
	}

}
