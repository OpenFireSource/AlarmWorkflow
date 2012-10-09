/**
 * 
 */
package com.alarmworkflow.app;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;

import org.apache.http.HttpResponse;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.DefaultHttpClient;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import org.json.JSONTokener;

/**
 * Contains wrappers to access the web services which are hosted by the AlarmWorkflow Windows Service. 
 * 
 * @author Chris
 *
 */
public class AlarmWorkflowServiceWrapper {

	/*
	 * TODO Make web service accessors more robust and generalize them!
	 * TODO Include a static method which checks for connectivity and Toast if there is no network available!
	 * TODO Use Gson for deserialization from JSON? See http://code.google.com/p/google-gson/downloads/list 
	 */
	
	/**
	 * Returns the IDs of the operations that match the given search criteria. 
	 * @param serverUri The host name of the server to connect to.
	 * @param maxAge The maximum age of the operations. A value of <b>'7'</b> is recommended.
	 * @param onlyNonAcknowledged Whether or not to return only operations which haven't been acknowledged yet.
	 * 			A value of <b>'true'</b> is recommended to include only operations which are "new". 
	 * @param limitAmount The maximum amount of operations to return. Keeps transfer data small. 
	 * 			A value of <b>'5'</b> is recommended.
	 * @return An array containing the IDs of all operations that have matched the given search criteria.
	 */
	public static int[] getOperationIds(String serverUri, int maxAge, boolean onlyNonAcknowledged, int limitAmount){
			
		// Construct URI
		StringBuilder sb = new StringBuilder();
		// TODO Use a better approach than this (URI Builder)!
		sb.append(serverUri);
		sb.append("AlarmWorkflow/AlarmWorkflowService/GetOperationIds/");
		sb.append(String.valueOf(maxAge));
		sb.append("&");
		sb.append(String.valueOf(onlyNonAcknowledged));
		sb.append("&");
		sb.append(String.valueOf(limitAmount));
		
		DefaultHttpClient client = new DefaultHttpClient();		
		try {
			HttpGet get = new HttpGet(sb.toString());
			HttpResponse response = client.execute(get);
			
			BufferedReader reader = new BufferedReader(new InputStreamReader(response.getEntity().getContent(), "UTF-8"));
			StringBuilder builder = new StringBuilder();
			for (String line = null; (line = reader.readLine()) != null;) {
			    builder.append(line).append("\n");
			}
			JSONTokener tokener = new JSONTokener(builder.toString());
			JSONArray finalResult = new JSONArray(tokener);

			int[] operationIds = new int[finalResult.length()];

			for (int i = 0; i < finalResult.length(); i++) {
				operationIds[i] = finalResult.getInt(i);				
			}
			
			return operationIds;
			
		} catch (ClientProtocolException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
				
		return new int[0];
	}
	
	/**
	 * Returns the Operation by its ID.
	 * @param serverUri The host name of the server to connect to.
	 * @param operationID The ID of the operation to return.
	 * @return The Operation by its ID. If there is no Operation by the given ID, null is returned.
	 */
	public static Operation getOperationByID(String serverUri, int operationID){
		
		// Construct URI
		StringBuilder sb = new StringBuilder();
		// TODO Use a better approach than this (URI Builder)!
		sb.append(serverUri);
		sb.append("AlarmWorkflow/AlarmWorkflowService/GetOperationById/");
		sb.append(String.valueOf(operationID));

		DefaultHttpClient client = new DefaultHttpClient();
		try {
			HttpGet get = new HttpGet(sb.toString());
			HttpResponse response = client.execute(get);

			BufferedReader reader = new BufferedReader(new InputStreamReader(response.getEntity().getContent(), "UTF-8"));
			StringBuilder builder = new StringBuilder();
			for (String line = null; (line = reader.readLine()) != null;) {
				builder.append(line).append("\n");
			}
			JSONTokener tokener = new JSONTokener(builder.toString());
			JSONObject finalResult = new JSONObject(tokener);

			Operation operation = new Operation();
			operation.OperationID = finalResult.getInt("Id");
			operation.OperationNumber = finalResult.getString("OperationNumber");
			operation.Messenger = finalResult.getString("Messenger");
			operation.Location = finalResult.getString("Location");	
			operation.Street = finalResult.getString("Street");
			operation.StreetNumber = finalResult.getString("StreetNumber");		
			operation.City = finalResult.getString("City");
			operation.ZipCode = finalResult.getString("ZipCode");
			operation.Property = finalResult.getString("Property");
			operation.Comment = finalResult.getString("Comment");
			operation.Keyword = finalResult.getString("Keyword");
			operation.IsAcknowledged = finalResult.getBoolean("IsAcknowledged");

			// HINT Date Format: "2012-10-09T18:43:13.543" --> "YYYY-MM-DDTHH:mm:ss.
			// TODO Somehow (through JSON?) the timestamp string gets messed up into like "2346798783209834+0200" which they can't parse.
			/*
			String timestamp = finalResult.getString("Timestamp");
			SimpleDateFormat format = new SimpleDateFormat();
			operation.Timestamp = format.parse(timestamp);
			*/
			
			return operation;

		} catch (ClientProtocolException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		return null;
	}
}
