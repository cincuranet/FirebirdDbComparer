using System;
using System.Collections.Generic;

namespace FirebirdDbComparer.DatabaseObjects
{
    internal static class TriggerTypeParser
    {
        private const int TRIGGER_TYPE_SHIFT = 13;
        private const ulong TRIGGER_TYPE_MASK = 3 << TRIGGER_TYPE_SHIFT;
        private const ulong TRIGGER_TYPE_DML = 0 << TRIGGER_TYPE_SHIFT;
        private const ulong TRIGGER_TYPE_DB = 1 << TRIGGER_TYPE_SHIFT;
        private const ulong TRIGGER_TYPE_DDL = 2 << TRIGGER_TYPE_SHIFT;

        private static class DML
        {
            private const ulong DML_TRIGGER_BEFORE = 0;
            private const ulong DML_TRIGGER_AFTER = 1;
            private const ulong DML_TRIGGER_INSERT = 1;
            private const ulong DML_TRIGGER_UPDATE = 2;
            private const ulong DML_TRIGGER_DELETE = 3;

            public static (TriggerBeforeAfterType, IList<TriggerEventType>) Parse(ulong type)
            {                
                var prefix = (type + 1) & 1;
                var suffix1 = SuffixHelper(type, 1);
                var suffix2 = SuffixHelper(type, 2);
                var suffix3 = SuffixHelper(type, 3);
                var beforeAfter = TriggerBeforeAfterTypeHelper(prefix) ?? throw UnknownTriggerTypeException(type);
                var events = new List<TriggerEventType>(3);
                events.Add(TriggerEventTypeHelper(suffix1) ?? throw UnknownTriggerTypeException(type));
                if (suffix2 != 0)
                {
                    events.Add(TriggerEventTypeHelper(suffix2) ?? throw UnknownTriggerTypeException(type));
                }
                if (suffix3 != 0)
                {
                    events.Add(TriggerEventTypeHelper(suffix3) ?? throw UnknownTriggerTypeException(type));
                }
                return (beforeAfter, events);
            }

            private static ulong SuffixHelper(ulong value, byte slot) => ((value + 1) >> (slot * 2 - 1)) & 3;

            private static TriggerBeforeAfterType? TriggerBeforeAfterTypeHelper(ulong value)
            {
                switch (value)
                {
                    case DML_TRIGGER_BEFORE:
                        return TriggerBeforeAfterType.Before;
                    case DML_TRIGGER_AFTER:
                        return TriggerBeforeAfterType.After;
                    default:
                        return null;
                }
            }

            private static TriggerEventType? TriggerEventTypeHelper(ulong value)
            {
                switch (value)
                {
                    case DML_TRIGGER_INSERT:
                        return TriggerEventType.Insert;
                    case DML_TRIGGER_UPDATE:
                        return TriggerEventType.Update;
                    case DML_TRIGGER_DELETE:
                        return TriggerEventType.Delete;
                    default:
                        return null;
                }
                throw UnknownTriggerTypeException(value);
            }
        }

        private static class DB
        {
            private const ulong DB_TRIGGER_CONNECT = 0;
            private const ulong DB_TRIGGER_DISCONNECT = 1;
            private const ulong DB_TRIGGER_TRANS_START = 2;
            private const ulong DB_TRIGGER_TRANS_COMMIT = 3;
            private const ulong DB_TRIGGER_TRANS_ROLLBACK = 4;

            public static IList<TriggerEventType> Parse(ulong type)
            {
                switch (type & ~TRIGGER_TYPE_DB)
                {
                    case DB_TRIGGER_CONNECT:
                        return new[] { TriggerEventType.Connect };
                    case DB_TRIGGER_DISCONNECT:
                        return new[] { TriggerEventType.Disconnect };
                    case DB_TRIGGER_TRANS_START:
                        return new[] { TriggerEventType.TransactionStart };
                    case DB_TRIGGER_TRANS_COMMIT:
                        return new[] { TriggerEventType.TransactionCommit };
                    case DB_TRIGGER_TRANS_ROLLBACK:
                        return new[] { TriggerEventType.TransactionRollback };
                }
                throw UnknownTriggerTypeException(type);
            }
        }

