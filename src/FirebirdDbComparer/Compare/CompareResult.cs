using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebirdDbComparer.Compare
{
    public sealed class CompareResult
    {
        public ScriptResult Script { get; }
        public IReadOnlyList<string> Statements { get; }

        public CompareResult(ScriptResult script, IReadOnlyList<string> statements)
        {
            Script = script;
            Statements = statements;
        }
    }
}
