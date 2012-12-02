package de.android.overlaymanager.lazyload;

import com.google.android.maps.GeoPoint;

import java.util.List;

import de.android.overlaymanager.ManagedOverlay;
import de.android.overlaymanager.ManagedOverlayItem;


public interface LazyLoadCallback {

	public List<? extends ManagedOverlayItem> lazyload(GeoPoint topLeft, GeoPoint bottomRight, ManagedOverlay overlay) throws LazyLoadException;

}


