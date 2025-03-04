using FluentValidation;

namespace Web_API.Models.Validators;
public static class ValidatorRegistrationExtension
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<ProjectDTO>, ProjectDTOValidator>();
        services.AddScoped<IValidator<TaskDTO>, TaskDTOValidator>();
        return services;
    }
}
