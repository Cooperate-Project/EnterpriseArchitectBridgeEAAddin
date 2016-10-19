using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnterpriseArchitectAutoRefresh
{
    /// <summary>
    /// This class contains the asynchronous synchronisation algorithm of the Plugin.
    /// </summary>
    class AutoRefresh
    {
        // Currently opened Repository
        private Repository repository;

        // Plugin settings
        private String prefix;
        private int refreshInterval;
        private System.Collections.Specialized.StringCollection tables;
        private bool showAlert;

        // Thread information
        private bool running = false;
        Thread thread = null;

        /// <summary>
        /// Creates a new AutoRefresh instance which can be started using startAsync().
        /// </summary>
        /// <param name="repository">The currently opened repository, passed in from EA.</param>
        public AutoRefresh(Repository repository)
        {
            this.repository = repository;

            this.prefix = Properties.Settings.Default.tablePrefix;
            this.refreshInterval = Properties.Settings.Default.refreshInterval;
            this.showAlert = Properties.Settings.Default.showAlertFirst;
            this.tables = Properties.Settings.Default.mappedTables;

            this.running = false;
        }

        /// <summary>
        /// Creates the table listeners from the list of monitored tables loaded from the plugin settings.
        /// </summary>
        /// <returns></returns>
        private List<TableListener> createTableListeners()
        {
            List<TableListener> listenerList = new List<TableListener>();

            foreach(String table in this.tables)
            {
                listenerList.Add(new TableListener(table, prefix, repository));
            }

            return listenerList;
        }

        /// <summary>
        /// Starts a new thread to synchronize the model and datebase asynchronously.
        /// </summary>
        public void startAsync()
        {
            ThreadStart start = new ThreadStart(refreshAll);
            thread = new Thread(start);

            running = true;
            thread.Start();
            
        }

        /// <summary>
        /// This is the real working method, called in every iteration of the synchronisation.
        /// </summary>
        private void refreshAll()
        {
            List<TableListener> listeners = createTableListeners();

            while(running)
            {
                foreach(TableListener listener in listeners)
                {
                    sync(listener);
                }

                Thread.Sleep(refreshInterval);
            }
        }

        /// <summary>
        /// Tries to stop the sync-Thread, finally aborts it.
        /// </summary>
        public void stop()
        {
            running = false;
            Thread.Sleep(refreshInterval);
            
            if(thread != null && thread.IsAlive)
            {
                thread.Abort();
            }
        }

        /// <summary>
        /// Returns if the synchronisation thread is currently running.
        /// </summary>
        /// <returns>True, if the thread is running.</returns>
        public bool isRunning()
        {
            return running;
        }

        /// <summary>
        /// This method takes an Listener (a logging table filled by a trigger), gets the updates and refreshes the affected views.
        /// </summary>
        /// <param name="syncTable"></param>
        private void sync(TableListener syncTable)
        {
            List<String> updates = syncTable.getUpdates();

            foreach (String update in updates)
            {
                // FIXME: Different Objects for packages, connectors, etc.?
                Element e = null;

                try
                {
                   e  = repository.GetElementByID(int.Parse(update));
                } catch
                {
                    MessageBox.Show("Error with update: " + update);
                }

                if (e == null)
                {
                    MessageBox.Show("No Element: " + update);
                    continue;
                }

                if (showAlert && MessageBox.Show("Update detected. Refresh now?", "Auto Refresh", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    continue;
                }

                // See: http://www.sparxsystems.com/enterprise_architect_user_guide/9.3/automation/repository3.html
                repository.AdviseElementChange(e.ElementID);

            }
        }
    }
}
