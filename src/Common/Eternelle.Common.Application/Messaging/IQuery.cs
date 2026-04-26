using Eternelle.Common.Domain;
using MediatR;

namespace Eternelle.Common.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
