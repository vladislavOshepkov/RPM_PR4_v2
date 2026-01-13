using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace Todo.Core
{
    public class TodoList
    {
        private readonly List<TodoItem> _items = new();
        public IReadOnlyList<TodoItem> Items => _items.AsReadOnly();

        public TodoItem Add(string title)
        {
            var item = new TodoItem(title);
            _items.Add(item);
            return item;
        }

        public bool Remove(Guid id) => _items.RemoveAll(i => i.Id == id) > 0;

        public IEnumerable<TodoItem> Find(string substring) =>
            _items.Where(i => i.Title.Contains(substring ?? string.Empty, StringComparison.OrdinalIgnoreCase));

        public int Count => _items.Count;

        // Метод для сохранения в JSON-файл
        public void Save(string path)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(_items, options);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save to file: {ex.Message}", ex);
            }
        }

        // Метод для загрузки из JSON-файла
        public void Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");

            try
            {
                var json = File.ReadAllText(path);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var items = JsonSerializer.Deserialize<List<TodoItem>>(json, options);

                _items.Clear();
                if (items != null)
                {
                    _items.AddRange(items);
                }
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException($"Invalid JSON format: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load from file: {ex.Message}", ex);
            }
        }
    }
}
