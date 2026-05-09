using Eternelle.Modules.Weddings.Domain.EntourageGroups;
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
        couple.Property(c => c.Note).HasMaxLength(EntourageCouple.MaxNoteLength);
        couple.Property(c => c.DisplayOrder).IsRequired();

        // FK member_a_id → entourage_members.id.
        // A composite (group_id, member_a_id) FK is not expressible via EF Core's fluent
        // API for owned-to-owned relationships — HasPrincipalKey is non-generic and
        // HasAlternateKey is unavailable on OwnedNavigationBuilder. Group-membership
        // is enforced at the domain level inside EntourageGroup.PairMembers().
        couple.HasOne<EntourageMember>()
            .WithMany()
            .HasForeignKey(c => c.MemberAId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_entourage_couples_member_a");

        // FK member_b_id → entourage_members.id. Same rationale as member A above.
        couple.HasOne<EntourageMember>()
            .WithMany()
            .HasForeignKey(c => c.MemberBId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_entourage_couples_member_b");

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
