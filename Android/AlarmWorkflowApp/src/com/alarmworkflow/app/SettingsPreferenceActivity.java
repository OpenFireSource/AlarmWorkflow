package com.alarmworkflow.app;

import android.os.Bundle;
import android.preference.PreferenceActivity;

public class SettingsPreferenceActivity extends PreferenceActivity {

	public SettingsPreferenceActivity() {
		
	}
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		
		addPreferencesFromResource(R.xml.activity_preferences);
	}

}
