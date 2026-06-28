using Microsoft.Extensions.Caching.Memory;
using WebApplication3.Models;
using WebApplication3.repo.@interface;
using WebApplication3.Services.@interface;

namespace WebApplication3.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IMenuItemRepository _repository;
        private readonly IMemoryCache _cache;
        private readonly string _cacheKey = "AllMenuItems";
        private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(24);

        public MenuItemService(IMenuItemRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public IEnumerable<MenuItem> GetAllMenuItems()
        {
            if (!_cache.TryGetValue(_cacheKey, out List<MenuItem>? cachedItems))
            {
                cachedItems = _repository.GetAll().ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(_cacheDuration);

                _cache.Set(_cacheKey, cachedItems, cacheOptions);
            }

            return cachedItems ?? new List<MenuItem>();
        }
        public MenuItem? GetMenuItemById(int id)
        {
            var allItems = GetAllMenuItems();
            return allItems.FirstOrDefault(item => item.MenuItemID == id);
        }

        public void AddMenuItem(MenuItem item)
        {
            _repository.Add(item);
            InvalidateCache();
        }
        public void UpdateMenuItem(MenuItem item)
        {
            _repository.Update(item);
            InvalidateCache();
        }

        public void DeleteMenuItem(int id)
        {
            _repository.Delete(id);
            InvalidateCache();
        }

        public void RefreshCache()
        {
            InvalidateCache();
        }
        private void InvalidateCache()
        {
            _cache.Remove(_cacheKey);
        }
    }
}
