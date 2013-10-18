package com.alarmworkflow.eAlarm.database;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.URLDecoder;
import java.security.GeneralSecurityException;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.KeyException;
import java.security.NoSuchAlgorithmException;
import java.text.DateFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.Locale;
import java.util.Set;
import java.util.TimeZone;

import javax.crypto.BadPaddingException;
import javax.crypto.Cipher;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.NoSuchPaddingException;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;

import org.json.JSONException;
import org.json.JSONObject;

import com.alarmworkflow.eAlarm.R;
import com.alarmworkflow.eAlarm.general.eAlarm;

import android.content.ContentValues;
import android.content.Context;
import android.content.SharedPreferences;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.net.Uri;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.util.Base64;


public class NotificationMessage extends DatabaseObject<NotificationMessage> {
	public final static NotificationMessage FACTORY = new NotificationMessage();

	// private static final String TAG = "eAlarm";
	private NotificationRule rule;
	private String title;
	private String timestamp;
	private String message;
	private JSONObject content;
	private Boolean seen;
	private final static String characterEncoding = "UTF-8";
	private final static String cipherTransformation = "AES/CBC/PKCS5Padding";
	private final static String aesEncryptionAlgorithm = "AES";

	public NotificationRule getRule() {
		return rule;
	}

	public void setRule(NotificationRule rule) {
		this.rule = rule;
	}

	public String getTitle() {
		return title;
	}

	public void setTitle(String title) {
		this.title = title;
	}

	public String getTimestamp() {
		return timestamp;
	}

	public void setTimestamp(String timestamp) {
		this.timestamp = timestamp;
	}

	public static Date parseISO8601String(String isoString)
			throws ParseException {
		SimpleDateFormat ISO8601DATEFORMAT = new SimpleDateFormat(
				"yyyy-MM-dd'T'HH:mm:ss", Locale.GERMAN);
		ISO8601DATEFORMAT.setTimeZone(TimeZone.getTimeZone("UTC"));
		return ISO8601DATEFORMAT.parse(isoString);
	}

	public static String formatUTCAsLocal(Date date) {
		DateFormat formatter = DateFormat.getDateTimeInstance();
		formatter.setTimeZone(TimeZone.getDefault());
		return formatter.format(date);
	}

	public String getDisplayTimestamp() {
		try {
			return NotificationMessage.formatUTCAsLocal(NotificationMessage
					.parseISO8601String(this.timestamp));
		} catch (ParseException e) {
			return "Parse error";
		}
	}

	public String getMessage() {
		return message;
	}

	public void setMessage(String message) {
		this.message = message;
	}

	public Boolean getSeen() {
		return seen;
	}

	public void setSeen(Boolean seen) {
		this.seen = seen;
	}

	public static String decode(String input) {
		if (input != null) {
			byte[] result = Base64.decode(input, Base64.DEFAULT);
			return new String(result);
		} else {
			return null;
		}
	}

