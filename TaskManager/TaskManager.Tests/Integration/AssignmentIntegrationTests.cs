using Microsoft.EntityFrameworkCore;
using Moq;
using TaskManager.Application.Services;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Models;
using TaskManager.Infra.Data.Context;
using Xunit;

namespace TaskManager.Tests.Integration
{
    public class AssignmentIntegrationTests
    {
        private TaskManagerContext CreateDatabase()
        {
            var dbName = $"test_database.db";
            var options = new DbContextOptionsBuilder<TaskManagerContext>()
                .UseSqlite($"Data Source={dbName};")
                .Options;

            var context = new TaskManagerContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }

        private void SeedTestData(TaskManagerContext context)
        {
            var assignments = new[]
            {
                new Assignment { Title = "Clean room", Completed = false, UserId = 1 },
                new Assignment { Title = "Do homework", Completed = true, UserId = 1 },
                new Assignment { Title = "Clean kitchen", Completed = false, UserId = 1 },
                new Assignment { Title = "Study math", Completed = false, UserId = 1 },
                new Assignment { Title = "Read book", Completed = false, UserId = 1 },
                new Assignment { Title = "Exercise", Completed = false, UserId = 1 },
                new Assignment { Title = "Cook dinner", Completed = true, UserId = 2 },
                new Assignment { Title = "Clean bathroom", Completed = false, UserId = 2 },
                new Assignment { Title = "Buy groceries", Completed = false, UserId = 3 },
                new Assignment { Title = "Walk dog", Completed = true, UserId = 3 }
            };

            context.Assignments.AddRange(assignments);
            context.SaveChanges();
        }

        #region 1. Testes de Filtros e Paginação

        [Fact]
        public void GetAllAssignments_Should_Filter_By_Title()
        {
            using var context = CreateDatabase();
            SeedTestData(context);

            var mockRepo = new Mock<IAssignmentRepository>();
            mockRepo.Setup(r => r.GetAllAssignments())
                   .Returns(context.Assignments.AsQueryable());

            var service = new AssignmentAppService(mockRepo.Object);

            var result = service.GetAllAssignments(title: "Clean");

            Assert.NotEmpty(result);
            Assert.All(result, a => Assert.Contains("Clean", a.Title));
            Assert.True(result.Count >= 2);
        }

        [Fact]
        public void GetAllAssignments_Should_Sort_By_Title_Ascending()
        {
            using var context = CreateDatabase();
            SeedTestData(context);

            var mockRepo = new Mock<IAssignmentRepository>();
            mockRepo.Setup(r => r.GetAllAssignments())
                   .Returns(context.Assignments.AsQueryable());

            var service = new AssignmentAppService(mockRepo.Object);

            var result = service.GetAllAssignments(sort: "title", order: "asc");

            Assert.NotEmpty(result);
            Assert.True(result.Count > 1);

            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.True(string.Compare(result[i].Title, result[i + 1].Title, StringComparison.OrdinalIgnoreCase) <= 0);
            }
        }

        [Fact]
        public void GetAllAssignments_Should_Paginate_Correctly()
        {
            using var context = CreateDatabase();
            SeedTestData(context);

            var mockRepo = new Mock<IAssignmentRepository>();
            mockRepo.Setup(r => r.GetAllAssignments())
                   .Returns(context.Assignments.AsQueryable());

            var service = new AssignmentAppService(mockRepo.Object);

            var page1 = service.GetAllAssignments(page: 1, pageSize: 3);
            var page2 = service.GetAllAssignments(page: 2, pageSize: 3);

            Assert.Equal(3, page1.Count);
            Assert.True(page2.Count > 0);

            var ids1 = page1.Select(a => a.Id).ToHashSet();
            var ids2 = page2.Select(a => a.Id).ToHashSet();
            Assert.False(ids1.Intersect(ids2).Any());
        }

        [Fact]
        public void GetAllAssignments_Should_Combine_Filter_Sort_And_Pagination()
        {
            using var context = CreateDatabase();
            SeedTestData(context);

            var mockRepo = new Mock<IAssignmentRepository>();
            mockRepo.Setup(r => r.GetAllAssignments())
                   .Returns(context.Assignments.AsQueryable());

            var service = new AssignmentAppService(mockRepo.Object);

            var result = service.GetAllAssignments(
                page: 1,
                pageSize: 2,
                title: "Clean",
                sort: "title",
                order: "asc");

            Assert.NotEmpty(result);
            Assert.True(result.Count <= 2);
            Assert.All(result, a => Assert.Contains("Clean", a.Title));

            if (result.Count > 1)
            {
                for (int i = 0; i < result.Count - 1; i++)
                {
                    Assert.True(string.Compare(result[i].Title, result[i + 1].Title, StringComparison.OrdinalIgnoreCase) <= 0);
                }
            }
        }

        #endregion

        #region 2. Testes de Regra de Negócio ou seja, o limite de 5 Tarefas

