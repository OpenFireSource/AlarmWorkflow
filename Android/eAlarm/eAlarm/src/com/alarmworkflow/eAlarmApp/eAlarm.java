package com.alarmworkflow.eAlarmApp;

import com.alarmworkflow.eAlarmApp.general.MusicPlayer;

import android.app.Application;
import android.content.Context;
import android.content.IntentFilter;

public class eAlarm extends Application {
	public static Context context;

	@Override
	public void onCreate() {
		super.onCreate();
		context = getApplicationContext();
        IntentFilter intentFilter = new IntentFilter("com.alarmworkflow.eAlarmApp.MusicPlayer");
		registerReceiver(MusicPlayer.getInstance(), intentFilter);
	}
	
}
