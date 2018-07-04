using System;
using System.Collections.Generic;

using FirebirdDbComparer.DatabaseObjects.EquatableKeys;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.DatabaseObjects
{
    public abstract class Primitive<T> : SqlElement<T>, ITypeObjectNameKey where T : class
    {
        private TypeObjectNameKey m_PrimitiveTypeKeyCache;

        protected Primitive(ISqlHelper sqlHelper)
            : base(sqlHelper)
        { }

        public TypeObjectNameKey TypeObjectNameKey => m_PrimitiveTypeKeyCache ?? (m_PrimitiveTypeKeyCache = new TypeObjectNameKey(typeof(T), OnPrimitiveTypeKeyObjectName()));

        public IEnumerable<Command> Create(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            EnsureParameters(sourceMetadata, targetMetadata, context);

            context.DroppedObjects.Remove(TypeObjectNameKey);
            if (context.CreatedObjects.Add(TypeObjectNameKey))
            {
                foreach (var item in OnCreate(sourceMetadata, targetMetadata, context))
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<Command> Drop(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            EnsureParameters(sourceMetadata, targetMetadata, context);

            context.CreatedObjects.Remove(TypeObjectNameKey);
            if (context.DroppedObjects.Add(TypeObjectNameKey))
            {
                foreach (var item in OnDrop(sourceMetadata, targetMetadata, context))
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<Command> Alter(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            EnsureParameters(sourceMetadata, targetMetadata, context);

            context.DroppedObjects.Remove(TypeObjectNameKey);
            context.CreatedObjects.Add(TypeObjectNameKey);

            return OnAlter(sourceMetadata, targetMetadata, context);
        }

        protected abstract IEnumerable<Command> OnCreate(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context);
        protected abstract IEnumerable<Command> OnDrop(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context);
        protected abstract IEnumerable<Command> OnAlter(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context);

        protected abstract Identifier OnPrimitiveTypeKeyObjectName();

        protected static T FindOtherChecked<TKey>(IDictionary<TKey, T> others, TKey name, string exceptionObjectType)
        {
            return others.TryGetValue(name, out var result)
                       ? result
                       : throw new InvalidOperationException($"Expected a {exceptionObjectType} named {name} in the target.");
        }

        private static void EnsureParameters(IMetadata sourceMetadata, IMetadata targetMetadata, IComparerContext context)
        {
            if (sourceMetadata == null)
            {
                throw new ArgumentNullException(nameof(sourceMetadata));
            }
            if (targetMetadata == null)
            {
                throw new ArgumentNullException(nameof(targetMetadata));
            }
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
        }
    }
}
