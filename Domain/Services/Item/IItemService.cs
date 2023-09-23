﻿using ItemEntity = Domain.Data.Item;

namespace Domain.Services.Item
{
    public interface IItemService
    {
        public Task<ItemEntity> GetItemDetailAsync(int itemId);
        public Task<List<ItemEntity>> GetNewestStoriesAsync();
    }
}
