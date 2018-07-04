using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FirebirdDbComparer.Compare
{
    public sealed class ScriptResult : IReadOnlyList<ScriptResult.Section>
    {
        public class Section : IReadOnlyList<IReadOnlyList<string>>
        {
            private readonly IReadOnlyList<IReadOnlyList<string>> m_Items;

            public string Header { get; }

            internal Section(string header, IReadOnlyList<IReadOnlyList<string>> items)
            {
                Header = header;
                m_Items = items;
            }

            public IReadOnlyList<string> this[int index] => m_Items[index];

            public int Count => m_Items.Count;

            public IEnumerator<IReadOnlyList<string>> GetEnumerator()
            {
                return m_Items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private readonly IReadOnlyList<Section> m_Items;

        private ScriptResult(IReadOnlyList<Section> items)
        {
            m_Items = items;
        }

        internal static ScriptResult Create(IEnumerable<Tuple<string, IEnumerable<IEnumerable<string>>>> data)
        {
            return new ScriptResult(data.Select(x => new Section(x.Item1, x.Item2.Select(y => y.ToList().AsReadOnly()).ToList().AsReadOnly())).ToList().AsReadOnly());
        }

        public Section this[int index] => m_Items[index];

        public int Count => m_Items.Count;

        public IEnumerable<string> AllStatements => this.SelectMany(x => x).SelectMany(x => x);

        public IEnumerator<Section> GetEnumerator()
        {
            return m_Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
