using Eternelle.Modules.Weddings.Domain.EntourageGroups;
using Eternelle.Modules.Weddings.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.EntourageGroups;

internal sealed class EntourageCoupleConfiguration
{
    public static void Configure(OwnedNavigationBuilder<EntourageGroup, EntourageCouple> couple)
    {
        // HasCheckConstraint is configured via ToTable() per EF Core conventions.
        couple.ToTable("entourage_couples", t => t.HasCheckConstraint(
            "ck_entourage_couples_member_order",
            "member_a_id < member_b_id"));

        couple.HasKey(c => c.Id);

        // group_id FK — cascade delete is the EF Core default for owned entities.
        couple.WithOwner().HasForeignKey(c => c.GroupId);

        couple.Property(c => c.MemberAId).IsRequired();
        couple.Property(c => c.MemberBId).IsRequired();
        couple.Property(c => c.Note)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? InternalNote.FromPersistence(v) : null)
            .HasMaxLength(InternalNote.MaxLength);
        couple.Property(c => c.DisplayOrder).IsRequired();

        // MemberAId / MemberBId: EF Core OwnedNavigationBuilder (EF Core 9) does not
        // support HasOne<T> relationships to other owned types under the same owner, so
        // the composite FK constraints
        //   (group_id, member_a_id) → entourage_members(group_id, id)
        //   (group_id, member_b_id) → entourage_members(group_id, id)
        // cannot be expressed in the fluent model.
        // They are added via migrationBuilder.AddForeignKey() in the next corrective
        // migration, referencing the uq_entourage_members_group_id_id unique index
        // declared in EntourageMemberConfiguration.
        // Group-membership is also enforced at the domain level in EntourageGroup.PairMembers().

        // Unique pair within a group — prevents duplicate pairings at the DB level.
        couple.HasIndex(c => new { c.GroupId, c.MemberAId, c.MemberBId })
            .IsUnique()
            .HasDatabaseName("ix_entourage_couples_group_member_pair");

        // Single-column indexes on FK columns for fast FK lookups (avoids full-table scans on restrict deletes).
        couple.HasIndex(c => c.MemberAId)
            .HasDatabaseName("ix_entourage_couples_member_a");

        couple.HasIndex(c => c.MemberBId)
            .HasDatabaseName("ix_entourage_couples_member_b");
    }
}
