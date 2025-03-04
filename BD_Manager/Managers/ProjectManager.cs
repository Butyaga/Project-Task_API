using API_Abstract.Managers;
using API_Abstract.POCO;
using DB_Manager.DBCntxt;
using DB_Manager.Models;
using Microsoft.EntityFrameworkCore;

namespace DB_Manager.Managers;
class ProjectManager(PgSQLContext _context) : IProjectManager
{
    public async Task<IProjectPOCO?> CreateProjectAsync(string name, string? description)
    {
        Project newProject = new() { Name = name, Description = description };
        await _context.Projects.AddAsync(newProject);
        int rezult = await _context.SaveChangesAsync();
        if (rezult == 0)
        {
            return null;
        }
        return newProject;
    }

    public async Task<bool> DeleteProjectAsync(int Id)
    {
        Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == Id);

        if (project is null)
        {
            return false;
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<IProjectPOCO>> GetPagedProjectsAsync(int pageIndex, int pageSize)
    {
        int countScipedPeges = pageIndex * pageSize;
        IQueryable<Project> page = _context.Projects.Skip(countScipedPeges).Take(pageSize);
        return await page.ToListAsync();
    }

    public async Task<IProjectPOCO?> GetProjectAsync(int Id)
    {
        Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == Id);

        if (project is null)
        {
            return null;
        }

        return project;
    }

    public async Task<IEnumerable<IProjectPOCO>> GetProjectsAsync()
    {
        return await _context.Projects.ToListAsync();
    }

    public async Task<IProjectPOCO?> UpdateProjectAsync(int Id, string? name, string? description)
    {
        Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == Id);
        if (project is null)
        {
            return null;
        }

        if (name is not null)
        {
            project.Name = name;
        }
        if (description is not null)
        {
            project.Description = description;
        }

        await _context.SaveChangesAsync();
        return project;
    }
}
