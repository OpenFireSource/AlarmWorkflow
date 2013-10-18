package com.alarmworkflow.eAlarm;

import java.util.HashMap;
import java.util.Locale;
import java.util.Vector;
import android.speech.tts.TextToSpeech;

import android.app.Service;
import android.content.Intent;
import android.content.SharedPreferences;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.media.AudioManager;
import android.os.Handler;
import android.os.IBinder;
import android.os.Message;
import android.preference.PreferenceManager;
import android.telephony.PhoneStateListener;
import android.telephony.TelephonyManager;

public class SpeakService extends Service implements SensorEventListener, TextToSpeech.OnInitListener
{
	private TextToSpeech tts = null;
	private Vector<String> queue = new Vector<String>();
	private boolean initialized = false;
	private boolean temporaryDisable = false;
	private SensorManager sensorMgr = null;
	private long lastUpdate = -1;
	private float x, y, z;
	private float last_x, last_y, last_z;
	private boolean shakeSensingOn = false;
	private SpeakService alternateThis = this;
	private int shakeThreshold = 1500;
	private HashMap<String, String> parameters = new HashMap<String, String>();

	@Override
	public IBinder onBind( Intent arg0 )
	{
		return null;
	}

	public void onInit( int status )
	{
		this.parameters.put(TextToSpeech.Engine.KEY_PARAM_STREAM, String.valueOf(AudioManager.STREAM_NOTIFICATION));
		tts.setLanguage(Locale.GERMAN);
		
		if( status == TextToSpeech.SUCCESS )
		{
			synchronized( queue )
			{
				initialized = true;

				for( String message : queue )
				{
					tts.speak(message, TextToSpeech.QUEUE_ADD, this.parameters);
				}

				queue.clear();
			}
		}
		else
		{
		}
	}

	private Handler handler = new Handler()
	{
		public void handleMessage( Message msg )
		{
			synchronized( queue )
			{
				if(!initialized )
				{
					queue.add((String) msg.obj);
				}
				else
				{
					tts.speak((String) msg.obj, TextToSpeech.QUEUE_ADD, parameters);
				}
			}
		}
	};

	private Handler sensorOffHandler = new Handler()
	{
		public void handleMessage( Message msg )
		{
			if( sensorMgr != null )
			{
				sensorMgr.unregisterListener(alternateThis);
				sensorMgr = null;
			}
			shakeSensingOn = false;
		}
	};

	@Override
	public void onCreate()
	{
		super.onCreate();
		TelephonyManager tm = (TelephonyManager) getSystemService(TELEPHONY_SERVICE);
		tm.listen(mPhoneListener, PhoneStateListener.LISTEN_CALL_STATE);

		this.tts = new TextToSpeech(this, this);
	}

	@Override
	public void onStart( final Intent intent, int startId )
	{
		super.onStart(intent, startId);

		if( intent == null )
		{
			return;
		}
		
		if( temporaryDisable )
		{
			return;
		}

		SharedPreferences settings = PreferenceManager.getDefaultSharedPreferences(this);

		boolean stopInstead = intent.getExtras().getBoolean("stopNow");
		String setting = settings.getString("delayReadingTime", "0");
		int delaySend = Integer.parseInt(setting);

		if( settings.getBoolean(getString(R.string.shakeToStop), false) )
		{
		
			if( !this.shakeSensingOn )
			{
				try
				{
					this.shakeThreshold = Integer.parseInt(settings.getString(getString(R.string.shakeThreshold), "1500"));
				}
				catch( NumberFormatException ex )
				{
					this.shakeThreshold = 1500;
				}
				Integer shakeWaitTime = 60;
				try
				{
					shakeWaitTime = Integer.parseInt(settings.getString(getString(R.string.shakeWaitTime), "60"));
				}
				catch( NumberFormatException ex )
				{
				}
				this.sensorMgr = (SensorManager) getSystemService(SENSOR_SERVICE);
				boolean accelSupported = this.sensorMgr.registerListener(
						this,
						this.sensorMgr.getDefaultSensor(Sensor.TYPE_ACCELEROMETER),
						SensorManager.SENSOR_DELAY_GAME,
						null);

				if( !accelSupported )
				{
					this.sensorMgr.unregisterListener(this);
				}
				else
				{
					this.shakeSensingOn = true;
					sensorOffHandler.sendMessageDelayed(Message.obtain(), shakeWaitTime * 1000);
				}
			}
		}

		if( stopInstead )
		{
			this.tts.stop();
		}
		else if( delaySend > 0 )
		{
			Message msg = Message.obtain();
			msg.obj = intent.getExtras().get("text");
			handler.sendMessageDelayed(msg, delaySend * 1000);
		}
		else
		{
			String text = intent.getExtras().getString("text");			
			synchronized( this.queue )
			{
				if( false == this.initialized )
				{
					this.queue.add(text);
				}
				else
				{
					this.tts.speak(text, TextToSpeech.QUEUE_ADD, this.parameters);
				}
			}
		}
	}

	@Override
	public void onDestroy()
	{
		synchronized( this.queue )
		{
			this.tts.shutdown();
			this.initialized = false;
		}
	}

	public void onAccuracyChanged( Sensor sensor, int accuracy )
	{
		// Oh well!
	}

	public void onSensorChanged( SensorEvent event )
	{		
		if( event.sensor.getType() == Sensor.TYPE_ACCELEROMETER )
		{
			long curTime = System.currentTimeMillis();
			if( (curTime - lastUpdate) > 100 )
			{
				long diffTime = (curTime - lastUpdate);
				lastUpdate = curTime;

				x = event.values[SensorManager.DATA_X];
				y = event.values[SensorManager.DATA_Y];
				z = event.values[SensorManager.DATA_Z];

				float speed = Math.abs(x + y + z - last_x - last_y - last_z) / diffTime * 10000;

				if( speed > this.shakeThreshold )
				{
					Intent intentData = new Intent(getBaseContext(), SpeakService.class);
					intentData.putExtra("stopNow", true);
					startService(intentData);
					sensorOffHandler.sendMessageDelayed(Message.obtain(), 0);
				}
				last_x = x;
				last_y = y;
				last_z = z;
			}
		}
	}

	private PhoneStateListener mPhoneListener = new PhoneStateListener()
	{
		public void onCallStateChanged( int state, String incomingNumber )
		{
			switch( state )
			{
				case TelephonyManager.CALL_STATE_RINGING:
				case TelephonyManager.CALL_STATE_OFFHOOK:
					tts.stop();
					temporaryDisable = true;
					break;
				case TelephonyManager.CALL_STATE_IDLE:
					temporaryDisable = false;
					break;
				default:
					break;
			}
		}
	};
}
