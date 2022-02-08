using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using FirebirdDbComparer.Compare;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Isql;

namespace FirebirdDbComparer.Tests;

public static class Helpers
{
    public static class Database
    {
        public enum DatabaseLocation
        {
            Source,
            Target,
        }

        public static string GetConnectionString(TargetVersion version, DatabaseLocation location)
        {
            var builder =
                new FbConnectionStringBuilder
                {
                    Database = GetDatabasePath(version, location),
                    UserID = "sysdba",
                    Password = "masterkey",
                    ServerType = FbServerType.Embedded,
                    ClientLibrary = Path.Combine(GetFirebirdLocation(version), $"fbclient.dll"),
                    Charset = "utf8",
                    Pooling = false,
                };
            return builder.ToString();
        }

        public static void CreateDatabase(TargetVersion version, DatabaseLocation location)
        {
            FbConnection.CreateDatabase(GetConnectionString(version, location), 16 * 1024, false, true);
        }

        public static void DropDatabase(TargetVersion version, DatabaseLocation location)
        {
            FbConnection.DropDatabase(GetConnectionString(version, location));
        }

        public static void ExecuteScript(TargetVersion version, DatabaseLocation location, IEnumerable<string> commands)
        {
            ExecuteScript(version, location, new FbScript(string.Join(Environment.NewLine, commands)));
        }

        public static void ExecuteScript(TargetVersion version, DatabaseLocation location, string script)
        {
            ExecuteScript(version, location, new FbScript(script));
        }

        public static string GetDatabaseStructure(TargetVersion version, DatabaseLocation location)
        {
            var builder = new StringBuilder();
            using (var isql = new Process())
            {
                isql.StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(GetFirebirdLocation(version), "isql.exe"),
                    Arguments = $"-x {GetDatabasePath(version, location)}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                };
                isql.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        if (Regex.IsMatch(e.Data, @"^/\* CREATE DATABASE '.+ \*/$", RegexOptions.CultureInvariant))
                        {
                            return;
                        }
                        builder.AppendLine(e.Data);
                    }
                };
                isql.Start();
                isql.BeginOutputReadLine();
                isql.WaitForExit();
            }
            return builder.ToString().Trim();
        }

        private static string GetFirebirdLocation(TargetVersion version)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "_Firebird", version.VersionSuffix());
        }

        private static void ExecuteScript(TargetVersion version, DatabaseLocation location, FbScript script)
        {
            // NETProvider#1026
            script.UnknownStatement += (sender, e) =>
            {
                var match = e.Statement.Text.StartsWith("ALTER EXTERNAL FUNCTION ", StringComparison.OrdinalIgnoreCase);
                if (match)
                {
                    e.Handled = true;
                    e.NewStatementType = SqlStatementType.AlterFunction;
                }
            };
            using (var connection = new FbConnection(GetConnectionString(version, location)))
            {
                script.Parse();
                if (script.Results.Any())
                {
                    var be = new FbBatchExecution(connection);
                    be.AppendSqlStatements(script);
                    be.Execute();
                }
            }
        }

        private static string GetDatabasePath(TargetVersion version, DatabaseLocation location)
        {
            return Path.Combine(Path.GetTempPath(), $"cmp_{version.VersionSuffix()}_{location}.fdb");
        }
    }
}
