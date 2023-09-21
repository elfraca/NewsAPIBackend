using ItemEntity = Domain.Data.Item;

namespace Domain.Services.Item
{
    public interface IItemService
    {
        public ItemEntity GetItemDetail(int itemId);
    }
}
