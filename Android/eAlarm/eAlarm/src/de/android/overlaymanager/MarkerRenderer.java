package de.android.overlaymanager;

import android.graphics.drawable.Drawable;

public interface MarkerRenderer {
    public Drawable render(ManagedOverlayItem item, Drawable defaultMarker, int bitState);
}
