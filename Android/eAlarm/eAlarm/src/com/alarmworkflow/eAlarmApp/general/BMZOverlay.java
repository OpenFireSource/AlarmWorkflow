package com.alarmworkflow.eAlarmApp.general;

import java.util.ArrayList;
import java.util.List;

import android.content.Context;
import android.graphics.drawable.Drawable;
import com.google.android.maps.MapView;
import com.google.android.maps.OverlayItem;
import com.readystatesoftware.mapviewballoons.BalloonItemizedOverlay;

public class BMZOverlay extends BalloonItemizedOverlay<OverlayItem> {

	public BMZOverlay(Drawable defaultMarker, MapView mapView) {
		super(defaultMarker, mapView);
		// TODO Auto-generated constructor stub
	}

	private List<OverlayItem> list = new ArrayList<OverlayItem>();
	private Context context;

	
	@Override
	protected OverlayItem createItem(int i) {
		return list.get(i);
	}

	@Override
	public int size() {
		return list.size();

	}

	public void addOverlays(List<OverlayItem> overlays) {
		list.addAll(overlays);
	}

	public void overwriteOverlays(List<OverlayItem> overlays) {
		list = overlays;
	}

	public Context getContext() {
		return context;
	}

	public void setContext(Context context) {
		this.context = context;
	};
}