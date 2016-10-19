using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnterpriseArchitectAutoRefresh
{
    /// <summary>
    /// This class contains EA-related Plugin Code and provides the menu structure and exit points.
    /// </summary>
    public class AutoRefreshAddin
    {
        // Menu Constants
        const string menuHeader = "-&AutoRefresh";
        const string menuActivate = "&Activate";
        const string menuDeactivate = "&Deactivate";
        const string menuInfo = "&Info";

        // Auto Refresh
        private AutoRefresh autoRefresh;

        /// <summary>
        /// Is called when EA loads the addin.
        /// </summary>
        /// <param name="Repository">The repository</param>
        /// <returns>Nothing to do here</returns>
        public String EA_Connect(EA.Repository Repository)
        {
            //No special processing required.
            this.autoRefresh = new AutoRefresh(Repository);
            return "";
        }

        ///
        /// Called when user Clicks Add-Ins Menu item from within EA.
        /// Populates the Menu with our desired selections.
        /// Location can be "TreeView" "MainMenu" or "Diagram".
        ///
        /// <param name="Repository" />the repository
        /// <param name="Location" />the location of the menu
        /// <param name="MenuName" />the name of the menu
        ///
        public object EA_GetMenuItems(EA.Repository Repository, string Location, string MenuName)
        {

            switch (MenuName)
            {
                // defines the top level menu option
                case "":
                    return menuHeader;
                // defines the submenu options
                case menuHeader:
                    string[] subMenus = { menuActivate, menuDeactivate, menuInfo };
                    return subMenus;
            }

            return "";
        }

        ///
        /// returns true if a project is currently opened
        ///
        /// <param name="Repository" />the repository
        /// true if a project is opened in EA
        bool IsProjectOpen(EA.Repository Repository)
        {
            try
            {
                EA.Collection c = Repository.Models;
                return true;
            }
            catch
            {
                return false;
            }
        }

        ///
        /// Called once Menu has been opened to see what menu items should active.
        ///
        /// <param name="Repository" />the repository
        /// <param name="Location" />the location of the menu
        /// <param name="MenuName" />the name of the menu
        /// <param name="ItemName" />the name of the menu item
        /// <param name="IsEnabled" />boolean indicating whethe the menu item is enabled
        /// <param name="IsChecked" />boolean indicating whether the menu is checked
        public void EA_GetMenuState(EA.Repository Repository, string Location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            if (IsProjectOpen(Repository))
            {
                switch (ItemName)
                {
                    case menuActivate:
                        IsEnabled = !autoRefresh.isRunning();
                        break;
                    case menuDeactivate:
                        IsEnabled = autoRefresh.isRunning();
                        break;
                    case menuInfo:
                        IsEnabled = true;
                        break;
                    default:
                        IsEnabled = false;
                        break;
                }
            }
            else
            {
                // If no open project, disable all menu options
                IsEnabled = false;
            }
        }

        ///
        /// Called when user makes a selection in the menu.
        ///
        /// <param name="Repository" />the repository
        /// <param name="Location" />the location of the menu
        /// <param name="MenuName" />the name of the menu
        /// <param name="ItemName" />the name of the selected menu item
        public void EA_MenuClick(EA.Repository Repository, string Location, string MenuName, string ItemName)
        {
            switch (ItemName)
            {
                case menuActivate:
                    this.activate(Repository);
                    break;
                case menuDeactivate:
                    this.deactivate();
                    break;
                case menuInfo:
                    showInfoDialog();
                    break;
            }
        }

        /// <summary>
        /// Displays an info dialog with information about the plugin settings.
        /// </summary>
        private void showInfoDialog()
        {
            StringBuilder info = new StringBuilder("Enterprise Architect Auto Refresh\n");
            info.AppendLine("Settings file: \"" + AppDomain.CurrentDomain.SetupInformation.ConfigurationFile + "\"");

            info.AppendLine("\nRefresh Interval: " + Properties.Settings.Default.refreshInterval);
            info.AppendLine("Table prefix: \"" + Properties.Settings.Default.tablePrefix + "\"");
            info.AppendLine("Show alert first: " + (Properties.Settings.Default.showAlertFirst ? "Yes" : "No"));

            MessageBox.Show(info.ToString(), "Info", MessageBoxButtons.OK);
        }

        ///
        /// Starts the AutoRefresh synchronisation.
        ///
        private void activate(Repository repo)
        {
            autoRefresh.startAsync();
        }

        ///
        /// Stops the AutoRefresh synchronisation before the next iteration.
        ///
        private void deactivate()
        {
            autoRefresh.stop();
        }

        ///
        /// EA calls this operation when it exists. Can be used to do some cleanup work.
        ///
        public void EA_Disconnect()
        {
            autoRefresh.stop();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

    }
}
