package de.android.overlaymanager.lazyload;

import de.android.overlaymanager.ManagedOverlay;

public interface LazyLoadListener {

    public void onBegin(ManagedOverlay overlay);
    public void onSuccess(ManagedOverlay overlay);
    public void onError(LazyLoadException exception, ManagedOverlay overlay);
    
}
