using Jira.Application.ProjectTasks.DTOs;
using Jira.Application.ProjectTasks.Models;

namespace Jira.Application.ProjectTasks.Interfaces;

public interface IProjectTaskService
{
    Task<ProjectTaskResponse> CreateAsync(CreateProjectTaskModel model, CancellationToken cancellationToken);

    Task<GetProjectTasksResponse> GetAllAsync(GetProjectTasksModel model, CancellationToken cancellationToken);

    Task<ProjectTaskResponse> GetByIdAsync(GetProjectTaskByIdModel model, CancellationToken cancellationToken);

    Task<ProjectTaskResponse> UpdateAsync(UpdateProjectTaskModel model, CancellationToken cancellationToken);

    Task DeleteAsync(DeleteProjectTaskModel model, CancellationToken cancellationToken);
}
