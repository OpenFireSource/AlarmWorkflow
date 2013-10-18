package com.alarmworkflow.eAlarm.database;

import java.util.ArrayList;
import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.net.Uri;

public class NotificationRule extends DatabaseObject<NotificationRule> {
	// private static final String TAG = "eAlarm";

	public static final NotificationRule FACTORY = new NotificationRule();

	private String title = null;
	private Boolean localEnabled = null;
	private Boolean useGlobalNotification = true;
	private Boolean vibrate = false;
	private Boolean toast = false;
	private Boolean ringtone = false;
	private String customRingtone = "";
	private Boolean ledFlash = false;
	private Boolean speakMessage = false;
	private Boolean overwritesystem = false;
	private Boolean open = false;
	private Boolean unlock = false;
	private String startTime = "00:00";
	private String stopTime = "00:00";
	private int priority = 0;
	private String searchText = "";
	

	/**
	 * Get the notification ID. This is the local rule ID as an integer.
	 * 
	 * @return
	 */
	public int getNotificationId() {
		// Yes, this casting will potentially lose precision. But unless
		// you've created a lot of local rules, you're unlikely to run
		// into it. If you run into this in production, please let me know.
		Long ruleId = this.getId();
		int notifyId = (int) (ruleId % Integer.MAX_VALUE);
		return notifyId;
	}

	public String getTitle() {
		return title;
	}

	public void setTitle(String title) {
		this.title = title;
	}

	public Boolean getLocalEnabled() {
		return localEnabled;
	}

	public void setLocalEnabled(Boolean localEnabled) {
		this.localEnabled = localEnabled;
	}

	public Boolean getUseGlobalNotification() {
		return useGlobalNotification;
	}

	public void setUseGlobalNotification(Boolean useGlobalNotification) {
		this.useGlobalNotification = useGlobalNotification;
	}

	public void setToast(Boolean toast) {
		this.toast = toast;
	}

	public Boolean getVibrate() {
		return vibrate;
	}

	public void setVibrate(Boolean vibrate) {
		this.vibrate = vibrate;
	}

	public Boolean getRingtone() {
		return ringtone;
	}

	public void setRingtone(Boolean ringtone) {
		this.ringtone = ringtone;
	}

	public String getCustomRingtone() {
		return customRingtone;
	}

	public void setCustomRingtone(String customRingtone) {
		this.customRingtone = customRingtone;
	}

	public Boolean getLedFlash() {
		return ledFlash;
	}

	public Boolean getToast() {
		return toast;
	}

	public void setLedFlash(Boolean ledFlash) {
		this.ledFlash = ledFlash;
	}

	public Boolean getSpeakMessage() {
		return speakMessage;
	}

	public void setSpeakMessage(Boolean speakMessage) {
		this.speakMessage = speakMessage;
	}

	public ArrayList<NotificationRule> listAll(Context context) {
		return NotificationRule.FACTORY.genericList(context, null, null,
				DatabaseAdapter.KEY_TITLE + " ASC");
	}

	public int countRules(Context context) {
		String query = null;
		String[] queryParams = null;
		return this.genericCount(context, query, queryParams);
	}

	@Override
	public Uri getContentUri() {
		return DatabaseAdapter.CONTENT_URI_RULES;
	}

	@Override
	protected ContentValues flatten() {
		ContentValues values = new ContentValues();
		values.put(DatabaseAdapter.KEY_LOCAL_ENABLED,
				this.getLocalEnabled() ? 1 : 0);
		values.put(DatabaseAdapter.KEY_TITLE, this.getTitle());
		values.put(DatabaseAdapter.KEY_USE_GLOBAL_NOTIFICATION,
				this.getUseGlobalNotification() ? 1 : 0);
		values.put(DatabaseAdapter.KEY_VIBRATE, this.getVibrate() ? 1 : 0);
		values.put(DatabaseAdapter.KEY_TOAST, this.getToast() ? 1 : 0);
		values.put(DatabaseAdapter.KEY_RINGTONE, this.getRingtone() ? 1 : 0);
		values.put(DatabaseAdapter.KEY_CUSTOM_RINGTONE,
				this.getCustomRingtone());
		values.put(DatabaseAdapter.KEY_SEARCHTEXT, this.getSearchText());
		values.put(DatabaseAdapter.KEY_STARTTIME, this.getStartTime());
		values.put(DatabaseAdapter.KEY_STOPTIME, this.getStopTime());
		values.put(DatabaseAdapter.KEY_PRIORITY, this.getPriority());
		values.put(DatabaseAdapter.KEY_LED_FLASH, this.getLedFlash() ? 1 : 0);
		values.put(DatabaseAdapter.KEY_SPEAK_MESSAGE,
				this.getSpeakMessage() ? 1 : 0);
		values.put(DatabaseAdapter.KEY_OVERWRITE_SYSTEM,
				this.getOverwritesystem() ? 1 : 0);
		values.put(DatabaseAdapter.KEY_UNLOCK, this.getUnlock() ? 1 : 0);
		values.put(DatabaseAdapter.KEY_OPEN, this.getOpen() ? 1 : 0);
		return values;
	}

