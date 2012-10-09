package com.alarmworkflow.app;

import java.util.ArrayList;
import java.util.List;

import android.os.Bundle;
import android.app.Activity;
import android.content.SharedPreferences;
import android.view.Menu;
import android.view.View;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.Toast;

public class MainActivity extends Activity {

    private static final String PREFS_NAME = "AlarmWorkflowAppPrefs";
    private static final String PREF_SERVERURI = "ServerUri";
    
    private static final int SERVICE_MAXAGE = 7;
    private static final boolean SERVICE_ONLYNONACKNOWLEDGED = true;
    private static final int SERVICE_LIMITAMOUNT = 5;
        
	private EditText _txtConnectServerUri;
	private ListView _lsvOperations;

	public MainActivity() {

	}

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);

		_txtConnectServerUri = (EditText) findViewById(R.id.txtConnectServerUri);
		_lsvOperations = (ListView) findViewById(R.id.lsvOperations);
		
		SharedPreferences prefs = getSharedPreferences(PREFS_NAME, 0);
		_txtConnectServerUri.setText(prefs.getString(PREF_SERVERURI, "http://10.0.2.2:60002/"));
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		getMenuInflater().inflate(R.menu.activity_main, menu);
		return true;
	}
    
	@Override
	protected void onDestroy() {
		super.onDestroy();

        SharedPreferences prefs = getSharedPreferences(PREFS_NAME, 0);
        SharedPreferences.Editor editor = prefs.edit();
        editor.putString(PREF_SERVERURI, _txtConnectServerUri.getEditableText().toString());
        
        editor.commit();
	}
	
	
	@Override
	protected void onRestoreInstanceState(Bundle savedInstanceState) {
		super.onRestoreInstanceState(savedInstanceState);

		if (savedInstanceState != null) {
			_txtConnectServerUri.setText(savedInstanceState.getString("serverUri"));
		}
	}

	@Override
	protected void onSaveInstanceState(Bundle outState) {
		super.onSaveInstanceState(outState);

		outState.putString("serverUri", _txtConnectServerUri.getEditableText()
				.toString());
	}

	public void btnRefreshList_onClick(View view) {
		// Check if the server URI is entered (a better check would involve checking the correctness )
		String serverUri = _txtConnectServerUri.getEditableText().toString();
		if (serverUri == null || serverUri.length() == 0) {
			Toast.makeText(this, R.string.ConnectServerUriInvalidAddress, Toast.LENGTH_LONG).show();
			return;
		}
				
		// Get all operation IDs
		int[] operationIDs = AlarmWorkflowServiceWrapper.getOperationIds(serverUri, SERVICE_MAXAGE, SERVICE_ONLYNONACKNOWLEDGED, SERVICE_LIMITAMOUNT);

		List<Operation> operations = new ArrayList<Operation>();
		
		// TODO Check which operations we have cached (caching not implemented yet!) and don't retrieve those!
		for (int i : operationIDs) {
			Operation operation = AlarmWorkflowServiceWrapper.getOperationByID(serverUri, i);
			if (operation != null){
				operations.add(operation);
			}
		}
		
		// Now create a new adapter for our list view and host the data in it
		OperationsAdapter adapter = new OperationsAdapter(getApplicationContext(), R.layout.operationitemlayout, operations);
		_lsvOperations.setAdapter(adapter);
	}
}
