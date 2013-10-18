package com.alarmworkflow.eAlarm.database;

import android.content.ContentProvider;
import android.content.ContentValues;
import android.content.Context;
import android.content.UriMatcher;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;
import android.net.Uri;

public class DatabaseAdapter extends ContentProvider {
	public static final String PROVIDER_NAME_RULES = "com.alarmworkflow.eAlarm.provider.Rules";
	public static final String PROVIDER_NAME_MESSAGES = "com.alarmworkflow.eAlarm.provider.Messages";

	public static final Uri CONTENT_URI_RULES = Uri.parse("content://"
			+ PROVIDER_NAME_RULES + "/rules");
	public static final Uri CONTENT_URI_MESSAGES = Uri.parse("content://"
			+ PROVIDER_NAME_MESSAGES + "/messages");

	private static final int RULES = 3;
	private static final int RULE_ID = 4;
	private static final int MESSAGES = 5;
	private static final int MESSAGE_ID = 6;

	private static final UriMatcher uriMatcher;
	static {
		uriMatcher = new UriMatcher(UriMatcher.NO_MATCH);
		uriMatcher.addURI(PROVIDER_NAME_RULES, "rules", RULES);
		uriMatcher.addURI(PROVIDER_NAME_RULES, "rules/#", RULE_ID);
		uriMatcher.addURI(PROVIDER_NAME_MESSAGES, "messages", MESSAGES);
		uriMatcher.addURI(PROVIDER_NAME_MESSAGES, "messages/#", MESSAGE_ID);
	}

	// private static final String TAG = "eAlarm";
	public static final String KEY_ID = "_id";
	public static final String KEY_ENABLED = "enabled";
	public static final String KEY_SERVER_REGISTRATION_ID = "server_registration_id";
	public static final String KEY_LOCAL_ENABLED = "local_enabled";
	public static final String KEY_LAST_GCM_ID = "last_gcm_id";
	public static final String KEY_TITLE = "title";
	public static final String KEY_RULE_ID = "rule_id";
	public static final String KEY_TIMESTAMP = "timestamp";
	public static final String KEY_MESSAGE = "message";
	public static final String KEY_SEEN = "seen";
	public static final String KEY_REQUIRES_SYNC = "requires_sync";
	public static final String KEY_USE_GLOBAL_NOTIFICATION = "use_global_notification";
	public static final String KEY_VIBRATE = "vibrate";
	public static final String KEY_TOAST = "toast";
	public static final String KEY_RINGTONE = "ringtone";
	public static final String KEY_CUSTOM_RINGTONE = "custom_ringtone";
	public static final String KEY_LED_FLASH = "led_flash";
	public static final String KEY_SPEAK_MESSAGE = "speak_message";
	public static final String KEY_OVERWRITE_SYSTEM = "overwritesystem";
	public static final String KEY_CONTENT = "content";
	public static final String KEY_STARTTIME = "startTime";
	public static final String KEY_STOPTIME = "stopTime";
	public static final String KEY_PRIORITY = "priority";
	public static final String KEY_SEARCHTEXT = "searchText";
	public static final String KEY_OPEN = "open";
	public static final String KEY_UNLOCK = "unlock";

	public static final String[] RULE_PROJECTION = new String[] { KEY_ID,
			KEY_STARTTIME, KEY_TITLE, KEY_STOPTIME, KEY_PRIORITY,
			KEY_SEARCHTEXT, KEY_LOCAL_ENABLED, KEY_USE_GLOBAL_NOTIFICATION,
			KEY_VIBRATE, KEY_TOAST, KEY_RINGTONE, KEY_CUSTOM_RINGTONE,
			KEY_LED_FLASH, KEY_SPEAK_MESSAGE, KEY_OVERWRITE_SYSTEM, KEY_UNLOCK,
			 KEY_OPEN };

	public static final String[] MESSAGE_PROJECTION = new String[] { KEY_ID,
			KEY_RULE_ID, KEY_TIMESTAMP, KEY_TITLE, KEY_MESSAGE, KEY_CONTENT,
			KEY_SEEN };

	private SQLiteDatabase db;

	private static final String DATABASE_CREATE_RULES = "create table RULES (_id integer primary key autoincrement, "
			+ "title text not null, "
			+ "local_enabled integer not null, "
			+ "use_global_notification integer not null, "
			+ "vibrate integer not null, "
			+ "toast integer not null, "
			+ "ringtone integer not null, "
			+ "custom_ringtone text not null, "
			+ "led_flash integer not null, "
			+ "overwritesystem integer not null,"
			+ "searchText text not null,"
			+ "startTime text not null,"
			+ "stopTime text not null,"
			+ "priority integer not null,"
			+ "speak_message integer not null, "
			+ "open integer not null,"
			+ "unlock integer not null" + ");";

	private static final String DATABASE_CREATE_MESSAGES = "create table messages (_id integer primary key autoincrement, "
			+ "rule_id integer not null, "
			+ "timestamp text not null, "
			+ "title text not null, "
			+ "message text not null, "
			+ "content text not null," + "seen integer not null " + ");";

	public static final String DATABASE_NAME = "eAlarm.db";

	private static final int DATABASE_VERSION = 7;

	/**
	 * Database helper class to create and manage the schema.
	 */
	private static class DatabaseHelper extends SQLiteOpenHelper {
		DatabaseHelper(Context context) {
			super(context, DATABASE_NAME, null, DATABASE_VERSION);
		}

		@Override
		public void onCreate(SQLiteDatabase db) {
			db.execSQL(DATABASE_CREATE_RULES);
			db.execSQL(DATABASE_CREATE_MESSAGES);
		}

		@Override
		public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
			// Upgrading from v1 (during BETA) to v3.
			if (oldVersion < 7) {
				db.execSQL("ALTER TABLE RULES ADD open integer not null DEFAULT(0)");
				db.execSQL("ALTER TABLE RULES ADD unlock integer not null DEFAULT(0)");
			} else {
				db.execSQL("DROP TABLE RULES;");
				db.execSQL("DROP TABLE messages;");
				db.execSQL(DATABASE_CREATE_RULES);
				db.execSQL(DATABASE_CREATE_MESSAGES);
			}

		}
	}

	@Override
	public String getType(Uri uri) {
		switch (uriMatcher.match(uri)) {
		case RULES:
			return "vnd.android.cursor.dir/vnd.eAlarm.RULES";
			// Get a single account.
		case RULE_ID:
			return "vnd.android.cursor.item/vnd.eAlarm.RULES";
			// Get all accounts.
		case MESSAGES:
			return "vnd.android.cursor.dir/vnd.eAlarm.messages";
			// Get a single account.
		case MESSAGE_ID:
			return "vnd.android.cursor.item/vnd.eAlarm.messages";
		default:
			throw new IllegalArgumentException("Unsupported URI: " + uri);
		}
	}

	@Override
	public boolean onCreate() {
		Context context = getContext();
		DatabaseHelper dbHelper = new DatabaseHelper(context);
		db = dbHelper.getWritableDatabase();
		return (db == null) ? false : true;
	}

	@Override
	public Cursor query(Uri uri, String[] projection, String selection,
			String[] selectionArgs, String sortOrder) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public Uri insert(Uri uri, ContentValues values) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public int delete(Uri uri, String selection, String[] selectionArgs) {
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	public int update(Uri uri, ContentValues values, String selection,
			String[] selectionArgs) {
		// TODO Auto-generated method stub
		return 0;
	}

}