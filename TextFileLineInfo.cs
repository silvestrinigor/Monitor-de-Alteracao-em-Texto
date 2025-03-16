using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Monitor_de_Alteração_em_Texto
{
    public class TextFileLineInfo
    {
        private int _textFileInfoId;
        private string? _content;
        private int _lineNumber;
        private SQLiteConnection? Connection { get; }

        public TextFileLineInfo(SQLiteConnection connection, int id)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            Id = id;
        }

        public int Id { get; private set; }

        public int TextFileInfoId
        {
            get => _textFileInfoId;
            set
            {
                if (Connection != null && Id > 0) // Only update if the record exists
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
                    _textFileInfoId = value;
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
                    _content = value;
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
                    _lineNumber = value;
                }
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