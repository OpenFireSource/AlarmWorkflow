package de.android.overlaymanager;

import android.view.GestureDetector;
import android.view.MotionEvent;
import android.os.Handler;
import android.content.Context;
import com.google.android.maps.GeoPoint;

public class ManagedOverlayGestureDetector extends GestureDetector {

    public static final String LOG_TAG = "ManagedOverlayGestureDetector";

    private ManagedOverlay overlay;
    private OnOverlayGestureListener overlayOnGestureListener;
    private OnGestureListener onGestureListener;
    private ManagedGestureListener managedGestureListener;

    protected GeoPoint lastTapPoint = null;
    protected ManagedOverlayItem lastTappedOverlayItem;
    protected MotionEvent longPressMotionEvent;
    protected ZoomEvent zoomEvent;
    protected boolean inLongPress;
    protected boolean inLongPressMoved;
    protected boolean inMoving;
    protected MotionEvent movingMotionEvent;
    protected MotionEvent movingMotionEvent1;
    protected float movingV;
    protected float movingV1;

    public ManagedOverlayGestureDetector(ManagedGestureListener managedGestureListener, ManagedOverlay overlay, Handler handler) {
        this(null, managedGestureListener, overlay, handler);
    }

    public ManagedOverlayGestureDetector(ManagedGestureListener managedGestureListener, ManagedOverlay overlay) {
        this(null, managedGestureListener, overlay, null);
    }

    public ManagedOverlayGestureDetector(Context context, ManagedGestureListener managedGestureListener, ManagedOverlay overlay) {
        this(context, managedGestureListener, overlay, null);
    }

    public ManagedOverlayGestureDetector(Context context, ManagedGestureListener managedGestureListener, ManagedOverlay overlay, Handler handler) {
        super(context, managedGestureListener, handler);
        this.overlay = overlay;
        this.managedGestureListener = managedGestureListener;
        this.managedGestureListener.setDetector(this);
    }

    public boolean invokeZoomEvent(int lastZoomLevel, int zoomLevel) {
        if (overlayOnGestureListener != null || overlay.isLazyLoadEnabled) {
            ZoomEvent zoomEvent = new ZoomEvent();
            zoomEvent.setEventTime(System.currentTimeMillis());
            zoomEvent.setZoomLevel(zoomLevel);
            if (lastZoomLevel > zoomLevel)
                zoomEvent.setAction(ZoomEvent.ZOOM_OUT);
            else
                zoomEvent.setAction(ZoomEvent.ZOOM_IN);
            this.zoomEvent = zoomEvent;
            //overlay.zoomFinished = true;
            invokeZoomFinished();
        }
        return true;
    }

    protected void invokeZoomFinished() {
        if (this.getOverlayOnGestureListener() != null) {
            this.overlayOnGestureListener.onZoom(zoomEvent, this.overlay);
            this.resetState();
            this.overlay.invokeLazyLoad(1000);
        }
    }

    protected void invokeLongPressFinished() {
        if (this.getOverlayOnGestureListener() != null) {
            this.getOverlayOnGestureListener().onLongPressFinished(longPressMotionEvent, overlay, lastTapPoint, lastTappedOverlayItem);
            if(this.inLongPressMoved)
                overlay.invokeLazyLoad(0);
            this.resetState();
          
        }
    }

    public OnOverlayGestureListener getOverlayOnGestureListener() {
        return overlayOnGestureListener;
    }

    public void setOverlayOnGestureListener(OnOverlayGestureListener overlayOnGestureListener) {
        this.overlayOnGestureListener = overlayOnGestureListener;
    }

    public OnGestureListener getOnGestureListener() {
        return onGestureListener;
    }

    public void setOnGestureListener(OnGestureListener onGestureListener) {
        this.onGestureListener = onGestureListener;
    }

    @Override
    public boolean onTouchEvent(MotionEvent motionEvent) {
        if (inMoving && motionEvent.getAction() == MotionEvent.ACTION_UP) {
            if (this.getOverlayOnGestureListener() != null) {
                this.getOverlayOnGestureListener().onScrolled(movingMotionEvent, movingMotionEvent1, movingV, movingV1, overlay);
                this.inMoving = false;
                this.resetState();
                this.overlay.invokeLazyLoad(0);
            }
        }
        if (inLongPress && !inMoving && motionEvent.getAction() == MotionEvent.ACTION_UP) {
            this.overlay.longPressFinished = true;
        }
        if (inLongPress && motionEvent.getAction() == MotionEvent.ACTION_MOVE) {
            this.inLongPressMoved = true;
        }

        return super.onTouchEvent(motionEvent);
    }

    protected void onTap(GeoPoint p) {
        this.lastTapPoint = p;
    }

