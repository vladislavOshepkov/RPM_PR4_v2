namespace Todo.Core.Tests
{
    public class TodoListTests
    {
        [Fact]
        public void Add_IncreasesCount()
        {
            var list = new TodoList();
            list.Add(" task ");
            Assert.Equal(1, list.Count);
            Assert.Equal("task", list.Items.First().Title);
        }
        [Fact]
        public void Remove_ById_Works()
        {
            var list = new TodoList();
            var item = list.Add("a");
            Assert.True(list.Remove(item.Id));
            Assert.Equal(0, list.Count);
        }
        [Fact]
        public void Find_ReturnsMatches()
        {
            var list = new TodoList();
            list.Add("Buy milk");
            list.Add("Read book");
            var found = list.Find("buy").ToList();
            Assert.Single(found);
            Assert.Equal("Buy milk", found[0].Title);
        }
        [Fact]
        public void Save_CreatesJsonFile()
        {
            // Arrange
            var list = new TodoList();
            var item1 = list.Add("Task 1");
            item1.MarkDone();
            list.Add("Task 2");

            string testFile = "test_save.json";

            try
            {
                // Act
                list.Save(testFile);

                // Assert
                Assert.True(File.Exists(testFile));
                var jsonContent = File.ReadAllText(testFile);
                Assert.Contains("task 1", jsonContent, StringComparison.OrdinalIgnoreCase);
                Assert.Contains("task 2", jsonContent, StringComparison.OrdinalIgnoreCase);
            }
            finally
            {
                // Cleanup
                if (File.Exists(testFile))
                    File.Delete(testFile);
            }
        }

        [Fact]
        public void Load_RestoresItemsCorrectly()
        {
            // Arrange
            var originalList = new TodoList();
            var item1 = originalList.Add("Completed task");
            item1.MarkDone();
            var item2 = originalList.Add("Pending task");

            string testFile = "test_load.json";

            try
            {
                // Save original list
                originalList.Save(testFile);

                // Act - Create new list and load from file
                var loadedList = new TodoList();
                loadedList.Load(testFile);

                // Assert
                Assert.Equal(2, loadedList.Count);

                // Check first item
                var loadedItem1 = loadedList.Items.First(i => i.Title == "Completed task");
                Assert.True(loadedItem1.IsDone);

                // Check second item
                var loadedItem2 = loadedList.Items.First(i => i.Title == "Pending task");
                Assert.False(loadedItem2.IsDone);
            }
            finally
            {
                // Cleanup
                if (File.Exists(testFile))
                    File.Delete(testFile);
            }
        }

        [Fact]
        public void SaveAndLoad_PreservesIds()
        {
            // Arrange
            var list = new TodoList();
            var item1 = list.Add("Task 1");
            var item2 = list.Add("Task 2");

            string testFile = "test_ids.json";

            try
            {
                // Act - Save and load
                list.Save(testFile);
                var newList = new TodoList();
                newList.Load(testFile);

                // Assert
                Assert.Equal(2, newList.Count);
                Assert.Contains(newList.Items, i => i.Id == item1.Id);
                Assert.Contains(newList.Items, i => i.Id == item2.Id);
            }
            finally
            {
                // Cleanup
                if (File.Exists(testFile))
                    File.Delete(testFile);
            }
        }

        [Fact]
        public void Load_ThrowsFileNotFoundException_WhenFileDoesNotExist()
        {
            // Arrange
            var list = new TodoList();
            string nonExistentFile = "non_existent.json";

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => list.Load(nonExistentFile));
        }
    }
}
