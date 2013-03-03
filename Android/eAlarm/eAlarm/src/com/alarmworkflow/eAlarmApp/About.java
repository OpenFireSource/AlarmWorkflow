package com.alarmworkflow.eAlarmApp;

import android.app.Activity;
import android.os.Bundle;
import android.widget.TextView;

public class About extends Activity {
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.about);
		TextView view = (TextView) findViewById(R.id.abouttext);
		view.append("\n Test");
		}
}
