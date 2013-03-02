package com.alarmworkflow.eAlarmApp;

import java.util.Map;

import com.alarmworkflow.eAlarmApp.R;
import com.alarmworkflow.eAlarmApp.datastorage.DataSource;
import com.alarmworkflow.eAlarmApp.datastorage.MySQLiteHelper;
import android.app.Activity;
import android.app.KeyguardManager;
import android.app.KeyguardManager.KeyguardLock;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.Window;
import android.view.WindowManager;
import android.widget.ImageButton;
import android.widget.TextView;

public class OperationDetail extends Activity {

	String longitude = "";
	String latitude = "";
	String id = "";
	String time = "";
	String header = "";
	String text = "";
	Map<String, String> details;
	private TextView headerView;
	private TextView textView;
	private TextView timestamp;
	private DataSource datasource;
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		SharedPreferences prefs = PreferenceManager
				.getDefaultSharedPreferences(this);
		boolean unlock = prefs.getBoolean("unlock", false);
		boolean deviceOn = prefs.getBoolean("deviceOn", false);
		Window window = getWindow();
		if (unlock) {
			window.addFlags(WindowManager.LayoutParams.FLAG_DISMISS_KEYGUARD);
		}
		if (deviceOn) {
			window.addFlags(WindowManager.LayoutParams.FLAG_SHOW_WHEN_LOCKED
					| WindowManager.LayoutParams.FLAG_TURN_SCREEN_ON);
		}

		requestWindowFeature(Window.FEATURE_NO_TITLE);
		setContentView(R.layout.operationdetail);
		headerView = (TextView) findViewById(R.id.operationheader);
		textView = (TextView) findViewById(R.id.operationtext);
		timestamp = (TextView) findViewById(R.id.operationtimestamp);
		Intent intent = getIntent();
		time = intent.getExtras().getString(MySQLiteHelper.COLUMN_TIMESTAMP);
		datasource = DataSource.getInstance(this);
		id = datasource.getID(time);
		setTexts();
		initButtons();

	}

	private void initButtons() {
		ImageButton mapButton = (ImageButton) findViewById(R.id.mapBut);
		mapButton.setOnClickListener(new OnClickListener() {

			@Override
			public void onClick(View v) {
				Intent intent = new Intent(getApplicationContext(),
						com.alarmworkflow.eAlarmApp.Map.class);
				intent.putExtra(MySQLiteHelper.COLUMN_ID, id);
				startActivityForResult(intent, 0);
			}
		});
		ImageButton homeButton = (ImageButton) findViewById(R.id.home);
		homeButton.setOnClickListener(new OnClickListener() {

			@Override
			public void onClick(View v) {
				Intent intent = new Intent(getApplicationContext(),
						com.alarmworkflow.eAlarmApp.OperationView.class);
				intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
				startActivity(intent);
			}
		});

	}

	private void setTexts() {
		if (id != null) {
			if (!id.equalsIgnoreCase("")) {
				details = datasource.getDetails(id);
				header = details.get(MySQLiteHelper.COLUMN_HEADER);
				text = details.get(MySQLiteHelper.COLUMN_TEXT);
				longitude = details.get(MySQLiteHelper.COLUMN_LONG);
				longitude = details.get(MySQLiteHelper.COLUMN_LAT);
				time = details.get(MySQLiteHelper.COLUMN_TIMESTAMP);
				headerView.setText(header);
				textView.setText(text);
				timestamp.setText(time);
			}
		}
	}

}
