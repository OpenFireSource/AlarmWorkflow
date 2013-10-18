package com.alarmworkflow.eAlarm;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Set;

import com.alarmworkflow.eAlarm.database.NotificationRule;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.res.Configuration;
import android.os.Bundle;
import android.preference.CheckBoxPreference;
import android.preference.Preference;
import android.preference.PreferenceManager;
import android.preference.Preference.OnPreferenceChangeListener;
import android.preference.Preference.OnPreferenceClickListener;
import android.preference.PreferenceActivity;
import android.view.View;
import android.widget.Toast;

public class RuleEditor extends PreferenceActivity {
	private final RuleEditor thisActivity = this;
	private NotificationRule rule = null;
	private ORMPreferencesMapper preferenceMapper;

	static List<String> togglePreferences = new ArrayList<String>();
	{
		togglePreferences.add("rule_ringtone");
		togglePreferences.add("rule_toast");
		togglePreferences.add("rule_vibrate");
		togglePreferences.add("rule_ledflash");
		togglePreferences.add("rule_speakmessage");
		togglePreferences.add("rule_customringtone");
		togglePreferences.add("rule_overwritesystem");
	}

	// This is heavily inspired by ConnectBot's host editor:
	// https://github.com/kruton/connectbot/blob/master/src/org/connectbot/HostEditorActivity.java
	public class ORMPreferencesMapper implements SharedPreferences {
		protected List<OnSharedPreferenceChangeListener> listeners = new LinkedList<OnSharedPreferenceChangeListener>();

		public boolean contains(String key) {
			Map<String, ?> values = this.getAll();
			return values.containsKey(key);
		}

		public Editor edit() {
			return new Editor();
		}

		public Map<String, ?> getAll() {
			Map<String, Object> values = new HashMap<String, Object>();
			values.put("rule_local_enable", thisActivity.rule.getLocalEnabled());
			values.put("rule_ringtone", thisActivity.rule.getRingtone());
			values.put("rule_vibrate", thisActivity.rule.getVibrate());
			values.put("rule_ledflash", thisActivity.rule.getLedFlash());
			values.put("rule_speakmessage", thisActivity.rule.getSpeakMessage());
			values.put("rule_global",
					thisActivity.rule.getUseGlobalNotification());
			values.put("rule_overwritesystem",
					thisActivity.rule.getOverwritesystem());
			values.put("rule_toast", thisActivity.rule.getToast());
			values.put("rule_customringtone",
					thisActivity.rule.getCustomRingtone());
			values.put("rule_title", thisActivity.rule.getTitle());
			values.put("rule_stopTime", thisActivity.rule.getStopTime());
			values.put("rule_startTime", thisActivity.rule.getStartTime());
			values.put("rule_searchtext", thisActivity.rule.getSearchText());
			values.put("rule_prio", thisActivity.rule.getPriority());
			values.put("rule_open", thisActivity.rule.getOpen());
			values.put("rule_unlock", thisActivity.rule.getUnlock());
			return values;
		}

		public boolean getBoolean(String key, boolean defValue) {
			if (key.equals("rule_local_enable")) {
				return thisActivity.rule.getLocalEnabled();
			} else if (key.equals("rule_ringtone")) {
				return thisActivity.rule.getRingtone();
			} else if (key.equals("rule_vibrate")) {
				return thisActivity.rule.getVibrate();
			} else if (key.equals("rule_ledflash")) {
				return thisActivity.rule.getLedFlash();
			} else if (key.equals("rule_speakmessage")) {
				return thisActivity.rule.getSpeakMessage();
			} else if (key.equals("rule_global")) {
				return thisActivity.rule.getUseGlobalNotification();
			} else if (key.equals("rule_overwritesystem")) {
				return thisActivity.rule.getOverwritesystem();
			} else if (key.equals("rule_toast")) {
				return thisActivity.rule.getToast();
			} else if (key.equals("rule_open")) {
				return thisActivity.rule.getOpen();
			} else if (key.equals("rule_unlock")) {
				return thisActivity.rule.getUnlock();
			}

			return false;
		}

		public float getFloat(String key, float defValue) {
			// No floats available.
			return 0;
		}

		public int getInt(String key, int defValue) {
			if (key.equals("rule_prio")) {
				return thisActivity.rule.getPriority();
			}
			return 0;
		}

		public long getLong(String key, long defValue) {
			// No longs available.
			return 0;
		}

		public String getString(String key, String defValue) {
			// rule_customringtone
			// rule_title
			if (key.equals("rule_title")) {
				return thisActivity.rule.getTitle();
			} else if (key.equals("rule_customringtone")) {
				return thisActivity.rule.getCustomRingtone();
			} else if (key.equals("rule_stopTime")) {
				return thisActivity.rule.getStopTime();
			} else if (key.equals("rule_startTime")) {
				return thisActivity.rule.getStartTime();
			} else if (key.equals("rule_searchtext")) {
				return thisActivity.rule.getSearchText();
			} else if (key.equals("rule_prio")) {
				return thisActivity.rule.getPriority() + "";
			}

			return null;
		}

