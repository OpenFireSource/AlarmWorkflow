package de.florianritterhoff.eAlarm.services;

import java.io.IOException;
import java.util.Date;
import java.util.HashMap;
import java.util.Map;
import java.util.UUID;

import de.florianritterhoff.eAlarm.C2DMClientActivity;
import de.florianritterhoff.eAlarm.OperationDetail;
import de.florianritterhoff.eAlarm.R;
import android.app.IntentService;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.media.AudioManager;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.os.PowerManager;
import android.preference.PreferenceManager;
import android.util.Log;

public class GCMIntent extends IntentService {

	private String email;
	private AudioManager audman;
	private int currentAudioVolume;
	private static String UUID = "uhwid";

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
	private static void generateNotification(Context context, String message,
			String time) {
		int icon = R.drawable.ic_launcher;
		long when = System.currentTimeMillis();
		NotificationManager notificationManager = (NotificationManager) context
				.getSystemService(Context.NOTIFICATION_SERVICE);
		Notification notification = new Notification(icon, message, when);

		String title = context.getString(R.string.app_name);

		Intent notificationIntent = new Intent(context, OperationDetail.class);
		// set intent so it does not start a new activity
		notificationIntent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP
				| Intent.FLAG_ACTIVITY_SINGLE_TOP);
		notificationIntent.putExtra(MySQLiteHelper.COLUMN_TIMESTAMP, time);
		PendingIntent intent = PendingIntent.getActivity(context, 0,
				notificationIntent, PendingIntent.FLAG_CANCEL_CURRENT);
		notification.setLatestEventInfo(context, title, message, intent);
		notification.flags |= Notification.FLAG_AUTO_CANCEL;
		// Play default notification sound
		notification.defaults |= Notification.DEFAULT_SOUND;
		// Vibrate if vibrate is enabled
		notification.defaults |= Notification.DEFAULT_VIBRATE;
		notificationManager.notify(0, notification);

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
		String header = intent.getExtras().getString("header");
		String text = intent.getExtras().getString("text");
		String longitude = intent.getExtras().getString("long");
		String latitude = intent.getExtras().getString("lat");
		Date date = new Date();
		String time = date.getTime() + "";
		DataSource.getInstance(this).addOperation(header, text, longitude,
				latitude, time);
		audman = ((AudioManager) getApplicationContext().getSystemService(
				"audio"));
		currentAudioVolume = audman.getStreamVolume(AudioManager.STREAM_MUSIC);
		audman.setStreamVolume(AudioManager.STREAM_MUSIC,
				audman.getStreamMaxVolume(AudioManager.STREAM_MUSIC), 0);
		generateNotification(getApplicationContext(), header, time);
		audman.setStreamVolume(AudioManager.STREAM_MUSIC, currentAudioVolume, 0);

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
				ServerConnection.post(
						"http://gymolching-portal.de/gcm/register.php", params);
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
}