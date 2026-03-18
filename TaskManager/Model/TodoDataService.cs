using System.IO;
using System.Text.Json;

namespace TaskManager.Model
{
    public class TodoDataService
    {
        private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks.json");

        //Настройки сериализации
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Асинхронная загрузка списка задач из файла
        /// </summary>
        public async Task<List<TodoTask>> LoadTasksAsync()
        {
            if (!File.Exists(_filePath))
                return [];

            try
            {
                using FileStream openStream = File.OpenRead(_filePath);
                var result = await JsonSerializer.DeserializeAsync<List<TodoTask>>(openStream, _jsonOptions);
                return result ?? [];
            }
            catch (Exception ex)
            {
                throw new Exception($"Не удалось прочитать файл задач: {ex.Message}");
            }
        }

        /// <summary>
        /// Асинхронное сохранение коллекции задач
        /// </summary>
        public async Task SaveTasksAsync(IEnumerable<TodoTask> tasks)
        {
            var tasksList = tasks.ToList();
            using FileStream createStream = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(createStream, tasksList, _jsonOptions);
        }

    }
}
