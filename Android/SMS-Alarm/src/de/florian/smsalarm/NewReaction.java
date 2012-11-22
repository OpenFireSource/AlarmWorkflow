package de.florian.smsalarm;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;

public class NewReaction extends Activity {
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.detailview);
		Button saveButton = (Button) this.findViewById(R.id.save);
		saveButton.setOnClickListener(new OnClickListener() {
			public void onClick(View v) {
				String number = ((((TextView) findViewById(R.id.editNumber))
						.getText().toString()));				
				String content = ((TextView) findViewById(R.id.editContent))
						.getText().toString();				
				String name = ((TextView) findViewById(R.id.editName))
						.getText().toString();
				DataSource d = new DataSource(getApplicationContext());
				d.open();

				d.addReaction(number, content,name);
				d.close();
				finish();
			}
		});
	}
}
