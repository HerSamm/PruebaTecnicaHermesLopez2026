using FluentValidation;

namespace PruebaTecnicaHermes.Api.Endpoints;

public class ValidationFilter<T>(IValidator<T> validator) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<T>().FirstOrDefault();
        if (request is null)
            return Results.BadRequest(new { error = "El body de la petición es obligatorio." });

        var result = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);
        if (!result.IsValid)
            return Results.ValidationProblem(result.ToDictionary());

        return await next(context);
    }
}

public static class ValidationFilterExtensions
{
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder) =>
        builder.AddEndpointFilter<ValidationFilter<T>>()
               .ProducesValidationProblem();
}
