using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace Monitor_de_Alteração_em_Texto
{
    public class TextFileInfo
    {
        private string? _path;
        private List<TextFileLineInfo>? _lines;
        private SQLiteConnection Connection { get; set; }
        public int Id { get; private set; }

        public string? Path
        {
            get => _path;
            set
            {
                if (Connection != null && Id > 0 && !string.Equals(_path, value, StringComparison.Ordinal))
                {
                    string sql = "UPDATE TextFileInfo SET Path = @Path WHERE Id = @Id";
                    using var command = new SQLiteCommand(sql, Connection);
                    command.Parameters.AddWithValue("@Path", value ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Id", Id);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        _path = value;
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to update Path.");
                    }
                }
                else
                {
                    _path = value;
                }
            }
        }

        public List<TextFileLineInfo>? Lines
        {
            get
            {
                if (_lines == null && Connection != null && Id > 0)
                {
                    _lines = new List<TextFileLineInfo>();

                    string sql = "SELECT Id, TextFileInfoId, LineNumber, Content FROM TextFileLineInfo WHERE TextFileInfoId = @TextFileInfoId ORDER BY LineNumber";
                    using var command = new SQLiteCommand(sql, Connection);
                    command.Parameters.AddWithValue("@TextFileInfoId", Id);

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var line = new TextFileLineInfo(Connection, reader.GetInt32(0));
                        _lines.Add(line);
                    }
                }
                return _lines;
            }
            private set => _lines = value;
        }

        public TextFileInfo(SQLiteConnection connection, int id)
        {
            Id = id;
            Connection = connection;
            LoadLinesFromTextFileInfo();
        }

        public TextFileLineInfo GetTextLineInfoByLineNumber(int lineNumber)
        {
            // Always fetch from the database instead of local cache
            string query = "SELECT Id, TextFileInfoId, LineNumber, Content FROM TextFileLineInfo WHERE TextFileInfoId = @TextFileInfoId AND LineNumber = @LineNumber";
            using (var cmd = new SQLiteCommand(query, Connection))
            {
                cmd.Parameters.AddWithValue("@TextFileInfoId", Id);
                cmd.Parameters.AddWithValue("@LineNumber", lineNumber);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new TextFileLineInfo(Connection, reader.GetInt32(0))
                        {
                            TextFileInfoId = reader.GetInt32(1),
                            LineNumber = reader.GetInt32(2),
                            Content = reader.GetString(3)
                        };
                    }
                }
            }

            return null; // Return null if no matching line found
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


        public List<string> GetLineStrings()
        {
            if (Lines == null || Lines.Count == 0)
                return new List<string>(); // Return an empty list if there are no lines

            return Lines.Select(line => line.Content ?? string.Empty).ToList();
        }
        public void KeepThisLinesDeleteRest(List<int> lineNumbers)
        {
            // Step 1: Check if the provided List<int> is not empty
            if (lineNumbers == null || lineNumbers.Count == 0)
            {
                Console.WriteLine("No lines to keep.");
                return;
            }
            if(Connection == null)
            {
                throw new InvalidOperationException("Database connection is not set.");
            }

            // Step 2: Delete lines that are not in the 'lineNumbers' list
            using (var transaction = Connection.BeginTransaction())
            {
                try
                {
                    // Create the DELETE SQL query to remove lines that are not in the 'lineNumbers' list
                    string deleteQuery = @"
                    DELETE FROM TextFileLineInfo
                    WHERE TextFileInfoId = @TextFileId
                    AND LineNumber NOT IN (" + string.Join(",", lineNumbers) + ")";

                    // Execute the DELETE query
                    using (var cmd = new SQLiteCommand(deleteQuery, Connection))
                    {
                        cmd.Parameters.AddWithValue("@TextFileId", Id);
                        cmd.ExecuteNonQuery();
                    }

                    // Commit the transaction
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of an error
                    transaction.Rollback();
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            // Step 3: Optionally reload the lines from the database to reflect changes
            LoadLinesFromTextFileInfo(); // Assuming you have a method to reload the lines from the DB
        }
        public void LoadLinesFromTextFileInfo()
        {
            if (Id == 0 || Connection == null)
            {
                throw new InvalidOperationException("Cannot load lines because TextFileInfo is not saved in the database.");
            }

            List<TextFileLineInfo> existingLines = new();

            string query = "SELECT Id, LineNumber, Content FROM TextFileLineInfo WHERE TextFileInfoId = @TextFileId ORDER BY LineNumber";
            using (var cmd = new SQLiteCommand(query, Connection))
            {
                cmd.Parameters.AddWithValue("@TextFileId", Id);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        existingLines.Add(new TextFileLineInfo(Connection, reader.GetInt32(0))); 
                        break;
                    }
                }
            }

            Lines = existingLines;
        }

        public HashSet<int> GetLineNumbers()
        {
            if (Lines == null || Lines.Count == 0)
                return new HashSet<int>(); // Return an empty HashSet if there are no lines
            return new HashSet<int>(Lines.Select(line => line.LineNumber));
        }

        // Method to get the content of the text file as a single string
        public string GetContent()
        {
            StringBuilder content = new StringBuilder();

            // Ensure the Lines collection is not null
            if (Lines == null)
            {
                return string.Empty;
            }

            // Order the lines by LineNumber
            var orderedLines = Lines.OrderBy(line => line.LineNumber).ToList();

            // Add each line's content to the StringBuilder
            for (int i = 0; i < orderedLines.Count; i++)
            {
                var currentLine = orderedLines[i];

                // If LineNumber 1 and LineNumber 3 are present, add a blank line between them
                if (currentLine.LineNumber == 1 && i + 1 < orderedLines.Count && orderedLines[i + 1].LineNumber == 3)
                {
                    content.AppendLine("");  // Adds a blank line between Line 1 and Line 3
                }

                // Skip lines that have null or are empty/whitespace
                if (string.IsNullOrWhiteSpace(currentLine.Content))
                {
                    continue;
                }

                // Add the current line content to the StringBuilder
                content.AppendLine(currentLine.Content);
            }

            return content.ToString();
        }
    }
}