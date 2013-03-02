package com.alarmworkflow.eAlarmApp.datastorage;

import android.content.Context;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;
import android.util.Log;

public class MySQLiteHelper extends SQLiteOpenHelper {
	public static final String TABLE_MAIN = "main";

	public static final String COLUMN_ID = "ID";
	public static final String COLUMN_TEXT = "text";
	public static final String COLUMN_LONG = "long";
	public static final String COLUMN_LAT = "lat";
	public static final String COLUMN_HEADER = "header";
	public static final String COLUMN_MD5 = "md5";
	public static final String COLUMN_TIMESTAMP = "time";
	private static final String DATABASE_NAME = "data.db";
	private static final int DATABASE_VERSION = 13;

	// Database creation sql statement
	private static final String DATABASE_CREATE = "create table " + TABLE_MAIN
			+ "( " + COLUMN_ID + " integer primary key autoincrement, "
			+ COLUMN_LONG + " text not null," + COLUMN_LAT + " text not null,"
			+ COLUMN_TEXT + " text not null," + COLUMN_HEADER
			+ " text not null," + COLUMN_TIMESTAMP + " text not null,"
			 + COLUMN_MD5 + " text not null);";

	public MySQLiteHelper(Context context) {
		super(context, DATABASE_NAME, null, DATABASE_VERSION);
	}

	@Override
	public void onCreate(SQLiteDatabase database) {
		database.execSQL(DATABASE_CREATE);
	}

	@Override
	public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
		Log.w(MySQLiteHelper.class.getName(),
				"Upgrading database from version " + oldVersion + " to "
						+ newVersion + ", which will destroy all old data");
		db.execSQL("DROP TABLE IF EXISTS " + TABLE_MAIN);
		onCreate(db);
	}

}
