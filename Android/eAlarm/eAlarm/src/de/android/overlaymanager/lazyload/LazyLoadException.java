package de.android.overlaymanager.lazyload;

import de.android.overlaymanager.ManagedOverlayException;

public class LazyLoadException extends ManagedOverlayException {

	private static final long serialVersionUID = 1L;

	public LazyLoadException() {
    }

    public LazyLoadException(String s) {
        super(s);
    }

    public LazyLoadException(String s, Throwable throwable) {
        super(s, throwable);
    }

    public LazyLoadException(Throwable throwable) {
        super(throwable);
    }
}