package com.alarmworkflow.eAlarm.database;

import java.util.ArrayList;

import android.content.ContentUris;
import android.content.ContentValues;
import android.content.Context;
import android.content.UriMatcher;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.net.Uri;

public abstract class DatabaseObject<T>
{
	protected static final String TAG = "eAlarm";	

	protected static final String DATABASE_NAME = "eAlarm.db";
	protected static final String DATABASE_TABLE_RULES = "rules";
	protected static final String DATABASE_TABLE_MESSAGES = "messages";
	

	public static final String PROVIDER_NAME_RULES = "com.alarmworkflow.eAlarm.provider.Rules";
	public static final String PROVIDER_NAME_MESSAGES = "com.alarmworkflow.eAlarm.provider.Messages";
	
    public static final Uri CONTENT_URI_RULES = Uri.parse("content://"+ PROVIDER_NAME_RULES + "/rules");
    public static final Uri CONTENT_URI_MESSAGES = Uri.parse("content://"+ PROVIDER_NAME_MESSAGES + "/messages");
    
	 protected static final int RULES = 3;
	 protected static final int RULES_ID = 4;
	 protected static final int MESSAGES = 5;
	 protected static final int MESSAGE_ID = 6;  
	
	protected static final UriMatcher uriMatcher;
    static
    {
    	uriMatcher = new UriMatcher(UriMatcher.NO_MATCH);
    	uriMatcher.addURI(PROVIDER_NAME_RULES, "rules", RULES);
    	uriMatcher.addURI(PROVIDER_NAME_RULES, "rules/#", RULES_ID);
    	uriMatcher.addURI(PROVIDER_NAME_MESSAGES, "messages", MESSAGES);
    	uriMatcher.addURI(PROVIDER_NAME_MESSAGES, "messages/#", MESSAGE_ID);
    }
    
	
	private Uri getContentUriFor( Uri uri )
	{
		switch( uriMatcher.match(uri) )
		{
			case RULES:
			case RULES_ID:
				return CONTENT_URI_RULES;
			case MESSAGES:
			case MESSAGE_ID:
				return CONTENT_URI_MESSAGES;
			default:
				throw new IllegalArgumentException("Unsupported URI: " + uri);
		}
	}	
	
	private String getTableFor( Uri uri )
	{
		switch( uriMatcher.match(uri) )
		{
			case RULES:
			case RULES_ID:
				return DATABASE_TABLE_RULES;
			case MESSAGES:
			case MESSAGE_ID:
				return DATABASE_TABLE_MESSAGES;
			default:
				throw new IllegalArgumentException("Unsupported URI: " + uri);
		}
	}
	/**
	 * The ID of this object.
	 */
	protected Long id;
	
	/**
	 * Save this object to the database.
	 * @param context
	 */
	public void save( Context context )
	{
		SQLiteDatabase db = context.openOrCreateDatabase(DATABASE_NAME, 0, null);
		if( this.getId() == null )
		{
			// Insert.
			ContentValues values = this.flatten();
			long rowID = db.insert(this.getTableFor(this.getContentUri()), "", values);
			
			// And on success...
			if( rowID > 0 )
			{
				Uri uri = ContentUris.withAppendedId(this.getContentUriFor(this.getContentUri()), rowID);
				this.setId(Long.parseLong(uri.getPathSegments().get(1)));
			}
		}
		else
		{
			// Update.
			ContentValues values = this.flatten();	
			db.update(this.getTableFor(this.getContentUri()), values, DatabaseAdapter.KEY_ID + "=" + this.getId(), null);
		}
	}
	
	/**
	 * Get the URI of this item.
	 * @return
	 */
	public Uri getItemUri()
	{
		return ContentUris.withAppendedId(this.getContentUri(), this.getId());
	}
	
	/**
	 * Delete this object.
	 * @param context
	 */
	public void delete( Context context )
	{
		SQLiteDatabase db = context.openOrCreateDatabase(DATABASE_NAME, 0, null);
		db.delete(this.getTableFor(this.getContentUri()), DatabaseAdapter.KEY_ID + "=" + this.getId(), null);
	}
	
	/**
	 * Factory method - delete the object when only the ID is known.
	 * @param context
	 * @param id
	 */
	public void deleteById( Context context, Long id )
	{
		SQLiteDatabase db = context.openOrCreateDatabase(DATABASE_NAME, 0, null);
		db.delete(this.getTableFor(this.getContentUri()), DatabaseAdapter.KEY_ID + "=" + id, null);
	}
	
