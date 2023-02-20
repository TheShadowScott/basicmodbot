using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ModBot;

static class DatabaseCommands
{

    // UPDATE THE CONNECTION STRING TO YOUR OWN DATABASE

    readonly static Regex scrub = new(@"^\s*['""]\s*\)\s*;", 
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
    internal static List<(string, string, long, long, string, string)> FetchData(ulong userId)
    {
        List<(string, string, long, long, string, string)> warnings = new();
        try
        {
            using (SqlConnection conn = new(Environment.GetEnvironmentVariable("CONN_STRING")))
            {
                conn.Open();

                SqlCommand cmd = new("SELECT * FROM user_warnings WHERE UserId = @userid", conn);
                cmd.Parameters.AddWithValue("@userid", (long)userId);

                using SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    warnings.Add(((string)rdr["Id"], (string)rdr["WarnType"], (long)rdr["UserId"], (long)rdr["PerpetratorId"], (string)rdr["WarningReason"], (string)rdr["TimeMark"]));
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return warnings;
    }

    internal static async Task ExecCommand(string type, ulong userId, string reason, DSharpPlus.Entities.DiscordUser Perp,
         string? time = null, bool suppressErrors = false)
    {
        try
        {
            using SqlConnection conn = new(Environment.GetEnvironmentVariable("CONN_STRING"));
            conn.Open();

            SqlCommand cmd = new("INSERT INTO user_warnings VALUES (@GUID, @type, @userid, @perpid, @reason, @time);", conn);
            cmd.Parameters.AddWithValue("@GUID", Guid.NewGuid().ToString());
            cmd.Parameters.AddWithValue("@userid", (long)userId);
            cmd.Parameters.AddWithValue("@perpid", (long)Perp.Id);
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@reason", scrub.Replace(reason, ""));
            cmd.Parameters.AddWithValue("@time", time ?? DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));

            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            if (!suppressErrors)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    internal static async Task Alter(string id, string reason)
    {
        try
        {
            using (SqlConnection conn = new(Environment.GetEnvironmentVariable("CONN_STRING")))
            {
                conn.Open();
                SqlCommand cmd = new("UPDATE user_warnings SET WarningReason = @reason WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@reason", reason);
                cmd.Parameters.AddWithValue("@id", id);
                await cmd.ExecuteNonQueryAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    internal static async Task Drop(string id)
    {
        try
        {
            using (SqlConnection conn = new(Environment.GetEnvironmentVariable("CONN_STRING")))
            {
                conn.Open();
                SqlCommand cmd = new("DELETE FROM user_warnings WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                await cmd.ExecuteNonQueryAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}