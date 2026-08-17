using MediatR;
using MultiTenantPolicyApi.Application.Common.Models;

namespace MultiTenantPolicyApi.Application.Features.Policies.Queries.GetExpiringPolicies;

public sealed record GetExpiringPoliciesQuery(int WithinDays) : IRequest<IReadOnlyList<ExpiringPolicyDto>>;
