package com.alarmworkflow.eAlarmApp.services;

import java.io.UnsupportedEncodingException;
import java.net.URLDecoder;
import java.util.Date;
import com.alarmworkflow.eAlarmApp.OperationDetail;
import com.alarmworkflow.eAlarmApp.R;
import com.alarmworkflow.eAlarmApp.datastorage.DataSource;
import com.alarmworkflow.eAlarmApp.datastorage.MySQLiteHelper;
import android.app.IntentService;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.os.PowerManager;
import android.os.Vibrator;
import android.preference.PreferenceManager;
import android.util.Log;

public class GCMIntent extends IntentService {

	private String header;
	private String text;
	private boolean vibrate;
	private boolean sound;
	private boolean openApp;

	public GCMIntent(String name) {
		super(name);
	}

	public GCMIntent() {
		super("");
	}

	private static PowerManager.WakeLock sWakeLock;
	private static final Object LOCK = GCMIntent.class;

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
		notification.defaults = Notification.DEFAULT_LIGHTS;

		notificationManager.notify(0, notification);

	}

	private Intent getNotifyIntent(String time) {
		Intent intent = new Intent(getApplicationContext(),
				OperationDetail.class);
		intent.putExtra(MySQLiteHelper.COLUMN_TIMESTAMP, time);
		intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP
				| Intent.FLAG_ACTIVITY_SINGLE_TOP
				| Intent.FLAG_ACTIVITY_NEW_TASK);

		return intent;
	}

	@Override
	public final void onHandleIntent(Intent intent) {
		try {
			Log.i(GCMIntent.class.getSimpleName(), "Incoming Data");
			String action = intent.getAction();
			if (action.equals("com.google.android.c2dm.intent.REGISTRATION")) {
				Log.i(GCMIntent.class.getSimpleName(),
						"Incoming RegistrationID");
				handleRegistration(intent);
			} else if (action.equals("com.google.android.c2dm.intent.RECEIVE")) {
				Log.i(GCMIntent.class.getSimpleName(), "Incoming Message");
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
		try {
			header = URLDecoder.decode(header, "UTF-8");
			text = URLDecoder.decode(text, "UTF-8");
		} catch (UnsupportedEncodingException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		String longitude = intent.getExtras().getString("long");
		String latitude = intent.getExtras().getString("lat");
		Date date = new Date();
		String time = date.getTime() + "";
		DataSource.getInstance(this).addOperation(header, text, longitude,
				latitude, time);
		initPreferences();		
		if (openApp) {
			openApplication(time);
		} else {
			generateNotification(getApplicationContext(), header, time);
		}
		if (sound) {
			Intent i = new Intent("com.alarmworkflow.eAlarmApp.MusicPlayer");
			sendBroadcast(i);
		}
		if (vibrate) {
			vibrate();
		}

	}

	void initPreferences() {
		SharedPreferences prefs = PreferenceManager
				.getDefaultSharedPreferences(this);
		vibrate = prefs.getBoolean("vibrate", false);
		sound = prefs.getBoolean("sound", false);
		openApp = prefs.getBoolean("openApp", false);
	}

	private void openApplication(String time) {
		startActivity(getNotifyIntent(time));
	}

	void vibrate() {
		new Thread(new Runnable() {
			public void run() {
				Vibrator v = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
				int on = 650;
				int off = 300;
				long[] pattern = { 0, // Start immediately
						on, off, on, off, on, off, on };

				// Only perform this pattern one time (-1 means "do not repeat")
				v.vibrate(pattern, -1);
			}
		}).start();

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
			edit.putString("auth", registrationId);
			edit.commit();
		}

		// unregistration succeeded
		if (unregistered != null) {

		}

		// last operation (registration or unregistration) returned an error;
		if (error != null) {
			if ("SERVICE_NOT_AVAILABLE".equals(error)) {
				// optionally retry using exponential back-off
				// (see Advanced Topics)
			} else {
				// Unrecoverable error, log it
				Log.i(GCMIntent.class.getSimpleName(), "Received error: "
						+ error);
			}
		}
	}

}