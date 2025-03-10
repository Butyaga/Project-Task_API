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
    public class TasksControllerTests
    {
        private readonly Mock<ITaskManager> _mockTaskManager = new();
        private readonly Mock<ILogger<TasksController>> _mockLogger = new();
        private readonly Mock<IValidator<Models.TaskDTO>> _mockValidator = new();
        private readonly TasksController _tasksController;

        public TasksControllerTests()
        {
            _tasksController = new(_mockTaskManager.Object, _mockLogger.Object);
        }

        #region GetTasks
        [Theory]
        [MemberData(nameof(GetWrongParams))]
        public async Task GetTasks_ReturnsBadRequest_WrongParamValue(bool isCompleted, int projectId)
        {
            // Arrange

            // Act
            var result = await _tasksController.GetTasks(isCompleted, projectId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ITask>>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetTasks_ReturnsAllTasks()
        {
            // Arrange
            _mockTaskManager.Setup(manager => manager.GetTasksAsync(It.IsAny<Nullable<bool>>(), It.IsAny<Nullable<int>>())).ReturnsAsync(AllTasks).Verifiable();
            int exprctedTaskCount = AllTasks.Count();

            // Act
            var result = await _tasksController.GetTasks(null, null);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ITask>>>(result);
            var actionOk = Assert.IsType<OkObjectResult>(actionResult.Result);
            var tasks = Assert.IsAssignableFrom<IEnumerable<ITask>>(actionOk.Value);
            Assert.Equal(exprctedTaskCount, tasks.Count());
            _mockTaskManager.Verify();
        }

        [Fact]
        public async Task GetTasks_ReturnsFilteredTasks()
        {
            // Arrange
            _mockTaskManager.Setup(manager => manager.GetTasksAsync(It.IsAny<bool>(), It.IsAny<int>())).ReturnsAsync(AllTasks).Verifiable();
            int exprctedTaskCount = AllTasks.Count();

            // Act
            var result = await _tasksController.GetTasks(false, 1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ITask>>>(result);
            var actionOk = Assert.IsType<OkObjectResult>(actionResult.Result);
            var projects = Assert.IsAssignableFrom<IEnumerable<ITask>>(actionOk.Value);
            Assert.Equal(exprctedTaskCount, projects.Count());
            _mockTaskManager.Verify();
        }
        #endregion

        #region GetTask
        [Theory]
        [MemberData(nameof(GetWrongTaskId))]
        public async Task GetTask_ReturnsBadRequest_WrongParamValue(int id)
        {
            // Arrange

            // Act
            var result = await _tasksController.GetTask(id);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ITask>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetTask_ReturnsNotFound()
        {
            // Arrange
            _mockTaskManager.Setup(manager => manager.GetTaskAsync(It.IsAny<int>())).ReturnsAsync(NullTask).Verifiable();

            // Act
            var result = await _tasksController.GetTask(99);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ITask>>(result);
            var actionNotFound = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            _mockTaskManager.Verify();
        }

        [Fact]
        public async Task GetTask_ReturnsTask()
        {
            // Arrange
            _mockTaskManager.Setup(manager => manager.GetTaskAsync(It.IsAny<int>())).ReturnsAsync(OneTask);
            ITask expectedProject = OneTask;

            // Act
            var result = await _tasksController.GetTask(4);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ITask>>(result);
            var actionOk = Assert.IsType<OkObjectResult>(actionResult.Result);
            var task = Assert.IsAssignableFrom<ITask>(actionOk.Value);

            Assert.Equal(OneTask.Id, task.Id);
            Assert.Equal(OneTask.Title, task.Title);
            Assert.Equal(OneTask.Description, task.Description);
        }
        #endregion

        #region PostTask
        [Fact]
        public async Task PostTask_ReturnsBadRequest()
        {
            // Arrange
            ValidationResult validationResult = new();
            validationResult.Errors.Add(new ValidationFailure("SomeProperty", "SomeError"));
            _mockValidator.Setup(validator => validator.ValidateAsync(It.IsAny<Models.TaskDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult).Verifiable();

            // Act
            var result = await _tasksController.PostTask(OneTaskDTO, _mockValidator.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ITask>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            _mockValidator.Verify();
        }

        [Fact]
        public async Task PostTask_ReturnsTask()
        {
            // Arrange
            _mockValidator.Setup(validator => validator.ValidateAsync(It.IsAny<Models.TaskDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult()).Verifiable();
            _mockTaskManager.Setup(manager => manager.CreateTaskAsync(It.IsAny<ITaskDTO>())).ReturnsAsync(OneTask).Verifiable();
            ITask expectedTask = OneTask;

            // Act
            var result = await _tasksController.PostTask(OneTaskDTO, _mockValidator.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ITask>>(result);
            var actionOk = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var task = Assert.IsAssignableFrom<ITask>(actionOk.Value);

            _mockValidator.Verify();
            _mockTaskManager.Verify();

            Assert.Equal(OneTask.Id, task.Id);
            Assert.Equal(OneTask.Title, task.Title);
            Assert.Equal(OneTask.Description, task.Description);
        }
        #endregion

        #region PutTask
        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task PutTask_ReturnsBadRequest_WrongId(int id)
        {
            // Arrange

            // Act
            var result = await _tasksController.PutTask(id, OneTaskDTO, _mockValidator.Object);

            // Assert
            var actionResult = Assert.IsAssignableFrom<ActionResult>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task PutTask_ReturnsBadRequest_Validation()
        {
            // Arrange
            ValidationResult validationResult = new();
            validationResult.Errors.Add(new ValidationFailure("SomeProperty", "SomeError"));
            _mockValidator.Setup(validator => validator.ValidateAsync(It.IsAny<Models.TaskDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult).Verifiable();

            // Act
            var result = await _tasksController.PutTask(7, OneTaskDTO, _mockValidator.Object);

            // Assert
            var actionResult = Assert.IsAssignableFrom<ActionResult>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            _mockValidator.Verify();
        }

        [Fact]
        public async Task PutTask_ReturnsNotFound()
        {
            // Arrange
            _mockTaskManager.Setup(manager => manager.UpdateTaskAsync(It.IsAny<int>(), It.IsAny<ITaskDTO>())).ReturnsAsync(false).Verifiable();
            _mockValidator.Setup(validator => validator.ValidateAsync(It.IsAny<Models.TaskDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult()).Verifiable();

            // Act
            var result = await _tasksController.PutTask(99, OneTaskDTO, _mockValidator.Object);

            // Assert
            var actionResult = Assert.IsAssignableFrom<ActionResult>(result);
            var actionNotFound = Assert.IsType<NotFoundObjectResult>(actionResult);

            _mockValidator.Verify();
            _mockTaskManager.Verify();
        }

        [Fact]
        public async Task PutTask_ReturnsOk()
        {
            // Arrange
            _mockTaskManager.Setup(manager => manager.UpdateTaskAsync(It.IsAny<int>(), It.IsAny<ITaskDTO>())).ReturnsAsync(true).Verifiable();
            _mockValidator.Setup(validator => validator.ValidateAsync(It.IsAny<Models.TaskDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult()).Verifiable();

            // Act
            var result = await _tasksController.PutTask(99, OneTaskDTO, _mockValidator.Object);

            // Assert
            var actionResult = Assert.IsAssignableFrom<ActionResult>(result);
            Assert.IsType<NoContentResult>(actionResult);

            _mockValidator.Verify();
            _mockTaskManager.Verify();
        }
        #endregion

        #region DeleteTask
        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task DeleteTask_ReturnsBadRequest_WrongId(int id)
        {
            // Arrange

            // Act
            var result = await _tasksController.DeleteTask(id);

            // Assert
            var actionResult = Assert.IsAssignableFrom<ActionResult>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task DeleteTask_ReturnsNotFound()
        {
            // Arrange
            _mockTaskManager.Setup(manager => manager.DeleteTaskAsync(It.IsAny<int>())).ReturnsAsync(false).Verifiable();

            // Act
            var result = await _tasksController.DeleteTask(99);

            // Assert
            var actionResult = Assert.IsAssignableFrom<ActionResult>(result);
            var actionNotFound = Assert.IsType<NotFoundObjectResult>(actionResult);

            _mockTaskManager.Verify();
        }

        [Fact]
        public async Task DeleteTask_ReturnsOk()
        {
            // Arrange
            _mockTaskManager.Setup(manager => manager.DeleteTaskAsync(It.IsAny<int>())).ReturnsAsync(true).Verifiable();

            // Act
            var result = await _tasksController.DeleteTask(9);

            // Assert
            var actionResult = Assert.IsAssignableFrom<ActionResult>(result);
            Assert.IsType<OkResult>(actionResult);

            _mockTaskManager.Verify();
        }
        #endregion

        #region MemberData
        public static IEnumerable<object[]> GetWrongParams => [ [true, -1], [false, 0], [true, - 7] ];
        public static IEnumerable<object[]> GetWrongTaskId => [ [-5], [-1], [0] ];

        #endregion

        #region DataSource
        private static IEnumerable<ITask> AllTasks => new List<ProjectTaskDTO>
            {
                new() { Id = 1, Title = "Test task 1", Description = "Description task 1", IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, ProjectId = 1 },
                new() { Id = 2, Title = "Test task 2", Description = "Description task 2", IsCompleted = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, ProjectId = 2 },
                new() { Id = 3, Title = "Test task 3", Description = "Description task 3", IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, ProjectId = 3 },
            };

        private static ProjectTaskDTO OneTask =>
            new() { Id = 4, Title = "Test task 4", Description = "Description task 4", IsCompleted = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, ProjectId = 4 };

        private static Models.TaskDTO OneTaskDTO => new() { title = "Test project 5", description = "Description project 5", isCompleted  = false, projectId = 5 };

        private static ITask? NullTask => null;
        #endregion
    }
}
