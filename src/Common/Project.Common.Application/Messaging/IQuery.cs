using FluentResults;
using MediatR;

namespace Project.Common.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
