namespace Codefarts.IoC.Tests
{
    using System.Collections.Generic;

    public class MockRepository : IRepository
    {
        private Dictionary<string, object> data = new Dictionary<string, object>();

        public void StoreData(string name, object value)
        {
            this.data[name] = value;
        }

        public object FetchData(string name)
        {
            return this.data[name];
        }
    }
}