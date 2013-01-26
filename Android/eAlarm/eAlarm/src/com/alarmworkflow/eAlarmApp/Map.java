package com.alarmworkflow.eAlarmApp;

import com.alarmworkflow.eAlarmApp.R;
import com.alarmworkflow.eAlarmApp.datastorage.DataSource;
import com.alarmworkflow.eAlarmApp.datastorage.MySQLiteHelper;
import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.SupportMapFragment;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.MarkerOptions;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentManager;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.ImageButton;

public class Map extends FragmentActivity {

	GoogleMap mMap;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.mapview);
		FragmentManager myFragmentManager = getSupportFragmentManager();
		SupportMapFragment mySupportMapFragment = (SupportMapFragment) myFragmentManager
				.findFragmentById(R.id.map);
		mMap = mySupportMapFragment.getMap();
		mMap.setTrafficEnabled(true);
		mMap.setMyLocationEnabled(true);
		mMap.getUiSettings().setAllGesturesEnabled(true);
		mMap.getUiSettings().setCompassEnabled(true);
		mMap.getUiSettings().setMyLocationButtonEnabled(true);
		LatLng alarm = generateAlarmMarker();
		if (savedInstanceState == null) {
			if (alarm != null)
				mMap.animateCamera(CameraUpdateFactory.newLatLngZoom(alarm, 17));
		}
		ImageButton homeButton = (ImageButton) findViewById(R.id.back);
		homeButton.setOnClickListener(new OnClickListener() {

			@Override
			public void onClick(View v) {
				finish();
			}
		});
	}

	private LatLng generateAlarmMarker() {
		Intent intent = getIntent();
		// ID des Einsatzes
		String id = intent.getExtras().getString(MySQLiteHelper.COLUMN_ID);
		DataSource data = DataSource.getInstance(this);
		java.util.Map<String, String> details = data.getDetails(id);
		// Latitude
		String lat = details.get(MySQLiteHelper.COLUMN_LAT);
		// Longitude
		String lon = details.get(MySQLiteHelper.COLUMN_LONG);
		String description = details.get(MySQLiteHelper.COLUMN_HEADER) + "\n"
				+ details.get(MySQLiteHelper.COLUMN_TEXT);
		try {
			double latitude = Double.parseDouble(lat);
			double longitude = Double.parseDouble(lon);
			LatLng alarm = new LatLng(latitude, longitude);
			mMap.addMarker(new MarkerOptions().position(alarm)
					.title("Einsatzort").snippet(description));

			return alarm;

		} catch (NumberFormatException e) {
		}
		return null;

	}

}
