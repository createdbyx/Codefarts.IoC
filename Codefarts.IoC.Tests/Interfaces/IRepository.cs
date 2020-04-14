namespace Codefarts.IoC.Tests
{
    public interface IRepository
    {
        void StoreData(string name, object value);
        object FetchData(string name);
    }
}