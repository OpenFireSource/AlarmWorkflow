package com.alarmworkflow.eAlarm;

import java.util.Date;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.pm.PackageManager.NameNotFoundException;
import android.content.pm.ResolveInfo;
import android.net.Uri;
import android.os.Bundle;
import android.os.Handler;
import android.preference.PreferenceManager;
import android.speech.tts.TextToSpeech;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.alarmworkflow.eAlarm.database.NotificationMessage;
import com.alarmworkflow.eAlarm.general.Constants;
import com.alarmworkflow.eAlarm.general.HealthCheck;
import com.google.android.gcm.GCMRegistrar;
import com.google.android.gms.common.GooglePlayServicesUtil;

public class eAlarmPush extends Activity {
	public final static String UPDATE_INTENT = "com.alarmworkflow.eAlarm.UpdateUI";
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.screen_home);

		new Handler();
		Intent checkIntent = new Intent();
		checkIntent.setAction(TextToSpeech.Engine.ACTION_CHECK_TTS_DATA);
		PackageManager pm = getPackageManager();
		ResolveInfo resolveInfo = pm.resolveActivity(checkIntent, PackageManager.MATCH_DEFAULT_ONLY );
		if(resolveInfo != null) {
			startActivityForResult(checkIntent, 0x1010);
		}
		registerReceiver(healthCheckReciever, new IntentFilter(UPDATE_INTENT));
		final String registrationId = GCMRegistrar.getRegistrationId(this);
		if (registrationId != null && !"".equals(registrationId)) {
		} else {
			GCMRegistrar.register(this, Constants.SENDER_ID);
		}
		Date olderThan = new Date();
		olderThan.setTime(olderThan.getTime() - 86400 * 1000);
		NotificationMessage.FACTORY.deleteOlderThan(this, olderThan);
	}


	public void onResume() {
		super.onResume();
		this.changeEnabledLabelFor(findViewById(R.id.home_disableAll));
		this.doHealthCheck();
	}


	public void doHealthCheck() {
		HealthCheck check = HealthCheck.performHealthcheck(this);

		TextView healthCheckArea = (TextView) findViewById(R.id.home_healthCheck);
		StringBuilder allText = new StringBuilder();
		for (String error : check.getErrors()) {
			allText.append("- ");
			allText.append(error);
			allText.append('\n');
		}
		for (String error : check.getWarnings()) {
			allText.append("- ");
			allText.append(error);
			allText.append('\n');
		}

		if (allText.length() == 0) {
			healthCheckArea.setText(getString(R.string.health_check_all_ok));
		} else {
			healthCheckArea.setText(allText.toString().trim());
		}
		final String registrationId = GCMRegistrar.getRegistrationId(this);
		if (registrationId != null && !"".equals(registrationId)) {
			Button rulesButton = (Button) findViewById(R.id.home_rules);
			rulesButton.setEnabled(true);
		}
	}

	/**
	 * Onclick handler to stop reading now.
	 * 
	 * @param view
	 */
	public void stopReadingNow(View view) {
		// Inform our service to stop reading now.
		Intent intentData = new Intent(getBaseContext(), SpeakService.class);
		intentData.putExtra("stopNow", true);
		startService(intentData);
	}
	
	/**
	 * Onclick handler to show help.
	 * 
	 * @param view
	 */
	public void showHelp(View view) {
		String url = "http://www.openfiresource.de";
		Intent i = new Intent(Intent.ACTION_VIEW);
		i.setData(Uri.parse(url));
		startActivity(i);
	}


	/**
	 * Onclick handler to toggle the master enable.
	 * 
	 * @param view
	 */
	public void disableEnableNotifications(View view) {
		SharedPreferences settings = PreferenceManager
				.getDefaultSharedPreferences(this);
		Editor editor = settings.edit();
		editor.putBoolean(getString(R.string.masterEnable),
				!settings.getBoolean(getString(R.string.masterEnable), true));
		editor.commit();

		this.changeEnabledLabelFor(view);

		this.doHealthCheck();
	}

	/**
	 * Based on the settings, change the text on the given view to match.
	 * 
	 * @param view
	 */
	public void changeEnabledLabelFor(View view) {
		SharedPreferences settings = PreferenceManager
				.getDefaultSharedPreferences(this);
		if (settings.getBoolean(getString(R.string.masterEnable), true)) {
			Button button = (Button) view;
			button.setText(R.string.disable_all_notifications);
		} else {
			Button button = (Button) view;
			button.setText(R.string.enable_all_notifications);
		}
	}

	/**
	 * Onclick handler to launch the settings dialog.
	 * 
	 * @param view
	 */
	public void launchSettings(View view) {
		Intent intent = new Intent(getBaseContext(), Settings.class);
		startActivity(intent);
	}

	/**
	 * Onclick handler to launch the SourcesActivity.
	 * 
	 * @param view
	 */
	public void launchRules(View view) {
		Intent intent = new Intent(getBaseContext(), RuleList.class);
		startActivity(intent);
	}

	/**
	 * Onclick handler to launch the recent messages dialog.
	 * 
	 * @param view
	 */
	public void launchRecentMessages(View view) {
		Intent intent = new Intent(getBaseContext(), MessageList.class);
		startActivity(intent);
	}

	/**
	 * Callback function for checking if the Text to Speech is installed. If
	 * not, it will redirect the user to download the text data.
	 */
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		if (requestCode == 0x1010) {
			if (resultCode == TextToSpeech.Engine.CHECK_VOICE_DATA_PASS) {
			} else {
				Toast.makeText(getApplicationContext(),
						R.string.need_tts_data_installed, Toast.LENGTH_LONG)
						.show();
				Intent installIntent = new Intent();
				installIntent
						.setAction(TextToSpeech.Engine.ACTION_INSTALL_TTS_DATA);
				PackageManager pm = getPackageManager();
				ResolveInfo resolveInfo = pm.resolveActivity(installIntent, PackageManager.MATCH_DEFAULT_ONLY );
				if(resolveInfo != null) {
					startActivity(installIntent);
				}
				
			}
		}
	}

	private final BroadcastReceiver healthCheckReciever = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			doHealthCheck();
		}
	};

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		MenuInflater inflater = getMenuInflater();
		inflater.inflate(R.menu.listmenu, menu);
		return true;
	}

	public String getVersion(Context context) {
		try {
			PackageInfo pInfo = context.getPackageManager().getPackageInfo(
					getPackageName(), PackageManager.GET_META_DATA);
			return pInfo.versionName + "-" + Integer.valueOf(pInfo.versionCode).toString();
		} catch (NameNotFoundException e) {
			return "unkown";
		}
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		switch (item.getItemId()) {		
		case R.id.menu_versioninfo:
			AlertDialog.Builder VersionDialog = new AlertDialog.Builder(this);
			VersionDialog.setTitle("Info");

			String versionName = getVersion(this);

			VersionDialog.setMessage("Version " + versionName);
			VersionDialog.setIcon(R.drawable.ic_launcher);

			VersionDialog.setPositiveButton("Website",
					new DialogInterface.OnClickListener() {
						public void onClick(DialogInterface dialog, int id) {

							String url = "http://www.openfiresource.de";
							Intent i = new Intent(Intent.ACTION_VIEW);
							i.setData(Uri.parse(url));
							startActivity(i);
						}
					});

			VersionDialog.show();

			return true;
			
		case R.id.menu_legalnotices:
		      String LicenseInfo = GooglePlayServicesUtil.getOpenSourceSoftwareLicenseInfo(
		        getApplicationContext());
		      AlertDialog.Builder LicenseDialog = new AlertDialog.Builder(this);
		      LicenseDialog.setTitle("Legal Notices");
		      LicenseDialog.setMessage(LicenseInfo);
		      LicenseDialog.show();
		  
		    return true;
		default:
			return super.onOptionsItemSelected(item);
		}
	}
}