        [Fact]
        public async Task UpdateAssignmentStatus_Should_Block_6th_Incomplete_Task()
        {
            using var context = CreateDatabase();
            SeedTestData(context);

            var mockRepo = new Mock<IAssignmentRepository>();

            var assignment = context.Assignments.First(a => a.AssignmentId == 2);
            mockRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(assignment);
            mockRepo.Setup(r => r.CountIncompleteAssignmentsByUserIdAsync(1)).ReturnsAsync(5);

            var service = new AssignmentAppService(mockRepo.Object);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.UpdateAssignmentStatusAsync(2, false));

            Assert.Contains("5 tarefas incompletas", exception.Message);
        }

        [Fact]
        public async Task UpdateAssignmentStatus_Should_Allow_When_Under_5_Incomplete_Tasks()
        {
            using var context = CreateDatabase();
            SeedTestData(context);

            var mockRepo = new Mock<IAssignmentRepository>();

            var assignment = context.Assignments.First(a => a.AssignmentId == 7);
            mockRepo.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(assignment);
            mockRepo.Setup(r => r.CountIncompleteAssignmentsByUserIdAsync(2)).ReturnsAsync(1);
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Assignment>())).Returns(Task.CompletedTask);

            var service = new AssignmentAppService(mockRepo.Object);

            var result = await service.UpdateAssignmentStatusAsync(7, false);

            Assert.True(result);
            Assert.False(assignment.Completed);
            mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Assignment>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAssignmentStatus_Should_Always_Allow_Marking_As_Complete()
        {
            using var context = CreateDatabase();
            SeedTestData(context);

            var mockRepo = new Mock<IAssignmentRepository>();

            var assignment = context.Assignments.First(a => a.AssignmentId == 1);
            mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(assignment);
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Assignment>())).Returns(Task.CompletedTask);

            var service = new AssignmentAppService(mockRepo.Object);

            var result = await service.UpdateAssignmentStatusAsync(1, true);

            Assert.True(result);
            Assert.True(assignment.Completed);
            mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Assignment>()), Times.Once);
        }

        #endregion

        #region 3. Testes do GET

        [Fact]
        public void GetAssignment_Should_Return_Assignment_When_Exists()
        {
            using var context = CreateDatabase();
            SeedTestData(context);

            var mockRepo = new Mock<IAssignmentRepository>();
            var expectedAssignment = context.Assignments.First(a => a.AssignmentId == 1);
            mockRepo.Setup(r => r.GetById(1)).Returns(expectedAssignment);

            var service = new AssignmentAppService(mockRepo.Object);

            var result = service.GetAssignment(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Clean room", result.Title);
            Assert.False(result.Completed);
        }

        [Fact]
        public void GetAssignment_Should_Return_Null_When_Not_Exists()
        {
            var mockRepo = new Mock<IAssignmentRepository>();
            mockRepo.Setup(r => r.GetById(999)).Returns((Assignment?)null);

            var service = new AssignmentAppService(mockRepo.Object);

            var result = service.GetAssignment(999);

            Assert.Null(result);
        }

        #endregion

        #region 4. Testes do PUT

        [Fact]
        public async Task UpdateAssignmentStatus_Should_Update_Successfully()
        {
            using var context = CreateDatabase();
            SeedTestData(context);

            var mockRepo = new Mock<IAssignmentRepository>();
            var assignment = context.Assignments.First(a => a.AssignmentId == 1);

            mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(assignment);
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Assignment>())).Returns(Task.CompletedTask);

            var service = new AssignmentAppService(mockRepo.Object);

            var result = await service.UpdateAssignmentStatusAsync(1, true);

            Assert.True(result);
            Assert.True(assignment.Completed);
            mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Assignment>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAssignmentStatus_Should_Throw_When_Assignment_Not_Exists()
        {
            var mockRepo = new Mock<IAssignmentRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Assignment?)null);

            var service = new AssignmentAppService(mockRepo.Object);

            var exception = await Assert.ThrowsAsync<Exception>(
                () => service.UpdateAssignmentStatusAsync(999, true));

            Assert.Contains("não encontrada", exception.Message);
        }

        #endregion

        #region 5. Teste de integração com banco

        [Fact]
        public void Integration_Database_Operations_Should_Work()
        {
            using var context = CreateDatabase();

            var assignment = new Assignment
            {
                Title = "Test Integration",
                Completed = false,
                UserId = 1
            };

            var newAssignment = context.Assignments.Add(assignment);
            context.SaveChanges();

            var found = context.Assignments.FirstOrDefault(a => a.AssignmentId == newAssignment.Entity.AssignmentId);

            if (found != null)
            {
                found.Completed = true;
                context.SaveChanges();
            }

            var updated = context.Assignments.FirstOrDefault(a => a.AssignmentId == newAssignment.Entity.AssignmentId);

            Assert.NotNull(found);
            Assert.NotNull(updated);
            Assert.True(updated.Completed);
            Assert.Equal("Test Integration", updated.Title);
        }

        #endregion
    }
}