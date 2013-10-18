package com.alarmworkflow.eAlarm;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.os.Bundle;
import android.preference.PreferenceManager;

import com.alarmworkflow.eAlarm.database.NotificationMessage;
import com.alarmworkflow.eAlarm.general.Constants;
import com.alarmworkflow.eAlarm.general.NotificationService;
import com.google.android.gcm.GCMBaseIntentService;

public class GCMIntentService extends GCMBaseIntentService {


	public GCMIntentService() {
		super(Constants.SENDER_ID);
	}

	/**
	 * Method called on device registered
	 **/
	@Override
	protected void onRegistered(Context context, String registrationId) {
		// Clear any errors we had.
		SharedPreferences settings = PreferenceManager
				.getDefaultSharedPreferences(context);
		Editor editor = settings.edit();
		editor.putString("dm_register_error", "");
		editor.commit();

		// Update the home screen.
		Intent updateUIIntent = new Intent(eAlarmPush.UPDATE_INTENT);
		context.sendBroadcast(updateUIIntent);

	}

	/**
	 * Method called on device unregistred
	 * */
	@Override
	protected void onUnregistered(Context context, String registrationId) {
	}

	/**
	 * Method called on Receiving a new message
	 * */
	@Override
	protected void onMessage(Context context, Intent intent) {
		Bundle extras = intent.getExtras();
		NotificationMessage[] message = null;

		NotificationMessage highestMessage = null;
		try {
			message = NotificationMessage.fromGCM(context, extras);
		} catch (Exception e) {
			return;
		}
		for (NotificationMessage notificationMessage : message) {
			if (notificationMessage.getRule() != null) {// Persist this
															// message to
															// the database.
				if (highestMessage == null
						|| highestMessage.getRule().getPriority() < notificationMessage
								.getRule().getPriority()) {
					highestMessage = notificationMessage;
				}
			}
		}
		// Send a notification to the notification service, which
		// will then
		// dispatch and handle everything else.
		if (highestMessage != null) {
			highestMessage.save(context);
			Intent intentData = new Intent(getBaseContext(),
					NotificationService.class);
			intentData.putExtra("messageId", highestMessage.getId());
			intentData.putExtra("operation", "notification");
			startService(intentData);
		}
		Intent brIntent = new Intent();
		brIntent.setAction("com.alarmworkflow.eAlarm.revicedMessage");
		sendBroadcast(brIntent);
	}

	/**
	 * Method called on Error
	 * */
	@Override
	public void onError(Context context, String errorId) {
		SharedPreferences settings = PreferenceManager
				.getDefaultSharedPreferences(context);
		Editor editor = settings.edit();
		editor.putString("dm_register_error", errorId);
		editor.commit();

		// Notify the user.
		// TODO: Do this.

		// Update the home screen.
		Intent updateUIIntent = new Intent(eAlarmPush.UPDATE_INTENT);
		context.sendBroadcast(updateUIIntent);
	}

	@Override
	protected boolean onRecoverableError(Context context, String errorId) {
		// log message
		return super.onRecoverableError(context, errorId);
	}

}