package com.alarmworkflow.eAlarm;

import com.google.android.gcm.GCMRegistrar;

import android.content.Intent;
import android.content.SharedPreferences;
import android.media.Ringtone;
import android.media.RingtoneManager;
import android.net.Uri;
import android.os.Bundle;
import android.preference.CheckBoxPreference;
import android.preference.EditTextPreference;
import android.preference.Preference;
import android.preference.PreferenceActivity;
import android.preference.PreferenceManager;
import android.preference.Preference.OnPreferenceChangeListener;
import android.preference.Preference.OnPreferenceClickListener;
import android.preference.RingtonePreference;
import android.text.ClipboardManager;
import android.widget.Toast;

public class Settings extends PreferenceActivity
{
	private EditTextPreference delayReading;
	private EditTextPreference shakeThreshold;
	private EditTextPreference shakeWaitTime;
	private CheckBoxPreference playRingtone;
	private CheckBoxPreference overwriteSystem;
	private RingtonePreference ringtonePref;

	@Override
	public void onCreate(Bundle savedInstanceState)
	{
		super.onCreate(savedInstanceState);
		addPreferencesFromResource(R.xml.preferences);

		Preference stopReadingNow = findPreference(getString(R.string.stopReadingNow));
		stopReadingNow.setOnPreferenceClickListener(stopSpeakingNowHandler);
		Preference pref = (Preference) findPreference("gcmid");
		final String auth = GCMRegistrar.getRegistrationId(this);
		pref.setSummary(auth);
		pref.setOnPreferenceClickListener(new OnPreferenceClickListener() {
			
			public boolean onPreferenceClick(Preference preference) {
				final ClipboardManager clipBoard = (ClipboardManager)getSystemService(CLIPBOARD_SERVICE);
				clipBoard.setText(auth);
				Toast.makeText(Settings.this, "GCM-ID in Zwischenablage kopiert", Toast.LENGTH_LONG).show();
				return true;
			}
		});
		Preference previewSpeech = findPreference(getString(R.string.previewSpeech));
		previewSpeech.setOnPreferenceClickListener(previewSpeechHandler);

		SharedPreferences settings = PreferenceManager.getDefaultSharedPreferences(this);
		delayReading = (EditTextPreference) findPreference(getString(R.string.delayReadingTime));
		delayReading.setOnPreferenceChangeListener(delayReadingHandler);
		updateDelaySummary(settings.getString(getString(R.string.delayReadingTime), "0"));
		
		shakeThreshold = (EditTextPreference) findPreference(getString(R.string.shakeThreshold));
		shakeThreshold.setOnPreferenceChangeListener(shakeThresholdHandler);
		updateThresholdSummary(settings.getString(getString(R.string.shakeThreshold), "1500"));
		
		shakeWaitTime = (EditTextPreference) findPreference(getString(R.string.shakeWaitTime));
		shakeWaitTime.setOnPreferenceChangeListener(shakeWaitTimeHandler);
		updateShakeWaitTimeSummary(settings.getString(getString(R.string.shakeWaitTime), "60"));
		
		playRingtone = (CheckBoxPreference) findPreference(getString(R.string.playRingtone));
		
		ringtonePref = (RingtonePreference) findPreference(getString(R.string.choosenNotification));
		ringtonePref.setSummary(getRingtoneName(settings.getString(getString(R.string.choosenNotification), "")));
		ringtonePref.setOnPreferenceChangeListener(ringtonePrefHandler);
		
		overwriteSystem = (CheckBoxPreference) findPreference(getString(R.string.overwritesystem_setting));
		overwriteSystem.setOnPreferenceClickListener(new OnPreferenceClickListener() {
			public boolean onPreferenceClick(Preference preference) {   
				 if (overwriteSystem.isChecked()) {
					  
					 if(!playRingtone.isChecked()) {
						 Toast.makeText(getApplicationContext(), 
									getString(R.string.playsound_not_checked), Toast.LENGTH_LONG).show();
						 return true;
					 }
					 
					 String tone = ringtonePref.getSummary().toString();
					 if(tone == "" || tone == "Stumm") {
						 Toast.makeText(getApplicationContext(), 
								 getString(R.string.no_sound_chosen), Toast.LENGTH_LONG).show();
					 }					 
				   } 
			   return true; 
			}
		});

	}
	
