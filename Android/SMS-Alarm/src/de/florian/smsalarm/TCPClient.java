package de.florian.smsalarm;

import java.io.*;
import java.net.*;
import android.content.Context;
import android.content.SharedPreferences;
import android.preference.PreferenceManager;

public class TCPClient {
	public static void notifyServer(String message,
			Context context) throws UnknownHostException,
			IOException {
		SharedPreferences sharedPrefs = PreferenceManager
				.getDefaultSharedPreferences(context);
		String server = sharedPrefs.getString("servername", "");
		Socket clientSocket = new Socket(server, 5555);
		DataOutputStream outToServer = new DataOutputStream(
				clientSocket.getOutputStream());
		outToServer.writeBytes("<event><timestamp>" + System.currentTimeMillis()
				+ "</timestamp><message>" + message + "</message></event>");
		outToServer.flush();
		clientSocket.close();
	}
}