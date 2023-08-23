namespace StateDaprDemo.Services;

public interface IItemsStateService
{
    Task<string> CreateItems(int count);
    Task<string> GetItem(Guid id);
    Task<string> DeleteItem(Guid id);
}