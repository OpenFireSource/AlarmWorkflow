package com.alarmworkflow.eAlarm;

import java.io.IOException;
import java.util.List;

import org.json.JSONException;

import com.alarmworkflow.eAlarm.database.NotificationMessage;
import com.alarmworkflow.eAlarm.general.eAlarm;
import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GooglePlayServicesUtil;
import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.GoogleMap.OnCameraChangeListener;
import com.google.android.gms.maps.SupportMapFragment;
import com.google.android.gms.maps.model.CameraPosition;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.MarkerOptions;

import android.annotation.SuppressLint;
import android.app.Dialog;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.location.Address;
import android.location.Geocoder;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentManager;

public class Map extends FragmentActivity {

	GoogleMap mMap;
	private float previousZoomLevel;
	private NotificationMessage message;
	private SharedPreferences prefs;

	@SuppressLint("NewApi")
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		this.getMessage();
		setContentView(R.layout.screen_map);
		int resultCode = GooglePlayServicesUtil
				.isGooglePlayServicesAvailable(this);
		switch (resultCode) {
		case ConnectionResult.SUCCESS: // proceed
			break;
		case ConnectionResult.SERVICE_MISSING:
		case ConnectionResult.SERVICE_VERSION_UPDATE_REQUIRED:
		case ConnectionResult.SERVICE_DISABLED:
			Dialog dialog = GooglePlayServicesUtil.getErrorDialog(resultCode,
					this, 1);
			dialog.show();
		}
		String location = null;
		try {
			location = this.getMessage().getContent().getString("awf_location");
		} catch (JSONException e2) {

			finish();
		}
		Geocoder coder = new Geocoder(this);
		List<Address> coordinats = null;
		if (location == null) {
			finish();
		}

		try {
			coordinats = coder.getFromLocationName(location, 1);
		} catch (IOException e1) {
			finish();
		}
		Address bestMatch = (coordinats.isEmpty() ? null : coordinats.get(0));
		if (bestMatch != null) {
			if (GooglePlayServicesUtil.isGooglePlayServicesAvailable(this) == ConnectionResult.SUCCESS) {

				FragmentManager myFragmentManager = getSupportFragmentManager();
				SupportMapFragment mySupportMapFragment = (SupportMapFragment) myFragmentManager
						.findFragmentById(R.id.map);
				mMap = mySupportMapFragment.getMap();
				mMap.setTrafficEnabled(true);

				mMap.setOnCameraChangeListener(getCameraChangeListener());
				mMap.setMyLocationEnabled(true);
				mMap.getUiSettings().setAllGesturesEnabled(true);
				mMap.getUiSettings().setCompassEnabled(true);
				mMap.getUiSettings().setMyLocationButtonEnabled(true);
				LatLng alarm = generateAlarmMarker(bestMatch);
				prefs = PreferenceManager
						.getDefaultSharedPreferences(eAlarm.context);
				if (savedInstanceState == null) {
					if (alarm != null) {
						float zoomLevel = prefs.getFloat(
								getString(R.string.zoomlevel), 17);
						mMap.animateCamera(CameraUpdateFactory.newLatLngZoom(
								alarm, zoomLevel));
					}
				}
			} else {
				finish();
			}
		} else {
			finish();
		}
	}

	private OnCameraChangeListener getCameraChangeListener() {
		return new OnCameraChangeListener() {
			public void onCameraChange(CameraPosition position) {
				if (previousZoomLevel != position.zoom) {
				}

				previousZoomLevel = position.zoom;

				Editor edit = prefs.edit();
				edit.putFloat(eAlarm.context.getString(R.string.zoomlevel),
						previousZoomLevel);
				edit.commit();
			}
		};
	}

	private LatLng generateAlarmMarker(Address address) {
		double lng;
		double lat;
		lat = address.getLatitude();
		lng = address.getLongitude();
		String description = this.getMessage().getMessage();
		String title = this.getMessage().getTitle();
		try {
			LatLng alarm = new LatLng(lat, lng);
			mMap.addMarker(new MarkerOptions().position(alarm).title(title)
					.snippet(description));

			return alarm;

		} catch (NumberFormatException e) {
		}
		return null;

	}

	/**
	 * Fetch the message.
	 * 
	 * @return
	 */
	public NotificationMessage getMessage() {
		if (this.message == null) {
			// Get the message from the intent.
			// We store it in a private variable to save us having to query the
			// DB each time.
			final Intent ruleIntent = getIntent();
			this.message = NotificationMessage.FACTORY.get(this,
					ruleIntent.getLongExtra("messageId", 0));

			if (this.message == null) {
				// Ie, we tried to load it but it's been deleted.
				// This can happen if you're going backwards through the
				// activity stack and a later activity was used to delete all
				// messages.
				// In this case, just exit this activity.
				this.finish();
			}

		}

		return this.message;
	}
}
