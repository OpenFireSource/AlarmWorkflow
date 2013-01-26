package com.alarmworkflow.eAlarmApp;

import android.os.Bundle;
import android.preference.Preference;
import android.preference.PreferenceActivity;
import android.preference.PreferenceManager;

import com.alarmworkflow.eAlarmApp.R;

public class Settings extends PreferenceActivity {

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		addPreferencesFromResource(R.xml.settings);
		Preference pref = findPreference("email");
		pref.setSummary(PreferenceManager.getDefaultSharedPreferences(this)
				.getString("email", "(n/A)"));
	}
}