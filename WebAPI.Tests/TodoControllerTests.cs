using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using WebAPI.Controllers;
using WebAPI.Data;
using WebAPI.Models;

namespace Tests
{
    public class TodoControllerTests
    {
        private DbContextOptions<TodoContext> _options;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: "TodoList")
                .Options;
        }

        [Test]
        public async Task GetTodoItems_ReturnsListOfTodoItems()
        {
            // Arrange
            using (var context = new TodoContext(_options))
            {
                var controller = new TodoController(context);
                await controller.PostTodoItem(new TodoItem { Title = "Buy milk", Description = "Just desc" });
                await controller.PostTodoItem(new TodoItem { Title = "Clean house", Description = "Just desc" });

                // Act
                var actionResult = await controller.GetTodoItems(null, null);

                // Assert
                Assert.IsInstanceOf<IEnumerable<TodoItem>>(actionResult.Value);
                var todoItems = actionResult.Value as IEnumerable<TodoItem>;
                Assert.AreEqual(2, todoItems.Count());
            }
        }

        [Test]
        public async Task GetTodoItem_ValidId_ReturnsTodoItem()
        {
            // Arrange
            using (var context = new TodoContext(_options))
            {
                var controller = new TodoController(context);
                var newTodoItem = new TodoItem { Title = "Buy milk", Description = "Just desc" };
                var response = await controller.PostTodoItem(newTodoItem);
                var createdResult = response.Result as CreatedAtActionResult;
                var id = (long)createdResult.RouteValues["id"];

                // Act
                var actionResult = await controller.GetTodoItem(id);

                // Assert
                Assert.IsInstanceOf<TodoItem>(actionResult.Value);
                var todoItem = actionResult.Value as TodoItem;
                Assert.AreEqual(id, todoItem.Id);
                Assert.AreEqual(newTodoItem.Title, todoItem.Title);
                Assert.AreEqual(newTodoItem.Description, todoItem.Description);
            }
        }

        [Test]
        public async Task PostTodoItem_ValidRequestBody_ReturnsCreatedResponse()
        {
            // Arrange
            using (var context = new TodoContext(_options))
            {
                var controller = new TodoController(context);
                var newTodoItem = new TodoItem { Title = "Buy milk", Description = "Just desc" };

                // Act
                var result = await controller.PostTodoItem(newTodoItem);

                // Assert
                Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
                var createdResult = result.Result as CreatedAtActionResult;
                Assert.IsInstanceOf<TodoItem>(createdResult.Value);
                var returnValue = createdResult.Value as TodoItem;
                Assert.That(returnValue.Id, Is.Not.EqualTo(0));
                Assert.AreEqual(newTodoItem.Title, returnValue.Title);
                Assert.AreEqual(newTodoItem.Description, returnValue.Description);
            }
        }

        [Test]
        public async Task PutTodoItem_ValidIdAndRequestBody_ReturnsNoContentResponse()
        {
            // Arrange
            using (var context = new TodoContext(_options))
            {
                var controller = new TodoController(context);
                var newTodoItem = new TodoItem { Title = "Buy milk", Description = "Just desc" };
                var response = await controller.PostTodoItem(newTodoItem);
                var createdResult = response.Result as CreatedAtActionResult;
                var id = (long)createdResult.RouteValues["id"];
                var updatedTodoItem = new TodoItem { Id = id, Title = "Buy almond milk", Description = "Just desc" };

                // Act
                var result = await controller.PutTodoItem(id, updatedTodoItem);

                // Assert
                Assert.IsInstanceOf<NoContentResult>(result);
            }
        }

        [Test]
        public async Task DeleteTodoItem_ValidId_ReturnsNoContentResponse()
        {
            // Arrange
            using (var context = new TodoContext(_options))
            {
                var controller = new TodoController(context);
                var newTodoItem = new TodoItem { Title = "Buy milk", Description = "Just desc" };
                var response = await controller.PostTodoItem(newTodoItem);
                var createdResult = response.Result as CreatedAtActionResult;
                var id = (long)createdResult.RouteValues["id"];

                // Act
                var result = await controller.DeleteTodoItem(id);

                // Assert
                Assert.IsInstanceOf<NoContentResult>(result);
            }
        }
    }
}