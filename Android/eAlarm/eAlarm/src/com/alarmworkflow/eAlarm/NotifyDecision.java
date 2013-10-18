/**
 * Notifry for Android.
 * 
 * Copyright 2011 Daniel Foote
 *
 * Licensed under the Apache License, Version 2.0 (the 'License');
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an 'AS IS' BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.alarmworkflow.eAlarm;

import com.alarmworkflow.eAlarm.database.NotificationMessage;

import android.content.Context;
import android.content.SharedPreferences;
import android.preference.PreferenceManager;

public class NotifyDecision
{
	private Boolean shouldNotify;
	private NotificationMessage message;
	private String outputMessage;

	public Boolean getShouldNotify()
	{
		return shouldNotify;
	}

	public void setShouldNotify( Boolean shouldNotify )
	{
		this.shouldNotify = shouldNotify;
	}

	public NotificationMessage getMessage()
	{
		return message;
	}

	public void setMessage( NotificationMessage message )
	{
		this.message = message;
	}
	
	public String getOutputMessage()
	{
		return outputMessage;
	}

	public void setOutputMessage( String outputMessage )
	{
		this.outputMessage = outputMessage;
	}

	/**
	 * Determine if we should notify about this message or now.
	 * 
	 * @param context
	 * @param message
	 * @return
	 */
	public static NotifyDecision shouldNotify( Context context, NotificationMessage message )
	{
		NotifyDecision decision = new NotifyDecision();
		decision.setShouldNotify(message.getRule().getLocalEnabled());
		
		// Set the message that should be spoken.
		decision.setMessage(message);

		SharedPreferences settings = PreferenceManager.getDefaultSharedPreferences(context);
		String format = settings.getString("speakFormat", "%t. %m");
		
		StringBuffer buffer = new StringBuffer(format);
		formatString(buffer, "%t", message.getTitle());
		formatString(buffer, "%m", message.getMessage());
		formatString(buffer, "%s", message.getRule().getTitle());
		formatString(buffer, "%%", "%");
		decision.setOutputMessage(buffer.toString());

		return decision;
	}
	
	/**
	 * Replace a keyword with a value every time it appears in the buffer.
	 * @param buffer
	 * @param keyword
	 * @param value
	 */
	private static void formatString( StringBuffer buffer, String keyword, String value )
	{
		int position = -1;
		while( (position = buffer.indexOf(keyword)) != -1 )
		{
			buffer.replace(position, position + keyword.length(), value);
		}
	}
}
