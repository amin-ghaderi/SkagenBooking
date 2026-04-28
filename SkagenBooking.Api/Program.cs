using SkagenBooking.Application;
using SkagenBooking.Infrastructure.Composition;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructureInMemory();

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async httpContext =>
    {
        var exception = httpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

        // Map common "bad input" exceptions to 400. Everything else becomes 500.
        httpContext.Response.StatusCode = exception is ArgumentException or InvalidOperationException
            ? StatusCodes.Status400BadRequest
            : StatusCodes.Status500InternalServerError;

        await Results.Problem(
                statusCode: httpContext.Response.StatusCode,
                title: httpContext.Response.StatusCode == 400 ? "Bad Request" : "Internal Server Error",
                detail: httpContext.Response.StatusCode == 400 ? exception?.Message : "An unexpected error occurred.")
            .ExecuteAsync(httpContext);
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