        private static class DDL
        {
            private const ulong DDL_TRIGGER_BEFORE = 0;
            private const ulong DDL_TRIGGER_AFTER = 1;
            private const ulong DDL_TRIGGER_ANY = 0x7FFFFFFFFFFFFFFF & ~TRIGGER_TYPE_MASK & ~1UL;
            private const ulong DDL_TRIGGER_CREATE_TABLE = 1;
            private const ulong DDL_TRIGGER_ALTER_TABLE = 2;
            private const ulong DDL_TRIGGER_DROP_TABLE = 3;
            private const ulong DDL_TRIGGER_CREATE_PROCEDURE = 4;
            private const ulong DDL_TRIGGER_ALTER_PROCEDURE = 5;
            private const ulong DDL_TRIGGER_DROP_PROCEDURE = 6;
            private const ulong DDL_TRIGGER_CREATE_FUNCTION = 7;
            private const ulong DDL_TRIGGER_ALTER_FUNCTION = 8;
            private const ulong DDL_TRIGGER_DROP_FUNCTION = 9;
            private const ulong DDL_TRIGGER_CREATE_TRIGGER = 10;
            private const ulong DDL_TRIGGER_ALTER_TRIGGER = 11;
            private const ulong DDL_TRIGGER_DROP_TRIGGER = 12;
            private const ulong DDL_TRIGGER_CREATE_EXCEPTION = 16;
            private const ulong DDL_TRIGGER_ALTER_EXCEPTION = 17;
            private const ulong DDL_TRIGGER_DROP_EXCEPTION = 18;
            private const ulong DDL_TRIGGER_CREATE_VIEW = 19;
            private const ulong DDL_TRIGGER_ALTER_VIEW = 20;
            private const ulong DDL_TRIGGER_DROP_VIEW = 21;
            private const ulong DDL_TRIGGER_CREATE_DOMAIN = 22;
            private const ulong DDL_TRIGGER_ALTER_DOMAIN = 23;
            private const ulong DDL_TRIGGER_DROP_DOMAIN = 24;
            private const ulong DDL_TRIGGER_CREATE_ROLE = 25;
            private const ulong DDL_TRIGGER_ALTER_ROLE = 26;
            private const ulong DDL_TRIGGER_DROP_ROLE = 27;
            private const ulong DDL_TRIGGER_CREATE_INDEX = 28;
            private const ulong DDL_TRIGGER_ALTER_INDEX = 29;
            private const ulong DDL_TRIGGER_DROP_INDEX = 30;
            private const ulong DDL_TRIGGER_CREATE_SEQUENCE = 31;
            private const ulong DDL_TRIGGER_ALTER_SEQUENCE = 32;
            private const ulong DDL_TRIGGER_DROP_SEQUENCE = 33;
            private const ulong DDL_TRIGGER_CREATE_USER = 34;
            private const ulong DDL_TRIGGER_ALTER_USER = 35;
            private const ulong DDL_TRIGGER_DROP_USER = 36;
            private const ulong DDL_TRIGGER_CREATE_COLLATION = 37;
            private const ulong DDL_TRIGGER_DROP_COLLATION = 38;
            private const ulong DDL_TRIGGER_ALTER_CHARACTER_SET = 39;
            private const ulong DDL_TRIGGER_CREATE_PACKAGE = 40;
            private const ulong DDL_TRIGGER_ALTER_PACKAGE = 41;
            private const ulong DDL_TRIGGER_DROP_PACKAGE = 42;
            private const ulong DDL_TRIGGER_CREATE_PACKAGE_BODY = 43;
            private const ulong DDL_TRIGGER_DROP_PACKAGE_BODY = 44;
            private const ulong DDL_TRIGGER_CREATE_MAPPING = 45;
            private const ulong DDL_TRIGGER_ALTER_MAPPING = 46;
            private const ulong DDL_TRIGGER_DROP_MAPPING = 47;