    protected boolean onTap(int index) {
        this.lastTappedOverlayItem = overlay.getItem(index);
        return true;
    }

    protected boolean resetState() {
        if (isReset())
            return false;
        //this.lastTapPoint = null;
        this.lastTappedOverlayItem = null;
        this.inLongPress = false;
        this.inLongPressMoved = false;
        this.inMoving = false;
        this.zoomEvent = null;
        this.longPressMotionEvent = null;
        this.inLongPressMoved = false;
        return true;
    }

    protected boolean isReset() {
        return (this.lastTappedOverlayItem == null && this.lastTapPoint == null);
    }

    public static class ManagedGestureListener extends GestureDetector.SimpleOnGestureListener {
        public static final String LOG_TAG = "ManagedGestureListener";
        protected ManagedOverlay overlay;
        protected ManagedOverlayGestureDetector detector;

        public ManagedGestureListener(ManagedOverlay overlay) {
            super();
            this.overlay = overlay;
        }

        public void setDetector(ManagedOverlayGestureDetector detector) {
            this.detector = detector;
        }

        @Override
        public void onLongPress(MotionEvent e) {
            if (!detector.inMoving) {
                detector.inLongPress = true;
                detector.longPressMotionEvent = e;
            }
            if (detector.getOnGestureListener() != null)
                detector.getOnGestureListener().onLongPress(e);
            super.onLongPress(e);
        }

        @Override
        public boolean onSingleTapConfirmed(MotionEvent e) {
            if (detector.getOverlayOnGestureListener() != null)
                detector.getOverlayOnGestureListener().onSingleTap(e, overlay, detector.lastTapPoint, detector.lastTappedOverlayItem);
            return super.onSingleTapConfirmed(e);
        }

        @Override
        public boolean onSingleTapUp(MotionEvent e) {
            if (detector.getOnGestureListener() != null)
                detector.getOnGestureListener().onSingleTapUp(e);
            return super.onSingleTapUp(e);
        }

        @Override
        public boolean onDoubleTap(MotionEvent e) {
            if (detector.getOverlayOnGestureListener() != null) {
                detector.getOverlayOnGestureListener().onDoubleTap(e, overlay, detector.lastTapPoint, detector.lastTappedOverlayItem);
            }
            return super.onDoubleTap(e);
        }

        @Override
        public boolean onDoubleTapEvent(MotionEvent e) {
            return super.onDoubleTapEvent(e);
        }

        @Override
        public boolean onDown(MotionEvent e) {
            detector.resetState();
            if (detector.getOnGestureListener() != null)
                detector.getOnGestureListener().onDown(e);
            return super.onDown(e);
        }

        @Override
        public void onShowPress(MotionEvent e) {
            if (detector.getOnGestureListener() != null)
                detector.getOnGestureListener().onShowPress(e);
            if (detector.getOverlayOnGestureListener() != null)
                detector.getOverlayOnGestureListener().onLongPress(e, overlay);
            super.onShowPress(e);
        }

        @Override
        public boolean onScroll(MotionEvent motionEvent, MotionEvent motionEvent1, float v, float v1) {
            detector.inMoving = true;
            detector.inLongPress = false;
            detector.movingMotionEvent = motionEvent;
            detector.movingMotionEvent1 = motionEvent1;
            detector.movingV = v;
            detector.movingV1 = v1;
            if (detector.getOnGestureListener() != null)
                detector.getOnGestureListener().onScroll(motionEvent, motionEvent1, v, v1);
            return super.onScroll(motionEvent, motionEvent1, v, v1);
        }

        @Override
        public boolean onFling(MotionEvent motionEvent, MotionEvent motionEvent1, float v, float v1) {
            if (detector.getOnGestureListener() != null)
                detector.getOnGestureListener().onFling(motionEvent, motionEvent1, v, v1);
            overlay.invokeLazyLoad(1000);
            return super.onFling(motionEvent, motionEvent1, v, v1);
        }
    }

    public static interface OnOverlayGestureListener {

        public boolean onZoom(ZoomEvent zoom, ManagedOverlay overlay);

        public boolean onDoubleTap(MotionEvent e, ManagedOverlay overlay, GeoPoint point, ManagedOverlayItem item);

        public void onLongPress(MotionEvent e, ManagedOverlay overlay);

        public void onLongPressFinished(MotionEvent e, ManagedOverlay overlay, GeoPoint point, ManagedOverlayItem item);

        public boolean onScrolled(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY, ManagedOverlay overlay);

        public boolean onSingleTap(MotionEvent e, ManagedOverlay overlay, GeoPoint point, ManagedOverlayItem item);
    }
}
