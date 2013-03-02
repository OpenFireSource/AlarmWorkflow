package com.alarmworkflow.eAlarmApp;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.alarmworkflow.eAlarmApp.R;
import com.alarmworkflow.eAlarmApp.datastorage.DataSource;
import com.alarmworkflow.eAlarmApp.datastorage.MySQLiteHelper;
import android.app.Activity;
import android.app.PendingIntent;
import android.content.Intent;
import android.content.SharedPreferences;
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
	private SharedPreferences prefs;
	private String auth;

	@SuppressWarnings("unchecked")
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		requestWindowFeature(Window.FEATURE_NO_TITLE);
		setContentView(R.layout.operationview);
		prefs = PreferenceManager.getDefaultSharedPreferences(this);
		auth = prefs.getString("auth", "n/a");
		if (auth == "n/a") {
			register();
		}
		lv = (AdapterView<ListAdapter>) findViewById(R.id.operationlist);
		initList();
	}

	public void register() {
		if (auth == "n/a") {
			Log.w(OperationView.class.getSimpleName(),
					"start registration process");
			Intent registrationIntent = new Intent(
					"com.google.android.c2dm.intent.REGISTER");
			// sets the app name in the intent
			registrationIntent.putExtra("app",
					PendingIntent.getBroadcast(this, 0, new Intent(), 0));
			registrationIntent.putExtra("sender", "885567138585");
			startService(registrationIntent);
		}

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
					HashMap<String, String> params = new HashMap<String, String>();
					params.put("header", "Testnarchicht");
					params.put("text", "Testtext");
					params.put("long", "0");
					params.put("lat", "0");
					params.put("opid", "Test");
					// Sending Testmessage!

				}
			}).start();

			return true;
		default:
			return super.onOptionsItemSelected(item);
		}
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
