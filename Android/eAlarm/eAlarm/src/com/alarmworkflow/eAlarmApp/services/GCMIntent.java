package com.alarmworkflow.eAlarmApp.services;

import java.io.IOException;
import java.util.Date;
import java.util.HashMap;
import java.util.Map;
import java.util.UUID;

import com.alarmworkflow.eAlarmApp.C2DMClientActivity;
import com.alarmworkflow.eAlarmApp.OperationDetail;
import com.alarmworkflow.eAlarmApp.R;

import android.app.IntentService;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.graphics.Color;
import android.media.AudioManager;
import android.media.MediaPlayer;
import android.net.Uri;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.os.PowerManager;
import android.os.PowerManager.WakeLock;
import android.os.Vibrator;
import android.preference.PreferenceManager;
import android.util.Log;

public class GCMIntent extends IntentService implements
		MediaPlayer.OnCompletionListener {

	private String email;
	private AudioManager audman;
	private int currentAudioVolume;
	private static String UUID = "uhwid";
	private MediaPlayer mediaPlayer;
	private String header;
	private String text;
	private boolean overridesound;
	private String alarmsound;
	private boolean vibrate;
	private boolean sound;
	private boolean openApp;
	private long timeout;
	private boolean deviceOn;
	private boolean screenoff;

	public GCMIntent(String name) {
		super(name);
		// TODO Auto-generated constructor stub
	}

	public GCMIntent() {
		super("");
	}

	private static PowerManager.WakeLock sWakeLock;
	private static final Object LOCK = GCMIntent.class;
	private static final String TAG = GCMIntent.class.toString();

	static void runIntentInService(Context context, Intent intent) {
		synchronized (LOCK) {
			if (sWakeLock == null) {
				PowerManager pm = (PowerManager) context
						.getSystemService(Context.POWER_SERVICE);
				sWakeLock = pm.newWakeLock(PowerManager.PARTIAL_WAKE_LOCK,
						"my_wakelock");
			}
		}
		sWakeLock.acquire();
		intent.setClassName(context, GCMIntent.class.getName());
		context.startService(intent);
	}

	/**
	 * Issues a notification to inform the user that server has sent a message.
	 * 
	 * @param time
	 */
	private void generateNotification(Context context, String message,
			String time) {
		int icon = R.drawable.ic_launcher;
		long when = System.currentTimeMillis();
		NotificationManager notificationManager = (NotificationManager) context
				.getSystemService(Context.NOTIFICATION_SERVICE);
		Notification notification = new Notification(icon, message, when);
		String title = context.getString(R.string.app_name);
		Intent notificationIntent = getNotifyIntent(time);
		PendingIntent intent = PendingIntent.getActivity(context, 0,
				notificationIntent, PendingIntent.FLAG_CANCEL_CURRENT);
		notification.setLatestEventInfo(context, title, message, intent);
		notification.flags |= Notification.FLAG_AUTO_CANCEL;
		notification.flags |= Notification.FLAG_NO_CLEAR;
		notification.flags |= Notification.FLAG_SHOW_LIGHTS;
		notification.ledARGB = Color.RED;
		notification.ledOnMS = 1;
		notification.ledOffMS = 0;
		notificationManager.notify(0, notification);

	}

	private Intent getNotifyIntent(String time) {
		Intent intent = new Intent(getApplicationContext(),
				OperationDetail.class);
		intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
		intent.addFlags(Intent.FLAG_ACTIVITY_MULTIPLE_TASK);
		intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
		intent.putExtra(MySQLiteHelper.COLUMN_TIMESTAMP, time);
		return intent;
	}

	@Override
	public final void onHandleIntent(Intent intent) {
		try {
			Log.i(this.toString(), "Iwas tut sich ;))");
			String action = intent.getAction();
			if (action.equals("com.google.android.c2dm.intent.REGISTRATION")) {
				Log.i(this.toString(), "Ah jemand tut sich registrieren");
				handleRegistration(intent);
			} else if (action.equals("com.google.android.c2dm.intent.RECEIVE")) {
				Log.i(this.toString(), "Ah jemand will was von mir :)");
				handleMessage(intent);
			}
		} finally {
			synchronized (LOCK) {
				sWakeLock.release();
			}
		}
	}

	private void handleMessage(Intent intent) {
		header = intent.getExtras().getString("header");
		text = intent.getExtras().getString("text");
		String longitude = intent.getExtras().getString("long");
		String latitude = intent.getExtras().getString("lat");
		Date date = new Date();
		String time = date.getTime() + "";
		DataSource.getInstance(this).addOperation(header, text, longitude,
				latitude, time);
		initPreferences();
		audman = ((AudioManager) getApplicationContext().getSystemService(
				"audio"));
		currentAudioVolume = audman.getStreamVolume(AudioManager.STREAM_ALARM);
		if (overridesound)
			audman.setStreamVolume(AudioManager.STREAM_ALARM,
					audman.getStreamMaxVolume(AudioManager.STREAM_ALARM), 0);
		if (deviceOn)
			switchDeviceOn();		
		if (sound && alarmsound != "")
			playSound();
		if (vibrate)
			vibrate();		
		if (openApp)
			openApplication(time);
		else
			generateNotification(getApplicationContext(), header, time);

	}

	void playSound() {

		mediaPlayer = new MediaPlayer();
		Uri uri = Uri.parse(alarmsound);
		try {
			mediaPlayer.setDataSource(this, uri);
			final AudioManager audioManager = (AudioManager) this
					.getSystemService(Context.AUDIO_SERVICE);
			if (audioManager.getStreamVolume(AudioManager.STREAM_ALARM) != 0) {
				mediaPlayer.setAudioStreamType(AudioManager.STREAM_ALARM);
				mediaPlayer.prepare();
				mediaPlayer.start();
				mediaPlayer.setLooping(false);
				mediaPlayer.setOnCompletionListener(this);
			}
		} catch (IOException e) {
			Log.i("THREAD", "oops");
		}

	}

	void initPreferences() {
		SharedPreferences prefs = PreferenceManager
				.getDefaultSharedPreferences(this);
		vibrate = prefs.getBoolean("vibrate", false);
		sound = prefs.getBoolean("sound", false);
		overridesound = prefs.getBoolean("overridesound", false);
		openApp = prefs.getBoolean("openApp", false);
		alarmsound = prefs.getString("ringsel", "");
		deviceOn = prefs.getBoolean("deviceOn", false);
		timeout = prefs.getInt("screentimeout", 0);
		screenoff = prefs.getBoolean("screenoff", true);
	}

	private void openApplication(String time) {

		startActivity(getNotifyIntent(time));
	}

	void vibrate() {
		new Thread(new Runnable() {
			public void run() {
				Vibrator v = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
				int dot = 200;
				int dash = 500;
				int short_gap = 200;
				int medium_gap = 500;
				int long_gap = 1000;
				long[] pattern = { 0, // Start immediately
						dot, short_gap, dot, short_gap, dot, // s
						medium_gap, dash, short_gap, dash, short_gap, dash, // o
						medium_gap, dot, short_gap, dot, short_gap, dot, // s
						long_gap };

				// Only perform this pattern one time (-1 means "do not repeat")
				v.vibrate(pattern, -1);
			}
		}).start();

	}

	private void switchDeviceOn() {
		PowerManager pm = (PowerManager) getApplicationContext()
				.getSystemService(Context.POWER_SERVICE);
		WakeLock wakeLock = pm
				.newWakeLock(
						(PowerManager.SCREEN_BRIGHT_WAKE_LOCK
								| PowerManager.FULL_WAKE_LOCK | PowerManager.ACQUIRE_CAUSES_WAKEUP),
						"eAlarm");
		if (!screenoff)
			wakeLock.acquire();
		else
			wakeLock.acquire(timeout);
	}

	private String generateDeviceId() {
		final String macAddr, androidId;
		WifiManager wifiMan = (WifiManager) this
				.getSystemService(Context.WIFI_SERVICE);
		WifiInfo wifiInf = wifiMan.getConnectionInfo();
		macAddr = wifiInf.getMacAddress();
		androidId = android.provider.Settings.Secure.getString(
				getContentResolver(),
				android.provider.Settings.Secure.ANDROID_ID);
		UUID deviceUuid = new UUID(androidId.hashCode(), macAddr.hashCode());
		return deviceUuid.toString();

	}

	private void handleRegistration(Intent intent) {
		String registrationId = intent.getStringExtra("registration_id");
		String error = intent.getStringExtra("error");
		String unregistered = intent.getStringExtra("unregistered");
		// registration succeeded
		if (registrationId != null) {
			SharedPreferences prefs = PreferenceManager
					.getDefaultSharedPreferences(this);
			Editor edit = prefs.edit();
			edit.putString(C2DMClientActivity.AUTH, registrationId);
			edit.commit();
			email = prefs.getString(C2DMClientActivity.EMAIL, "n/A");
			Map<String, String> params = new HashMap<String, String>();
			params.put(C2DMClientActivity.AUTH, registrationId);
			params.put(C2DMClientActivity.EMAIL, email);
			params.put(UUID, generateDeviceId());
			try {
				ServerConnection
						.post("https://gymolching-portal.de/gcm/register.php",
								params);
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}

		// unregistration succeeded
		if (unregistered != null) {
			// get old registration ID from shared preferences
			// notify 3rd-party server about the unregistered ID
		}

		// last operation (registration or unregistration) returned an error;
		if (error != null) {
			if ("SERVICE_NOT_AVAILABLE".equals(error)) {
				// optionally retry using exponential back-off
				// (see Advanced Topics)
			} else {
				// Unrecoverable error, log it
				Log.i(TAG, "Received error: " + error);
			}
		}
	}

	@Override
	public void onCompletion(MediaPlayer mp) {
		mediaPlayer.stop();
		audman.setStreamVolume(AudioManager.STREAM_ALARM, currentAudioVolume, 0);
	}
}