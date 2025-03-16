using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Monitor_de_Alteração_em_Texto
{
    public class DatabaseManager : IDisposable
    {
        public SQLiteConnection Connection { get; private set; }

        public DatabaseManager()
        {
            Connection = new SQLiteConnection("Data Source=hello.db;Version=3;");
            Connection.Open();
            CreateTablesIfNotExists();
        }

        private void CreateTablesIfNotExists()
        {
            string sql =
            @"
                CREATE TABLE IF NOT EXISTS TextFileInfo(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Path TEXT NOT NULL UNIQUE
                );

                CREATE TABLE IF NOT EXISTS TextFileLineInfo(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    TextFileInfoId INTEGER NOT NULL,
                    LineNumber INTEGER NOT NULL,
                    Content TEXT NOT NULL,
                    FOREIGN KEY (TextFileInfoId) REFERENCES TextFileInfo(Id) ON DELETE CASCADE,
                    CONSTRAINT unique_line_per_file UNIQUE (TextFileInfoId, LineNumber)
                );
            ";

            // Execute the SQL to create the tables if they do not already exist
            using var command = new SQLiteCommand(sql, Connection);
            command.ExecuteNonQuery();
        }

        public TextFileInfo? GetTextFileInfo(int textFileId)
        {
            var textFileInfo = new TextFileInfo(Connection, textFileId);
            return textFileInfo;
        }

        public int? GetTextFileInfoIdByPath(string filePath)
        {
            int? textFileId = null;

            // Query to get the Id of the TextFile based on the Path
            string query = "SELECT Id FROM TextFileInfo WHERE Path = @Path";
            using (var cmd = new SQLiteCommand(query, Connection))
            {
                cmd.Parameters.AddWithValue("@Path", filePath);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        textFileId = reader.GetInt32(0);  // Retrieve Id
                    }
                }
            }
            return textFileId;
        }

        public TextFileInfo CreateTextFileInfo(string path)
        {
            string query = "INSERT INTO TextFileInfo (Path) VALUES (@Path); SELECT last_insert_rowid();";
            using (var cmd = new SQLiteCommand(query, Connection))
            {
                cmd.Parameters.AddWithValue("@Path", path);
                var result = cmd.ExecuteScalar();

                // Ensure that we have a valid result before using it
                if (result != null)
                {
                    var textFileInfoId = Convert.ToInt32(result);
                    var textFileInfo = new TextFileInfo(Connection, textFileInfoId);
                    return textFileInfo;
                }
                else
                {
                    throw new InvalidOperationException("Failed to create TextFileInfo.");
                }
            }
        }

        public void Dispose()
        {
            if (Connection != null)
            {
                Connection.Close();
                Connection.Dispose();
            }
        }
    }
}
