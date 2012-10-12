package com.alarmworkflow.app;

import java.util.Date;

/**
 * Represents an Operation that was fetched from the AlarmWorkflow-service.
 * 
 * @author Chris
 * 
 */
public class Operation {

	/**
	 * The ID of the operation.
	 */
	public int OperationID;
	/**
	 * The Timestamp of the operation. This is usually parsed from the fax.
	 */
	public Date Timestamp;
	/**
	 * The Operation number (not to be confused with OperationID!). This is call
	 * center-specific!
	 */
	public String OperationNumber;
	/**
	 * The name/address of the guy who told the call center about the
	 * accident/whatever.
	 */
	public String Messenger;
	/**
	 * The name of the location of the accident.
	 */
	public String Location;
	/**
	 * The name of the street of the accident. May contain the StreetNumber,
	 * depending on fax format (call center-specific).
	 */
	public String Street;
	/**
	 * The street number of the accident. The street number may be contained
	 * within Street, depending on fax format (call center-specific).
	 */
	public String StreetNumber;
	/**
	 * The name of the city.
	 */
	public String City;
	/**
	 * The zip code of the city.
	 */
	public String ZipCode;
	/**
	 * The name of the property that is affected.
	 */
	public String Property;
	/**
	 * Some optional comment or hint, which may tell short details about the
	 * accident (call center-specific).
	 */
	public String Comment;
	/**
	 * A specific keyword associated with the accident (call center-specific).
	 */
	public String Keyword;
	/**
	 * Whether or not this operation is acknowledged. Acknowledged operations
	 * are usually past operations.
	 */
	public boolean IsAcknowledged;
	
	/**
	 * Default constructor
	 */
	public Operation(){
		this.Timestamp = new Date();
	}
	
	@Override
	public String toString() {
		// TODO Auto-generated method stub
		return super.toString();
	}
	
	/* (non-Javadoc)
	 * @see java.lang.Object#equals(java.lang.Object)
	 */
	@Override
	public boolean equals(Object o) {

		if(o.getClass() == this.getClass()){
			Operation oo = (Operation)o;
			return oo.OperationID == this.OperationID;
		}
		
		return super.equals(o);
	}
	
}