            public static (TriggerBeforeAfterType, IList<TriggerEventType>) Parse(ulong type)
            {
                var prefix = type & 1;
                var beforeAfter = TriggerBeforeAfterTypeHelper(prefix) ?? throw UnknownTriggerTypeException(type);
                if ((type & DDL_TRIGGER_ANY) == DDL_TRIGGER_ANY)
                {
                    return (beforeAfter, new[] { TriggerEventType.Any });
                }
                else
                {
                    var events = new List<TriggerEventType>();
                    var value = type - (TRIGGER_TYPE_DDL + prefix);
                    while (value > 0)
                    {
                        var x = (ulong)Math.Floor(Math.Log(value, 2));
                        events.Add(TriggerEventTypeHelper(x) ?? throw UnknownTriggerTypeException(type));
                        value -= (ulong)Math.Pow(2, x);
                    }
                    return (beforeAfter, events);
                }
            }


            private static TriggerBeforeAfterType? TriggerBeforeAfterTypeHelper(ulong value)
            {
                switch (value)
                {
                    case DDL_TRIGGER_BEFORE:
                        return TriggerBeforeAfterType.Before;
                    case DDL_TRIGGER_AFTER:
                        return TriggerBeforeAfterType.After;
                    default:
                        return null;
                }
            }
            private static TriggerEventType? TriggerEventTypeHelper(ulong value)
            {
                switch (value)
                {
                    case DDL_TRIGGER_CREATE_TABLE:
                        return TriggerEventType.CreateTable;
                    case DDL_TRIGGER_ALTER_TABLE:
                        return TriggerEventType.AlterTable;
                    case DDL_TRIGGER_DROP_TABLE:
                        return TriggerEventType.DropTable;
                    case DDL_TRIGGER_CREATE_PROCEDURE:
                        return TriggerEventType.CreateProcedure;
                    case DDL_TRIGGER_ALTER_PROCEDURE:
                        return TriggerEventType.AlterProcedure;
                    case DDL_TRIGGER_DROP_PROCEDURE:
                        return TriggerEventType.DropProcedure;
                    case DDL_TRIGGER_CREATE_FUNCTION:
                        return TriggerEventType.CreateFunction;
                    case DDL_TRIGGER_ALTER_FUNCTION:
                        return TriggerEventType.AlterFunction;
                    case DDL_TRIGGER_DROP_FUNCTION:
                        return TriggerEventType.DropFunction;
                    case DDL_TRIGGER_CREATE_TRIGGER:
                        return TriggerEventType.CreateTrigger;
                    case DDL_TRIGGER_ALTER_TRIGGER:
                        return TriggerEventType.AlterTrigger;
                    case DDL_TRIGGER_DROP_TRIGGER:
                        return TriggerEventType.DropTrigger;
                    case DDL_TRIGGER_CREATE_EXCEPTION:
                        return TriggerEventType.CreateException;
                    case DDL_TRIGGER_ALTER_EXCEPTION:
                        return TriggerEventType.AlterException;
                    case DDL_TRIGGER_DROP_EXCEPTION:
                        return TriggerEventType.DropException;
                    case DDL_TRIGGER_CREATE_VIEW:
                        return TriggerEventType.CreateView;
                    case DDL_TRIGGER_ALTER_VIEW:
                        return TriggerEventType.AlterView;
                    case DDL_TRIGGER_DROP_VIEW:
                        return TriggerEventType.DropView;
                    case DDL_TRIGGER_CREATE_DOMAIN:
                        return TriggerEventType.CreateDomain;
                    case DDL_TRIGGER_ALTER_DOMAIN:
                        return TriggerEventType.AlterDomain;
                    case DDL_TRIGGER_DROP_DOMAIN:
                        return TriggerEventType.DropDomain;
                    case DDL_TRIGGER_CREATE_ROLE:
                        return TriggerEventType.CreateRole;
                    case DDL_TRIGGER_ALTER_ROLE:
                        return TriggerEventType.AlterRole;
                    case DDL_TRIGGER_DROP_ROLE:
                        return TriggerEventType.DropRole;
                    case DDL_TRIGGER_CREATE_INDEX:
                        return TriggerEventType.CreateIndex;
                    case DDL_TRIGGER_ALTER_INDEX:
                        return TriggerEventType.AlterIndex;
                    case DDL_TRIGGER_DROP_INDEX:
                        return TriggerEventType.DropIndex;
                    case DDL_TRIGGER_CREATE_SEQUENCE:
                        return TriggerEventType.CreateSequence;
                    case DDL_TRIGGER_ALTER_SEQUENCE:
                        return TriggerEventType.AlterSequence;
                    case DDL_TRIGGER_DROP_SEQUENCE:
                        return TriggerEventType.DropSequence;
                    case DDL_TRIGGER_CREATE_USER:
                        return TriggerEventType.CreateUser;
                    case DDL_TRIGGER_ALTER_USER:
                        return TriggerEventType.AlterUser;
                    case DDL_TRIGGER_DROP_USER:
                        return TriggerEventType.DropUser;
                    case DDL_TRIGGER_CREATE_COLLATION:
                        return TriggerEventType.CreateCollation;
                    case DDL_TRIGGER_DROP_COLLATION:
                        return TriggerEventType.DropCollation;
                    case DDL_TRIGGER_ALTER_CHARACTER_SET:
                        return TriggerEventType.AlterCharacterSet;
                    case DDL_TRIGGER_CREATE_PACKAGE:
                        return TriggerEventType.CreatePackage;
                    case DDL_TRIGGER_ALTER_PACKAGE:
                        return TriggerEventType.AlterPackage;
                    case DDL_TRIGGER_DROP_PACKAGE:
                        return TriggerEventType.DropPackage;
                    case DDL_TRIGGER_CREATE_PACKAGE_BODY:
                        return TriggerEventType.CreatePackageBody;
                    case DDL_TRIGGER_DROP_PACKAGE_BODY:
                        return TriggerEventType.DropPackageBody;
                    case DDL_TRIGGER_CREATE_MAPPING:
                        return TriggerEventType.CreateMapping;
                    case DDL_TRIGGER_ALTER_MAPPING:
                        return TriggerEventType.AlterMapping;
                    case DDL_TRIGGER_DROP_MAPPING:
                        return TriggerEventType.DropMapping;
                    default:
                        return null;
                }
            }
        }

