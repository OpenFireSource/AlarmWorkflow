package com.alarmworkflow.app;

import java.util.List;

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
			String inflater = Context.LAYOUT_INFLATER_SERVICE;
			LayoutInflater vi = (LayoutInflater) getContext().getSystemService(inflater);
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
		// TODO More elaborate please (or override toString()-function).
		StringBuilder sbHeadlineText = new StringBuilder();
		sbHeadlineText.append(operation.OperationNumber);
		sbHeadlineText.append(", ");
		sbHeadlineText.append(operation.Location);
		sbHeadlineText.append(", ");
		sbHeadlineText.append(operation.Street);
		sbHeadlineText.append(", ");
		sbHeadlineText.append(operation.ZipCode);
		sbHeadlineText.append(" ");
		sbHeadlineText.append(operation.City);			
		
		headlineText.setText(sbHeadlineText.toString());
		headlineDate.setText(operation.Timestamp.toString());

		return operationView;
	}
}
