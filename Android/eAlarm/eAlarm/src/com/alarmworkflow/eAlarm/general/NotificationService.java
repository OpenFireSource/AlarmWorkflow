package com.alarmworkflow.eAlarm.general;

import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.IBinder;
import android.preference.PreferenceManager;
import android.support.v4.app.NotificationCompat;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.TextView;
import android.widget.Toast;

import com.alarmworkflow.eAlarm.MessageDetail;
import com.alarmworkflow.eAlarm.MessageList;
import com.alarmworkflow.eAlarm.NotifyDecision;
import com.alarmworkflow.eAlarm.R;
import com.alarmworkflow.eAlarm.SpeakService;
import com.alarmworkflow.eAlarm.database.NotificationMessage;
import com.alarmworkflow.eAlarm.database.NotificationRule;

public class NotificationService extends Service {
	private static final String TAG = "eAlarm";
	private NotificationManager notificationManager = null;

	@Override
	public IBinder onBind(Intent arg0) {
		return null;
	}

	public void onCreate() {
		super.onCreate();

		// Fetch out our notification service.
		this.notificationManager = (NotificationManager) getSystemService(NOTIFICATION_SERVICE);
	}

	public android.support.v4.app.NotificationCompat.Builder setLatestEventInfo(
			NotificationRule rule, NotificationMessage message) {

		int icon = R.drawable.ic_launcher;
		long when = System.currentTimeMillis();

		android.support.v4.app.NotificationCompat.Builder builder = new NotificationCompat.Builder(
				this);

		builder.setSmallIcon(icon);
		builder.setWhen(when);
		int unreadMessagesOfType = 0;
		String contentTitle = "";
		String contentText = "";

		unreadMessagesOfType = NotificationMessage.FACTORY.countUnread(this,
				rule);
		PendingIntent contentIntent = null;
		if (unreadMessagesOfType == 1) {
			message = NotificationMessage.FACTORY.getUnread(this, rule);
		}
		if (unreadMessagesOfType == 1 && message != null) {
			// Only one message of this type. Set the title to be the message's
			// title, and then
			// content to be the message itself.
			contentTitle = message.getTitle();
			contentText = message.getMessage();
			// Generate the intent to go to that message list.// Launch the
			// message detail.
			Intent intent = new Intent(getBaseContext(), MessageDetail.class);
			intent.putExtra("messageId", message.getId());
			contentIntent = PendingIntent.getActivity(this, 0, intent,
					PendingIntent.FLAG_UPDATE_CURRENT);
		} else {
			// More than one message. Instead, the title is the rule name,
			// and the content is the number of unseen messages.
			contentTitle = rule.getTitle();
			contentText = String.format("%d neue Alarme", unreadMessagesOfType);

			// Generate the intent to go to that message list.
			Intent notificationIntent = new Intent(this, MessageList.class);
			notificationIntent.putExtra("ruleId", rule.getId());
			contentIntent = PendingIntent.getActivity(this, 0,
					notificationIntent, PendingIntent.FLAG_UPDATE_CURRENT);
		}

		builder.setContentTitle(contentTitle);
		builder.setContentText(contentText);
		builder.setContentIntent(contentIntent);

		return builder;
	}

	private void ShowToast(String text) {

		// get the LayoutInflater and inflate the custom_toast layout
		// LayoutInflater inflater = getLayoutInflater();
		// View layout = inflater.inflate(R.layout.custom_toast, (ViewGroup)
		// findViewById(R.id.toast_layout_root));

		LayoutInflater layoutInflater = (LayoutInflater) this
				.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
		View layout = layoutInflater.inflate(R.layout.custom_toast, null);

		// get the TextView from the custom_toast layout
		TextView tv = (TextView) layout.findViewById(R.id.toastText);
		tv.setText((CharSequence) text);

		// create the toast object, set display duration,
		// set the view as layout that's inflated above and then call show()
		Toast t = new Toast(getApplicationContext());
		t.setDuration(Toast.LENGTH_LONG);
		t.setView(layout);
		t.setGravity(Gravity.CENTER, 0, 0);
		t.show();

	}

	public boolean globalOrOverrideBoolean(int setting,
			SharedPreferences preferences, NotificationRule rule,
			boolean defaultValue) {
		return globalOrOverrideBoolean(setting, preferences, rule,
				defaultValue, this);

	}

