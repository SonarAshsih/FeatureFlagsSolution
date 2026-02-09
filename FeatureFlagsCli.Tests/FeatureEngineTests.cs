using Xunit;
using Microsoft.Data.Sqlite;
using FeatureFlagsCli;
using System.IO;

public class FeatureEngineTests
{
    private void ResetDb()
    {
        if (File.Exists(Engine.DbPath))
            File.Delete(Engine.DbPath);
        Engine.InitDb();
    }

    private void Seed(string sql)
    {
        using var c = new SqliteConnection($"Data Source={Engine.DbPath}");
        c.Open();
        var cmd = c.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    [Fact]
    public void Full_Precedence_Matrix_Works()
    {
        ResetDb();
        Seed("INSERT INTO features VALUES ('f1',0,'')");
        Seed("INSERT INTO overrides VALUES (NULL,'f1','region','eu',1)");
        Seed("INSERT INTO overrides VALUES (NULL,'f1','group','g1',0)");
        Seed("INSERT INTO overrides VALUES (NULL,'f1','user','u1',1)");

        Assert.True(Engine.Evaluate("f1","u1","g1","eu"));
    }
}