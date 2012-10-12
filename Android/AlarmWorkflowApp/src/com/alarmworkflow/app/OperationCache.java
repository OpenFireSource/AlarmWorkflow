/**
 * 
 */
package com.alarmworkflow.app;

import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintStream;
import java.util.Date;

import android.util.SparseArray;

/**
 * Caches operations so that traffic keeps low.
 * 
 * @author Chris
 *
 */
public class OperationCache {

    static final String PERSISTENT_STORAGE_FILENAME = "AlarmWorkflowAppOperationCache.txt";
	
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
	            	operation.Timestamp = new Date(Date.parse(tokens[1]));
	            	operation.IsAcknowledged = Boolean.parseBoolean(tokens[2]);
	            	operation.ZipCode = tokens[3];
	            	operation.City = tokens[4];
	            	operation.Street = tokens[5];
	            	operation.StreetNumber = tokens[6];
	            	operation.Comment = tokens[7];
	            	operation.Keyword = tokens[8];
	            	operation.Location = tokens[9];
	            	
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
