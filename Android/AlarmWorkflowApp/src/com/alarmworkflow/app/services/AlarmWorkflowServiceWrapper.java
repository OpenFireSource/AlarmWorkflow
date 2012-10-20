/**
 * 
 */
package com.alarmworkflow.app.services;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.Calendar;
import java.util.Date;

import org.apache.http.HttpResponse;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.DefaultHttpClient;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import org.json.JSONTokener;

import com.alarmworkflow.app.util.Operation;

/**
 * Contains wrappers to access the web services which are hosted by the AlarmWorkflow Windows Service. 
 * 
 * @author Chris
 *
 */
public class AlarmWorkflowServiceWrapper {

	/*
	 * TODO Make web service accessors more robust and generalize them!
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
		sb.append("maxAge=" + String.valueOf(maxAge));
		sb.append("&");
		sb.append("ona=" + String.valueOf(onlyNonAcknowledged));
		sb.append("&");
		sb.append("limit=" + String.valueOf(limitAmount));
		
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
		sb.append("id=" + String.valueOf(operationID));
		sb.append("&");
		sb.append("detail=0");		// < A value of "0" represents "minimum" detail level, which contains everything except the Custom Data and the Route image.

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

			// HINT: .Net transfers Dates like 1234567890+0200, so we cut off the +0200 and add two hours afterwards.
			// See http://weblogs.asp.net/bleroy/archive/2008/01/18/dates-and-json.aspx
			String timestampS = finalResult.getString("Timestamp");
			timestampS = timestampS.substring(6, timestampS.indexOf("+"));
 
			Calendar cal = Calendar.getInstance();
			cal.setTime(new Date(Long.valueOf(timestampS)));
			// TODO: Hard-coded offset of 2 hours (Germany).
			cal.add(Calendar.HOUR_OF_DAY, 2);

			operation.Timestamp = cal.getTime();

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
