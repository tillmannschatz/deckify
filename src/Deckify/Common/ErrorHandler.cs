using System;
using System.Windows.Forms;

namespace Deckify.Common
{
    public static class ErrorHandler
    {
        public static void SafeExecute(Action action, string actionName)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, actionName);
                MessageBox.Show($"An error occurred during '{actionName}':\n\n{ex.Message}\n\nSee log file for details.", 
                                "Error", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Error);
            }
        }
    }
}
