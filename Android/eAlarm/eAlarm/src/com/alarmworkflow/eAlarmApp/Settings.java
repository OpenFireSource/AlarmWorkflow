package com.alarmworkflow.eAlarmApp;

import android.os.Bundle;
import android.preference.Preference;
import android.preference.Preference.OnPreferenceClickListener;
import android.preference.PreferenceActivity;
import android.preference.PreferenceManager;
import android.text.ClipboardManager;
import android.widget.Toast;

import com.alarmworkflow.eAlarmApp.R;

public class Settings extends PreferenceActivity {

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		addPreferencesFromResource(R.xml.settings);
		Preference pref = (Preference) findPreference("gcmid");
		final String auth = PreferenceManager.getDefaultSharedPreferences(this)
		.getString("auth", "(n/A)");
		pref.setSummary(auth);
		pref.setOnPreferenceClickListener(new OnPreferenceClickListener() {
			
			@Override
			public boolean onPreferenceClick(Preference preference) {
				final ClipboardManager clipBoard = (ClipboardManager)getSystemService(CLIPBOARD_SERVICE);
				clipBoard.setText(auth);
				Toast.makeText(Settings.this, "GCM-ID in Zwischenablage kopiert", Toast.LENGTH_LONG).show();
				return true;
			}
		});
	}
}