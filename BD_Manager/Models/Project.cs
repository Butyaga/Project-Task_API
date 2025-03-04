using API_Abstract.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Manager.Models;

public class Project : IProjectPOCO
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public IEnumerable<ITaskPOCO> Tasks { get; set; } = [];
}
