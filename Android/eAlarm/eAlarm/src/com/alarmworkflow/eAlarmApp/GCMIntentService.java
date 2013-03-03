package com.alarmworkflow.eAlarmApp;

import java.io.UnsupportedEncodingException;
import java.net.URLDecoder;
import java.util.Date;

import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Vibrator;
import android.preference.PreferenceManager;
import android.util.Log;

import com.alarmworkflow.eAlarmApp.OperationDetail;
import com.alarmworkflow.eAlarmApp.R;
import com.alarmworkflow.eAlarmApp.eAlarm;
import com.alarmworkflow.eAlarmApp.datastorage.DataSource;
import com.alarmworkflow.eAlarmApp.datastorage.MySQLiteHelper;
import com.google.android.gcm.GCMBaseIntentService;

import static com.alarmworkflow.eAlarmApp.general.CommonUtilities.SENDER_ID;

public class GCMIntentService extends GCMBaseIntentService {

	private static final String TAG = "GCMIntentService";
	private boolean vibrate;
	private boolean sound;
	private boolean openApp;

	public GCMIntentService() {
		super(SENDER_ID);
	}

	/**
	 * Method called on device registered
	 **/
	@Override
	protected void onRegistered(Context context, String registrationId) {
		Log.i(TAG, "Device registered: regId = " + registrationId);
		
	}

	/**
	 * Method called on device unregistred
	 * */
	@Override
	protected void onUnregistered(Context context, String registrationId) {
		Log.i(TAG, "Device unregistered");
	}

	/**
	 * Method called on Receiving a new message
	 * */
	@Override
	protected void onMessage(Context context, Intent intent) {
		Log.i(TAG, "Received message");
		String header = intent.getExtras().getString("header");
		String text = intent.getExtras().getString("text");
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
		DataSource.getInstance(this).addOperation(header, text, latitude,
				longitude, time);
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

	void initPreferences() {
		SharedPreferences prefs = PreferenceManager
				.getDefaultSharedPreferences(this);
		vibrate = prefs.getBoolean("vibrate", false);
		sound = prefs.getBoolean("sound", false);
		openApp = prefs.getBoolean("openApp", false);
	}

	/**
	 * Method called on Error
	 * */
	@Override
	public void onError(Context context, String errorId) {
		Log.i(TAG, "Received error: " + errorId);
	}

	@Override
	protected boolean onRecoverableError(Context context, String errorId) {
		// log message
		Log.i(TAG, "Received recoverable error: " + errorId);
		return super.onRecoverableError(context, errorId);
	}

	private static Intent getNotifyIntent(String time) {
		Intent intent = new Intent(eAlarm.context, OperationDetail.class);
		intent.putExtra(MySQLiteHelper.COLUMN_TIMESTAMP, time);
		intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP
				| Intent.FLAG_ACTIVITY_SINGLE_TOP
				| Intent.FLAG_ACTIVITY_NEW_TASK);

		return intent;
	}

	/**
	 * Issues a notification to inform the user that server has sent a message.
	 */
	private static void generateNotification(Context context, String message,
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

}