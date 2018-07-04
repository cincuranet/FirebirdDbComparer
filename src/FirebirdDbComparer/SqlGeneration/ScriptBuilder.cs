using System;
using System.Collections.Generic;
using System.Linq;

using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.SqlGeneration
{
    public sealed class ScriptBuilder
    {
        string m_CurrentTerminator;
        ISqlHelper m_SqlHelper;

        public ScriptBuilder(ISqlHelper sqlHelper)
        {
            m_SqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof(sqlHelper));
            m_CurrentTerminator = m_SqlHelper.Terminator;
        }

        public IEnumerable<IEnumerable<string>> Build(IEnumerable<CommandGroup> items)
        {
            var enumerator = items.GetEnumerator();
            if (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                while (enumerator.MoveNext())
                {
                    yield return ProcessGroup(current, false);
                    current = enumerator.Current;
                }
                yield return ProcessGroup(current, true);
            }
        }

        private IEnumerable<string> ProcessGroup(CommandGroup group, bool last)
        {
            foreach (var command in group.Commands)
            {
                if (command is PSqlCommand && !InPSqlMode)
                {
                    yield return SwitchToPSqlMode();
                }
                if (!(command is PSqlCommand) && InPSqlMode)
                {
                    yield return SwitchFromPSqlMode();
                }
                yield return $"{command}{m_CurrentTerminator}";
            }
            if (last)
            {
                if (InPSqlMode)
                {
                    yield return SwitchFromPSqlMode();
                }
            }
        }

        private string SwitchToPSqlMode()
        {
            var result = $"SET TERM {m_SqlHelper.AlternativeTerminator}{m_CurrentTerminator}";
            m_CurrentTerminator = m_SqlHelper.AlternativeTerminator;
            return result;
        }

        private string SwitchFromPSqlMode()
        {
            var result = $"SET TERM {m_SqlHelper.Terminator}{m_CurrentTerminator}";
            m_CurrentTerminator = m_SqlHelper.Terminator;
            return result;
        }

        private bool InPSqlMode => m_CurrentTerminator.Equals(m_SqlHelper.AlternativeTerminator, StringComparison.Ordinal);
    }
}
