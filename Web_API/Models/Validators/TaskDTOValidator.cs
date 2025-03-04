using FluentValidation;

namespace Web_API.Models.Validators;
public class TaskDTOValidator : AbstractValidator<TaskDTO>
{
    public TaskDTOValidator()
    {
        RuleFor(task => task.title).Length(2, 100);
        RuleFor(task => task.projectId).GreaterThan(0);
    }
}
