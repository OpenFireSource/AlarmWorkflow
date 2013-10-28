/**
 * Sample script for demonstrating how to use the Extended Expression Formatter
 * to insert the result of a custom procedure into the expression string.
 *
 * This script shows how to insert a list of resource names, if any contains the name of your town.
 * It is designed to run within the OperationPrinter job.
 * 
 * Please play around. The best way to edit this script is to drag it onto Visual Studio,
 * so you get as much of IntelliSense as possible.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using AlarmWorkflow.Shared.Core;

public class Script
{
    private const string YourResourceName = "NEA";
    private const string YourResourceDelimiter = "<br/>";

    public static string Function(object graph)
    {
        // The following line accesses the internal object to retrieve the "Operation" property, which we need.
        // We have to do it this way because the 'graph' object represents a type that is internal (hidden) and cannot be accessed directly here.
        // Please note that this line applies to the OperationPrinter only.
        var op = (Operation)graph.GetType().GetProperty("Operation", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).GetValue(graph, null);

        IEnumerable<string> names = op.Resources.Where(r => r.FullName.Contains(YourResourceName)).Select(r => r.FullName);
        return string.Join(YourResourceDelimiter, names);
    }
}