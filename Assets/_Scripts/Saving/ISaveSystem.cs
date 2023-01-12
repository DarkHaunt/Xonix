


namespace Xonix.Saving.Core
{
    public interface ISaveSystem
    {
        void Save(object saveObject, string fileName);
        T Load<T>(string itemName);
    } 
}
