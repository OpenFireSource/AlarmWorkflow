package com.alarmworkflow.app.util;

import java.util.Date;

import java.io.IOException;
import java.io.Serializable;


/**
 * Represents an Operation that was fetched from the AlarmWorkflow-service.
 * 
 * @author Chris
 * 
 */
public class Operation implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 8832004349235095674L;
	
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
	
	/**
	 * Constructs and returns the destination location as a string.
	 * @return The destination location as a string in format: "[[Street] [StreetNumber]], [ZipCode] [City]".
	 */
	public String getDestinationLocation(){
		StringBuilder sb = new StringBuilder();
		
		if (!Helpers.isNullOrWhiteSpace(Street, true))
        {
            sb.append(Street);

            if (!Helpers.isNullOrWhiteSpace(StreetNumber, true))
            {
                sb.append(" ");
                sb.append(StreetNumber);
            }
            sb.append(", ");
        }

        boolean hasZipCode = !Helpers.isNullOrWhiteSpace(ZipCode, true);
        if (hasZipCode)
        {
            sb.append(ZipCode);
        }

        if (!Helpers.isNullOrWhiteSpace(City, true))
        {
            if (hasZipCode)
            {
                sb.append(" ");
            }

            sb.append(City);
        }
		
		return sb.toString().trim();
	}
	
	@Override
	public String toString() {
		// TODO Auto-generated method stub
		return super.toString();
	}

	/**
	 * Returns whether or not this instance and the other instance are equal.
	 * Two instances are considered equal if their OperationIDs do match.
	 * @param o The other instance to compare.
	 * @return Whether or not this instance and the other instance are equal.
	 */
	@Override
	public boolean equals(Object o) {

		if(o.getClass() == this.getClass()){
			Operation oo = (Operation)o;
			return oo.OperationID == this.OperationID;
		}
		
		return super.equals(o);
	}
	
	private void writeObject(java.io.ObjectOutputStream out) throws IOException {
		// write 'this' to 'out'...
		out.writeInt(OperationID);
		out.writeObject(Timestamp);
		out.writeUTF(OperationNumber != null ? OperationNumber : "");
		out.writeUTF(Messenger != null ? Messenger : "");
		out.writeUTF(Location != null ? Location : "");
		out.writeUTF(Street != null ? Street : "");
		out.writeUTF(StreetNumber != null ? StreetNumber : "");
		out.writeUTF(City != null ? City : "");
		out.writeUTF(ZipCode != null ? ZipCode : "");
		out.writeUTF(Property != null ? Property : "");
		out.writeUTF(Comment != null ? Comment : "");
		out.writeUTF(Keyword != null ? Keyword : "");
		out.writeBoolean(IsAcknowledged);
//		out.putFields().put("", OperationNumber);
	}

	private void readObject(java.io.ObjectInputStream in) throws IOException,
			ClassNotFoundException {
		// populate the fields of 'this' from the data in 'in'...
		this.OperationID = in.readInt();
		this.Timestamp = (Date)in.readObject();
		this.OperationNumber = in.readUTF();
		this.Messenger = in.readUTF();
		this.Location = in.readUTF();
		this.Street = in.readUTF();
		this.StreetNumber = in.readUTF();
		this.City = in.readUTF();
		this.ZipCode = in.readUTF();
		this.Property = in.readUTF();
		this.Comment = in.readUTF();
		this.Keyword = in.readUTF();
		this.IsAcknowledged = in.readBoolean();
		
	}

}
