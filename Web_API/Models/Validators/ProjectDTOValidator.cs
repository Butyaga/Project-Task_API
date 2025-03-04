using FluentValidation;

namespace Web_API.Models.Validators;
public class ProjectDTOValidator : AbstractValidator<ProjectDTO>
{
    public ProjectDTOValidator()
    {
        RuleFor(project => project.name).Length(2, 100);
    }
}
