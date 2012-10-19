package com.alarmworkflow.app;

import java.util.ArrayList;

import android.os.AsyncTask;
import android.preference.PreferenceManager;

public class FetchOperationsTask extends AsyncTask<Integer, Integer, Operation[]> {
	

    private static final String PREF_KEY_FETCH_AMOUNT = "pref_key_fetch_amount";
	private static final String PREF_KEY_SERVER_URI = "pref_key_server_uri";
    
    private static final int SERVICE_MAXAGE = 7;
    private static final boolean SERVICE_ONLYNONACKNOWLEDGED = true;
	
	private final MainActivity _mainActivity;
	
	public FetchOperationsTask(MainActivity mainActivity){
		_mainActivity = mainActivity;
	}
	
	@Override
	protected Operation[] doInBackground(Integer... arg0) {

		ArrayList<Operation> list = new ArrayList<Operation>();
		
		// Check if the server URI is entered (a better check would involve checking the correctness )
		String serverUri = PreferenceManager.getDefaultSharedPreferences(_mainActivity).getString(PREF_KEY_SERVER_URI, "http://10.0.2.2:60002/");
		int limitAmount = Integer.valueOf(PreferenceManager.getDefaultSharedPreferences(_mainActivity).getString(PREF_KEY_FETCH_AMOUNT, "5"));

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

			// Add the operation to the list
			list.add(operation);
		}
		
		return list.toArray(new Operation[0]);
	}
	
	@Override
	protected void onPostExecute(Operation[] result) {
		super.onPostExecute(result);

		// Tell the activity about each operation
		for (Operation operation : result) {
			_mainActivity.checkAndAddOperationToList(operation);
		}
	}
}
