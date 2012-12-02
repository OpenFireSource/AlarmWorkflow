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

package de.android.overlaymanager;

import android.graphics.drawable.Drawable;

import com.google.android.maps.GeoPoint;
import com.google.android.maps.MapView;
import com.google.android.maps.OverlayItem;

public class ManagedOverlayItem extends OverlayItem {

    private ManagedOverlay parentOverlay;
    private Drawable customRenderedDrawable;

    public ManagedOverlayItem(GeoPoint point, String title, String snippet) {
        super(point, title, snippet);
    }

    public ManagedOverlay getOverlay() {
        return parentOverlay;
    }

    public void setOverlay(ManagedOverlay parentOverlay) {
        this.parentOverlay = parentOverlay;
    }

    @Override
    public Drawable getMarker(int i) {
        if (parentOverlay.customMarkerRenderer != null)
            return parentOverlay.customMarkerRenderer.render(this, parentOverlay.defaultMarker, i);
        else
            return super.getMarker(i);
    }

    public MapView getMapView() {
        return getOverlay().getManager().getMapView();
    }
    
    public Drawable getCustomRenderedDrawable() {
		return customRenderedDrawable;
	}

	public void setCustomRenderedDrawable(Drawable customRenderedDrawable) {
		this.customRenderedDrawable = customRenderedDrawable;
	}


    public static class Builder {
        private GeoPoint p = null;
        private String title = "";
        private String snippet = "";

        public Builder() {
        }

        public Builder(GeoPoint p) {
            this.p = p;
        }

        public Builder name(String title) {
            this.title = title;
            return this;
        }

        public Builder snippet(String snippet) {
            this.snippet = snippet;
            return this;
        }

        public ManagedOverlayItem create() {
            return new ManagedOverlayItem(p, title, snippet);
        }

    }
}
