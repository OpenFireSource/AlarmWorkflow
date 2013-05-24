/**
 * 
 */
package com.alarmworkflow.app.util;

import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintStream;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;

import android.util.SparseArray;

/**
 * Caches operations so that traffic keeps low.
 * 
 * @author Chris
 *
 */
public class OperationCache {
	
	private static OperationCache _instance;
	/**
	 * @return The singleton instance.
	 */
	public static OperationCache getInstance() {
		
		if(_instance==null){
			_instance=new OperationCache();
		}
		
		return _instance;
	}
	
	private SparseArray<Operation> _operations;
	
	public OperationCache() {
		_operations = new SparseArray<Operation>();
	}

	/**
	 * Returns a configurable amount of operations that are the most recent (if any).
	 * @param maxAge The maximum age in days.
	 * @param amount The amount of operations to return.
	 * @return The most recent operations (if any), limited by amount.
	 */
	public Operation[] getRecentOperations(int maxAge, int amount){
		Calendar cal = Calendar.getInstance();
		cal.add(Calendar.DATE, -(maxAge * 24 * 60));
		
		ArrayList<Operation> list = new ArrayList<Operation>();
		for (int i = 0; i < _operations.size(); i++) {
			int key = _operations.keyAt(i);
			Operation value = _operations.get(key);
			
			if(value.Timestamp.before(cal.getTime())) {
				continue;
			}
			
			list.add(value);
			
			if(amount > 0 && (list.size() >= amount)){
				break;
			}
		}	
		
		return list.toArray(new Operation[0]);
	}
	
	/**
	 * Returns the cached operation, if present. Otherwise returns null.
	 * @param operationID The ID of the operation to return.
	 * @return The cached operation, if present. Otherwise null.
	 */
	public Operation getCachedOperation(int operationID){
		return _operations.get(operationID);
	}
	
	public void addCachedOperation(Operation operation){
		_operations.put(operation.OperationID, operation);
	}
	
	/**
	 * Persists the data in this cache to the file system.
	 * @param fileOutputStream The file stream to write to.
	 */
	public void persistCache(FileOutputStream fileOutputStream){
		if(fileOutputStream == null){
			return;
		}

		// Write all operations line by line into the file
		PrintStream print = new PrintStream(fileOutputStream);

		for (int i = 0; i < _operations.size(); i++) {
			Operation operation = _operations.valueAt(i);

			StringBuilder sb = new StringBuilder();
			sb.append(String.valueOf(operation.OperationID));
			sb.append(";");
			sb.append(String.valueOf(operation.OperationNumber));
			sb.append(";");
			sb.append(String.valueOf(operation.Timestamp));
			sb.append(";");
			sb.append(String.valueOf(operation.IsAcknowledged));
			sb.append(";");
			sb.append(String.valueOf(operation.ZipCode));
			sb.append(";");
			sb.append(String.valueOf(operation.City));
			sb.append(";");
			sb.append(String.valueOf(operation.Street));
			sb.append(";");
			sb.append(String.valueOf(operation.StreetNumber));
			sb.append(";");
			sb.append(String.valueOf(operation.Comment));
			sb.append(";");
			sb.append(String.valueOf(operation.Keyword));
			sb.append(";");
			sb.append(String.valueOf(operation.Location));
			
			print.println(sb.toString());
		}
		

		print.close();		
	}
	
	/**
	 * Loads the persisted data from the file system, if available.
	 * @param fileInputStream The file stream to read from.
	 */
	public void loadPersistedCache(FileInputStream fileInputStream){
		if(fileInputStream == null){
			return;
		}

		try {		
			InputStreamReader isr = new InputStreamReader(fileInputStream);
			BufferedReader reader = new BufferedReader(isr);

			String readString;
            while ((readString = reader.readLine()) != null) {
            	String[] tokens = readString.split(";");
            	
            	try {
	            	Operation operation = new Operation();
	            	operation.OperationID = Integer.parseInt(tokens[0]);
	            	operation.OperationNumber = tokens[1];
	            	operation.Timestamp = new Date(Date.parse(tokens[2]));
	            	operation.IsAcknowledged = Boolean.parseBoolean(tokens[3]);
	            	operation.ZipCode = tokens[4];
	            	operation.City = tokens[5];
	            	operation.Street = tokens[6];
	            	operation.StreetNumber = tokens[7];
	            	operation.Comment = tokens[8];
	            	operation.Keyword = tokens[9];
	            	operation.Location = tokens[10];
	            	
	            	addCachedOperation(operation);					
        		} catch	(Exception e){
        			// Usually any other exception happens through parsing errors.
        			// In these cases just go to the next operation
        			// (the operation will be retrieved again if necessary)
        			e.printStackTrace();
        		}
            }
			
			isr.close();			
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
	}
}
