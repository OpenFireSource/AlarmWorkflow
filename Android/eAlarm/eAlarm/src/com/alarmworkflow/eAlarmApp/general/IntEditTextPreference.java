package com.alarmworkflow.eAlarmApp.general;

import android.content.Context;
import android.preference.EditTextPreference;
import android.util.AttributeSet;

public class IntEditTextPreference extends EditTextPreference {

	public IntEditTextPreference(Context context) {
		super(context);
	}

	public IntEditTextPreference(Context context, AttributeSet attrs) {
		super(context, attrs);
	}

	public IntEditTextPreference(Context context, AttributeSet attrs,
			int defStyle) {
		super(context, attrs, defStyle);
	}

	@Override
	protected String getPersistedString(String defaultReturnValue) {
		return String.valueOf(getPersistedInt(-1));
	}

	@Override
	protected boolean persistString(String value) {
		if (value.equalsIgnoreCase("") || value.length() == 0 )
			return persistInt(0);
		else
			return persistInt(Integer.valueOf(value));
	}

}
