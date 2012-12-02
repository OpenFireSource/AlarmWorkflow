package de.android.overlaymanager.lazyload;

import android.widget.ImageView;
import android.graphics.drawable.AnimationDrawable;
import android.graphics.drawable.Drawable;
import android.view.View;

import java.io.InputStream;

public class LazyLoadAnimation {

	ImageView imageView;
	public static final int FRAME_DURATION = 150;

	AnimationDrawable anim;
	private boolean first = true;

	public LazyLoadAnimation(ImageView imageView) {
		this.imageView = imageView;
		this.imageView.setVisibility(View.INVISIBLE);
	}

	private void initDefault() {
		String baseurl = "/de/android/overlaymanager/lazyload/anim/";
		anim = new AnimationDrawable();
		anim.setOneShot(false);

		for (int i = 0; i < 8; i++) {
			String uri = baseurl + String.format("loader0%s.png", i);
			InputStream is = getClass().getResourceAsStream(uri);
			Drawable drawable = Drawable.createFromStream(is, null);
			drawable.setBounds(0, 0, drawable.getIntrinsicWidth(), drawable.getIntrinsicHeight());
			anim.addFrame(drawable, FRAME_DURATION);
		}
		this.imageView.setBackgroundDrawable(anim);
	}

	public void stop() {
		imageView.setVisibility(View.INVISIBLE);
		anim.setVisible(false, false);
		anim.stop();
		imageView.postInvalidate();
	}

	public void start() {
		if (first) {
			if (anim == null) {
				initDefault();
				anim.setBounds(0, 0, anim.getMinimumWidth(), anim.getMinimumHeight());
			}
			first = false;
		}
		imageView.setVisibility(View.VISIBLE);
		anim.setVisible(true, true);
		anim.start();
		imageView.postInvalidate();
	}

	public void setAnimationDrawable(AnimationDrawable anim) {
		this.anim = anim;
		this.imageView.setBackgroundDrawable(anim);
	}
}
