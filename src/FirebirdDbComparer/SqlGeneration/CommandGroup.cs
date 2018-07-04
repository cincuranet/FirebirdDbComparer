using System.Collections.Generic;
using System.Linq;

namespace FirebirdDbComparer.SqlGeneration
{
    public sealed class CommandGroup
    {
        private readonly List<Command> m_Commands;

        public CommandGroup()
        {
            m_Commands = new List<Command>();
        }

        public IReadOnlyList<Command> Commands => m_Commands.AsReadOnly();

        public bool IsEmpty => m_Commands.Count == 0 || m_Commands.All(c => c.IsEmpty);

        public CommandGroup Append(Command command)
        {
            m_Commands.Add(command);
            return this;
        }

        public CommandGroup Append(IEnumerable<Command> commands)
        {
            m_Commands.AddRange(commands);
            return this;
        }
    }
}
