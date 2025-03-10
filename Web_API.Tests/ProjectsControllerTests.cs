using API_Abstract.DTO;
using API_Abstract.Managers;
using API_Abstract.POCO;
using DB_Manager.Models.DTO;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Web_API.Controllers;

namespace Web_API.Tests
{
    public class ProjectsControllerTests
    {
        private readonly Mock<IProjectManager> _mockProjectManager = new();
        private readonly Mock<ILogger<ProjectsController>> _mockLogger = new();
        private readonly Mock<IValidator<Models.ProjectDTO>> _mockValidator = new();
        private readonly ProjectsController _projectsController;

        public ProjectsControllerTests()
        {
            _projectsController = new(_mockProjectManager.Object, _mockLogger.Object);
        }

        #region GetProjects
        [Theory]
        [MemberData(nameof(GetWrongPageParams))]
        public async Task GetProjects_ReturnsBadRequest_WrongParamValue(int page, int pageSize)
        {
            // Arrange

            // Act
            var result = await _projectsController.GetProjects(page, pageSize);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<IProject>>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetProjects_ReturnsAllProjects()
        {
            // Arrange
            _mockProjectManager.Setup(manager => manager.GetProjectsAsync()).ReturnsAsync(AllProjects).Verifiable();
            int exprctedProjectCount = AllProjects.Count();

            // Act
            var result = await _projectsController.GetProjects();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<IProject>>>(result);
            var actionOk = Assert.IsType<OkObjectResult>(actionResult.Result);
            var projects = Assert.IsAssignableFrom<IEnumerable<IProject>>(actionOk.Value);
            Assert.Equal(exprctedProjectCount, projects.Count());
            _mockProjectManager.Verify(manager => manager.GetProjectsAsync());
        }

        [Fact]
        public async Task GetProjects_ReturnsPageProjects()
        {
            // Arrange
            _mockProjectManager.Setup(manager => manager.GetPagedProjectsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(AllProjects).Verifiable();
            int exprctedProjectCount = AllProjects.Count();

            // Act
            var result = await _projectsController.GetProjects(10, 10);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<IProject>>>(result);
            var actionOk = Assert.IsType<OkObjectResult>(actionResult.Result);
            var projects = Assert.IsAssignableFrom<IEnumerable<IProject>>(actionOk.Value);
            Assert.Equal(exprctedProjectCount, projects.Count());
            _mockProjectManager.Verify(manager => manager.GetPagedProjectsAsync(It.IsAny<int>(), It.IsAny<int>()));
        }
        #endregion

        #region GetProject
        [Theory]
        [MemberData(nameof(GetWrongProjectId))]
        public async Task GetProject_ReturnsBadRequest_WrongParamValue(int id)
        {
            // Arrange

            // Act
            var result = await _projectsController.GetProject(id);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IProject>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetProject_ReturnsNotFound()
        {
            // Arrange
            _mockProjectManager.Setup(manager => manager.GetProjectAsync(It.IsAny<int>())).ReturnsAsync(NullProject).Verifiable();

            // Act
            var result = await _projectsController.GetProject(10);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IProject>>(result);
            var actionNotFound = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            _mockProjectManager.Verify();

        }

        [Fact]
        public async Task GetProject_ReturnsProject()
        {
            // Arrange
            _mockProjectManager.Setup(manager => manager.GetProjectAsync(It.IsAny<int>())).ReturnsAsync(OneProject);
            IProject expectedProject = OneProject;

            // Act
            var result = await _projectsController.GetProject(7);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IProject>>(result);
            var actionOk = Assert.IsType<OkObjectResult>(actionResult.Result);
            var project = Assert.IsAssignableFrom<IProject>(actionOk.Value);

            Assert.Equal(OneProject.Id, project.Id);
            Assert.Equal(OneProject.Name, project.Name);
            Assert.Equal(OneProject.Description, project.Description);
        }
        #endregion

        #region PostProject
        [Fact]
        public async Task PostProject_ReturnsBadRequest()
        {
            // Arrange
            ValidationResult validationResult = new();
            validationResult.Errors.Add(new ValidationFailure("SomeProperty", "SomeError"));
            _mockValidator.Setup(validator => validator.ValidateAsync(It.IsAny<Models.ProjectDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult).Verifiable();

            // Act
            var result = await _projectsController.PostProject(OneProjectDTO, _mockValidator.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IProject>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            _mockValidator.Verify();

        }

        [Fact]
        public async Task PostProject_ReturnsProject()
        {
            // Arrange
            _mockValidator.Setup(validator => validator.ValidateAsync(It.IsAny<Models.ProjectDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult()).Verifiable();
            _mockProjectManager.Setup(manager => manager.CreateProjectAsync(It.IsAny<IProjectDTO>())).ReturnsAsync(OneProject).Verifiable();
            IProject expectedProject = OneProject;

            // Act
            var result = await _projectsController.PostProject(OneProjectDTO, _mockValidator.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IProject>>(result);
            var actionOk = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var project = Assert.IsAssignableFrom<IProject>(actionOk.Value);

            _mockValidator.Verify();
            _mockProjectManager.Verify();

            Assert.Equal(OneProject.Id, project.Id);
            Assert.Equal(OneProject.Name, project.Name);
            Assert.Equal(OneProject.Description, project.Description);
        }
        #endregion

        #region PutProject
        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task PutProject_ReturnsBadRequest_WrongId(int id)
        {
            // Arrange

            // Act
            var result = await _projectsController.PutProject(id, OneProjectDTO, _mockValidator.Object);

            // Assert
            var actionResult = Assert.IsAssignableFrom<ActionResult>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task PutProject_ReturnsBadRequest_Validation()
        {
            // Arrange
            ValidationResult validationResult = new();
            validationResult.Errors.Add(new ValidationFailure("SomeProperty", "SomeError"));
            _mockValidator.Setup(validator => validator.ValidateAsync(It.IsAny<Models.ProjectDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult).Verifiable();

            // Act
            var result = await _projectsController.PutProject(7, OneProjectDTO, _mockValidator.Object);

            // Assert
            var actionResult = Assert.IsAssignableFrom<ActionResult>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            _mockValidator.Verify();
        }

        [Fact]
        public async Task PutProject_ReturnsNotFound()
        {
            // Arrange
            _mockProjectManager.Setup(manager => manager.UpdateProjectAsync(It.IsAny<int>(), It.IsAny<IProjectDTO>())).ReturnsAsync(false).Verifiable();
            _mockValidator.Setup(validator => validator.ValidateAsync(It.IsAny<Models.ProjectDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult()).Verifiable();

            // Act
            var result = await _projectsController.PutProject(99, OneProjectDTO, _mockValidator.Object);

            // Assert
            var actionResult = Assert.IsAssignableFrom<ActionResult>(result);
            var actionNotFound = Assert.IsType<NotFoundObjectResult>(actionResult);

            _mockValidator.Verify();
            _mockProjectManager.Verify();
        }

        [Fact]
        public async Task PutProject_ReturnsOk()
        {
            // Arrange
            _mockProjectManager.Setup(manager => manager.UpdateProjectAsync(It.IsAny<int>(), It.IsAny<IProjectDTO>())).ReturnsAsync(true).Verifiable();
            _mockValidator.Setup(validator => validator.ValidateAsync(It.IsAny<Models.ProjectDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult()).Verifiable();

            // Act
            var result = await _projectsController.PutProject(99, OneProjectDTO, _mockValidator.Object);

            // Assert
            var actionResult = Assert.IsAssignableFrom<ActionResult>(result);
            Assert.IsType<NoContentResult>(actionResult);

            _mockValidator.Verify();
            _mockProjectManager.Verify();
        }
        #endregion

        #region DeleteProject
        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task DeleteProject_ReturnsBadRequest_WrongId(int id)
        {
            // Arrange

            // Act
            var result = await _projectsController.DeleteProject(id);

            // Assert
            var actionResult = Assert.IsAssignableFrom<ActionResult>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task DeleteProject_ReturnsNotFound()
        {
            // Arrange
            _mockProjectManager.Setup(manager => manager.DeleteProjectAsync(It.IsAny<int>())).ReturnsAsync(false).Verifiable();

            // Act
            var result = await _projectsController.DeleteProject(99);

            // Assert
            var actionResult = Assert.IsAssignableFrom<ActionResult>(result);
            var actionNotFound = Assert.IsType<NotFoundObjectResult>(actionResult);

            _mockProjectManager.Verify();
        }

        [Fact]
        public async Task DeleteProject_ReturnsOk()
        {
            // Arrange
            _mockProjectManager.Setup(manager => manager.DeleteProjectAsync(It.IsAny<int>())).ReturnsAsync(true).Verifiable();

            // Act
            var result = await _projectsController.DeleteProject(99);

            // Assert
            var actionResult = Assert.IsAssignableFrom<ActionResult>(result);
            Assert.IsType<OkResult>(actionResult);

            _mockProjectManager.Verify();
        }
        #endregion

        #region MemberData
        public static IEnumerable<object[]> GetWrongPageParams => [ [-1, 5], [0, -1], [-7, -4 ] ];
        public static IEnumerable<object[]> GetWrongProjectId => [ [-5], [-1], [0] ];

        #endregion

        #region DataSource
        private static IEnumerable<IProject> AllProjects => new List<ProjectDTO>
            {
                new() { Id = 1, Name = "Test project 1", Description = "Description project 1", CreatedAt = DateTime.Now, Tasks = [] },
                new() { Id = 2, Name = "Test project 2", Description = "Description project 2", CreatedAt = DateTime.Now, Tasks = [] },
                new() { Id = 3, Name = "Test project 3", Description = "Description project 3", CreatedAt = DateTime.Now, Tasks = [] },
            };

        private static ProjectDTO OneProject =>
            new() { Id = 4, Name = "Test project 4", Description = "Description project 4", CreatedAt = DateTime.Now, Tasks = [] };

        private static Models.ProjectDTO OneProjectDTO => new() { name = "Test project 5", description = "Description project 5"};


        private static IProject? NullProject => null;
        #endregion
    }
}
