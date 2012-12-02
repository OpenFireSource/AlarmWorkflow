package de.android.overlaymanager;

public class ManagedOverlayException extends Exception{

    /**
	 * 
	 */
	private static final long serialVersionUID = 177773476583395692L;

	public ManagedOverlayException() {
    }

    public ManagedOverlayException(String s) {
        super(s);
    }

    public ManagedOverlayException(String s, Throwable throwable) {
        super(s, throwable);
    }

    public ManagedOverlayException(Throwable throwable) {
        super(throwable);
    }
}
