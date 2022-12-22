namespace Main.Data.Test
{
    public class TestService: ITestService
    {
        public string Id { get; private set; }
        public TestService(IServiceProvider provider)
        {
            Id = Guid.NewGuid().ToString();
        }
    }

    public interface ITestService
    {
        string Id { get; }
    }
}
