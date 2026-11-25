namespace MovieVault.Api.Constants;

// Rollenavnene samlet ét sted, så de ikke står som løse strenge rundt i koden.
// Bruges både når rollerne seedes og i [Authorize(Roles = RoleNames.Admin)].
public static class RoleNames
{
    public const string Actor = "Actor";
    public const string Director = "Director";
    public const string Reviewer = "Reviewer";
    public const string Admin = "Admin";

    public static readonly string[] All = { Actor, Director, Reviewer, Admin };
}
