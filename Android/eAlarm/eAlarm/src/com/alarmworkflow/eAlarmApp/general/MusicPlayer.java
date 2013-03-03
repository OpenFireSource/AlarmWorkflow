package com.alarmworkflow.eAlarmApp.general;

import java.io.IOException;

import com.alarmworkflow.eAlarmApp.eAlarm;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.media.AudioManager;
import android.media.MediaPlayer;
import android.net.Uri;
import android.preference.PreferenceManager;
import android.util.Log;

public class MusicPlayer extends BroadcastReceiver implements
		MediaPlayer.OnCompletionListener {
	private static MusicPlayer instance;
	private int currentAudioVolume;
	private MediaPlayer mediaPlayer;

	private MusicPlayer() {
	}

	public static synchronized MusicPlayer getInstance() {
		if (MusicPlayer.instance == null) {
			MusicPlayer.instance = new MusicPlayer();
		}
		return MusicPlayer.instance;
	}

	@Override
	public void onCompletion(MediaPlayer mp) {
		AudioManager audioManager = ((AudioManager) eAlarm.context
				.getSystemService("audio"));
		audioManager.setStreamVolume(AudioManager.STREAM_ALARM,
				getCurrentAudioVolume(), 0);
		Log.i(this.toString(), "Reseted Volume to: "+currentAudioVolume);
	}

	public void play() {
		SharedPreferences prefs = PreferenceManager
				.getDefaultSharedPreferences(eAlarm.context);
		String alarmsound = prefs.getString("ringsel", "");
		mediaPlayer = new MediaPlayer();
		Uri uri = Uri.parse(alarmsound);
		final AudioManager audioManager = (AudioManager) eAlarm.context
				.getSystemService(Context.AUDIO_SERVICE);
		currentAudioVolume = audioManager
				.getStreamVolume(AudioManager.STREAM_ALARM);
		boolean overridesound = prefs.getBoolean("overridesound", false);
		if (overridesound) {
			audioManager.setStreamVolume(AudioManager.STREAM_ALARM,
					audioManager.getStreamMaxVolume(AudioManager.STREAM_ALARM),
					0);
		}
		try {
			mediaPlayer.setDataSource(eAlarm.context, uri);
			if (audioManager.getStreamVolume(AudioManager.STREAM_ALARM) != 0) {
				mediaPlayer.setAudioStreamType(AudioManager.STREAM_ALARM);
				mediaPlayer.prepare();
				mediaPlayer.start();
				mediaPlayer.setLooping(false);
				mediaPlayer.setOnCompletionListener(MusicPlayer.getInstance());
			}
		} catch (IOException e) {
			Log.e(MusicPlayer.class.getSimpleName(),
					"Mediaplayer crashed during playing the sound "
							+ e.getLocalizedMessage());
		}
	}

	public int getCurrentAudioVolume() {
		return currentAudioVolume;
	}

	public void setCurrentAudioVolume(int currentAudioVolume) {
		this.currentAudioVolume = currentAudioVolume;
	}

	@Override
	public void onReceive(Context arg0, Intent arg1) {
		play();

	}
}
