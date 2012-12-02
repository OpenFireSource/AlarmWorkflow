package de.android.overlaymanager;

public class ZoomEvent {
    public static final int ZOOM_IN = 1;
    public static final int ZOOM_OUT = -1;

    private int action;
    private long eventTime;
    private int zoomLevel;

    public int getAction() {
        return action;
    }

    public void setAction(int action) {
        this.action = action;
    }

    public long getEventTime() {
        return eventTime;
    }

    public void setEventTime(long eventTime) {
        this.eventTime = eventTime;
    }

    public int getZoomLevel() {
        return zoomLevel;
    }

    public void setZoomLevel(int zoomLevel) {
        this.zoomLevel = zoomLevel;
    }

    @Override
    public String toString() {
        return new StringBuilder("ZoomEvent={").append("action=").append(action).append(" eventTime=").append(eventTime).append(" zoomLevel=").append(zoomLevel).append("}").toString();
    }
}
