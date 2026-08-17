using MediatR;
using MultiTenantPolicyApi.Application.Common.Models;

namespace MultiTenantPolicyApi.Application.Features.Policies.Queries.GetCustomerPolicy;

public sealed record GetCustomerPolicyQuery(Guid CustomerId) : IRequest<PolicyDto?>;
