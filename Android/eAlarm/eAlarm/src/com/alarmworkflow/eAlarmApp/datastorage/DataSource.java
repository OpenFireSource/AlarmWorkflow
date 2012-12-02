package com.alarmworkflow.eAlarmApp.datastorage;

import java.io.UnsupportedEncodingException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.SQLException;
import android.database.sqlite.SQLiteDatabase;

public class DataSource {

	private static DataSource instance;
	// Database fields
	private SQLiteDatabase database;
	private MySQLiteHelper dbHelper;
	private static String PATTERN = "EEEE', 'dd. MMMM yyyy 'um' HH:mm:ss:SSS";

	public static DataSource getInstance(Context context) {

		if (instance == null) {
			instance = new DataSource(context);
			instance.open();
		}

		return instance;
	}

	public DataSource(Context context) {
		dbHelper = new MySQLiteHelper(context);
		dbHelper.close();
	}

	public void open() throws SQLException {
		database = dbHelper.getWritableDatabase();
	}

	public void close() {
		dbHelper.close();
	}

	public void deleteReaction(int id) {
		System.out.println("Comment deleted with id: " + id);
		database.delete(MySQLiteHelper.TABLE_MAIN, MySQLiteHelper.COLUMN_ID
				+ " = " + id, null);
	}

	public void clearList() {
		database.delete(MySQLiteHelper.TABLE_MAIN, null, null);
	}

