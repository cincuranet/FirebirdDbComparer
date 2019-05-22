using System.Collections.Generic;

using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.DatabaseObjects.Elements;
using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.DatabaseObjects.Primitives;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Interfaces
{
    public interface IMetadataFunctions : IDatabaseObject
    {
        IDictionary<FunctionArgumentKey, FunctionArgument> FunctionArguments { get; }
        IDictionary<Identifier, Function> FunctionsByName { get; }
        IDictionary<Identifier, Function> LegacyFunctionsByName { get; }
        IDictionary<Identifier, Function> NewFunctionsByName { get; }
        IEnumerable<CommandGroup> CreateLegacyFunctions(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> AlterLegacyFunctions(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> DropLegacyFunctions(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> CreateEmptyNewFunctions(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> AlterNewFunctionsToFullBody(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> AlterNewFunctionsToEmptyBodyForAlteringOrDropping(IMetadata other, IComparerContext context);
        IEnumerable<CommandGroup> DropNewFunctions(IMetadata other, IComparerContext context);
    }
}
