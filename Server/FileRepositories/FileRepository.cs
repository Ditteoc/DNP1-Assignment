using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories
{
    public class FileRepository<T> : IRepository<T> where T : class
    {
        protected List<T> entities = new List<T>();

        private readonly string _filePath;
        private readonly Func<T, int> _getId;
        private readonly Action<T, int> _setId;

        public FileRepository(string filePath, Func<T, int> getId, Action<T, int> setId)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath)); 
            _getId = getId ?? throw new ArgumentNullException(nameof(getId));
            _setId = setId ?? throw new ArgumentNullException(nameof(setId));

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
            
            LoadEntitiesAsync().GetAwaiter().GetResult();
        }

        private async Task LoadEntitiesAsync()
        {
            try
            {
                string json = await File.ReadAllTextAsync(_filePath);
                entities = JsonSerializer.Deserialize<List<T>>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading data from: {_filePath}: {e.Message}");
                entities = new List<T>(); // why a new list?
            }
        }

        private async Task SaveEntitiesAsync()
        {
            try
            {
                string json = JsonSerializer.Serialize(entities);
                await File.WriteAllTextAsync(_filePath, json);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving data to: {_filePath}: {e.Message}");
            }
        }
        
        public async Task<T> AddAsync(T entity)
        {
            int newId = entities.Any() ? entities.Max(e => _getId(e)) + 1 : 1;
            _setId(entity, newId);
            entities.Add(entity);
            await SaveEntitiesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            int entityId = _getId( entity);
            var existingEntity = entities.SingleOrDefault(e => _getId(e) == entityId);

            if (existingEntity == null)
            {
                throw new InvalidOperationException($"Entity with ID '{entityId}' not found");
            }

            entities.Remove(existingEntity);
            entities.Add(entity);
            await SaveEntitiesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entityToRemove = entities.SingleOrDefault(e => _getId(e) == id);

            if (entityToRemove == null)
            {
                throw new InvalidOperationException($"Entity with ID '{id}' not found");
            }

            entities.Remove(entityToRemove);
            await SaveEntitiesAsync();
        }

        public Task<T> GetSingleAsync(int id)
        {
            var entity = entities.SingleOrDefault(e => _getId(e) == id);

            if (entity == null)
            {
                return Task.FromException<T>(new InvalidOperationException($"Entity with ID '{id}' not found."));
            }

            return Task.FromResult(entity);
        }

        public async Task<IEnumerable<T>> GetManyAsync()
        {
            await LoadEntitiesAsync();
            return entities.AsEnumerable();
        }
        
    }
}