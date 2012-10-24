package com.alarmworkflow.app.util;

import java.util.ArrayList;

import com.alarmworkflow.app.MainActivity;
import com.alarmworkflow.app.services.AlarmWorkflowServiceWrapper;

import android.os.AsyncTask;
import android.preference.PreferenceManager;

public class FetchOperationsTask extends AsyncTask<Integer, Integer, Operation[]> {
    	
	private final MainActivity _mainActivity;
	
	public FetchOperationsTask(MainActivity mainActivity){
		_mainActivity = mainActivity;
	}
	
	@Override
	protected Operation[] doInBackground(Integer... arg0) {

		ArrayList<Operation> list = new ArrayList<Operation>();
		
		// Check if the server URI is entered (a better check would involve checking the correctness )
		String serverUri = PreferenceManager.getDefaultSharedPreferences(_mainActivity).getString(Constants.PREF_KEY_SERVER_URI, "http://10.0.2.2:60002/");
		int limitAmount = Integer.valueOf(PreferenceManager.getDefaultSharedPreferences(_mainActivity).getString(Constants.PREF_KEY_FETCH_AMOUNT, "5"));
		boolean onlyNonAcknowledged = Boolean.valueOf(PreferenceManager.getDefaultSharedPreferences(_mainActivity).getBoolean(Constants.PREF_KEY_FETCH_ONLYNONACKNOWLEDGED, true));
		int maxAge = Integer.valueOf(PreferenceManager.getDefaultSharedPreferences(_mainActivity).getString(Constants.PREF_KEY_FETCH_MAXAGE, "7"));

		// Get all operation IDs
		int[] operationIDs = AlarmWorkflowServiceWrapper.getOperationIds(serverUri, maxAge, onlyNonAcknowledged, limitAmount);

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
