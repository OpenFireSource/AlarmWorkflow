package android.preference;

import android.content.Context;
import android.content.DialogInterface;
import android.util.AttributeSet;
import android.view.View;
import android.widget.TimePicker;

public class TimePickerPreference extends DialogPreference implements
		TimePicker.OnTimeChangedListener {

	/**
	 * The validation expression for this preference
	 */
	private static final String VALIDATION_EXPRESSION = "[0-2]*[0-9]:[0-5]*[0-9]";

	/**
	 * The default value for this preference
	 */
	private String defaultValue;
	private String result;
	private TimePicker tp;

	/**
	 * @param context
	 * @param attrs
	 */
	public TimePickerPreference(Context context, AttributeSet attrs) {
		super(context, attrs);
		defaultValue = attrs.getAttributeValue(
				"http://schemas.android.com/apk/res/android", "defaultValue");
		initialize();
	}

	/**
	 * @param context
	 * @param attrs
	 * @param defStyle
	 */
	public TimePickerPreference(Context context, AttributeSet attrs,
			int defStyle) {
		super(context, attrs, defStyle);
		defaultValue = attrs.getAttributeValue(
				"http://schemas.android.com/apk/res/android", "defaultValue");
		initialize();
	}

	/**
	 * Initialize this preference
	 */
	private void initialize() {
		setPersistent(true);
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see android.preference.DialogPreference#onCreateDialogView()
	 */
	@Override
	protected View onCreateDialogView() {

		tp = new TimePicker(getContext());
		tp.setOnTimeChangedListener(this);

		int h = getHour();
		int m = getMinute();
		tp.setCurrentHour(h);
		tp.setCurrentMinute(m);
		tp.setIs24HourView(false);

		return tp;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see
	 * android.widget.TimePicker.OnTimeChangedListener#onTimeChanged(android
	 * .widget.TimePicker, int, int)
	 */

	public void onTimeChanged(TimePicker view, int hour, int minute) {
		result = intToString(hour, 2) + ":" + intToString(minute, 2);
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see
	 * android.preference.DialogPreference#onDismiss(android.content.DialogInterface
	 * )
	 */
	@Override
	public void onDismiss(DialogInterface dialog) {
		super.onDismiss(dialog);
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see android.preference.DialogPreference#onDialogClosed(boolean)
	 */
	@Override
	protected void onDialogClosed(boolean positiveResult) {
		super.onDialogClosed(positiveResult);
		if (positiveResult) {
			tp.clearFocus(); // to get value of number if edited in text field,
								// and clicking OK without clicking outside the
								// field first (bug in NumberPicker)
			result = intToString(tp.getCurrentHour(), 2) + ":" +intToString(tp.getCurrentMinute(), 2) ;
			persistString(result);
			callChangeListener(result);
			setSummary(result);
		}
	}

	/**
	 * Get the hour value (in 24 hour time)
	 * 
	 * @return The hour value, will be 0 to 23 (inclusive)
	 */
	private int getHour() {
		String time = getPersistedString(this.defaultValue);
		if (time == null || !time.matches(VALIDATION_EXPRESSION)) {
			return -1;
		}

		return Integer.valueOf(time.split(":|/")[0]);
	}

	/**
	 * Get the minute value
	 * 
	 * @return the minute value, will be 0 to 59 (inclusive)
	 */
	private int getMinute() {
		String time = getPersistedString(this.defaultValue);
		if (time == null || !time.matches(VALIDATION_EXPRESSION)) {
			return -1;
		}

		return Integer.valueOf(time.split(":|/")[1]);
	}

	public static String intToString(int num, int digits) {
		String output = Integer.toString(num);
		while (output.length() < digits)
			output = "0" + output;
		return output;
	}
}