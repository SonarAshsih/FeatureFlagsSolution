using Microsoft.Data.Sqlite;

namespace FeatureFlagsCli;

public static class Engine
{
    public const string DbPath = "flags.db";

    public static SqliteConnection Open()
    {
        var c = new SqliteConnection($"Data Source={DbPath}");
        c.Open();
        return c;
    }

    public static void InitDb()
    {
        using var c = Open();
        var cmd = c.CreateCommand();
        cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS features (
            key TEXT PRIMARY KEY,
            default_enabled INTEGER NOT NULL,
            description TEXT
        );
        CREATE TABLE IF NOT EXISTS overrides (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            feature_key TEXT NOT NULL,
            level TEXT NOT NULL,
            target_id TEXT NOT NULL,
            enabled INTEGER NOT NULL,
            UNIQUE(feature_key, level, target_id)
        );";
        cmd.ExecuteNonQuery();
    }

    public static bool Evaluate(string key, string? user, string? group, string? region)
    {
        using var c = Open();
        var f = c.CreateCommand();
        f.CommandText = "SELECT default_enabled FROM features WHERE key=$k";
        f.Parameters.AddWithValue("$k", key);
        var def = f.ExecuteScalar();
        if (def == null) throw new Exception("Feature not found");

        if (user != null && TryOverride(c, key, "user", user, out var u)) return u;
        if (group != null && TryOverride(c, key, "group", group, out var g)) return g;
        if (region != null && TryOverride(c, key, "region", region, out var r)) return r;

        return (long)def == 1;
    }

    private static bool TryOverride(SqliteConnection c, string f, string l, string t, out bool v)
    {
        var cmd = c.CreateCommand();
        cmd.CommandText = "SELECT enabled FROM overrides WHERE feature_key=$f AND level=$l AND target_id=$t";
        cmd.Parameters.AddWithValue("$f", f);
        cmd.Parameters.AddWithValue("$l", l);
        cmd.Parameters.AddWithValue("$t", t);
        var r = cmd.ExecuteScalar();
        if (r == null) { v = false; return false; }
        v = (long)r == 1;
        return true;
    }
}

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0) return;
        if (args[0] == "init") Engine.InitDb();
    }
}