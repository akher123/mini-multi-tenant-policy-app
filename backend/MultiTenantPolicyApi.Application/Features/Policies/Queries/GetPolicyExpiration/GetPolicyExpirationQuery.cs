using MediatR;
using MultiTenantPolicyApi.Application.Common.Models;

namespace MultiTenantPolicyApi.Application.Features.Policies.Queries.GetPolicyExpiration;

public sealed record GetPolicyExpirationQuery(string PolicyNumber) : IRequest<PolicyExpirationDto?>;
