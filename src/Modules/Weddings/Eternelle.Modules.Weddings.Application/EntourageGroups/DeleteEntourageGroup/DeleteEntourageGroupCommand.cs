using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.DeleteEntourageGroup;

public sealed record DeleteEntourageGroupCommand(Guid WeddingId, Guid EntourageGroupId) : ICommand;
