package de.florian.smsalarm;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.Window;
import android.widget.ArrayAdapter;
import android.widget.ListView;

public class SMSAlarmActivity extends Activity {
	private ListView listView;	
	/** Called when the activity is first created. */
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		// Remove title bar
		this.requestWindowFeature(Window.FEATURE_NO_TITLE);
		setContentView(R.layout.main);
		listView = (ListView) findViewById(R.id.ListView);
		// registerForContextMenu(listView);
		fillList();		
	}

	@Override
	public void onResume() {
		super.onResume();
		fileList();
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		MenuInflater inflater = getMenuInflater();
		inflater.inflate(R.menu.menu, menu);
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		switch (item.getItemId()) {
		case R.id.settings:
			startActivity(new Intent(this, Preferences.class));
			return true;
		case R.id.add:
			startActivity(new Intent(this, NewReaction.class));
			return true;
		}
		return false;
	}

	private void fillList() {

		DataSource d = new DataSource(this);
		d.open();
		String[] values = d.getReactionsList();

		// First paramenter - Context
		// Second parameter - Layout for the row
		// Third parameter - ID of the View to which the data is written
		// Forth - the Array of data
		ArrayAdapter<String> adapter = new ArrayAdapter<String>(this,
				R.layout.textview, android.R.id.text1, values);

		// Assign adapter to ListView
		listView.setAdapter(adapter);
		d.close();
	}
	
}