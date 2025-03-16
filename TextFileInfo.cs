using System.Text;
using System.Data.SQLite;

namespace Monitor_de_Alteração_em_Texto
{
    public class TextFileInfo
    {
        private string? _path;
        private SQLiteConnection Connection { get; set; }
        public int Id { get; private set; }
        public bool IsDeleted { get; set; } // Local property, not stored in DB

        public string? Path
        {
            get
            {
                if (_path == null && Connection != null && Id > 0)
                {
                    string query = "SELECT Path FROM TextFileInfo WHERE Id = @Id";
                    using (var command = new SQLiteCommand(query, Connection))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        var result = command.ExecuteScalar();
                        _path = result != DBNull.Value && result != null ? result.ToString() : string.Empty;
                    }
                }
                return _path;
            }
            set
            {
                if (Connection != null && Id > 0 && !string.Equals(_path, value, StringComparison.Ordinal))
                {
                    string sql = "UPDATE TextFileInfo SET Path = @Path WHERE Id = @Id";
                    using var command = new SQLiteCommand(sql, Connection);
                    command.Parameters.AddWithValue("@Path", value ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Id", Id);
                    if (command.ExecuteNonQuery() > 0) _path = value;
                }
                else
                {
                    _path = value;
                }
            }
        }

        public List<TextFileLineInfo>? Lines { get; private set; }

        public TextFileInfo(SQLiteConnection connection, int id)
        {
            Id = id;
            Connection = connection;
            LoadInfos();
        }

        public void LoadInfos()
        {
            if (Connection == null || Id <= 0)
                throw new InvalidOperationException("Invalid database connection or record ID.");

            // Load TextFileInfo data (Path)
            string query = "SELECT Path FROM TextFileInfo WHERE Id = @Id";
            using (var command = new SQLiteCommand(query, Connection))
            {
                command.Parameters.AddWithValue("@Id", Id);
                using var reader = command.ExecuteReader();
                if (reader.Read())
                    _path = reader.IsDBNull(0) ? null : reader.GetString(0);
                else
                    throw new InvalidOperationException("Record not found in the database.");
            }

            // Load associated lines
            Lines = LoadLinesFromDatabase();
        }

        private List<TextFileLineInfo> LoadLinesFromDatabase()
        {
            var loadedLines = new List<TextFileLineInfo>();
            string lineQuery = "SELECT Id FROM TextFileLineInfo WHERE TextFileInfoId = @TextFileId ORDER BY LineNumber";
            using (var lineCommand = new SQLiteCommand(lineQuery, Connection))
            {
                lineCommand.Parameters.AddWithValue("@TextFileId", Id);
                using var lineReader = lineCommand.ExecuteReader();
                while (lineReader.Read())
                {
                    var line = new TextFileLineInfo(Connection, lineReader.GetInt32(0));
                    line.LoadInfos();
                    loadedLines.Add(line);
                }
            }
            return loadedLines;
        }

        public TextFileLineInfo? GetTextLineInfoByLineNumber(int lineNumber)
        {
            string query = "SELECT Id FROM TextFileLineInfo WHERE TextFileInfoId = @TextFileInfoId AND LineNumber = @LineNumber";
            using (var cmd = new SQLiteCommand(query, Connection))
            {
                cmd.Parameters.AddWithValue("@TextFileInfoId", Id);
                cmd.Parameters.AddWithValue("@LineNumber", lineNumber);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                    return new TextFileLineInfo(Connection, reader.GetInt32(0));
            }
            return null;
        }

        public List<string> GetLineStrings()
        {
            return Lines?.Select(line => line.Content ?? string.Empty).ToList() ?? new List<string>();
        }

        public HashSet<int> GetLineNumbers()
        {
            return Lines == null ? new HashSet<int>() : new HashSet<int>(Lines.Select(line => line.LineNumber));
        }

        public string GetContent()
        {
            LoadInfos();
            if (Lines == null)
                throw new InvalidOperationException("Lines collection is not initialized.");

            var content = new StringBuilder();
            var orderedLines = Lines.OrderBy(line => line.LineNumber).ToList();

            for (int i = 0; i < orderedLines.Count; i++)
            {
                var currentLine = orderedLines[i];
                if (currentLine.Content == null)
                    continue;
                if(i == orderedLines.Count - 1)
                    content.Append(currentLine.Content);
                else
                    content.AppendLine(currentLine.Content);
            }
            return content.ToString();
        }
        public TextFileLineInfo CreateTextLineInfo(int lineNumber, string content)
        {
            if (Connection == null)
            {
                throw new InvalidOperationException("Database connection is not initialized.");
            }

            if (Id == 0)
            {
                throw new InvalidOperationException("TextFileInfo must be saved before adding lines.");
            }

            // Ensure line number does not already exist using more efficient query
            string checkQuery = "SELECT 1 FROM TextFileLineInfo WHERE TextFileInfoId = @TextFileInfoId AND LineNumber = @LineNumber LIMIT 1";
            using (var checkCmd = new SQLiteCommand(checkQuery, Connection))
            {
                checkCmd.Parameters.AddWithValue("@TextFileInfoId", Id);
                checkCmd.Parameters.AddWithValue("@LineNumber", lineNumber);

                using var reader = checkCmd.ExecuteReader();
                if (reader.Read())
                {
                    throw new InvalidOperationException($"Line number {lineNumber} already exists in the file.");
                }
            }

            // Insert new line
            string insertQuery = "INSERT INTO TextFileLineInfo (TextFileInfoId, LineNumber, Content) VALUES (@TextFileInfoId, @LineNumber, @Content); SELECT last_insert_rowid();";
            int newId;
            using (var insertCmd = new SQLiteCommand(insertQuery, Connection))
            {
                insertCmd.Parameters.AddWithValue("@TextFileInfoId", Id);
                insertCmd.Parameters.AddWithValue("@LineNumber", lineNumber);
                insertCmd.Parameters.AddWithValue("@Content", content);

                newId = Convert.ToInt32(insertCmd.ExecuteScalar());
            }

            // Create a new TextFileLineInfo object and add it to the list
            var newLine = new TextFileLineInfo(Connection, newId)
            {
                TextFileInfoId = Id,
                LineNumber = lineNumber,
                Content = content
            };

            if (Lines == null)
            {
                Lines = new List<TextFileLineInfo>();
            }

            Lines.Add(newLine);
            Lines.Sort((a, b) => a.LineNumber.CompareTo(b.LineNumber)); // More efficient sorting in place

            return newLine;
        }
        public void DeleteTextFileInfo()
        {
            if (Connection == null || Id <= 0)
                throw new InvalidOperationException("Invalid database connection or record ID.");

            // Delete associated lines first
            string deleteLinesQuery = "DELETE FROM TextFileLineInfo WHERE TextFileInfoId = @TextFileInfoId";
            using (var command = new SQLiteCommand(deleteLinesQuery, Connection))
            {
                command.Parameters.AddWithValue("@TextFileInfoId", Id);
                command.ExecuteNonQuery();
            }

            // Now delete the TextFileInfo record
            string deleteFileQuery = "DELETE FROM TextFileInfo WHERE Id = @Id";
            using (var command = new SQLiteCommand(deleteFileQuery, Connection))
            {
                command.Parameters.AddWithValue("@Id", Id);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    // Successfully deleted
                    _path = null; // Reset the path after deletion
                    Lines = null; // Clear the lines collection
                }
                else
                {
                    throw new InvalidOperationException("Failed to delete the TextFileInfo record.");
                }
            }
        }
    }
}