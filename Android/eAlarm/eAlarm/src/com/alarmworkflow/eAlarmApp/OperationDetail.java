package com.alarmworkflow.eAlarmApp;

import java.io.IOException;
import java.util.HashMap;
import java.util.Map;

import com.alarmworkflow.eAlarmApp.R;
import com.alarmworkflow.eAlarmApp.datastorage.DataSource;
import com.alarmworkflow.eAlarmApp.datastorage.MySQLiteHelper;
import com.alarmworkflow.eAlarmApp.datastorage.ServerConnection;

import android.app.Activity;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.TextView;

public class OperationDetail extends Activity {

	String longitude = "";
	String latitude = "";
	String id = "";
	String time = "";
	String header = "";
	String text = "";
	Map<String, String> details;
	private TextView headerView;
	private TextView textView;
	private TextView timestamp;
	private DataSource datasource;
	private String feedback;
	private String opID;
	private String email;
	private boolean unlock;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		SharedPreferences prefs = PreferenceManager
				.getDefaultSharedPreferences(this);
		unlock = prefs.getBoolean("unlock", false);
		email = prefs.getString(C2DMClientActivity.EMAIL, "n/a");
		Window window = getWindow();
		if (unlock)
			window.addFlags(WindowManager.LayoutParams.FLAG_DISMISS_KEYGUARD);
		window.addFlags(WindowManager.LayoutParams.FLAG_SHOW_WHEN_LOCKED);
		requestWindowFeature(Window.FEATURE_NO_TITLE);
		setContentView(R.layout.operationdetail);
		headerView = (TextView) findViewById(R.id.operationheader);
		textView = (TextView) findViewById(R.id.operationtext);
		timestamp = (TextView) findViewById(R.id.operationtimestamp);
		Intent intent = getIntent();
		time = intent.getExtras().getString(MySQLiteHelper.COLUMN_TIMESTAMP);
		datasource = DataSource.getInstance(this);
		id = datasource.getID(time);
		setTexts();
		initButtons();
		if (feedback != null)
			if (feedback.equals("true"))
				disableFeedback();
	}

	private void initButtons() {
		ImageButton mapButton = (ImageButton) findViewById(R.id.mapBut);
		mapButton.setOnClickListener(new OnClickListener() {

			@Override
			public void onClick(View v) {
				Intent intent = new Intent(getApplicationContext(),
						com.alarmworkflow.eAlarmApp.Map.class);
				intent.putExtra(MySQLiteHelper.COLUMN_ID, id);
				startActivity(intent);
			}
		});
		ImageButton homeButton = (ImageButton) findViewById(R.id.home);
		homeButton.setOnClickListener(new OnClickListener() {

			@Override
			public void onClick(View v) {
				Intent intent = new Intent(getApplicationContext(),
						com.alarmworkflow.eAlarmApp.OperationView.class);

				intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
				intent.addFlags(Intent.FLAG_ACTIVITY_NO_HISTORY);

				startActivity(intent);
			}
		});
		Button yes = (Button) findViewById(R.id.yes);
		if (yes != null) {
			yes.setOnClickListener(new OnClickListener() {

				@Override
				public void onClick(View v) {
					datasource.setFeedback(true, id);
					disableFeedback();
					Map<String, String> params = new HashMap<String, String>();
					params.put("email", email);
					params.put("opid", opID);
					params.put("status", "yes");
					FeedbackRunnable run = new FeedbackRunnable(params);
					new Thread(run).start();
				}
			});
		}
		Button maybe = (Button) findViewById(R.id.usertext);
		if (maybe != null) {
			maybe.setOnClickListener(new OnClickListener() {

				@Override
				public void onClick(View v) {
					datasource.setFeedback(true, id);
					disableFeedback();
					Map<String, String> params = new HashMap<String, String>();
					params.put("email", email);
					params.put("opid", opID);
					params.put("status", "maybe");
					FeedbackRunnable run = new FeedbackRunnable(params);
					new Thread(run).start();
				}
			});
		}
		Button no = (Button) findViewById(R.id.no);
		if (no != null) {
			no.setOnClickListener(new OnClickListener() {

				@Override
				public void onClick(View v) {
					datasource.setFeedback(true, id);
					disableFeedback();
					Map<String, String> params = new HashMap<String, String>();
					params.put("email", email);
					params.put("opid", opID);
					params.put("status", "no");
					FeedbackRunnable run = new FeedbackRunnable(params);
					new Thread(run).start();
				}
			});
		}
	}

	private void setTexts() {
		if (id != null) {
			if (!id.equalsIgnoreCase("")) {
				details = datasource.getDetails(id);
				header = details.get(MySQLiteHelper.COLUMN_HEADER);
				text = details.get(MySQLiteHelper.COMLUM_TEXT);
				longitude = details.get(MySQLiteHelper.COLUMN_LONG);
				longitude = details.get(MySQLiteHelper.COLUMN_LAT);
				time = details.get(MySQLiteHelper.COLUMN_TIMESTAMP);
				feedback = details.get(MySQLiteHelper.COLUMN_FEEDBACK);
				opID = details.get(MySQLiteHelper.COLUMN_OPID);
				headerView.setText(header);
				textView.setText(text);
				timestamp.setText(time);
			}
		}

	}

	void disableFeedback() {
		findViewById(R.id.feedbacklayout).setVisibility(View.GONE);

	}

	public class FeedbackRunnable implements Runnable {

		private Map<String, String> params;

		public FeedbackRunnable(Map<String, String> params) {
			this.params = params;
		}

		public void run() {
			try {
				ServerConnection
						.post("https://gymolching-portal.de/gcm/feedback.php",
								params);
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}

		}

	}

}
