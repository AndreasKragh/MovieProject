using Microsoft.AspNetCore.Authorization;
using MovieVault.Api.Entities;

namespace MovieVault.Api.Authorization;

// Requirement: "må ændre/slette denne anmeldelse"
public class CanManageReviewRequirement : IAuthorizationRequirement { }

// Resource-based handler (slide 63-64, ligesom IsRecipeOwnerHandler): resource er anmeldelsen.
// Admin: alle anmeldelser. Reviewer: kun egne. Controlleren skal have indlæst review.Reviewer.
public class ReviewOwnerHandler
    : AuthorizationHandler<CanManageReviewRequirement, Review>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanManageReviewRequirement requirement,
        Review resource)
    {
        var userId = context.User.GetUserId();

        if (context.User.IsAdmin() ||
            (userId != null && resource.Reviewer?.ApiUserId == userId))
        {
            context.Succeed(requirement);
        }

        // Ellers gør vi ingenting -> requirement ikke opfyldt -> controlleren returnerer 403
        return Task.CompletedTask;
    }
}
