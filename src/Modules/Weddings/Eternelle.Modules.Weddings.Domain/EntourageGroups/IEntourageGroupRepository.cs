using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

public interface IEntourageGroupRepository
{
    Task<EntourageGroup?> GetAsync(EntourageGroupId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the group with its Members and Couples collections fully loaded.
    /// Use this overload when adding, updating, removing members or couples.
    /// </summary>
    Task<EntourageGroup?> GetWithMembersAsync(EntourageGroupId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the group with its Members and Couples collections fully loaded,
    /// looked up by a member ID. Use this when the caller only has a member ID
    /// (Update / Remove member flows) and needs the owning aggregate.
    /// </summary>
    Task<EntourageGroup?> GetWithMembersByMemberIdAsync(EntourageMemberId memberId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EntourageGroup>> GetByWeddingIdAsync(WeddingId weddingId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all groups for the wedding with their Members and Couples collections
    /// fully loaded. Use this overload when rendering the full entourage section.
    /// </summary>
    Task<IReadOnlyList<EntourageGroup>> GetByWeddingIdWithMembersAsync(WeddingId weddingId, CancellationToken cancellationToken = default);

    void Insert(EntourageGroup entourageGroup);

    void Update(EntourageGroup entourageGroup);

    void Delete(EntourageGroup entourageGroup);
}