	public static boolean globalOrOverrideBoolean(int setting,
			SharedPreferences preferences, NotificationRule rule,
			boolean defaultValue, Context context) {
		if (rule.getUseGlobalNotification()) {
			// Use the global setting for this notification.
			return preferences.getBoolean(context.getString(setting),
					defaultValue);
		} else {
			// Determine the setting, and then return the appropriate setting.
			if (R.string.showToast == setting) {
				return rule.getToast();
			} else if (R.string.playRingtone == setting) {
				return rule.getRingtone();
			} else if (R.string.vibrateNotify == setting) {
				return rule.getVibrate();
			} else if (R.string.ledFlash == setting) {
				return rule.getLedFlash();
			} else if (R.string.speakMessage == setting) {
				return rule.getSpeakMessage();
			} else if (R.string.overwritesystem_setting == setting) {
				return rule.getOverwritesystem();
			} else if (R.string.showToast == setting) {
				return rule.getToast();
			} else if (R.string.open == setting) {
				return rule.getOpen();
			} else if (R.string.unlock == setting) {
				return rule.getUnlock();
			}
		}

		return defaultValue;
	}

	public String globalOrOverrideString(int setting,
			SharedPreferences preferences, NotificationRule rule,
			String defaultValue) {
		return globalOrOverrideString(setting, preferences, rule, defaultValue,
				this);
	}

	public static String globalOrOverrideString(int setting,
			SharedPreferences preferences, NotificationRule rule,
			String defaultValue, Context context) {
		if (rule.getUseGlobalNotification()) {
			// Use the global setting for this notification.
			return preferences.getString(context.getString(setting),
					defaultValue);
		} else {
			// Determine the setting, and then return the appropriate setting.
			if (R.string.choosenNotification == setting) {
				return rule.getCustomRingtone();
			}
		}

		return defaultValue;
	}

	public void onStart(Intent intent, int startId) {
		super.onStart(intent, startId);

		// On null intent, give up.
		if (intent == null) {
			return;
		}

		// Determine our action.
		String operation = intent.getStringExtra("operation");

		if (operation.equals("notification")) {
			// Is the master enable off? Then don't bother doing anything.
			SharedPreferences preferences = PreferenceManager
					.getDefaultSharedPreferences(this);
			if (preferences.getBoolean(getString(R.string.masterEnable), true) == false) {
				return;
			}

			// We were provided with a message ID. Load it and then handle it.
			final Long messageId = intent.getLongExtra("messageId", 0);

			NotificationMessage message = NotificationMessage.FACTORY.get(this,
					messageId);
			if (message == null) {
				return;
			}

			// Make a decision on the message.
			NotifyDecision decision = NotifyDecision
					.shouldNotify(this, message);

			if (decision.getShouldNotify()) {
				NotificationRule rule = message.getRule();
				if (globalOrOverrideBoolean(R.string.open, preferences, rule,
						false)) {
					Intent open = new Intent(getBaseContext(),
							MessageDetail.class);
					open.putExtra("messageId", message.getId());
					open.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
					startActivity(open);
				} else {
					if (globalOrOverrideBoolean(R.string.showToast,
							preferences, rule, true)) {
						ShowToast(message.getTitle() + ":\n"
								+ message.getMessage());
					}

				}
				// Ok, let's start notifying!
				android.support.v4.app.NotificationCompat.Builder notification = this
						.setLatestEventInfo(rule, message);

				if (globalOrOverrideBoolean(R.string.playRingtone, preferences,
						rule, true)) {
					Intent i = new Intent(
							"com.alarmworkflow.eAlarm.MusicPlayer");
					i.putExtra("messageId", messageId);
					sendBroadcast(i);

				}
				if (globalOrOverrideBoolean(R.string.vibrateNotify,
						preferences, rule, true)) {
					notification.setDefaults(Notification.DEFAULT_VIBRATE);
				}
				if (globalOrOverrideBoolean(R.string.ledFlash, preferences,
						rule, true)) {
					notification.setLights(0xffff0000, 500, 300);
				}
				// Put the notification in the tray. Use the rule's local ID
				// to identify it.
				this.notificationManager.notify(rule.getNotificationId(),
						notification.getNotification());

				// If we're speaking, dispatch the message to the speaking
				// service.

				if (globalOrOverrideBoolean(R.string.speakMessage, preferences,
						rule, false)) {
					Intent intentData = new Intent(getBaseContext(),
							SpeakService.class);
					intentData.putExtra("text", decision.getOutputMessage());
					startService(intentData);
				}

			}
		} else if (operation.equals("update")) {
			// Clear the notifications for a given rule - if there are no
			// unread messages.
			NotificationRule rule = NotificationRule.FACTORY.get(this,
					intent.getLongExtra("ruleId", 0));

			if (rule != null) {
				this.updateNotificationFor(rule);
			} else {
				this.notificationManager.cancelAll();
			}
		}

		return;
	}

	private void updateNotificationFor(NotificationRule rule) {
		if (NotificationMessage.FACTORY.countUnread(this, rule) == 0) {
			this.notificationManager.cancel(rule.getNotificationId());
		} else {
			// Change it to the real number of read messages.
			android.support.v4.app.NotificationCompat.Builder notification = setLatestEventInfo(
					rule, null);
			this.notificationManager.notify(rule.getNotificationId(),
					notification.getNotification());
		}
	}
}
