package com.alarmworkflow.eAlarm;

import java.io.IOException;
import java.util.List;

import org.json.JSONException;

import com.alarmworkflow.eAlarm.database.NotificationMessage;
import com.alarmworkflow.eAlarm.general.NotificationService;
import com.alarmworkflow.eAlarm.general.eAlarm;
import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GooglePlayServicesUtil;
import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.SupportMapFragment;
import com.google.android.gms.maps.GoogleMap.OnCameraChangeListener;
import com.google.android.gms.maps.model.CameraPosition;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.MarkerOptions;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.location.Address;
import android.location.Geocoder;
import android.os.Build;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentManager;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.LinearLayout.LayoutParams;
import android.widget.ShareActionProvider;
import android.widget.TextView;

public class MessageDetail extends FragmentActivity {
	private final static int BACK_TO_LIST = 1;
	private static final int SHARE = 2;
	private NotificationMessage message = null;

	private float previousZoomLevel;
	private GoogleMap mMap;
	private SharedPreferences prefs;
	private ShareActionProvider provider;

	/** Called when the activity is first created. */
	public void onCreate(Bundle savedInstanceState) {
		SharedPreferences preferences = PreferenceManager
				.getDefaultSharedPreferences(this);
		super.onCreate(savedInstanceState);
		setContentView(R.layout.screen_message_detail);
		this.getMessage();

		boolean unlock = NotificationService.globalOrOverrideBoolean(R.string.unlock, preferences, message.getRule(), false, this);
		if (unlock) {
			Window window = getWindow();
			window.addFlags(WindowManager.LayoutParams.FLAG_DISMISS_KEYGUARD);
			window.addFlags(WindowManager.LayoutParams.FLAG_SHOW_WHEN_LOCKED
					| WindowManager.LayoutParams.FLAG_TURN_SCREEN_ON);
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

	public void onResume() {
		super.onResume();
		NotificationMessage notMessage = this.getMessage();
		this.loadFromMessage(notMessage);

		// Clear the notification.
		Intent intentData = new Intent(getBaseContext(),
				NotificationService.class);
		intentData.putExtra("operation", "update");
		intentData.putExtra("ruleId", this.getMessage().getRule().getId());
		startService(intentData);
		if (GooglePlayServicesUtil.isGooglePlayServicesAvailable(this) == ConnectionResult.SUCCESS) {
			FragmentManager myFragmentManager = getSupportFragmentManager();
			SupportMapFragment mySupportMapFragment = (SupportMapFragment) myFragmentManager
					.findFragmentById(R.id.map);
			String location = null;
			try {
				location = this.getMessage().getContent().getString("awf_location");
			} catch (JSONException e2) {
				hideMapstuff();
			}
			Geocoder coder = new Geocoder(this);
			List<Address> coordinats = null;
			if (location == null) {
				hideMapstuff();
			}

			try {
				coordinats = coder.getFromLocationName(location, 1);
			} catch (IOException e1) {
				hideMapstuff();
			}
			if (coordinats == null) {
				hideMapstuff();
			} else {

				Address bestMatch = (coordinats.isEmpty() ? null : coordinats
						.get(0));
				if (bestMatch != null) {
					if (!bestMatch.hasLatitude() && !bestMatch.hasLongitude()) {
						hideMapstuff();
					}
					if (mySupportMapFragment == null) {
						return;
					}
					mMap = mySupportMapFragment.getMap();
					if (mMap == null) {
						return;
					}

					mMap.setTrafficEnabled(true);

					mMap.setOnCameraChangeListener(getCameraChangeListener());
					mMap.setMyLocationEnabled(true);
					mMap.getUiSettings().setAllGesturesEnabled(true);
					mMap.getUiSettings().setCompassEnabled(true);
					mMap.getUiSettings().setMyLocationButtonEnabled(true);
					LatLng alarm = generateAlarmMarker(bestMatch);
					prefs = PreferenceManager
							.getDefaultSharedPreferences(eAlarm.context);
					if (alarm != null) {
						float zoomLevel = prefs.getFloat(
								getString(R.string.zoomlevel), 17);
						mMap.animateCamera(CameraUpdateFactory.newLatLngZoom(
								alarm, zoomLevel));
					}
				} else {
					hideMapstuff();
				}
			}
		} else {
			hideMapstuff();
		}
	}

	void hideMapstuff() {
		FragmentManager myFragmentManager = getSupportFragmentManager();
		SupportMapFragment mySupportMapFragment = (SupportMapFragment) myFragmentManager
				.findFragmentById(R.id.map);
		Button locationButton = (Button) findViewById(R.id.locationButton);
		if (locationButton != null) {
			locationButton.setVisibility(View.GONE);
		}
		if (mySupportMapFragment == null) {
			return;
		}
		mMap = mySupportMapFragment.getMap();
		if (mMap == null) {
			return;
		}
		getSupportFragmentManager().beginTransaction()
				.hide(mySupportMapFragment).commit();
		View findViewById = findViewById(R.id.scrollView1);
		LayoutParams params = new LinearLayout.LayoutParams(0,
				LayoutParams.MATCH_PARENT, 1f);
		findViewById.setLayoutParams(params);

		return;

	}

	/**
	 * Load this activity from the given message.
	 * 
	 * @param message
	 */
	public void loadFromMessage(NotificationMessage message) {
		TextView title = (TextView) findViewById(R.id.message_detail_title);
		title.setText(message.getTitle());

		TextView rule = (TextView) findViewById(R.id.message_detail_rule);
		rule.setText(message.getRule().getTitle());

		TextView timestamp = (TextView) findViewById(R.id.message_detail_timestamp);
		timestamp.setText(message.getDisplayTimestamp());

		TextView content = (TextView) findViewById(R.id.message_detail_content);
		content.setText(message.getMessage());
	}

	@SuppressLint("NewApi")
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		boolean result = super.onCreateOptionsMenu(menu);
		menu.add(0, BACK_TO_LIST, 0, R.string.message_list).setIcon(
				android.R.drawable.ic_menu_agenda);
		if (Build.VERSION.SDK_INT >= 14) {
			menu.add(0, SHARE, 0, "Teilen")
					.setActionProvider(new ShareActionProvider(this))
					.setShowAsAction(MenuItem.SHOW_AS_ACTION_IF_ROOM);
			// Get the ActionProvider
			provider = (ShareActionProvider) menu.findItem(SHARE)
					.getActionProvider();
			provider.setShareHistoryFileName(ShareActionProvider.DEFAULT_SHARE_HISTORY_FILE_NAME);
			// Initialize the share intent
			Intent intent = new Intent(Intent.ACTION_SEND);
			intent.setType("text/plain");
			intent.putExtra(Intent.EXTRA_TEXT, message.getMessage());
			provider.setShareIntent(intent);
		}
		return result;
	}

	@SuppressLint("NewApi")
	public void doShare() {
		// Populare the share intent with data
		Intent intent = new Intent(Intent.ACTION_SEND);
		intent.setType("text/plain");
		intent.putExtra(Intent.EXTRA_TEXT, message.getMessage());
		provider.setShareIntent(intent);
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		switch (item.getItemId()) {
		case BACK_TO_LIST:
			Intent intent = new Intent(this, MessageList.class);
			intent.putExtra("ruleId", getMessage().getRule().getId());
			startActivity(intent);
			return true;
		case SHARE:
			doShare();
			return true;
		}

		return super.onOptionsItemSelected(item);
	}

	/**
	 * Onclick handler to launch the Maps dialog.
	 * 
	 * @param view
	 */
	public void launchMap(View view) {
		Intent intent = new Intent(getBaseContext(), Map.class);
		intent.putExtra("messageId", getMessage().getId());
		startActivity(intent);
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

			// Change the seen flag if required.
			if (this.message.getSeen() == false) {
				this.message.setSeen(true);
				this.message.save(this);
			}
		}

		return this.message;
	}
}