		public void registerOnSharedPreferenceChangeListener(
				OnSharedPreferenceChangeListener listener) {
			this.listeners.add(listener);
		}

		public void unregisterOnSharedPreferenceChangeListener(
				OnSharedPreferenceChangeListener listener) {
			this.listeners.remove(listener);
		}

		public class Editor implements SharedPreferences.Editor {

			public android.content.SharedPreferences.Editor clear() {
				// Not applicable for this object.
				return null;
			}

			public boolean commit() {
				thisActivity.rule.save(thisActivity);
				return true;
			}

			public android.content.SharedPreferences.Editor putBoolean(
					String key, boolean value) {
				if (key.equals("rule_local_enable")) {
					thisActivity.rule.setLocalEnabled(value);
				} else if (key.equals("rule_ringtone")) {
					thisActivity.rule.setRingtone(value);
				} else if (key.equals("rule_vibrate")) {
					thisActivity.rule.setVibrate(value);
				} else if (key.equals("rule_ledflash")) {
					thisActivity.rule.setLedFlash(value);
				} else if (key.equals("rule_speakmessage")) {
					thisActivity.rule.setSpeakMessage(value);
				} else if (key.equals("rule_global")) {
					thisActivity.rule.setUseGlobalNotification(value);
				} else if (key.equals("rule_overwritesystem")) {

					if (value == true) {
						if (!getBoolean("rule_ringtone", true)) {
							Toast.makeText(
									getApplicationContext(),
									getResources().getString(
											R.string.playsound_not_checked),
									Toast.LENGTH_LONG).show();
						}
						String customTone = getString("rule_customringtone", "");
						if (customTone.trim().length() <= 0) {
							Toast.makeText(
									getApplicationContext(),
									getResources().getString(
											R.string.no_sound_chosen),
									Toast.LENGTH_LONG).show();
						}
					}

					thisActivity.rule.setOverwritesystem(value);
				} else if (key.equals("rule_toast")) {
					thisActivity.rule.setToast(value);
				} else if (key.equals("rule_open")) {
					thisActivity.rule.setOpen(value);
				} else if (key.equals("rule_unlock")) {
					thisActivity.rule.setUnlock(value);
				}

				return this;
			}

			public android.content.SharedPreferences.Editor putFloat(
					String key, float value) {
				// No floats to set.
				return this;
			}

			public android.content.SharedPreferences.Editor putInt(String key,
					int value) {

				return this;
			}

			public android.content.SharedPreferences.Editor putLong(String key,
					long value) {
				// No longs to set.
				return this;
			}

			public android.content.SharedPreferences.Editor putString(
					String key, String value) {
				// NOTE: This is not transactionally safe as it modifies the
				// parent's object.
				// rule_customringtone
				// rule_title
				if (key.equals("rule_title")) {
					thisActivity.rule.setTitle(value);
				} else if (key.equals("rule_customringtone")) {
					thisActivity.rule.setCustomRingtone(value);
				} else if (key.equals("rule_stopTime")) {
					thisActivity.rule.setStopTime(value);
				} else if (key.equals("rule_startTime")) {
					thisActivity.rule.setStartTime(value);
				} else if (key.equals("rule_searchtext")) {
					thisActivity.rule.setSearchText(value);
				} else if (key.equals("rule_prio")) {
					thisActivity.rule.setPriority(Integer.parseInt(value));
				}
				return this;
			}

			public android.content.SharedPreferences.Editor remove(String key) {
				// Nothing to do here - this doesn't make sense.
				return this;
			}

			public void apply() {
				thisActivity.rule.save(thisActivity);
			}

			public android.content.SharedPreferences.Editor putStringSet(
					String arg0, Set<String> arg1) {
				// TODO Auto-generated method stub
				return null;
			}
		}

		public Set<String> getStringSet(String arg0, Set<String> arg1) {
			// TODO Auto-generated method stub
			return null;
		}
	}

	public SharedPreferences getSharedPreferences(String name, int mode) {
		return this.preferenceMapper;
	}

	/** Called when the activity is first created. */
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		this.getSource();
		this.preferenceMapper = new ORMPreferencesMapper();

		this.addPreferencesFromResource(R.xml.rule_preference_editor);

