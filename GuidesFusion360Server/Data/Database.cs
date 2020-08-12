using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace GuidesFusion360Server.Data
{
    /// <summary>
    /// Class to work with sqlite database.
    /// </summary>
    public static class Database
    {
        private static readonly string DbPath;

        /// <summary>
        /// Creates all db tables.
        /// </summary>
        static Database()
        {
            var dbDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Storage");
            DbPath = Path.Combine(dbDirectory, "db.sqlite3");
            var userTable = "CREATE TABLE IF NOT EXISTS 'Users' (" +
                        "'id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                        "'email' TEXT NOT NULL UNIQUE," +
                        "'password' TEXT NOT NULL," +
                        "'firstName' TEXT NOT NULL," +
                        "'lastName' TEXT NOT NULL," +
                        "'access' TEXT NOT NULL," +
                        "'studyGroup' TEXT);";
            var guideTable = "CREATE TABLE IF NOT EXISTS 'Guides' (" +
                         "'id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                         "'name' TEXT NOT NULL," +
                         "'description' TEXT," +
                         "'ownerId' INTEGER NOT NULL," + // No foreign key to be able to delete users while keeping their guides
                         "'hidden' TEXT NOT NULL);";
            var partGuideTable = "CREATE TABLE IF NOT EXISTS 'PartGuides' (" +
                             "'id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                             "'guideId' INTEGER NOT NULL," +
                             "'name' TEXT NOT NULL," +
                             "'content' TEXT," +
                             "'sortKey' INTEGER NOT NULL," +
                             "FOREIGN KEY('guideId') REFERENCES 'Guides'('id'));";
            var modelAnnotationsTable = "CREATE TABLE IF NOT EXISTS 'ModelAnnotations' (" +
                                    "'guideId' INTEGER NOT NULL," +
                                    "'x' INTEGER NOT NULL," +
                                    "'y' INTEGER NOT NULL," +
                                    "'z' INTEGER NOT NULL," +
                                    "'text' TEXT NOT NULL," +
                                    "FOREIGN KEY('guideId') REFERENCES 'Guides'('id'));";
            Directory.CreateDirectory(dbDirectory);
            var tables = new[] {userTable, guideTable, partGuideTable, modelAnnotationsTable};
            using var db = new SqliteConnection($"Filename={DbPath}");
            db.Open();
            foreach (var table in tables)
            {
                using var command = new SqliteCommand(table, db);
                command.ExecuteNonQuery();
            }
            db.Close();
        }

        /// <summary>
        /// Returns only first row of select command.
        /// </summary>
        /// <param name="sqlQuery">Values are ordered the way the ary in a passed command.</param>
        /// <returns></returns>
        public static List<string> SelectRowData(string sqlQuery)
            => SelectData(sqlQuery, true) as List<string>;

        /// <summary>
        /// Returns all rows of select command.
        /// </summary>
        /// <param name="sqlQuery">Values are ordered the way the ary in a passed command.</param>
        /// <returns></returns>
        public static List<List<string>> SelectAllData(string sqlQuery)
            => SelectData(sqlQuery) as List<List<string>>;

        /// <summary>
        /// Lets you pass INSERT, UPDATE, DELETE commands.
        /// </summary>
        /// <param name="sqlQuery">
        /// Returns id of last inserted item.
        /// Returns -1 on error occur.
        /// </param>
        /// <returns></returns>
        public static int ChangeData(string sqlQuery)
        {
            using var db = new SqliteConnection($"Filename={DbPath}");
            db.Open();
            using var command = new SqliteCommand(sqlQuery, db);
            long id;
            try
            {
                command.ExecuteNonQuery();
                command.CommandText = "SELECT last_insert_rowid();";
                id = (Int64)command.ExecuteScalar();
            }
            catch
            {
                id = -1;
            }
            db.Close();
            return (int)id;
        }
        
        private static object SelectData(string sqlQuery, bool firstRowOnly = false)
        {
            using var db = new SqliteConnection($"Filename={DbPath}");
            db.Open();
            using var command = new SqliteCommand(sqlQuery, db);

            var data = new List<List<string>>();
            var results = command.ExecuteReader();
            while (results.Read())
            {
                var result = new List<string>();
                for (var i = 0; i < results.FieldCount; i++)
                {
                    result.Add(results.GetValue(i).ToString());
                }
                data.Add(result);
            }

            db.Close();
            return firstRowOnly ? data[0] as object : data;
        }
    }
}
