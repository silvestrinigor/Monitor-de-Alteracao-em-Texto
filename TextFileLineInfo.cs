using System.Data.SQLite;

namespace Monitor_de_Alteração_em_Texto
{
    public class TextFileLineInfo
    {
        private int _textFileInfoId;
        private string? _content;
        private int _lineNumber;
        private SQLiteConnection? Connection { get; }
        public bool IsDeleted = false;

        public TextFileLineInfo(SQLiteConnection connection, int id)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            Id = id;
            LoadInfos();
        }

        public int Id { get; private set; }

        public int TextFileInfoId
        {
            get => _textFileInfoId;
            set
            {
                if (Connection != null && Id > 0)
                {
                    string sql = "UPDATE TextFileLineInfo SET TextFileInfoId = @TextFileInfoId WHERE Id = @Id";
                    using var command = new SQLiteCommand(sql, Connection);
                    command.Parameters.AddWithValue("@TextFileInfoId", value);
                    command.Parameters.AddWithValue("@Id", Id);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        _textFileInfoId = value;
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to update TextFileInfoId.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("The line does not exist.");
                }
            }
        }

        public string? Content
        {
            get => _content;
            set
            {
                if (Connection != null && Id > 0)
                {
                    string sql = "UPDATE TextFileLineInfo SET Content = @Content WHERE Id = @Id";
                    using var command = new SQLiteCommand(sql, Connection);
                    command.Parameters.AddWithValue("@Content", value ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Id", Id);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        _content = value;
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to update Content.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("The line does not exist.");
                }
            }
        }

        public int LineNumber
        {
            get => _lineNumber;
            set
            {
                if (Connection != null && Id > 0)
                {
                    string sql = "UPDATE TextFileLineInfo SET LineNumber = @LineNumber WHERE Id = @Id";
                    using var command = new SQLiteCommand(sql, Connection);
                    command.Parameters.AddWithValue("@LineNumber", value);
                    command.Parameters.AddWithValue("@Id", Id);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        _lineNumber = value;
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to update LineNumber.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("The line does not exist.");
                }
            }
        }

        public void LoadInfos()
        {
            if (Connection != null && Id > 0)
            {
                string query = "SELECT TextFileInfoId, Content, LineNumber FROM TextFileLineInfo WHERE Id = @Id";
                using var command = new SQLiteCommand(query, Connection);
                command.Parameters.AddWithValue("@Id", Id);
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    _textFileInfoId = reader.GetInt32(0);
                    _content = reader.IsDBNull(1) ? null : reader.GetString(1);
                    _lineNumber = reader.GetInt32(2);
                }
                else
                {
                    throw new InvalidOperationException("Line information not found in the database.");
                }
            }
            else
            {
                throw new InvalidOperationException("Invalid database connection or Id.");
            }
        }

        public void DeleteLine()
        {
            if (Connection != null && Id > 0)
            {
                string sql = "DELETE FROM TextFileLineInfo WHERE Id = @Id";
                using var command = new SQLiteCommand(sql, Connection);
                command.Parameters.AddWithValue("@Id", Id);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    IsDeleted = true;
                    Id = 0;
                    _textFileInfoId = 0;
                    _content = null;
                    _lineNumber = 0;
                }
                else
                {
                    throw new InvalidOperationException("Failed to delete the line.");
                }
            }
            else
            {
                throw new InvalidOperationException("The line does not exist.");
            }
        }
    }
}