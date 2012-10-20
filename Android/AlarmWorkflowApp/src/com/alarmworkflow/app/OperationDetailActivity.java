package com.alarmworkflow.app;

import com.alarmworkflow.app.util.Helpers;
import com.alarmworkflow.app.util.Operation;

import android.app.Activity;
import android.os.Bundle;
import android.widget.TextView;

public class OperationDetailActivity extends Activity {

	public OperationDetailActivity() {

	}

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_operationdetail);

		// Retrieve bundle from MainActivity
		Bundle bundle = getIntent().getExtras();

		// Get Operation instance out of it, which we shall show in detail
		Operation operation = (Operation) bundle.getSerializable("Operation");

		// Get all needed layout elements and set values
		if (!Helpers.isNullOrWhiteSpace(operation.OperationNumber)) {
			TextView txtOperationNumber = (TextView) findViewById(R.id.txtOperationNumber);
			txtOperationNumber.setText(operation.OperationNumber);
		}

		{
			TextView txtTimestamp = (TextView) findViewById(R.id.txtTimestamp);
			txtTimestamp.setText(operation.Timestamp.toLocaleString());
		}

		if (!Helpers.isNullOrWhiteSpace(operation.Keyword)) {
			TextView txtKeyword = (TextView) findViewById(R.id.txtKeyword);
			txtKeyword.setText(operation.Keyword);
		}

		if (!Helpers.isNullOrWhiteSpace(operation.Comment)) {
			TextView txtComment = (TextView) findViewById(R.id.txtComment);
			txtComment.setText(operation.Comment);
		}

		String destinationString = operation.getDestinationLocation();
		if (!Helpers.isNullOrWhiteSpace(destinationString)) {
			TextView txtDestination = (TextView) findViewById(R.id.txtDestination);
			txtDestination.setText(destinationString);			
		}
	}

}
