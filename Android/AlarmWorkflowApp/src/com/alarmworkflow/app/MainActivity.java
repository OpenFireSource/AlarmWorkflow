package com.alarmworkflow.app;

import java.io.FileNotFoundException;
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
	
	private OperationsAdapter _adapter;
	private List<Operation> _adapterList;

	public MainActivity() {

	}

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);

		// Retrieve controls from layout
		_txtConnectServerUri = (EditText) findViewById(R.id.txtConnectServerUri);
		_lsvOperations = (ListView) findViewById(R.id.lsvOperations);
		
		// Load shared preferences
		SharedPreferences prefs = getSharedPreferences(PREFS_NAME, 0);
		_txtConnectServerUri.setText(prefs.getString(PREF_SERVERURI, "http://10.0.2.2:60002/"));

		// Load the persisted cache
		try {
			OperationCache.getInstance().loadPersistedCache(openFileInput(OperationCache.PERSISTENT_STORAGE_FILENAME));
		} catch (FileNotFoundException e) {
			// It's ok if the file does not exist --> ignore exception
		}
		
		// Now create a new adapter for our list view and host the data in it
		_adapterList = new ArrayList<Operation>();
		_adapter = new OperationsAdapter(getApplicationContext(), R.layout.operationitemlayout, _adapterList);
		_lsvOperations.setAdapter(_adapter);
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		getMenuInflater().inflate(R.menu.activity_main, menu);
		return true;
	}
    
	@Override
	protected void onDestroy() {
		super.onDestroy();

		// Store shared preferences
        SharedPreferences prefs = getSharedPreferences(PREFS_NAME, 0);
        SharedPreferences.Editor editor = prefs.edit();
        editor.putString(PREF_SERVERURI, _txtConnectServerUri.getEditableText().toString());
        
        editor.commit();
        
        // Persist operation cache
        try {
			OperationCache.getInstance().persistCache(openFileOutput(OperationCache.PERSISTENT_STORAGE_FILENAME, MODE_PRIVATE));
		} catch (FileNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
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

		outState.putString("serverUri", _txtConnectServerUri.getEditableText().toString());
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

		for (int i : operationIDs) {
			Operation operation = OperationCache.getInstance().getCachedOperation(i);
			
			if(operation == null){				
				// Now retrieve the operation from the web service and store it in the cache for next time
				operation = AlarmWorkflowServiceWrapper.getOperationByID(serverUri, i);
				if(operation != null){
					OperationCache.getInstance().addCachedOperation(operation);
				}
			}							
			
			// Add the operation to the list (if it is not already present) and update it
			checkAndAddOperationToList(operation);
		}
	}
	
	private void checkAndAddOperationToList(Operation operation){
		if (operation == null){
			return;
		}

		if (_adapterList.contains(operation)){
			return;
		}
		
		// Add the operation to the adapter and notify
		_adapter.add(operation);		
		_adapter.notifyDataSetChanged();
	}

}
