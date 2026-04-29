using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

public static class EntourageGroupErrors
{
    public static Error NotFound(EntourageGroupId id) =>
        Error.NotFound(
            "EntourageGroups.NotFound",
            $"The entourage group with the identifier {id.Value} was not found");

    public static Error MemberNotFound(EntourageMemberId id) =>
        Error.NotFound(
            "EntourageGroups.MemberNotFound",
            $"The entourage member with the identifier {id.Value} was not found in this group");

    public static Error CoupleNotFound(EntourageCoupleId id) =>
        Error.NotFound(
            "EntourageGroups.CoupleNotFound",
            $"The couple pairing with the identifier {id.Value} was not found in this group");

    public static Error CoupleAlreadyExists(EntourageMemberId firstId, EntourageMemberId secondId) =>
        Error.Conflict(
            "EntourageGroups.CoupleAlreadyExists",
            $"A couple pairing already exists for members {firstId.Value} and {secondId.Value}");

    public static readonly Error CannotPairMemberWithSelf =
        Error.Problem(
            "EntourageGroups.CannotPairMemberWithSelf",
            "A member cannot be paired with themselves");

    public static Error CouplesNotAllowed(EntourageGroupType groupType) =>
        Error.Problem(
            "EntourageGroups.CouplesNotAllowed",
            $"The group type '{groupType}' does not support couple pairings");

    public static Error ReorderListMismatch() =>
        Error.Conflict(
            "EntourageGroups.ReorderListMismatch",
            "The provided ID list must contain every entourage group for this wedding exactly once");

    public static Error MemberReorderListMismatch() =>
        Error.Conflict(
            "EntourageGroups.MemberReorderListMismatch",
            "The provided ID list must contain every entourage member in this group exactly once");
}
