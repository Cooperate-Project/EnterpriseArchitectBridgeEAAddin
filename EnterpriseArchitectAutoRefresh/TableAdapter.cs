using EA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace EnterpriseArchitectAutoRefresh
{
    /// <summary>
    /// This class represents a logging table, maintained by a database trigger.
    /// </summary>
    class TableAdapter
    {
        // Logging table information
        private readonly String table;
        private readonly String prefix;
        private readonly Repository repository;
        private String timestamp;

        // Constants from the logging tables, must be similar to those in the Trigger-Generator
        private const String COLUMN_ID = "ID";
        private const String COLUMN_TIMESTAMP = "Timestamp";

        /// <summary>
        /// Creates a new TableListener with given table name and logging table prefix.
        /// </summary>
        /// <param name="table">The table name of the EA-related mapping (e.G. "t_object")</param>
        /// <param name="prefix">The prefix of the logging tables from the Trigger-Generator (e.G. "ht_")</param>
        /// <param name="repositry">The currently opened repository</param>
        public TableAdapter(String table, String prefix, Repository repositry)
        {
            this.table = table;
            this.prefix = prefix;
            this.repository = repositry;

            // Set NOW() as initial timestamp
            getDataBaseTimestamp();
        }

        /// <summary>
        /// Returns the primary keys (e.G. ElementIDs) from all database entries that changed in the last iteration.
        /// </summary>
        /// <returns></returns>
        public List<String> getUpdates()
        {

            String updateStatement = "SELECT DATE_FORMAT(NOW(6), '%Y-%m-%d %H:%i:%s.%f') AS `TS`, `{0}` AS `ID` FROM `{1}` WHERE `{2}` >= STR_TO_DATE('{3}', '%Y-%m-%d %H:%i:%s.%f')";


            String updatesXML = repository.SQLQuery(String.Format(updateStatement, COLUMN_ID, prefix + table, COLUMN_TIMESTAMP, timestamp));

            List<String> updateList = new List<String>();

            using (XmlReader reader = XmlReader.Create(new StringReader(updatesXML)))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "TS")
                        {
                            this.timestamp = reader.ReadElementContentAsString();
                        }
                        if (reader.Name == "ID")
                        {
                            updateList.Add(reader.ReadElementContentAsString());
                        }
                    }
                }
            }

            return updateList;
        }

        /// <summary>
        /// Sets the initial timestamp by getting the exact date from the database system.
        /// </summary>
        private void getDataBaseTimestamp()
        {
            String currentTimeXML = repository.SQLQuery("SELECT DATE_FORMAT(NOW(6), '%Y-%m-%d %H:%i:%s.%f') AS TS");

            using (XmlReader reader = XmlReader.Create(new StringReader(currentTimeXML)))
            {
                reader.ReadToFollowing("TS");
                timestamp = reader.ReadElementContentAsString();
            }
        }
    }
}