	public void addOperation(String header, String text, String longitude,
			String latitude, String timestamp, String opID) {
		String[] Colums = { MySQLiteHelper.COLUMN_ID,
				MySQLiteHelper.COLUMN_LAT, MySQLiteHelper.COLUMN_LONG,
				MySQLiteHelper.COMLUM_TEXT, MySQLiteHelper.COLUMN_HEADER,
				MySQLiteHelper.COLUMN_MD5, MySQLiteHelper.COLUMN_TIMESTAMP,
				MySQLiteHelper.COLUMN_OPID, MySQLiteHelper.COLUMN_FEEDBACK };
		ContentValues values = new ContentValues();
		values.put(MySQLiteHelper.COMLUM_TEXT, text);
		values.put(MySQLiteHelper.COLUMN_LAT, longitude);
		values.put(MySQLiteHelper.COLUMN_LONG, latitude);
		values.put(MySQLiteHelper.COLUMN_HEADER, header);
		values.put(MySQLiteHelper.COLUMN_TIMESTAMP, timestamp);
		values.put(MySQLiteHelper.COLUMN_FEEDBACK, "false");
		values.put(MySQLiteHelper.COLUMN_OPID, opID);
		try {
			values.put(MySQLiteHelper.COLUMN_MD5, MD5(text + " " + header + " "
					+ timestamp));
		} catch (NoSuchAlgorithmException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (UnsupportedEncodingException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		long insertId = database
				.insert(MySQLiteHelper.TABLE_MAIN, null, values);
		Cursor cursor = database.query(MySQLiteHelper.TABLE_MAIN, Colums,
				MySQLiteHelper.COLUMN_ID + " = " + insertId, null, null, null,
				null);
		cursor.moveToFirst();
		cursor.close();

	}

	private static String convertToHex(byte[] data) {
		StringBuffer buf = new StringBuffer();
		for (int i = 0; i < data.length; i++) {
			int halfbyte = (data[i] >>> 4) & 0x0F;
			int two_halfs = 0;
			do {
				if ((0 <= halfbyte) && (halfbyte <= 9))
					buf.append((char) ('0' + halfbyte));
				else
					buf.append((char) ('a' + (halfbyte - 10)));
				halfbyte = data[i] & 0x0F;
			} while (two_halfs++ < 1);
		}
		return buf.toString();
	}

	public static String MD5(String text) throws NoSuchAlgorithmException,
			UnsupportedEncodingException {
		MessageDigest md;
		md = MessageDigest.getInstance("MD5");
		byte[] md5hash = new byte[32];
		md.update(text.getBytes("iso-8859-1"), 0, text.length());
		md5hash = md.digest();
		return convertToHex(md5hash);
	}

	public String getID(String timestamp) {
		String time = timestamp;
		if (timestamp.contains(" ")) {
			SimpleDateFormat sdf = new SimpleDateFormat();
			sdf.applyPattern(PATTERN);

			try {
				time = sdf.parse(timestamp).getTime() + "";
			} catch (ParseException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
		Cursor c = null;
		c = database.query(MySQLiteHelper.TABLE_MAIN,
				new String[] { MySQLiteHelper.COLUMN_ID },
				MySQLiteHelper.COLUMN_TIMESTAMP + "=" + time, null, null, null,
				null);

		while (c.moveToFirst()) {
			return c.getString(c.getColumnIndex(MySQLiteHelper.COLUMN_ID));
		}
		return "";

	}

	public Map<String, String> getDetails(String id) {
		Map<String, String> data = new HashMap<String, String>();
		Cursor c = database.query(MySQLiteHelper.TABLE_MAIN, new String[] {
				MySQLiteHelper.COLUMN_TIMESTAMP, MySQLiteHelper.COLUMN_HEADER,
				MySQLiteHelper.COLUMN_LAT, MySQLiteHelper.COLUMN_LONG,
				MySQLiteHelper.COMLUM_TEXT, MySQLiteHelper.COLUMN_TIMESTAMP,
				MySQLiteHelper.COLUMN_OPID, MySQLiteHelper.COLUMN_FEEDBACK },
				MySQLiteHelper.COLUMN_ID + "=" + id, null, null, null, null);

		if (c.moveToFirst()) {
			data.put(MySQLiteHelper.COLUMN_TIMESTAMP, c.getString(c
					.getColumnIndex(MySQLiteHelper.COLUMN_TIMESTAMP)));
			data.put(MySQLiteHelper.COLUMN_HEADER,
					c.getString(c.getColumnIndex(MySQLiteHelper.COLUMN_HEADER)));
			data.put(MySQLiteHelper.COLUMN_LONG,
					c.getString(c.getColumnIndex(MySQLiteHelper.COLUMN_LONG)));
			data.put(MySQLiteHelper.COLUMN_LAT,
					c.getString(c.getColumnIndex(MySQLiteHelper.COLUMN_LAT)));
			data.put(MySQLiteHelper.COMLUM_TEXT,
					c.getString(c.getColumnIndex(MySQLiteHelper.COMLUM_TEXT)));
			data.put(MySQLiteHelper.COLUMN_OPID,
					c.getString(c.getColumnIndex(MySQLiteHelper.COLUMN_OPID)));
			data.put(MySQLiteHelper.COLUMN_FEEDBACK, c.getString(c
					.getColumnIndex(MySQLiteHelper.COLUMN_FEEDBACK)));
			String time = c.getString(c
					.getColumnIndex(MySQLiteHelper.COLUMN_TIMESTAMP));
			Date date = new Date(Long.parseLong(time));
			SimpleDateFormat sdf = new SimpleDateFormat();
			sdf.applyPattern(PATTERN);
			time = sdf.format(date);
			data.put(MySQLiteHelper.COLUMN_TIMESTAMP, time);
		}
		return data;

	}

	public void setFeedback(Boolean setting, String id) {
		ContentValues args = new ContentValues();
		String value = "false";
		if (setting) {
			value = "true";
		}
		args.put(MySQLiteHelper.COLUMN_FEEDBACK, value);
		database.update(MySQLiteHelper.TABLE_MAIN, args,
				MySQLiteHelper.COLUMN_ID + "=" + id, null);
	}

	public List<Map<String, String>> getOperations() {
		List<Map<String, String>> array = new ArrayList<Map<String, String>>();

		Cursor c = database.query(MySQLiteHelper.TABLE_MAIN,
				new String[] { MySQLiteHelper.COLUMN_HEADER,
						MySQLiteHelper.COLUMN_TIMESTAMP }, null, null, null,
				null, MySQLiteHelper.COLUMN_TIMESTAMP + " DESC");
		while (c.moveToNext()) {
			String header = c.getString(c
					.getColumnIndex(MySQLiteHelper.COLUMN_HEADER));
			String time = c.getString(c
					.getColumnIndex(MySQLiteHelper.COLUMN_TIMESTAMP));
			Date date = new Date(Long.parseLong(time));
			SimpleDateFormat sdf = new SimpleDateFormat();
			sdf.applyPattern(PATTERN);
			time = sdf.format(date);
			array.add(new HashMap<String, String>());
			array.get(array.size() - 1).put(MySQLiteHelper.COLUMN_HEADER,
					header);
			array.get(array.size() - 1).put(MySQLiteHelper.COLUMN_TIMESTAMP,
					time);
		}

		return array;
	}

}