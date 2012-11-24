package com.alarmworkflow.eAlarmApp;

import java.util.Map;

import com.google.android.maps.MapActivity;
import com.google.android.maps.MapView;

import com.alarmworkflow.eAlarmApp.R;
import com.alarmworkflow.eAlarmApp.services.DataSource;
import com.alarmworkflow.eAlarmApp.services.MySQLiteHelper;

import android.app.Activity;
import android.app.KeyguardManager;
import android.app.KeyguardManager.KeyguardLock;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.Window;
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

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		requestWindowFeature(Window.FEATURE_NO_TITLE);
		setContentView(R.layout.operationdetail);
		TextView headerView = (TextView) findViewById(R.id.operationheader);
		TextView textView = (TextView) findViewById(R.id.operationtext);
		TextView timestamp = (TextView) findViewById(R.id.operationtimestamp);
		Intent intent = getIntent();
		time = intent.getExtras().getString(MySQLiteHelper.COLUMN_TIMESTAMP);
		DataSource datasource = DataSource.getInstance(this);
		id = datasource.getID(time);
		if (id != null) {
			if (!id.equalsIgnoreCase("")) {
				details = datasource.getDetails(id);
				header = details.get(MySQLiteHelper.COLUMN_HEADER);
				text = details.get(MySQLiteHelper.COMLUM_TEXT);
				longitude = details.get(MySQLiteHelper.COLUMN_LONG);
				longitude = details.get(MySQLiteHelper.COLUMN_LAT);
				time = details.get(MySQLiteHelper.COLUMN_TIMESTAMP);
				headerView.setText(header);
				textView.setText(text);
				timestamp.setText(time);
			}
		}
		ImageButton mapButton = (ImageButton) findViewById(R.id.mapBut);
		mapButton.setOnClickListener(new OnClickListener() {

			@Override
			public void onClick(View v) {
				Intent intent = new Intent(getApplicationContext(),
						com.alarmworkflow.eAlarmApp.Map.class);
				intent.putExtra(MySQLiteHelper.COLUMN_ID, id);
				startActivity(intent);
			}
		});
		KeyguardManager keyguardManager = (KeyguardManager) getApplicationContext()
				.getSystemService(Context.KEYGUARD_SERVICE);
		KeyguardLock keyguardLock = keyguardManager.newKeyguardLock("eAlarm");
		keyguardLock.reenableKeyguard();		
	}

	@Override
	public void onBackPressed() {
		Intent intent = new Intent();
		setResult(RESULT_OK, intent);
		finish();
	}

}
