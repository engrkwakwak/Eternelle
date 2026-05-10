using Eternelle.Modules.Weddings.Domain.EntourageGroups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.EntourageGroups;

internal sealed class EntourageMemberConfiguration
{
    public static void Configure(OwnedNavigationBuilder<EntourageGroup, EntourageMember> member)
    {
        member.ToTable("entourage_members");
        member.HasKey(m => m.Id);

        // group_id FK — cascade delete is the EF Core default for owned entities.
        member.WithOwner().HasForeignKey(m => m.GroupId);

        // wedding_id — denormalized for direct queries by WeddingId without loading the group.
        // Converter registered globally in WeddingsDbContext.ConfigureConventions.
        member.Property(m => m.WeddingId).IsRequired();

        member.Property(m => m.Name).IsRequired().HasMaxLength(EntourageMember.MaxNameLength);
        member.Property(m => m.Role).IsRequired().HasMaxLength(EntourageMember.MaxRoleLength);
        member.Property(m => m.ImageUrl);
        member.Property(m => m.Message).HasMaxLength(EntourageMember.MaxMessageLength);
        member.Property(m => m.Note).HasMaxLength(EntourageMember.MaxNoteLength);
        member.Property(m => m.Seed);
        member.Property(m => m.DisplayOrder).IsRequired();

        // (group_id, id) unique index — required as the principal-column set for the
        // composite FKs on entourage_couples.member_a_id / member_b_id.
        // Because id is already the PK, uniqueness is trivial, but PostgreSQL requires
        // an explicit unique index on the exact column set referenced by a composite FK.
        // HasAlternateKey is not exposed on OwnedNavigationBuilder (EF Core 9 limitation),
        // so HasIndex().IsUnique() is used to produce the same DB-level unique index.
        member.HasIndex(m => new { m.GroupId, m.Id })
            .IsUnique()
            .HasDatabaseName("uq_entourage_members_group_id_id");

        member.HasIndex(m => m.GroupId)
            .HasDatabaseName("ix_entourage_members_group_id");

        member.HasIndex(m => m.WeddingId)
            .HasDatabaseName("ix_entourage_members_wedding_id");
    }
}
