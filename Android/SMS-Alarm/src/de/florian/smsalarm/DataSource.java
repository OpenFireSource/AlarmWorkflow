package de.florian.smsalarm;

import java.io.UnsupportedEncodingException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.ArrayList;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.SQLException;
import android.database.sqlite.SQLiteDatabase;

public class DataSource {

	// Database fields
	private SQLiteDatabase database;
	private MySQLiteHelper dbHelper;

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

	// Commentarea
	public void deleteReaction(int id) {

		System.out.println("Comment deleted with id: " + id);
		database.delete(MySQLiteHelper.TABLE_MAIN, MySQLiteHelper.COLUMN_ID
				+ " = " + id, null);
	}

	public void addReaction(String number, String content,
			 String name) {
		String[] Colums = { MySQLiteHelper.COLUMN_ID,
				 MySQLiteHelper.COLUMN_CONTENT,
				MySQLiteHelper.COLUMN_NAME, MySQLiteHelper.COLUMN_NUMBER,
				 MySQLiteHelper.COLUMN_MD5 };
		ContentValues values = new ContentValues();
		values.put(MySQLiteHelper.COLUMN_NUMBER, number);
		values.put(MySQLiteHelper.COLUMN_NAME, name);
		values.put(MySQLiteHelper.COLUMN_CONTENT, content);
		try {
			values.put(MySQLiteHelper.COLUMN_MD5, MD5(name + " " + number));
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

	public String[] getID(int number) {
		ArrayList<Integer> ids = new ArrayList<Integer>();
		Cursor c = database.query(MySQLiteHelper.TABLE_MAIN,
				new String[] { MySQLiteHelper.COLUMN_ID },
				MySQLiteHelper.COLUMN_NUMBER + "=" + number, null, null, null,
				null);

		while (c.moveToNext()) {
			ids.add(c.getInt(c.getColumnIndex(MySQLiteHelper.COLUMN_ID)));
		}
		String[] id = ids.toArray(new String[ids.size()]);
		return id;

	}

	/**
	 * 
	 * @param name
	 *            Die Kombination aus 'Name + " " Addresse'
	 * @return
	 */
	public String[] getID(String name) {
		ArrayList<Integer> ids = new ArrayList<Integer>();
		Cursor c = null;
		try {
			c = database.query(MySQLiteHelper.TABLE_MAIN,
					new String[] { MySQLiteHelper.COLUMN_ID },
					MySQLiteHelper.COLUMN_MD5 + "=" + MD5(name), null, null,
					null, null);
		} catch (NoSuchAlgorithmException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (UnsupportedEncodingException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		while (c.moveToNext()) {
			ids.add(c.getInt(c.getColumnIndex(MySQLiteHelper.COLUMN_ID)));
		}
		String[] id = ids.toArray(new String[ids.size()]);
		return id;

	}

	public void removeReaction(String id) {

	}

	public String getDetails(String id) {
		String data = "";
		Cursor c = database.query(MySQLiteHelper.TABLE_MAIN, new String[] {
				MySQLiteHelper.COLUMN_CONTENT,  MySQLiteHelper.COLUMN_NAME },
				MySQLiteHelper.COLUMN_ID + "=" + id, null, null, null, null);

		if (c.moveToFirst()) {			
			data = c.getString(c
					.getColumnIndex(MySQLiteHelper.COLUMN_CONTENT));
		}
		return data;

	}

	public String[] getReactionsList() {
		ArrayList<String> addresses = new ArrayList<String>();
		Cursor c = database.query(MySQLiteHelper.TABLE_MAIN, new String[] {
				MySQLiteHelper.COLUMN_NAME, MySQLiteHelper.COLUMN_NUMBER },
				null, null, null, null, null);
		while (c.moveToNext()) {
			addresses.add(c.getString(c
					.getColumnIndex(MySQLiteHelper.COLUMN_NAME))
					+ " "
					+ c.getString(c
							.getColumnIndex(MySQLiteHelper.COLUMN_NUMBER)));
		}
		String[] strings = addresses.toArray(new String[addresses.size()]);
		return strings;
	}

}