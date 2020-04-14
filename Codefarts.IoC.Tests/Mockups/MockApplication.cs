namespace Codefarts.IoC.Tests
{
    public class MockApplication
    {
        private IRepository repo;

        public MockApplication(IRepository repo)
        {
            this.repo = repo;
        }

        public IRepository Repository
        {
            get
            {
                return this.repo;
            }
        }
    }
}