using System;
using System.Security.Principal;

namespace AlarmWorkflow.Windows.Configuration
{
    static class Helper
    {
        internal static bool IsCurrentUserAdministrator()
        {
            // Courtesy of http://stackoverflow.com/questions/1089046/in-net-c-test-if-user-is-an-administrative-user
            //bool value to hold our return value
            bool isAdmin;
            try
            {
                //get the currently logged in user
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException)
            {
                isAdmin = false;
            }
            catch (Exception)
            {
                isAdmin = false;
            }
            return isAdmin;
        }
    }
}