	public static NotificationMessage[] fromGCM(Context context, Bundle extras)
			throws Exception {
		NotificationMessage incoming = new NotificationMessage();
		Set<String> keys = extras.keySet();
		JSONObject content = new JSONObject();
		ArrayList<NotificationMessage> messages = new ArrayList<NotificationMessage>();
		SharedPreferences prefs = PreferenceManager
				.getDefaultSharedPreferences(eAlarm.context);
		boolean encryption = prefs.getBoolean(
				eAlarm.context.getString(R.string.encryption), false);
		String encryptionKey = prefs.getString(
				eAlarm.context.getString(R.string.encryption_key), "");
		for (String string : keys) {
			if (string.equals("awf_message")) {
				String message = URLDecoder.decode(extras.getString(string),
						"UTF-8");
				if (encryption) {
					message = decrypt(message, encryptionKey);
				}
				incoming.setMessage(message);
			} else if (string.equals("awf_title")) {
				String title = URLDecoder.decode(extras.getString(string),
						"UTF-8");
				if (encryption) {
					title = decrypt(title, encryptionKey);
				}
				incoming.setTitle(title);
			} else {
				try {
					String value = URLDecoder.decode(extras.getString(string),
							"UTF-8");
					if (string.startsWith("awf_")) {
						if (encryption) {
							value = decrypt(value, encryptionKey);
						}
					}
					content.put(string, value);
				} catch (JSONException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			}
		}
		incoming.setContent(content);
		incoming.setSeen(false);
		SimpleDateFormat dateFormat = new SimpleDateFormat(
				"yyyy-MM-dd'T'HH:mm:ss.SSSSSS", Locale.GERMAN);
		dateFormat.setTimeZone(TimeZone.getTimeZone("UTC"));
		Date d = new Date();
		String timeStamp = dateFormat.format(d);
		ArrayList<NotificationRule> rules = NotificationRule.FACTORY
				.listAll(context);
		ArrayList<NotificationRule> possibleRules = new ArrayList<NotificationRule>();
		for (NotificationRule rule : rules) {
			Calendar now = Calendar.getInstance();
			Calendar startValue = Calendar.getInstance();
			Calendar stopValue = Calendar.getInstance();
			SimpleDateFormat ho = new SimpleDateFormat("HH:mm", Locale.GERMAN);
			try {
				Date start = ho.parse(rule.getStartTime());
				Date stop = ho.parse(rule.getStopTime());

				startValue.set(Calendar.HOUR_OF_DAY, start.getHours());
				startValue.set(Calendar.MINUTE, start.getMinutes());
				stopValue.set(Calendar.HOUR_OF_DAY, stop.getHours());
				stopValue.set(Calendar.MINUTE, stop.getMinutes());
				if (start.after(stop) && start.after(now.getTime())) {
					startValue.add(Calendar.DATE, -1);
				} else if (start.after(stop) && start.before(now.getTime())) {
					stopValue.add(Calendar.DATE, +1);
				}
			} catch (ParseException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			if ("".matches(rule.getSearchText())
					|| incoming.getTitle().matches(rule.getSearchText())
					|| incoming.getMessage().matches(rule.getSearchText())) {
				if (Calendar.getInstance().after(startValue)
						&& Calendar.getInstance().before(stopValue)
						|| startValue.getTime() == Calendar.getInstance()
								.getTime()
						|| stopValue.getTime() == Calendar.getInstance()
								.getTime()) {
					possibleRules.add(rule);
				}
			}
		}
		if (possibleRules.size() == 0) {
			throw new Exception();
		}
		for (NotificationRule rule : possibleRules) {
			NotificationMessage message = new NotificationMessage();
			message.setContent(incoming.getContent());
			message.setRule(rule);
			message.setSeen(false);
			message.setMessage(incoming.getMessage());
			message.setTitle(incoming.getTitle());
			message.setTimestamp(timeStamp);
			messages.add(message);
		}
		return (NotificationMessage[]) messages
				.toArray(new NotificationMessage[possibleRules.size()]);
	}

	public ArrayList<NotificationMessage> list(Context context,
			NotificationRule rule) {
		String query = "";
		if (rule != null) {
			query = DatabaseAdapter.KEY_RULE_ID + "=" + rule.getId();
		}

		return this.genericList(context, query, null,
				DatabaseAdapter.KEY_TIMESTAMP + " DESC");
	}

	private String getTableFor(Uri uri) {
		switch (uriMatcher.match(uri)) {
		case RULES:
		case RULES_ID:
			return DATABASE_TABLE_RULES;
		case MESSAGES:
		case MESSAGE_ID:
			return DATABASE_TABLE_MESSAGES;
		default:
			throw new IllegalArgumentException("URI: " + uri);
		}
	}

	public Cursor cursorList(Context context, NotificationRule rule) {
		String table = this.getTableFor(this.getContentUri());
		String[] proj = new String[] { DatabaseAdapter.KEY_ID,
				DatabaseAdapter.KEY_TITLE, DatabaseAdapter.KEY_MESSAGE,
				DatabaseAdapter.KEY_TIMESTAMP, DatabaseAdapter.KEY_SEEN };
		String fields = "";
		for (int i = 0; i < proj.length; i++) {
			fields += proj[i];
			if (i < proj.length - 1)
				fields += ",";
		}

		String query = "select " + fields + " from " + table;

		if (rule != null) {
			query += " where " + DatabaseAdapter.KEY_RULE_ID + "="
					+ rule.getId();
		}

		query += " order by " + DatabaseAdapter.KEY_TIMESTAMP + " DESC";

		SQLiteDatabase db = context
				.openOrCreateDatabase(DATABASE_NAME, 0, null);
		Cursor cursor = db.rawQuery(query, null);
		return cursor;
	}

	public int countUnread(Context context, NotificationRule rule) {
		String query = DatabaseAdapter.KEY_SEEN + " = 0 ";
		if (rule != null) {
			query += " AND " + DatabaseAdapter.KEY_RULE_ID + "=" + rule.getId();
		}
		return this.genericCount(context, query, null);
	}

	public NotificationMessage getUnread(Context context, NotificationRule rule) {
		NotificationMessage message = null;
		String query = "";
		query = DatabaseAdapter.KEY_RULE_ID + "=" + rule.getId() + " AND "
				+ DatabaseAdapter.KEY_SEEN + " = 0 ";

		ArrayList<NotificationMessage> messages = this.genericList(context,
				query, null, null);
		if (messages.size() == 1) {
			message = messages.get(0);
		}
		return message;
	}

	public void markAllAsSeen(Context context, NotificationRule rule) {
		ContentValues values = new ContentValues();
		values.put(DatabaseAdapter.KEY_SEEN, 1);

		String whereclause = null;
		if (rule != null) {
			whereclause = DatabaseAdapter.KEY_RULE_ID + " = " + rule.getId();
		}
		SQLiteDatabase db = context
				.openOrCreateDatabase(DATABASE_NAME, 0, null);
		db.update(this.getTableFor(this.getContentUri()), values, whereclause,
				null);
	}

	public void deleteMessagesByRule(Context context, NotificationRule rule,
			boolean onlyRead) {
		String query = null;
		if (rule != null) {
			query = DatabaseAdapter.KEY_RULE_ID + "=" + rule.getId();
		}
		if (onlyRead) {
			if (query != null) {
				query += " AND ";
			} else {
				query = "";
			}

			query += DatabaseAdapter.KEY_SEEN + "= 1";
		}

		this.genericDelete(context, query, null);
	}

	public void deleteOlderThan(Context context, Date date) {
		// Parse it, and display in LOCAL timezone.
		SimpleDateFormat ISO8601DATEFORMAT = new SimpleDateFormat(
				"yyyy-MM-dd'T'HH:mm:ss.SSSSSS", Locale.GERMAN);
		ISO8601DATEFORMAT.setTimeZone(TimeZone.getTimeZone("UTC"));
		String formattedDate = ISO8601DATEFORMAT.format(date);

		// So, everything older than formattedDate should be removed.
		this.genericDelete(context, DatabaseAdapter.KEY_TIMESTAMP + " < ?",
				new String[] { formattedDate });
	}

	public void deleteById(Context context, int id) {
		String whereclause = "_id = ?";
		String ident = String.valueOf(id);
		String[] whereclauseArgs = new String[1];
		whereclauseArgs[0] = ident;

		SQLiteDatabase db = context
				.openOrCreateDatabase(DATABASE_NAME, 0, null);
		db.delete(this.getTableFor(this.getContentUri()), whereclause,
				whereclauseArgs);
	}

	@Override
	public Uri getContentUri() {
		return DatabaseAdapter.CONTENT_URI_MESSAGES;
	}

	@Override
	protected ContentValues flatten() {
		ContentValues values = new ContentValues();
		values.put(DatabaseAdapter.KEY_TITLE, this.getTitle());
		values.put(DatabaseAdapter.KEY_RULE_ID, this.getRule().getId());
		values.put(DatabaseAdapter.KEY_MESSAGE, this.getMessage());
		values.put(DatabaseAdapter.KEY_TIMESTAMP, this.getTimestamp());
		values.put(DatabaseAdapter.KEY_SEEN, this.getSeen() ? 1 : 0);
		values.put(DatabaseAdapter.KEY_CONTENT, this.content.toString());
		return values;
	}

	@Override
	protected NotificationMessage inflate(Context context, Cursor cursor) {
		NotificationMessage message = new NotificationMessage();
		message.setId(cursor.getLong(cursor
				.getColumnIndex(DatabaseAdapter.KEY_ID)));
		message.setTitle(cursor.getString(cursor
				.getColumnIndex(DatabaseAdapter.KEY_TITLE)));
		message.setMessage(cursor.getString(cursor
				.getColumnIndex(DatabaseAdapter.KEY_MESSAGE)));
		message.setRule(NotificationRule.FACTORY.get(context, cursor
				.getLong(cursor.getColumnIndex(DatabaseAdapter.KEY_RULE_ID))));
		message.setSeen(cursor.getLong(cursor
				.getColumnIndex(DatabaseAdapter.KEY_SEEN)) == 0 ? false : true);
		message.setTimestamp(cursor.getString(cursor
				.getColumnIndex(DatabaseAdapter.KEY_TIMESTAMP)));
		try {
			message.setContent(new JSONObject(cursor.getString(cursor
					.getColumnIndex(DatabaseAdapter.KEY_CONTENT))));
		} catch (JSONException e) {
			message.setContent(new JSONObject());
		}
		return message;
	}

	@Override
	protected String[] getProjection() {
		return DatabaseAdapter.MESSAGE_PROJECTION;
	}

	public JSONObject getContent() {
		return content;
	}

	public void setContent(JSONObject content) {
		this.content = content;
	}

	public class UnruleableMessage extends Exception {
		private static final long serialVersionUID = 1L;
	}

	public static byte[] decrypt(byte[] cipherText, byte[] key,
			byte[] initialVector) throws NoSuchAlgorithmException,
			NoSuchPaddingException, InvalidKeyException,
			InvalidAlgorithmParameterException, IllegalBlockSizeException,
			BadPaddingException {
		Cipher cipher = Cipher.getInstance(cipherTransformation);
		SecretKeySpec secretKeySpecy = new SecretKeySpec(key,
				aesEncryptionAlgorithm);
		IvParameterSpec ivParameterSpec = new IvParameterSpec(initialVector);
		cipher.init(Cipher.DECRYPT_MODE, secretKeySpecy, ivParameterSpec);
		cipherText = cipher.doFinal(cipherText);
		return cipherText;
	}

	private static byte[] getKeyBytes(String key)
			throws UnsupportedEncodingException {
		byte[] keyBytes = new byte[16];
		byte[] parameterKeyBytes = key.getBytes(characterEncoding);
		System.arraycopy(parameterKeyBytes, 0, keyBytes, 0,
				Math.min(parameterKeyBytes.length, keyBytes.length));
		return keyBytes;
	}

	// / <summary>
	// / Decrypts a base64 encoded string using the given key (AES 128bit key
	// and a Chain Block Cipher)
	// / </summary>
	// / <param name="encryptedText">Base64 Encoded String</param>
	// / <param name="key">Secret Key</param>
	// / <returns>Decrypted String</returns>
	public static String decrypt(String encryptedText, String key)
			throws KeyException, GeneralSecurityException,
			GeneralSecurityException, InvalidAlgorithmParameterException,
			IllegalBlockSizeException, BadPaddingException, IOException {
		byte[] cipheredBytes = Base64.decode(encryptedText, Base64.DEFAULT);
		byte[] keyBytes = getKeyBytes(key);
		return new String(decrypt(cipheredBytes, keyBytes, keyBytes),
				characterEncoding);
	}
}
