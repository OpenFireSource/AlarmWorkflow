package com.alarmworkflow.eAlarm;

import java.util.ArrayList;
import java.util.List;

import android.app.AlertDialog;
import android.app.ListActivity;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.res.Configuration;
import android.os.Bundle;
import android.text.Editable;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.EditText;
import android.widget.ListView;

import com.alarmworkflow.eAlarm.database.NotificationRule;

public class RuleList extends ListActivity implements View.OnClickListener {

	/** Called when the activity is first created. */
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.screen_rule);
		getListView().setTextFilterEnabled(true);
		getListView().setClickable(true);
	}

	/** {@inheritDoc} */
	@Override
	protected void onListItemClick(ListView l, View v, int pos, long id) {
		super.onListItemClick(l, v, pos, id);
		NotificationRule rule = (NotificationRule) getListView()
				.getItemAtPosition(pos);
		clickSource(rule);
	}

	@Override
	public void onAttachedToWindow() {
		super.onAttachedToWindow();
		// openOptionsMenu();
	}

	public void onConfigurationChanged(Configuration newConfig) {
		super.onConfigurationChanged(newConfig);
		setContentView(R.layout.screen_rule);
	}

	public void onResume() {
		super.onResume();
		// When coming back, refresh our list of accounts.
		refreshView();

	}

	/**
	 * Refresh the list of rules viewed by this activity.
	 */
	public void refreshView() {
		// Refresh our list of rules.
		List<NotificationRule> rules = NotificationRule.FACTORY.listAll(this);
		ArrayAdapter<NotificationRule> adapter = new ArrayAdapter<NotificationRule>(
				this, android.R.layout.simple_list_item_1, rules);
		this.setListAdapter(adapter);
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		MenuInflater inflater = getMenuInflater();
		inflater.inflate(R.menu.rulemenu, menu);

		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		switch (item.getItemId()) {
		case R.id.add_rule:
			askForSourceName();
			return true;
		}

		return super.onOptionsItemSelected(item);
	}

	/**
	 * Helper function to show a dialog to ask for a rule name.
	 */
	private void askForSourceName() {
		final EditText input = new EditText(this);

		new AlertDialog.Builder(this)
				.setTitle(getString(R.string.create_rule))
				.setMessage(getString(R.string.create_rule_message))
				.setView(input)
				.setPositiveButton(getString(R.string.create),
						new DialogInterface.OnClickListener() {
							public void onClick(DialogInterface dialog,
									int whichButton) {
								Editable value = input.getText();
								if (value.length() > 0) {
									// Fire it off to the create rule
									// function.
									createSource(value.toString());
								}
							}
						})
				.setNegativeButton(getString(R.string.cancel),
						new DialogInterface.OnClickListener() {
							public void onClick(DialogInterface dialog,
									int whichButton) {
								// No need to take any action.
							}
						}).show();
	}

	/**
	 * Helper function to create a rule.
	 * 
	 * @param title
	 */
	public void createSource(String title) {
		NotificationRule rule = new NotificationRule();
		rule.setTitle(title);
		rule.setLocalEnabled(true);
		rule.save(this);
		refreshView();

	}

	/**
	 * Handler for when you click an rule.
	 * 
	 * @param account
	 */
	public void clickSource(NotificationRule rule) {
		// Launch the rule editor.
		Intent intent = new Intent(getBaseContext(), RuleEditor.class);
		intent.putExtra("ruleId", rule.getId());
		startActivity(intent);
	}

	public void onClick(View clickedView) {
		NotificationRule rule = (NotificationRule) clickedView.getTag();
		clickSource(rule);
	}

}