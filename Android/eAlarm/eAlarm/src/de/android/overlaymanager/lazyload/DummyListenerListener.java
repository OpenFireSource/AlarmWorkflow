/*
 * OverlayManager - This is a library for android.
 * Copyright (c) 2009.  Christoph Widulle
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

package de.android.overlaymanager.lazyload;

import de.android.overlaymanager.ManagedOverlay;
import de.android.overlaymanager.ManagedOverlayGestureDetector;
import de.android.overlaymanager.ManagedOverlayItem;
import de.android.overlaymanager.ZoomEvent;
import android.view.MotionEvent;
import com.google.android.maps.GeoPoint;

public class DummyListenerListener implements ManagedOverlayGestureDetector.OnOverlayGestureListener{
    @Override
    public boolean onZoom(ZoomEvent zoom, ManagedOverlay overlay) {
        return false;
    }

    @Override
    public boolean onDoubleTap(MotionEvent e, ManagedOverlay overlay, GeoPoint point, ManagedOverlayItem item) {
        return false;
    }

    @Override
    public void onLongPress(MotionEvent e, ManagedOverlay overlay) {

    }

    @Override
    public void onLongPressFinished(MotionEvent e, ManagedOverlay overlay, GeoPoint point, ManagedOverlayItem item) {

    }

    @Override
    public boolean onScrolled(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY, ManagedOverlay overlay) {
        return false;
    }

    @Override
    public boolean onSingleTap(MotionEvent e, ManagedOverlay overlay, GeoPoint point, ManagedOverlayItem item) {
        return false;
    }
}
