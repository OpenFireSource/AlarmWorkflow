package de.florian.smsalarm;

import java.io.IOException;
import java.net.UnknownHostException;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.telephony.SmsMessage;
import android.widget.Toast;

public class SMSReciver extends BroadcastReceiver {

	/**
	 * @see android.content.BroadcastReceiver#onReceive(android.content.Context,
	 *      android.content.Intent)
	 */
	@Override
	public void onReceive(Context context, Intent intent) {
		Bundle bundle = intent.getExtras();
		Object messages[] = (Object[]) bundle.get("pdus");
		SmsMessage smsMessage[] = new SmsMessage[messages.length];
		for (int n = 0; n < messages.length; n++) {
			smsMessage[n] = SmsMessage.createFromPdu((byte[]) messages[n]);
		}
		int sender = Integer.parseInt(smsMessage[0].getOriginatingAddress());
		String message = smsMessage[0].getMessageBody().toString();
		DataSource d = new DataSource(context);
		d.open();
		String[] ret = d.getID(sender);		
		if (ret.length != 0) {			
			for (int i = 0; i < ret.length; i++) {
				String content = d.getDetails(ret[i]);
							
				if (message.contains(content)) {
					try {
						Toast.makeText(
								context,
								"Received Alarm-SMS: "
										+ smsMessage[0].getMessageBody(),
								Toast.LENGTH_LONG).show();
						TCPClient.notifyServer(message, context);
					} catch (UnknownHostException e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					} catch (IOException e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					}
				}
			}
		}
	}
}
