package de.android.overlaymanager;

import android.content.Context;
import android.graphics.drawable.Drawable;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.graphics.Color;
import com.google.android.maps.MapView;
import com.google.android.maps.Overlay;


import java.util.List;
import java.util.ArrayList;

public class OverlayManager {

    List<ManagedOverlay> overlays = new ArrayList<ManagedOverlay>();
    MapView mapView;
    Context ctx;

    /**
     * Populates layers to the MapView.
     * Overlayer not managed by OverlayManager will be untouched.
     */
    public void populate() {
        List<Overlay> mapoverlays = mapView.getOverlays();
        List<Overlay> newoverlays = new ArrayList<Overlay>();

        for (int i = 0; i < mapoverlays.size(); i++) {
            Overlay overlay = mapoverlays.get(i);
            if (!(overlay instanceof ManagedOverlay)) {
                newoverlays.add(overlay);
            }
        }
        for (int i = 0; i < overlays.size(); i++) {
            ManagedOverlay overlay = overlays.get(i);
            overlay.init();
            newoverlays.add(overlay);
        }
        mapoverlays.clear();
        mapoverlays.addAll(newoverlays);
        mapView.invalidate();
    }

    public void close(){
        for (ManagedOverlay managedOverlay : overlays) {
            managedOverlay.close();
        }
    }

    public Context getContext() {
        return ctx;
    }

    public OverlayManager(Context ctx, MapView mapView) {
        this.mapView = mapView;
        this.ctx = ctx;
    }

    public ManagedOverlay createOverlay(String name, Drawable defaultMarker) {
        ManagedOverlay overlay = new ManagedOverlay(this, name, defaultMarker);
        overlays.add(overlay);
        return overlay;
    }

    public ManagedOverlay createOverlay(Drawable defaultMarker) {
        return createOverlay(null, defaultMarker);
    }

    public ManagedOverlay createOverlay(String name) {
        return createOverlay(name, createDefaultMarker());
    }

    public ManagedOverlay createOverlay() {
        return createOverlay(null, createDefaultMarker());
    }

    public boolean removeOverlay(ManagedOverlay overlay) {
        if (overlay != null) {
            this.overlays.remove(overlay);
            return true;
        } else {
            return false;
        }
    }

    public boolean removeOverlay(String name) {
        ManagedOverlay o = getOverlay(name);
        return removeOverlay(o);
    }

    public ManagedOverlay getOverlay(int i) {
        if (overlays.size() >= i) {
            return overlays.get(i);
        } else {
            return null;
        }
    }

    public MapView getMapView() {
        return mapView;
    }

    public ManagedOverlay getOverlay(String name) {
        for (int i = 0; i < overlays.size(); i++) {
            ManagedOverlay overlay = overlays.get(i);
            if (name.equals(overlay.getName()))
                return overlay;
        }
        return null;
    }

    protected Drawable createDefaultMarker() {
        Bitmap bitmap = Bitmap.createBitmap(16, 16, Bitmap.Config.ARGB_8888);
        Canvas canvas = new Canvas(bitmap);
        Paint p = new Paint();
        p.setColor(Color.BLUE);
        p.setAlpha(50);
        p.setAntiAlias(true);
        canvas.drawCircle(8, 8, 8, p);
        BitmapDrawable bd = new BitmapDrawable(bitmap);
        bd.setBounds(0, 0, bd.getIntrinsicWidth(), bd.getIntrinsicHeight());
          return bd;
    }

}
