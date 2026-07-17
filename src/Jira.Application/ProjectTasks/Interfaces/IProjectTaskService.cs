using Jira.Application.ProjectTasks.DTOs;
using Jira.Application.ProjectTasks.Models;

namespace Jira.Application.ProjectTasks.Interfaces;

public interface IProjectTaskService
{
    Task<ProjectTaskResponse> CreateAsync(CreateProjectTaskModel model);

    Task<IEnumerable<ProjectTaskResponse>> GetAllAsync(GetProjectTasksModel model);

    Task<ProjectTaskResponse> GetByIdAsync(GetProjectTaskByIdModel model);

    Task<ProjectTaskResponse> UpdateAsync(UpdateProjectTaskModel model);

    Task DeleteAsync(DeleteProjectTaskModel model);
}
