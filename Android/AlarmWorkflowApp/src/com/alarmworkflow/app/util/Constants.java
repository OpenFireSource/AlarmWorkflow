package com.alarmworkflow.app.util;

/**
 * Stores various constant values that are needed throughout the application.
 * 
 * @author Chris
 *
 */
public final class Constants {

	/**
	 * Defines the name of the OperationCache file.
	 */
    public static final String PERSISTENT_STORAGE_FILENAME = "AlarmWorkflowAppOperationCache.txt";
    /**
     * Defines the key of the setting determining the limit of operations to get.
     */
    public static final String PREF_KEY_FETCH_AMOUNT = "pref_key_fetch_amount";
    /**
     * Defines the key of the setting determining whether to fetch only non-acknowledged operations.
     */
    public static final String PREF_KEY_FETCH_ONLYNONACKNOWLEDGED = "pref_key_fetch_only_nonacknowledged";
    /**
     * Defines the key of the setting determining the maximum age of operations to get.
     */
    public static final String PREF_KEY_FETCH_MAXAGE = "pref_key_fetch_maxage";
    /**
     * Defines the key of the setting determining the server URI.
     */
    public static final String PREF_KEY_SERVER_URI = "pref_key_server_uri";

}
