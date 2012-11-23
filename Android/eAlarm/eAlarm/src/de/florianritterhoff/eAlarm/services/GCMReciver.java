package de.florianritterhoff.eAlarm.services;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;

public class GCMReciver extends BroadcastReceiver {

    @Override
    public final void onReceive(Context context, Intent intent) {
        GCMIntent.runIntentInService(context, intent);
        setResult(Activity.RESULT_OK, null, null);
    }
}