	// Get the ringtone display name from //media//foo/bar path
	private String getRingtoneName(String path)
	{
		/*bit of a hack: 
		RingtoneManager return null regardless if the default ringtone or silent was chosen.
		to determine it right I found that uri-path is empty if silent was chosen
		and RingtoneManager still returns null if it is the default ringtone.
		*/
		String name = "Standardklingelton";
		if(path == "") {
			name = "Stumm";			
		}
		Uri ringtoneUri = Uri.parse(path);
		Ringtone ringtone = RingtoneManager.getRingtone(getBaseContext(), ringtoneUri);
		if(ringtone != null) {
			name = ringtone.getTitle(getBaseContext());
		}
		return name;		
	}
	
	// On Preference change listener to update the ringtone summary.
	OnPreferenceChangeListener ringtonePrefHandler = new OnPreferenceChangeListener()
	{
		public boolean onPreferenceChange(Preference preference, Object newValue)
		{
				ringtonePref.setSummary(getRingtoneName((String) newValue));
				return true;
		}
	};
	

	// On click handler for stopping the text in motion.
	OnPreferenceClickListener stopSpeakingNowHandler = new OnPreferenceClickListener()
	{
		public boolean onPreferenceClick(Preference preference)
		{
			Intent intentData = new Intent(getBaseContext(), SpeakService.class);
			intentData.putExtra("stopNow", true);
			startService(intentData);
			return true;
		}
	};

	// On click handler for previewing speech.
	OnPreferenceClickListener previewSpeechHandler = new OnPreferenceClickListener()
	{
		public boolean onPreferenceClick(Preference preference)
		{
			Intent intentData = new Intent(getBaseContext(), SpeakService.class);
			intentData.putExtra("text", getString(R.string.preview_speak));
			startService(intentData);
			return true;
		}
	};

	// On Preference change listener to update the delay summary.
	OnPreferenceChangeListener delayReadingHandler = new OnPreferenceChangeListener()
	{
		public boolean onPreferenceChange(Preference preference, Object newValue)
		{
			Intent intentData = new Intent(getBaseContext(), SpeakService.class);
			intentData.putExtra("delay", Integer.parseInt(newValue.toString()));
			startService(intentData);
			
			updateDelaySummary((String) newValue);
			return true;
		}
	};

	// On Preference change listener to update the delay summary.
	OnPreferenceChangeListener shakeThresholdHandler = new OnPreferenceChangeListener()
	{
		public boolean onPreferenceChange(Preference preference, Object newValue)
		{
			updateThresholdSummary((String) newValue);
			return true;
		}
	};	

	// On Preference change listener to update the delay summary.
	OnPreferenceChangeListener shakeWaitTimeHandler = new OnPreferenceChangeListener()
	{
		public boolean onPreferenceChange(Preference preference, Object newValue)
		{
			updateShakeWaitTimeSummary((String) newValue);
			return true;
		}
	};

	// Helper function to update the delay summary.
	private void updateDelaySummary(String value)
	{
		String template = getString(R.string.delay_readout_summary);

		try
		{
			Integer intValue = Integer.parseInt(value);
			String plural = "s";

			if (intValue == 1)
			{
				plural = "";
			}
			String result = String.format(template, intValue, plural);
			delayReading.setSummary(result);
		}
		catch( NumberFormatException ex )
		{
			// Not a valid number... ignore.
		}
	}
	
	// Helper function to update the threshold summary.
	private void updateThresholdSummary(String value)
	{
		String template = getString(R.string.shakethreshhold_summary);

		try
		{
			Integer intValue = Integer.parseInt(value);
			String result = String.format(template, intValue);
			shakeThreshold.setSummary(result);
		}
		catch( NumberFormatException ex )
		{
			// Not a valid number... ignore.
		}
	}
	
	// Helper function to update the wait time summary.
	private void updateShakeWaitTimeSummary(String value)
	{
		String template = getString(R.string.shakewaittime_summary);

		try
		{
			Integer intValue = Integer.parseInt(value);
			String result = String.format(template, intValue);
			shakeWaitTime.setSummary(result);
		}
		catch( NumberFormatException ex )
		{
			// Not a valid number... ignore.
		}
	}
}
