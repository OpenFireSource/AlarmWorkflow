package com.alarmworkflow.app.util;

public class Helpers {

	/**
	 * Checks if the given String is either null, of zero length or consists only of whitespaces.
	 * @param string The String to check.
	 * @return Whether or not the given String is either null, of zero length or consists only of whitespaces.
	 */
	public static boolean isNullOrWhiteSpace(final String string){
		return isNullOrWhiteSpace(string, false);
	}
	
	/**	 * 
	 * Checks if the given String is either null, of zero length or consists only of whitespaces.
	 * @param string The String to check.
	 * @param checkForNullText Whether or not to include the text "null" to count as null too.
	 * @return Whether or not the given String is either null, of zero length or consists only of whitespaces.
	 */
	public static boolean isNullOrWhiteSpace(String string, boolean checkForNullText){
		if(string == null){
			return true;
		}
		
		// Keep string for "null"-text-check
		string = string.trim();
		
		boolean isEmpty = string.length() == 0;
		if(!isEmpty && (checkForNullText && string.equalsIgnoreCase("null"))) {
			return true;
		}
		
		return isEmpty;
	}

}
