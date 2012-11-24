package com.alarmworkflow.eAlarmApp;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.alarmworkflow.eAlarmApp.R;
import com.alarmworkflow.eAlarmApp.services.DataSource;
import com.alarmworkflow.eAlarmApp.services.MySQLiteHelper;

import android.app.Activity;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.Window;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.ListAdapter;
import android.widget.SimpleAdapter;
import android.widget.TextView;

public class OperationView extends Activity {
	private AdapterView<ListAdapter> lv;
	private SharedPreferences prefs;
	private String auth;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		requestWindowFeature(Window.FEATURE_NO_TITLE);
		setContentView(R.layout.operationview);
		prefs = PreferenceManager.getDefaultSharedPreferences(this);
		auth = prefs.getString(C2DMClientActivity.AUTH, "n/a");
		if (auth == "n/a") {
			Intent intent = new Intent(this, C2DMClientActivity.class);
			intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
			intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
			startActivity(intent);
		}
		lv = (AdapterView<ListAdapter>) findViewById(R.id.operationlist);
		initList();
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
		case R.id.about:
			startActivity(new Intent(this, About.class));
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
				@SuppressWarnings("unused")
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
