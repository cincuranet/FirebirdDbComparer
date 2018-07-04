using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using FirebirdDbComparer.Compare;

namespace FirebirdDbComparer.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Compare(
                (TargetVersion)Enum.Parse(typeof(TargetVersion), ConfigurationManager.AppSettings["version"], true),
                ConfigurationManager.AppSettings["source"],
                ConfigurationManager.AppSettings["target"]);
        }

        private static void Compare(TargetVersion targetVersion, string sourceConnectionString, string targetConnectionString)
        {
            var scriptResult = Comparer.ForTwoDatabases(new ComparerSettings(targetVersion), sourceConnectionString, targetConnectionString)
                .Compare()
                .Script;


            using (var output = new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(false), 512 * 1024))
            {
                output.Write(Header($"GENERATED: {DateTimeOffset.Now.ToString(CultureInfo.InvariantCulture)}"));
                output.Write(Environment.NewLine);

                foreach (var item in scriptResult)
                {
                    output.Write(Header(item.Header));
                    if (!item.Any())
                    {
                        output.Write(Environment.NewLine);
                    }
                    foreach (var groups in item)
                    {
                        output.Write(Environment.NewLine);
                        foreach (string statement in groups)
                        {
                            output.Write(statement);
                            output.Write(Environment.NewLine);
                        }
                    }
                    if (item.Any())
                    {
                        output.Write(Environment.NewLine);
                    }
                }

                output.Write(Header("END OF SCRIPT"));
            }
        }

        private static string Header(string text)
        {
            const string Dashes = "-------------------------------------------------------------------------";

            return
                new StringBuilder()
                    .Append($"/* {Dashes} */")
                    .AppendLine()
                    .Append($"/* {text,-73} */")
                    .AppendLine()
                    .Append($"/* {Dashes} */")
                    .AppendLine()
                    .ToString();
        }
    }
}
