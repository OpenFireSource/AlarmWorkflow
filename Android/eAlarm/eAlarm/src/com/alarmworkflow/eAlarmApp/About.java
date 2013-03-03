package com.alarmworkflow.eAlarmApp;

import com.google.android.gms.common.GooglePlayServicesUtil;

import android.app.AlertDialog;
import android.os.Bundle;
import android.support.v4.app.FragmentActivity;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;

public class About extends FragmentActivity {
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.about);
		Button button = (Button) findViewById(R.id.license);
		button.setOnClickListener(new OnClickListener(){

			@Override
			public void onClick(View v) {
				String LicenseInfo = GooglePlayServicesUtil
						.getOpenSourceSoftwareLicenseInfo(getApplicationContext());
				AlertDialog.Builder LicenseDialog = new AlertDialog.Builder(About.this);
				
				LicenseDialog.setTitle("Legal Notices");
				LicenseDialog.setMessage(LicenseInfo);
				LicenseDialog.show();
				
			}
			
		});
		

	}
}
