package com.alarmworkflow.eAlarm.general;

import java.io.IOException;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.media.AudioManager;
import android.media.MediaPlayer;
import android.preference.PreferenceManager;

import com.alarmworkflow.eAlarm.R;
import com.alarmworkflow.eAlarm.database.NotificationMessage;

public class MusicPlayer extends BroadcastReceiver implements
		MediaPlayer.OnCompletionListener {
	private static MusicPlayer instance;
	private int currentAudioVolume;
	private MediaPlayer mediaPlayer;
	private NotificationMessage message;
	private int currentRinger;

	private MusicPlayer() {
	}

	public static synchronized MusicPlayer getInstance() {
		if (MusicPlayer.instance == null) {
			MusicPlayer.instance = new MusicPlayer();
		}
		return MusicPlayer.instance;
	}

	public void onCompletion(MediaPlayer mp) {

		SharedPreferences prefs = PreferenceManager
				.getDefaultSharedPreferences(eAlarm.context);
		if (NotificationService.globalOrOverrideBoolean(
				R.string.overwritesystem_setting, prefs, message.getRule(),
				false, eAlarm.context)) {
			AudioManager audioManager = ((AudioManager) eAlarm.context
					.getSystemService("audio"));
			audioManager.setStreamVolume(AudioManager.STREAM_SYSTEM,
					getCurrentAudioVolume(), 0);
			audioManager.setRingerMode(currentRinger);
		}
		try {
			mediaPlayer.stop();
		} catch (Exception ex) {
		}
	}

	public void play() {
		SharedPreferences prefs = PreferenceManager
				.getDefaultSharedPreferences(eAlarm.context);
		mediaPlayer = new MediaPlayer();
		String tone = NotificationService.globalOrOverrideString(
				R.string.choosenNotification, prefs, message.getRule(), "",
				eAlarm.context);
		if (tone.equals("")) {
			tone = android.provider.Settings.System.DEFAULT_NOTIFICATION_URI
					.getPath();
		}
		final AudioManager audioManager = (AudioManager) eAlarm.context
				.getSystemService(Context.AUDIO_SERVICE);
		currentAudioVolume = audioManager
				.getStreamVolume(AudioManager.STREAM_SYSTEM);
		if (NotificationService.globalOrOverrideBoolean(
				R.string.overwritesystem_setting, prefs, message.getRule(),
				false, eAlarm.context))
			audioManager
					.setStreamVolume(AudioManager.STREAM_SYSTEM, audioManager
							.getStreamMaxVolume(AudioManager.STREAM_SYSTEM), 0);
		currentRinger = audioManager.getRingerMode();

		try {
			mediaPlayer.setDataSource(tone);
			if (audioManager.getStreamVolume(AudioManager.STREAM_SYSTEM) != 0) {
				mediaPlayer.setAudioStreamType(AudioManager.STREAM_SYSTEM);
				mediaPlayer.prepare();
				mediaPlayer.setLooping(false);
				mediaPlayer.setOnCompletionListener(MusicPlayer.getInstance());
				mediaPlayer.start();

			}
		} catch (IOException e) {
		}
	}

	public int getCurrentAudioVolume() {
		return currentAudioVolume;
	}

	public void setCurrentAudioVolume(int currentAudioVolume) {
		this.currentAudioVolume = currentAudioVolume;
	}

	@Override
	public void onReceive(Context arg0, Intent intent) {
		// We were provided with a message ID. Load it and then handle it.
		Long messageId = intent.getLongExtra("messageId", 0);

		NotificationMessage message = NotificationMessage.FACTORY.get(
				eAlarm.context, messageId);
		if (message == null) {			
			return;
		}
		MusicPlayer.getInstance().setMessage(message);
		play();

	}

	public NotificationMessage getMessage() {
		return message;
	}

	public void setMessage(NotificationMessage message) {
		this.message = message;
	}
}
