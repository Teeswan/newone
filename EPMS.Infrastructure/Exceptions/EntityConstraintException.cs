using System;

namespace EPMS.Infrastructure.Exceptions
{
    public class EntityConstraintException : Exception
    {
        public string EntityName { get; }
        public string ConstraintName { get; }
        public object EntityId { get; }

        public EntityConstraintException(string message, string entityName, string constraintName, object entityId)
            : base(message)
        {
            EntityName = entityName;
            ConstraintName = constraintName;
            EntityId = entityId;
        }

        public EntityConstraintException(string message, Exception innerException)
            : base(message, innerException)
        {
            EntityName = string.Empty;
            ConstraintName = string.Empty;
            EntityId = 0;
        }
    }

    public class RelatedEntityExistsException : EntityConstraintException
    {
        public string RelatedEntityType { get; }
        public int RelatedEntityId { get; }

        public RelatedEntityExistsException(string entityName, object entityId, string relatedEntityType, int relatedEntityId)
            : base(
                $"Cannot delete {entityName} with ID {entityId} because it is referenced by {relatedEntityType} records.",
                entityName,
                "FK_REFERENCE_CONSTRAINT",
                entityId)
        {
            RelatedEntityType = relatedEntityType;
            RelatedEntityId = relatedEntityId;
        }
    }
}