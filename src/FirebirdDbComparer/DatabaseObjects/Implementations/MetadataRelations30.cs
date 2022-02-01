using System;

using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.DatabaseObjects.Implementations;

public class MetadataRelations30 : MetadataRelations25
{
    public MetadataRelations30(IMetadata metadata, ISqlHelper sqlHelper)
        : base(metadata, sqlHelper)
    { }

    protected override string RelationFieldCommandText => @"
select trim(RF.RDB$RELATION_NAME) as RDB$RELATION_NAME,
       trim(RF.RDB$FIELD_NAME) as RDB$FIELD_NAME,
       trim(RF.RDB$FIELD_SOURCE) as RDB$FIELD_SOURCE,
       RF.RDB$FIELD_POSITION,
       trim(RF.RDB$BASE_FIELD) as RDB$BASE_FIELD,
       RF.RDB$VIEW_CONTEXT,
       RF.RDB$DESCRIPTION,
       iif(coalesce(RF.RDB$NULL_FLAG, 0) = 0, 0, 1) as RDB$NULL_FLAG,
       RF.RDB$DEFAULT_SOURCE,
       RF.RDB$COLLATION_ID,
       RF.RDB$SYSTEM_FLAG,
       trim(RF.RDB$GENERATOR_NAME) as RDB$GENERATOR_NAME,
       RF.RDB$IDENTITY_TYPE
  from RDB$RELATION_FIELDS RF";

    protected override string ViewRelationCommandText => @"
select trim(VR.RDB$VIEW_NAME) as RDB$VIEW_NAME,
       trim(VR.RDB$RELATION_NAME) as RDB$RELATION_NAME,
       VR.RDB$VIEW_CONTEXT,
       trim(VR.RDB$CONTEXT_NAME) as RDB$CONTEXT_NAME,
       VR.RDB$CONTEXT_TYPE,
       trim(VR.RDB$PACKAGE_NAME) as RDB$PACKAGE_NAME
  from RDB$VIEW_RELATIONS VR";

    public override void FinishInitialization()
    {
        base.FinishInitialization();

        foreach (var relationField in RelationFields.Values)
        {
            if (relationField.GeneratorName != null)
            {
                relationField.Generator = Metadata.MetadataGenerators.GeneratorsByName[relationField.GeneratorName];
            }
        }

        foreach (var viewRelation in ViewRelations)
        {
            viewRelation.View = Views[viewRelation.ViewName];
            switch (viewRelation.ContextType)
            {
                case ContextTypeType.Table:
                    viewRelation.ContextRelation = Relations[viewRelation.ViewContextName];
                    break;
                case ContextTypeType.View:
                    viewRelation.ContextView = Views[viewRelation.ViewContextName];
                    break;
                case ContextTypeType.Procedure:
                    viewRelation.ContextProcedure = Metadata.MetadataProcedures.ProceduresByName[viewRelation.ViewContextName];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (viewRelation.PackageName != null)
            {
                viewRelation.Package = Metadata.MetadataPackages.PackagesByName[viewRelation.PackageName];
            }
        }
    }
}