	@Override
	protected NotificationRule inflate(Context context, Cursor cursor) {
		NotificationRule rule = new NotificationRule();
		rule.setId(cursor.getLong(cursor.getColumnIndex(DatabaseAdapter.KEY_ID)));
		rule.setLocalEnabled(cursor.getLong(cursor
				.getColumnIndex(DatabaseAdapter.KEY_LOCAL_ENABLED)) == 0 ? false
				: true);
		rule.setTitle(cursor.getString(cursor
				.getColumnIndex(DatabaseAdapter.KEY_TITLE)));

		rule.setUseGlobalNotification(cursor.getLong(cursor
				.getColumnIndex(DatabaseAdapter.KEY_USE_GLOBAL_NOTIFICATION)) == 0 ? false
				: true);
		rule.setVibrate(cursor.getLong(cursor
				.getColumnIndex(DatabaseAdapter.KEY_VIBRATE)) == 0 ? false
				: true);
		rule.setToast(cursor.getLong(cursor
				.getColumnIndex(DatabaseAdapter.KEY_TOAST)) == 0 ? false : true);
		rule.setRingtone(cursor.getLong(cursor
				.getColumnIndex(DatabaseAdapter.KEY_RINGTONE)) == 0 ? false
				: true);
		rule.setLedFlash(cursor.getLong(cursor
				.getColumnIndex(DatabaseAdapter.KEY_LED_FLASH)) == 0 ? false
				: true);
		rule.setCustomRingtone(cursor.getString(cursor
				.getColumnIndex(DatabaseAdapter.KEY_CUSTOM_RINGTONE)));
		rule.setSpeakMessage(cursor.getLong(cursor
				.getColumnIndex(DatabaseAdapter.KEY_SPEAK_MESSAGE)) == 0 ? false
				: true);
		rule.setOverwritesystem(cursor.getInt(cursor
				.getColumnIndex(DatabaseAdapter.KEY_OVERWRITE_SYSTEM)) == 0 ? false
				: true);

		rule.setPriority(cursor.getInt(cursor
				.getColumnIndex(DatabaseAdapter.KEY_PRIORITY)));
		rule.setSearchText(cursor.getString(cursor
				.getColumnIndex(DatabaseAdapter.KEY_SEARCHTEXT)));
		rule.setStartTime(cursor.getString(cursor
				.getColumnIndex(DatabaseAdapter.KEY_STARTTIME)));
		rule.setStopTime(cursor.getString(cursor
				.getColumnIndex(DatabaseAdapter.KEY_STOPTIME)));

		rule.setOpen(cursor.getInt(cursor
				.getColumnIndex(DatabaseAdapter.KEY_OPEN)) == 0 ? false : true);
		rule.setUnlock(cursor.getInt(cursor
				.getColumnIndex(DatabaseAdapter.KEY_UNLOCK)) == 0 ? false
				: true);
		return rule;
	}

	@Override
	protected String[] getProjection() {
		return DatabaseAdapter.RULE_PROJECTION;
	}

	public Boolean getOverwritesystem() {
		return overwritesystem;
	}

	public void setOverwritesystem(Boolean overwritesystem) {
		this.overwritesystem = overwritesystem;
	}

	public String getStartTime() {
		return startTime;
	}

	public void setStartTime(String startTime) {
		this.startTime = startTime;
	}

	public String getStopTime() {
		return stopTime;
	}

	public void setStopTime(String stopTime) {
		this.stopTime = stopTime;
	}

	public int getPriority() {
		return priority;
	}

	public void setPriority(int priority) {
		this.priority = priority;
	}

	public String getSearchText() {
		return searchText;
	}

	public void setSearchText(String searchText) {
		this.searchText = searchText;
	}

	@Override
	public String toString() {
		return String.format("%s (%s - %s)", this.getTitle(),
				this.getStartTime(), this.getStopTime());

	}

	public Boolean getUnlock() {
		return unlock;
	}

	public void setUnlock(Boolean unlock) {
		this.unlock = unlock;
	}


	public Boolean getOpen() {
		return open;
	}

	public void setOpen(Boolean open) {
		this.open = open;
	}
}