	/**
	 * Generic delete - delete by arbitrary parameters.
	 * @param context
	 * @param selection
	 * @param selectionArgs
	 */
	protected void genericDelete( Context context, String selection, String[] selectionArgs )
	{
		SQLiteDatabase db = context.openOrCreateDatabase(DATABASE_NAME, 0, null);
		db.delete(this.getTableFor(this.getContentUri()), selection, selectionArgs);
	}
	
	/**
	 * Get a single instance of this class by ID.
	 * @param context
	 * @param id
	 * @return
	 */
	public T get( Context context, Long id )
	{
		return this.getOne(context, DatabaseAdapter.KEY_ID + "=" + id, null);
	}
	
	/**
	 * Get the ID of this object - NULL if not yet saved.
	 * @return
	 */
	public Long getId()
	{
		return this.id;
	}

	/**
	 * Set the ID of this object.
	 * @param id
	 */
	protected void setId( Long id )
	{
		this.id = id;
	}

	
	/**
	 * List entries from the database, inflating them as required.
	 * @param context
	 * @param selection
	 * @param selectionArgs
	 * @param sortOrder
	 * @return
	 */
	protected ArrayList<T> genericList( Context context, String selection, String[] selectionArgs, String sortOrder )
	{
		String table = this.getTableFor(this.getContentUri());
		String fields = "";
		for(int i=0; i< this.getProjection().length; i++)
		{
			fields += this.getProjection()[i];
		    if(i < this.getProjection().length - 1)
		    	fields += ",";
		}
		
		String query = "select " + fields + " from " + table; 
		
		if(selection != null && selection != "")
		{
			if(selection.contains("?"))
			{
				StringBuffer s1 = new StringBuffer(selection);
				int i=0;
				int index = s1.indexOf("?");
				while(index >= 0)
				{
					s1.replace(index, index+1, "'" + selectionArgs[i] + "'");
					i++;
					index = s1.indexOf("?", index+1);
				}
				selection = s1.toString();
			}
			
			query += " where " + selection;
		}
		
		if(sortOrder != null && sortOrder != "")
			query += " order by " + sortOrder;

		SQLiteDatabase db = context.openOrCreateDatabase(DATABASE_NAME, 0, null);
		Cursor cursor = db.rawQuery(query, null);
				
		ArrayList<T> result = new ArrayList<T>();
		if( cursor.moveToFirst() )
		{
			do
			{
				result.add(this.inflate(context, cursor));
			}
			while( cursor.moveToNext() );
		}
		cursor.close();
		return result;
	}
	
	
	/**
	 * Count entries from the database, without inflating them.
	 * @param context
	 * @param selection
	 * @param selectionArgs
	 * @return
	 */
	protected int genericCount( Context context, String selection, String[] selectionArgs )
	{
		String table = this.getTableFor(this.getContentUri());
		String fields = "";
		for(int i=0; i< this.getProjection().length; i++)
		{
			fields += this.getProjection()[i];
		    if(i < this.getProjection().length - 1)
		    	fields += ",";
		}
		String query = "select " + fields + " from " + table; 
		
		if(selection != null && selection != "")
		{
			if(selection.contains("?"))
			{
				StringBuffer s1 = new StringBuffer(selection);
				int i=0;
				int index = s1.indexOf("?");
				while(index >= 0)
				{
					s1.replace(index, index+1, "'" + selectionArgs[i] + "'");
					i++;
					index = s1.indexOf("?", index+1);
				}
				selection = s1.toString();
			}
			
			query += " where " + selection;
		}

		SQLiteDatabase db = context.openOrCreateDatabase(DATABASE_NAME, 0, null);
		Cursor cursor = db.rawQuery(query, null);
		
		int count = cursor.getCount();
		
		cursor.close();
		return count;
	}	
	
	/**
	 * Get a single entry from the database matching the query, or NULL if not found.
	 * @param context
	 * @param selection
	 * @param selectionArgs
	 * @return
	 */
	protected T getOne( Context context, String selection, String[] selectionArgs )
	{
		ArrayList<T> list = this.genericList(context, selection, selectionArgs, null);
		
		if( list.size() == 0 )
		{
			return null;
		}
		else
		{
			return list.get(0);
		}
	}

	/**
	 * Get the content URI for this ORM type.
	 * @return
	 */
	public abstract Uri getContentUri();
	
	/**
	 * Flatten the objects data into a set of content values.
	 * @return
	 */
	protected abstract ContentValues flatten();
	/**
	 * Inflate this object from the given cursor.
	 * @param cursor
	 * @return
	 */
	protected abstract T inflate( Context context, Cursor cursor );
	/**
	 * Get the projection required when querying this object.
	 * @return
	 */
	protected abstract String[] getProjection();
}