		// Find and attach the onclick handlers.
		Preference titlePreference = this.findPreference("rule_title");
		titlePreference.setOnPreferenceChangeListener(changeListener);
		Preference messagesPreference = this.findPreference("rule_messages");
		messagesPreference.setOnPreferenceClickListener(messagesClickHandler);
		Preference deletePreference = this.findPreference("rule_delete");
		deletePreference.setOnPreferenceClickListener(deleteClickHandler);
		Preference globalNotificationPreference = this
				.findPreference("rule_global");
		globalNotificationPreference
				.setOnPreferenceChangeListener(globalNofificationCheckListener);
		Preference start = findPreference("rule_startTime");
		start.setSummary(this.rule.getStartTime());
		Preference stop = findPreference("rule_stopTime");
		stop.setSummary(this.rule.getStopTime());
	}

	public void onConfigurationChanged(Configuration newConfig) {
		super.onConfigurationChanged(newConfig);
	}

	public void onResume() {
		super.onResume();

		// Reload our rule data.
		this.rule = null;
		// And finally, update the screen.
		this.loadFromSource(this.getSource());
	}

	/**
	 * Load this activity from the given rule.
	 * 
	 * @param rule
	 */
	public void loadFromSource(NotificationRule rule) {
		// Create a new preferences mapper with updated rule data.
		this.preferenceMapper = new ORMPreferencesMapper();

		Preference titlePreference = this.findPreference("rule_title");
		titlePreference.setSummary(rule.getTitle());

		// Force update the server enabled preference.
		CheckBoxPreference localEnabled = (CheckBoxPreference) this
				.findPreference("rule_local_enable");
		localEnabled.setChecked(rule.getLocalEnabled());
		this.toggleNotificationPreferences(rule.getUseGlobalNotification());
	}

	/**
	 * Save this rule.
	 */
	public void save(View view) {
		// User clicked save button.
		// Prepare the new local object.
		this.rule = null;
		NotificationRule rule = this.getSource();

		Preference titlePreference = this.findPreference("rule_title");
		rule.setTitle((String) titlePreference.getSummary());
		CheckBoxPreference localEnabled = (CheckBoxPreference) this
				.findPreference("enabled_locally");
		rule.setLocalEnabled(localEnabled.isChecked());
		SharedPreferences prefs = PreferenceManager
				.getDefaultSharedPreferences(getApplicationContext());
		String deviceKey = prefs.getString(getString(R.string.devicekey), "");

	}

	public void toggleNotificationPreferences(boolean globalOn) {
		for (String preferenceName : RuleEditor.togglePreferences) {
			Preference preference = this.findPreference(preferenceName);
			preference.setEnabled(!globalOn);
		}
	}

	/**
	 * Toggle the global notifications enabled.
	 */
	OnPreferenceChangeListener globalNofificationCheckListener = new OnPreferenceChangeListener() {
		public boolean onPreferenceChange(Preference preference, Object newValue) {
			thisActivity.toggleNotificationPreferences((Boolean) newValue);
			return true;
		}
	};

	/**
	 * Delete this rule.
	 */
	OnPreferenceClickListener deleteClickHandler = new OnPreferenceClickListener() {
		public boolean onPreferenceClick(Preference preference) {
			// User clicked delete button.
			// Confirm that's what they want.
			new AlertDialog.Builder(thisActivity)
					.setTitle(getString(R.string.create_rule))
					.setMessage(getString(R.string.delete_rule_message))
					.setPositiveButton(getString(R.string.delete),
							new DialogInterface.OnClickListener() {
								public void onClick(DialogInterface dialog,
										int whichButton) {
									// Fire it off to the delete rule
									// function.
									deleteSource(thisActivity.getSource());
								}
							})
					.setNegativeButton(getString(R.string.cancel),
							new DialogInterface.OnClickListener() {
								public void onClick(DialogInterface dialog,
										int whichButton) {
									// No need to take any action.
								}
							}).show();

			return true;
		}
	};

	/**
	 * Send a request to the backend to delete the rule.
	 * 
	 * @param rule
	 */
	public void deleteSource(NotificationRule rule) {
		rule.delete(thisActivity);
	}

	/**
	 * Send a request to the backend to test this rule.
	 * 
	 * @param view
	 */
	OnPreferenceClickListener testClickHandler = new OnPreferenceClickListener() {
		public boolean onPreferenceClick(Preference preference) {

			return true;
		}
	};
	/**
	 * A listener for changes that must occur immediately on the server.
	 */
	OnPreferenceChangeListener changeListener = new OnPreferenceChangeListener() {
		public boolean onPreferenceChange(Preference preference, Object newValue) {
			preference.setSummary((CharSequence) newValue);
			return true;
		}
	};
	/**
	 * View the messages of this rule.
	 */
	OnPreferenceClickListener messagesClickHandler = new OnPreferenceClickListener() {
		public boolean onPreferenceClick(Preference preference) {
			Intent intent = new Intent(thisActivity, MessageList.class);
			intent.putExtra("ruleId", thisActivity.getSource().getId());
			startActivity(intent);
			return true;
		}
	};

	/**
	 * Fetch the account that this rule list is for.
	 * 
	 * @return
	 */
	public NotificationRule getSource() {
		if (this.rule == null) {
			// Get the rule from the intent.
			// We store it in a private variable to save us having to query the
			// DB each time.
			Intent ruleIntent = getIntent();
			long id = ruleIntent.getLongExtra("ruleId", 0);
			this.rule = NotificationRule.FACTORY.get(this,
					ruleIntent.getLongExtra("ruleId", 0));
		}

		return this.rule;
	}
}
