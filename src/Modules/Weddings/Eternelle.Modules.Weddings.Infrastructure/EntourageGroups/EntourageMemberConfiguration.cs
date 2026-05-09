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

        // Alternate key on (group_id, id) is intentionally omitted:
        // OwnedNavigationBuilder does not expose HasAlternateKey in EF Core 9.
        // Group-membership uniqueness is enforced at the domain level.
        member.HasIndex(m => m.GroupId)
            .HasDatabaseName("ix_entourage_members_group_id");

        member.HasIndex(m => m.WeddingId)
            .HasDatabaseName("ix_entourage_members_wedding_id");
    }
}
