package com.alarmworkflow.app.util;

import java.util.List;

import com.alarmworkflow.app.R;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.LinearLayout;
import android.widget.TextView;

public class OperationsAdapter extends ArrayAdapter<Operation> {

	private int _resource;

	public OperationsAdapter(Context context, int textViewResourceId, List<Operation> objects) {
		super(context, textViewResourceId, objects);

		this._resource = textViewResourceId;
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent) {
		
		LinearLayout operationView;
		// Get the current alert object
		Operation operation = getItem(position);

		// Inflate the view
		if (convertView == null) {
			operationView = new LinearLayout(getContext());
			LayoutInflater vi = (LayoutInflater) getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
			vi.inflate(_resource, operationView, true);
		} 
		else
		{
			operationView = (LinearLayout) convertView;
		}
		// Get the text boxes from the listitem.xml file
		TextView headlineText = (TextView) operationView.findViewById(R.id.txtHeadline);
		TextView headlineDate = (TextView) operationView.findViewById(R.id.txtTimestamp);

		// Assign the appropriate data from our alert object above
		StringBuilder sbHeadlineText = new StringBuilder();
		sbHeadlineText.append(operation.OperationNumber);
		sbHeadlineText.append(", ");
		if (operation.Keyword != "" && operation.Keyword != "null") {
			sbHeadlineText.append(operation.Keyword);
			sbHeadlineText.append(", ");
		}
		if (operation.Comment != "" && operation.Comment != "null") {
			sbHeadlineText.append(operation.Comment);
			sbHeadlineText.append(", ");
		}

		headlineText.setText(sbHeadlineText.toString());
		headlineDate.setText(operation.Timestamp.toString());

		return operationView;
	}
	
	
}
