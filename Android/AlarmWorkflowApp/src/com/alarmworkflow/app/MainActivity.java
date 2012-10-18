package com.alarmworkflow.app;

import java.io.FileNotFoundException;
import java.util.ArrayList;
import java.util.List;

import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.app.Activity;
import android.content.Intent;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.ListView;
import android.widget.Toast;

public class MainActivity extends Activity {

    private static final String PREF_KEY_FETCH_AMOUNT = "pref_key_fetch_amount";
	private static final String PREF_KEY_SERVER_URI = "pref_key_server_uri";
    
    private static final int SERVICE_MAXAGE = 7;
    private static final boolean SERVICE_ONLYNONACKNOWLEDGED = true;

	private ListView _lsvOperations;
	
	private OperationsAdapter _adapter;
	private List<Operation> _adapterList;

	public MainActivity() {

	}

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);

		PreferenceManager.setDefaultValues(this, R.xml.activity_preferences, false);
		
		// Retrieve controls from layout
		_lsvOperations = (ListView) findViewById(R.id.lsvOperations);
		
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
	public boolean onOptionsItemSelected(MenuItem item) {

		switch (item.getItemId()) {
			case R.id.menu_refresh:
				refreshOperationsList();
				return true;
			case R.id.menu_settings:
				Intent intent = new Intent(MainActivity.this, SettingsPreferenceActivity.class);
			    startActivity(intent);
				return true;

			default:
				return super.onOptionsItemSelected(item);
		}
	}

	@Override
	protected void onDestroy() {
		super.onDestroy();
        
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
	}

	@Override
	protected void onSaveInstanceState(Bundle outState) {
		super.onSaveInstanceState(outState);
	}
	
	private boolean isNetworkAvailable(boolean showToastIfUnavailable){
		ConnectivityManager cm = (ConnectivityManager)getSystemService(CONNECTIVITY_SERVICE);
		 
		NetworkInfo activeNetwork = cm.getActiveNetworkInfo();
		boolean isConnected = activeNetwork.isConnectedOrConnecting();
		if(!isConnected && showToastIfUnavailable){
			Toast.makeText(this, R.string.toast_no_network_connection, Toast.LENGTH_LONG).show();
		}
		return isConnected;
	}
	
	private void refreshOperationsList(){
		// Check if we have network connection.
		if(!isNetworkAvailable(true)){
			return;
		}
		
		// Check if the server URI is entered (a better check would involve checking the correctness )
		String serverUri = PreferenceManager.getDefaultSharedPreferences(this).getString(PREF_KEY_SERVER_URI, "http://10.0.2.2:60002/");
		int limitAmount = Integer.valueOf(PreferenceManager.getDefaultSharedPreferences(this).getString(PREF_KEY_FETCH_AMOUNT, "5"));

		// Get all operation IDs
		int[] operationIDs = AlarmWorkflowServiceWrapper.getOperationIds(serverUri, SERVICE_MAXAGE, SERVICE_ONLYNONACKNOWLEDGED, limitAmount);

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