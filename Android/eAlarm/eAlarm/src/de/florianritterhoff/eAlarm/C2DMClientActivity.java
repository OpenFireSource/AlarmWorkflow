package de.florianritterhoff.eAlarm;

import android.accounts.Account;
import android.accounts.AccountManager;
import android.app.Activity;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.util.Log;
import android.view.View;
import android.view.Window;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Spinner;
import android.widget.Toast;
import de.florianritterhoff.eAlarm.R;

public class C2DMClientActivity extends Activity {

	public final static String AUTH = "authentication";
	public final static String EMAIL = "email";
	public final static String EINSATZ = "einsatz";
	private String email = "";
	private String auth = "";
	private SharedPreferences prefs;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		requestWindowFeature(Window.FEATURE_NO_TITLE);
		setContentView(R.layout.main);
		start();

	}

	void start() {
		prefs = PreferenceManager.getDefaultSharedPreferences(this);
		auth = prefs.getString(AUTH, "n/a");
		Spinner acc = (Spinner) findViewById(R.id.account);

		AccountManager am = AccountManager.get(this);
		Account[] accounts = am.getAccountsByType("com.google");

		if (accounts.length < 1) {
			Toast.makeText(this, "Keine passenden Google Accounts gefunden",
					Toast.LENGTH_LONG).show();
		} else {
			String[] items = new String[accounts.length];
			for (int i = 0; i < accounts.length; i++) {
				items[i] = accounts[i].name;
			}
			ArrayAdapter<Object> aa = new ArrayAdapter<Object>(this,
					android.R.layout.simple_spinner_item, items);
			aa.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
			acc.setAdapter(aa);
			acc.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
				public void onItemSelected(AdapterView<?> parent, View view,
						int pos, long id) {
					email = parent.getItemAtPosition(pos).toString();
					Editor edit = prefs.edit();
					if (prefs.contains(EMAIL)) {

						edit.remove(EMAIL);

					}
					edit.putString(EMAIL, email);
					edit.commit();
				}

				public void onNothingSelected(AdapterView<?> parent) {
				}
			});

		}

	}

	@Override
	public void onBackPressed() {
		if (auth != "n/a") {
			super.onBackPressed();									
		} else {
			
		}
	}

	public void register(View view) {
		if (auth == "n/a") {
			Log.w("C2DM", "start registration process");
			Intent registrationIntent = new Intent(
					"com.google.android.c2dm.intent.REGISTER");
			// sets the app name in the intent
			registrationIntent.putExtra("app",
					PendingIntent.getBroadcast(this, 0, new Intent(), 0));
			registrationIntent.putExtra("sender", "885567138585");
			startService(registrationIntent);

			new Thread(new Runnable() {
				public synchronized void run() {
					for (int i = 0; i < 10; i++) {
						try {
							Thread.sleep(2000);
							auth = prefs.getString(AUTH, "n/a");

						} catch (InterruptedException e) {
							e.printStackTrace();
						}
						if (auth != "n/a") {

							C2DMClientActivity.this
									.runOnUiThread(new Runnable() {
										public void run() {
											Intent i = getBaseContext()
													.getPackageManager()
													.getLaunchIntentForPackage(
															getBaseContext()
																	.getPackageName());
											i.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
											startActivity(i);
										}
									});
							break;
						}
					}

				}
			}).start();
		}

	}	

	Context getContext() {
		return this;
	}

	/**
	 * @return the email
	 */
	public String getEmail() {
		return email;
	}

	/**
	 * @param email
	 *            the email to set
	 */
	public void setEmail(String email) {
		this.email = email;
	}

}
