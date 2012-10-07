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

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);

		_serviceWrapper = new AlarmWorkflowServiceWrapper();
		_connectServerUri = (EditText) findViewById(R.id.txtConnectServerUri);
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		getMenuInflater().inflate(R.menu.activity_main, menu);
		return true;
	}

	public void txtConnectServerUri_onClick(View view) {
		String serverUri = _connectServerUri.getEditableText().toString();
		if (serverUri == null || serverUri.length() == 0)
		{
			Toast.makeText(this, R.string.ConnectServerUriInvalidAddress, Toast.LENGTH_LONG).show();
			return;
		}
	}
}
