namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

/// <summary>
/// Controls how the group's members are laid out in the rendered section.
///
///   Cards — photo cards with name and role (default)
///   List  — compact text rows
///
/// Maps to wedding.entourage_render_mode PostgreSQL ENUM.
/// </summary>
public enum EntourageRenderMode
{
    Cards = 1,
    List = 2
}
