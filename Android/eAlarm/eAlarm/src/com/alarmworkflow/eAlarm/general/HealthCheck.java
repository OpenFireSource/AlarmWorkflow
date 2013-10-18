
package com.alarmworkflow.eAlarm.general;

import java.util.ArrayList;
import java.util.List;

import android.content.Context;
import android.content.SharedPreferences;
import android.preference.PreferenceManager;

import com.alarmworkflow.eAlarm.R;
import com.alarmworkflow.eAlarm.database.NotificationRule;
import com.google.android.gcm.GCMRegistrar;

public class HealthCheck {
	public ArrayList<String> errors = new ArrayList<String>();
	public ArrayList<String> warnings = new ArrayList<String>();

	protected void addError(String error) {
		this.errors.add(error);
	}

	protected void addWarning(String warning) {
		this.warnings.add(warning);
	}

	public ArrayList<String> getErrors() {
		return this.errors;
	}

	public ArrayList<String> getWarnings() {
		return this.warnings;
	}

	public boolean healthy() {
		return (this.errors.size() == 0);
	}

	public static HealthCheck performHealthcheck(Context context) {
		// Get ready...
		HealthCheck check = new HealthCheck();
		SharedPreferences settings = PreferenceManager
				.getDefaultSharedPreferences(context);

		if (false == settings.getBoolean(
				context.getString(R.string.masterEnable), true)) {
			check.addError(context.getString(R.string.health_check_disabled));
		}

		// Do we have a C2DM ID?
		final String registrationId = GCMRegistrar.getRegistrationId(context);
		if (registrationId == null || "".equals(registrationId)) {
			check.addError(context.getString(R.string.health_error_no_gcm_id));
		}

		// Was there an error getting the C2DM ID?
		String gcmError = settings.getString("dm_register_error", "");
		if (!gcmError.equals("")) {
			check.addError(context.getString(R.string.health_error_gcm_error)
					+ gcmError);
		}
		int ruleCounts = 0;
		int ruleLocalDisabledCounts = 0;
		List<NotificationRule> rules = NotificationRule.FACTORY.listAll(context);
		ruleCounts = rules.size();

		for (NotificationRule rule : rules) {			
			if (!rule.getLocalEnabled()) {
				ruleLocalDisabledCounts++;
			}
		}
		if( ruleCounts == 0 )
		{
			String rawString = context.getString(R.string.health_error_no_rules);
			check.addWarning(rawString);
		}
		
		if (ruleLocalDisabledCounts > 0) {
			String plural = "";
			if (ruleLocalDisabledCounts > 1) {
				plural = "n";
			}
			String rawString = context
					.getString(R.string.health_error_disabled_local_rules);
			check.addWarning(String.format(rawString,
					ruleLocalDisabledCounts, plural));
		}

		return check;
	}
}
