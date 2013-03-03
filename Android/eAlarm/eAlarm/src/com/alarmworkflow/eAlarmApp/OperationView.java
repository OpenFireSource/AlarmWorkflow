package com.alarmworkflow.eAlarmApp;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import sun.misc.GC.LatencyRequest;

import com.alarmworkflow.eAlarmApp.R;
import com.alarmworkflow.eAlarmApp.datastorage.DataSource;
import com.alarmworkflow.eAlarmApp.datastorage.MySQLiteHelper;
import com.google.android.gcm.GCMRegistrar;
import com.google.android.gcm.server.Message;
import com.google.android.gcm.server.Result;
import com.google.android.gcm.server.Sender;

import android.app.Activity;
import android.content.Intent;
import android.location.Location;
import android.location.LocationManager;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.util.Log;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.Window;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.ListAdapter;
import android.widget.SimpleAdapter;

public class OperationView extends Activity {
	private AdapterView<ListAdapter> lv;

	@SuppressWarnings("unchecked")
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		requestWindowFeature(Window.FEATURE_NO_TITLE);
		setContentView(R.layout.operationview);
		PreferenceManager.getDefaultSharedPreferences(this);
		GCMRegistrar.checkDevice(this);
		GCMRegistrar.checkManifest(this);
		String regId = GCMRegistrar.getRegistrationId(this);
		if (regId == "") {
			register();
		}
		lv = (AdapterView<ListAdapter>) findViewById(R.id.operationlist);
		initList();
	}

	public void register() {
		GCMRegistrar.register(this,
				com.alarmworkflow.eAlarmApp.general.CommonUtilities.SENDER_ID);
	}

	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {
		super.onActivityResult(requestCode, resultCode, data);
		initList();
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		MenuInflater inflater = getMenuInflater();
		inflater.inflate(R.menu.menu, menu);
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		// Handle item selection
		switch (item.getItemId()) {
		case R.id.settings:
			startActivity(new Intent(this, Settings.class));
			return true;
		case R.id.clear:
			DataSource.getInstance(getApplicationContext()).clearList();
			fillList();
			return true;
		case R.id.testnotification:

			new Thread(new Runnable() {
				public void run() {
					
					try {
						Sender sender = new Sender(
								"AIzaSyA5hhPTlYxJsEDniEoW8OgfxWyiUBEPiS0");
						String locationProvider = LocationManager.NETWORK_PROVIDER;
						// Acquire a reference to the system Location Manager
						LocationManager locationManager = (LocationManager) eAlarm.context.getSystemService(eAlarm.context.LOCATION_SERVICE);
						Location lastKnownLocation = locationManager.getLastKnownLocation(locationProvider);
						Log.d("eAlarm", lastKnownLocation.getLongitude() + "  " + lastKnownLocation.getLatitude());
						Message message = new Message.Builder()
								.delayWhileIdle(true)
								.addData("header", "Testnarchicht")
								.addData("text", "Testtext")
								.addData("long", lastKnownLocation.getLongitude()+"").addData("lat", lastKnownLocation.getLatitude()+"")
								.build();

						Result result = sender.send(message,
								GCMRegistrar.getRegistrationId(eAlarm.context),
								1);

						System.out.println(result.toString());

					} catch (Exception e) {
						e.printStackTrace();
					}

				}
			}).start();

			return true;
		default:
			return super.onOptionsItemSelected(item);
		}
	}

	@Override
	protected void onDestroy() {
		try {
			GCMRegistrar.onDestroy(eAlarm.context);
		} catch (Exception e) {
			Log.e("UnRegister Receiver Error", "> " + e.getMessage());
		}
		super.onDestroy();
	}

	@Override
	public void onResume() {
		super.onResume();
		initList();
	}

	void initList() {
		addListener();
		fillList();
	}

	private void fillList() {
		List<Map<String, String>> data = DataSource.getInstance(this)
				.getOperations();
		SimpleAdapter adapter = new SimpleAdapter(this, data,
				android.R.layout.simple_list_item_2, new String[] {
						MySQLiteHelper.COLUMN_HEADER,
						MySQLiteHelper.COLUMN_TIMESTAMP }, new int[] {
						android.R.id.text1, android.R.id.text2 });
		lv.setAdapter(adapter);

	}

	private void addListener() {
		lv.setOnItemClickListener(new OnItemClickListener() {

			public void onItemClick(AdapterView<?> arg0, View view,
					int position, long arg3) {
				// selected item
				@SuppressWarnings({ "unchecked" })
				HashMap<String, String> o = (HashMap<String, String>) lv
						.getItemAtPosition(position);
				String timestamp = o.get(MySQLiteHelper.COLUMN_TIMESTAMP);

				// Launching new Activity on selecting single List Item
				Intent i = new Intent(getApplicationContext(),
						OperationDetail.class); // sending data to new activity
				i.putExtra(MySQLiteHelper.COLUMN_TIMESTAMP, timestamp);
				startActivity(i);

			}
		});

	}
}
