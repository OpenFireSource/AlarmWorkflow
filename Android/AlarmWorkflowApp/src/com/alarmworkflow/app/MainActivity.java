package com.alarmworkflow.app;

import android.os.Bundle;
import android.app.Activity;
import android.view.Menu;
import android.view.View;
import android.widget.EditText;
import android.widget.Toast;

public class MainActivity extends Activity {

	private EditText _connectServerUri;
	private AlarmWorkflowServiceWrapper _serviceWrapper;

	public MainActivity() {
		_serviceWrapper = new AlarmWorkflowServiceWrapper();
	}

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);

		_connectServerUri = (EditText) findViewById(R.id.txtConnectServerUri);
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		getMenuInflater().inflate(R.menu.activity_main, menu);
		return true;
	}
	
	@Override
	protected void onRestoreInstanceState(Bundle savedInstanceState) {
		super.onRestoreInstanceState(savedInstanceState);

		if (savedInstanceState != null) {
			_connectServerUri.setText(savedInstanceState.getString("serverUri"));
		}
	}

	@Override
	protected void onSaveInstanceState(Bundle outState) {
		super.onSaveInstanceState(outState);

		outState.putString("serverUri", _connectServerUri.getEditableText()
				.toString());
	}

	public void txtConnectServerUri_onClick(View view) {
		// Check if the server Uri is entered (a better check would involve
		// checking the correctness)
		String serverUri = _connectServerUri.getEditableText().toString();
		if (serverUri == null || serverUri.length() == 0) {
			Toast.makeText(this, R.string.ConnectServerUriInvalidAddress,
					Toast.LENGTH_LONG).show();
			return;
		}
	}
}
