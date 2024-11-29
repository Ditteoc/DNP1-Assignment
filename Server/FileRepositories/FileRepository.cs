using System.Linq.Expressions;
using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories
{
    public class FileRepository<T> : IRepository<T> where T : class
    {
        protected List<T> _entities = new List<T>();

        private readonly string _filePath;
        private readonly Func<T, int> _getId;
        private IRepository<T> _repositoryImplementation;


        public FileRepository(string filePath, Func<T, int> getId)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath)); 
            _getId = getId ?? throw new ArgumentNullException(nameof(getId));

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
                Console.WriteLine($"Loading data from: {_filePath}");
                string json = await File.ReadAllTextAsync(_filePath);
                _entities = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading data from {_filePath}: {e.Message}");
                _entities = new List<T>();
            }
        }

        private async Task SaveEntitiesAsync()
        {
            try
            {
                string json = JsonSerializer.Serialize(_entities);
                await File.WriteAllTextAsync(_filePath, json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving data to {_filePath}: {e.Message}");
            }
        }
        
        public async Task<T> AddAsync(T entity)
        {
            int newId = _entities.Any() ? _entities.Max(e => _getId(e)) + 1 : 1;

            // Reflektion bruges til at sætte `Id` automatisk
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null || !idProperty.CanWrite)
            {
                throw new InvalidOperationException("Entity must have a writable Id property.");
            }

            idProperty.SetValue(entity, newId);

            _entities.Add(entity);
            await SaveEntitiesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            int entityId = _getId(entity);
            var existingEntity = _entities.SingleOrDefault(e => _getId(e) == entityId);

            if (existingEntity == null)
            {
                throw new InvalidOperationException($"Entity with ID '{entityId}' not found");
            }

            _entities.Remove(existingEntity);
            _entities.Add(entity);
            await SaveEntitiesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entityToRemove = _entities.SingleOrDefault(e => _getId(e) == id);

            if (entityToRemove == null)
            {
                throw new InvalidOperationException($"Entity with ID '{id}' not found");
            }

            _entities.Remove(entityToRemove);
            await SaveEntitiesAsync();
        }

        public Task<T> GetSingleAsync(int id)
        {
            var entity = _entities.SingleOrDefault(e => _getId(e) == id);
            return Task.FromResult(entity); // Returner null hvis ikke fundet, uden exception
        }

        public Task<IEnumerable<T>> GetManyAsync()
        {
            return Task.FromResult(_entities.AsEnumerable());
        }

        public Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            return _repositoryImplementation.GetSingleAsync(predicate);
        }
    }
}