        public static (TriggerClassType, TriggerBeforeAfterType?, IList<TriggerEventType>) Parse(ulong type)
        {
            var @class = ParseTriggerClassType(type);
            switch (@class)
            {
                case TriggerClassType.DML:
                    {
                        var (beforeAfter, @event) = DML.Parse(type);
                        return (@class, beforeAfter, @event);
                    }
                case TriggerClassType.DB:
                    {
                        return (@class, null, DB.Parse(type));
                    }
                case TriggerClassType.DDL:
                    {
                        var (beforeAfter, @event) = DDL.Parse(type);
                        return (@class, beforeAfter, @event);
                    }
            }
            throw UnknownTriggerTypeException(type);
        }        

        private static TriggerClassType ParseTriggerClassType(ulong type)
        {
            switch (type & TRIGGER_TYPE_MASK)
            {
                case TRIGGER_TYPE_DML:
                    return TriggerClassType.DML;
                case TRIGGER_TYPE_DB:
                    return TriggerClassType.DB;
                case TRIGGER_TYPE_DDL:
                    return TriggerClassType.DDL;
            }
            throw UnknownTriggerTypeException(type);
        }

        private static Exception UnknownTriggerTypeException(ulong type) => new ArgumentOutOfRangeException($"Unknown trigger type {type}.");
    }
}
