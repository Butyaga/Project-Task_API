using API_Abstract.DTO;
using API_Abstract.Managers;
using API_Abstract.POCO;
using DB_Manager.DBCntxt;
using DB_Manager.Models;
using DB_Manager.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace DB_Manager.Managers;
public class ProjectManager(PgSQLContext _context) : IProjectManager
{
    public async Task<IProject> CreateProjectAsync(IProjectDTO projectDTO)
    {
        Project newProject = new() { Name = projectDTO.name, Description = projectDTO.description };
        await _context.Projects.AddAsync(newProject);
        int result = await _context.SaveChangesAsync();
        if (result == 0)
        {
            throw new Exception("Project creation error in DB");
        }
        return newProject.MapToDTO();
    }

    public async Task<bool> DeleteProjectAsync(int Id)
    {
        Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == Id);

        if (project is null)
        {
            return false;
        }

        _context.Projects.Remove(project);
        int result = await _context.SaveChangesAsync();
        if (result == 0)
        {
            throw new Exception("Project creation error in DB");
        }
        return true;
    }

    public async Task<IEnumerable<IProject>> GetPagedProjectsAsync(int pageIndex, int pageSize)
    {
        int countSkippedItems = pageIndex * pageSize;
        IQueryable<Project> requestedPageQuery = _context.Projects.AsNoTracking().Skip(countSkippedItems).Take(pageSize);
        List<Project> projects = await requestedPageQuery.ToListAsync();
        IEnumerable<IProject> projectCollection = [.. from project in projects select project.MapToDTO()];
        return projectCollection;
    }

    public async Task<IProject?> GetProjectAsync(int Id)
    {
        Project? project = await _context.Projects.AsNoTracking().Include(p => p.Tasks).FirstOrDefaultAsync(p => p.Id == Id);

        if (project is null)
        {
            return null;
        }

        return project.MapToDTO();
    }

    public async Task<IEnumerable<IProject>> GetProjectsAsync()
    {
        List<Project> projects = await _context.Projects.AsNoTracking().ToListAsync();
        IEnumerable<IProject> projetsDTO = [.. from project in projects select project.MapToDTO()];
        return projetsDTO;
    }

    public async Task<bool> UpdateProjectAsync(int Id, IProjectDTO projectDTO)
    {
        Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == Id);
        if (project is null)
        {
            return false;
        }

        project.Name = projectDTO.name;
        project.Description = projectDTO.description;

        int result = await _context.SaveChangesAsync();
        if (result == 0)
        {
            throw new Exception("Project update error in DB");
        }
        return true;
    }
}
