package com.alarmworkflow.eAlarm;

import java.text.ParseException;

import android.app.ListActivity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.database.Cursor;
import android.graphics.Typeface;
import android.os.Bundle;
import android.view.ContextMenu;
import android.view.ContextMenu.ContextMenuInfo;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView.AdapterContextMenuInfo;
import android.widget.ListView;
import android.widget.SimpleCursorAdapter;
import android.widget.TextView;

import com.alarmworkflow.eAlarm.database.DatabaseAdapter;
import com.alarmworkflow.eAlarm.database.NotificationMessage;
import com.alarmworkflow.eAlarm.database.NotificationRule;
import com.alarmworkflow.eAlarm.general.NotificationService;

public class MessageList extends ListActivity {
	private NotificationRule rule = null;
	private SimpleCursorAdapter adapter;
	private static final int DELETE_ID = 1;
	private BroadcastReceiver receiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			updateNotifications();
		}
	};

	/** Called when the activity is first created. */
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		// Set the layout.
		setContentView(R.layout.screen_recent_messages);
		//refreshView(); //not required anymore as we call the method now in onResume ?!?
	}

	@SuppressWarnings("deprecation")
	public void refreshView() {
		// Set up our cursor and list adapter. This automatically updates
		// as messages are updated and changed.
		Cursor cursor = NotificationMessage.FACTORY.cursorList(this,
				this.getRule());
		this.startManagingCursor(cursor);
		adapter = new SimpleCursorAdapter(
				this,
				R.layout.message_list_row,
				cursor,
				new String[] { DatabaseAdapter.KEY_TITLE,
						DatabaseAdapter.KEY_MESSAGE,
						DatabaseAdapter.KEY_TIMESTAMP, DatabaseAdapter.KEY_SEEN },
				new int[] { R.id.message_row_title, R.id.message_row_text,
						R.id.message_row_timestamp });

		adapter.setViewBinder(new MessageViewBinder());

		this.setListAdapter(adapter);
		ListView list = (ListView) findViewById(android.R.id.list);
		registerForContextMenu(list);

	}

	@Override
	public void onCreateContextMenu(ContextMenu menu, View v,
			ContextMenuInfo menuInfo) {
		menu.add(0, DELETE_ID, 0, R.string.delete).setIcon(
				android.R.drawable.ic_menu_delete);
	}

	public boolean onContextItemSelected(MenuItem item) {
		AdapterContextMenuInfo info = (AdapterContextMenuInfo) item
				.getMenuInfo();
		switch (item.getItemId()) {
		case DELETE_ID:

			// String[] menuItems = getRerules().getStringArray(R.array.menu);
			// String menuItemName = menuItems[menuItemIndex];
			// String listItemName = Countries[info.position];

			NotificationMessage.FACTORY.deleteById(this, (int) info.id);
			refreshView();
			return true;
		default:
			return super.onContextItemSelected(item);
		}
	}

	public void updateNotifications() {
		Intent intentData = new Intent(getBaseContext(),
				NotificationService.class);
		intentData.putExtra("operation", "update");
		if (this.getRule() != null) {
			intentData.putExtra("ruleId", this.getRule().getId());
		}
		startService(intentData);

		refreshView();
	}

	public void onResume() {
		super.onResume();

		// Force updating the rule.
		this.rule = null;

		// And tell the notification service to clear the notification.
		if (this.getRule() != null) {
			// Set the title of this activity.
			setTitle(String.format(getString(R.string.messages_rule_title),
					this.getRule().getTitle()));
			updateNotifications();
		} else {
			// Set the title of this activity.
			setTitle(getString(R.string.messages_all_title));
		}

		IntentFilter filterSend = new IntentFilter();
		filterSend.addAction("com.alarmworkflow.eAlarm.revicedMessage");
		registerReceiver(receiver, filterSend);
		
		refreshView();
	}

	/**
	 * Fetch the rule that this message list is for (optional!)
	 * 
	 * @return
	 */
	public NotificationRule getRule() {
		if (this.rule == null) {
			// Get the rule from the intent.
			// We store it in a private variable to save us having to query the
			// DB each time.
			Intent ruleIntent = getIntent();
			Long ruleId = ruleIntent.getLongExtra("ruleId", 0);

			if (ruleId > 0) {
				this.rule = NotificationRule.FACTORY.get(this, ruleId);
			}
		}

		return this.rule;
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		MenuInflater inflater = getMenuInflater();
		inflater.inflate(R.menu.messagemenu, menu);
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		switch (item.getItemId()) {
		case R.id.delete_all:
			deleteAll(false);
			return true;
		case R.id.delete_read:
			deleteAll(true);
			return true;
		case R.id.mark_seen:
			markAllAsSeen();
			return true;
		case R.id.home:
			Intent homeIntent = new Intent(this, eAlarmPush.class);
			this.startActivity(homeIntent);
			return true;
		}

		return super.onOptionsItemSelected(item);
	}

	public void deleteAll(boolean onlySeen) {
		// Delete all messages. Optionally, those matching the given rule.
		NotificationMessage.FACTORY.deleteMessagesByRule(this, rule,
				onlySeen);
		updateNotifications();
	}

	public void markAllAsSeen() {
		Cursor cursor = NotificationMessage.FACTORY.cursorList(this,
				this.getRule());
		while (cursor.moveToNext()) {
			String id = cursor.getString(cursor
					.getColumnIndex(DatabaseAdapter.KEY_ID));
			NotificationMessage message = NotificationMessage.FACTORY.get(this,
					Long.parseLong(id));
			if (!message.getSeen()) {

			}
		}
		NotificationMessage.FACTORY.markAllAsSeen(this, this.getRule());
		updateNotifications();
	}

	/**
	 * When an item in the listview is clicked...
	 */
	protected void onListItemClick(ListView l, View v, int position, long id) {
		
		// Launch the message detail.
		Intent intent = new Intent(getBaseContext(), MessageDetail.class);
		intent.putExtra("messageId", id);
		startActivity(intent);
	}
	public void onPause() {
        super.onPause();
        this.unregisterReceiver(this.receiver);
    }
	/**
	 * List item view binding class - used to format dates and make the title
	 * bold.
	 * 
	 * @author daniel
	 */
	private class MessageViewBinder implements SimpleCursorAdapter.ViewBinder {
		public boolean setViewValue(View view, Cursor cursor, int columnIndex) {
			// Format the timestamp as local time.
			if (columnIndex == cursor
					.getColumnIndex(DatabaseAdapter.KEY_TIMESTAMP)) {
				TextView timestamp = (TextView) view;
				try {
					timestamp
							.setText(NotificationMessage.formatUTCAsLocal(NotificationMessage.parseISO8601String(cursor.getString(cursor
									.getColumnIndex(DatabaseAdapter.KEY_TIMESTAMP)))));
				} catch (ParseException ex) {
					timestamp.setText("UNKNOWN");
				}
				return true;
			}
			// Make the title bold if it's unseen.
			if (columnIndex == cursor.getColumnIndex(DatabaseAdapter.KEY_TITLE)) {
				TextView title = (TextView) view;
				title.setText(cursor.getString(cursor
						.getColumnIndex(DatabaseAdapter.KEY_TITLE)));

				if (cursor.getLong(cursor
						.getColumnIndex(DatabaseAdapter.KEY_SEEN)) == 0) {
					title.setTypeface(Typeface.DEFAULT_BOLD);
				} else {
					title.setTypeface(Typeface.DEFAULT);
				}
				return true;
			}

			return false;
		}
	}
}