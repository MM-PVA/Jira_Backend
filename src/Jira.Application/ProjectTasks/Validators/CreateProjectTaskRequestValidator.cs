using FluentValidation;

using Jira.Application.ProjectTasks.DTOs;

namespace Jira.Application.ProjectTasks.Validators;

public sealed class CreateProjectTaskRequestValidator : AbstractValidator<CreateProjectTaskRequest>
{
    public CreateProjectTaskRequestValidator()
    {
        _ = RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(100)
            .WithMessage("Title cannot exceed 100 characters.")
            // Regex
            .Matches(@"^[a-zA-Z0-9\s]+$")
            .WithMessage("Title must not contain special characters.");

        _ = RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters.")
            .Matches(@"^[^0-9].*")
            .WithMessage("Description must not start with a number.");

        _ = RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Invalid task priority.");

        _ = RuleFor(x => x.AssigneeId)
            .NotEqual(Guid.Empty)
            .WithMessage("Assignee is required.");

        _ = RuleFor(x => x.DueDate)
            .Must(date => !date.HasValue || date.Value > DateTime.UtcNow)
            .WithMessage("Due date must be in the future.");
    }
}
