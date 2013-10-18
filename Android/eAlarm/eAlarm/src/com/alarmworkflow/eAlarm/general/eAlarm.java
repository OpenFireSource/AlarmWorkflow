package com.alarmworkflow.eAlarm.general;



import java.util.List;

import android.app.Application;
import android.content.Context;
import android.content.IntentFilter;
import com.alarmworkflow.eAlarm.database.NotificationRule;

public class eAlarm extends Application {
	public static Context context;

	@Override
	public void onCreate() {
		super.onCreate();
		context = getApplicationContext();
        IntentFilter intentFilter = new IntentFilter("com.alarmworkflow.eAlarm.MusicPlayer");
		registerReceiver(MusicPlayer.getInstance(), intentFilter);
		List<NotificationRule> rules = NotificationRule.FACTORY.listAll(context);
		if(rules.size() == 0){
			NotificationRule rule = new NotificationRule();
			rule.setTitle("Alle Narchichten");
			rule.setLocalEnabled(true);
			rule.setStartTime("00:00");
			rule.setStopTime("23:59");
			rule.save(context);
		}
	}	
}
