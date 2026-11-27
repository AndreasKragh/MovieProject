using Microsoft.AspNetCore.Authorization;
using MovieVault.Api.Entities;

namespace MovieVault.Api.Authorization;

// Requirement: "må se denne skuespillers movie-performance"
public class CanViewActorPerformanceRequirement : IAuthorizationRequirement { }

// Resource-based handler: resource er den Actor, der spørges om.
// Admin: alle skuespillere. Actor: kun sig selv ("own performance only").
public class ActorPerformanceHandler
    : AuthorizationHandler<CanViewActorPerformanceRequirement, Actor>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanViewActorPerformanceRequirement requirement,
        Actor resource)
    {
        var userId = context.User.GetUserId();

        if (context.User.IsAdmin() ||
            (userId != null && resource.ApiUserId == userId))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
