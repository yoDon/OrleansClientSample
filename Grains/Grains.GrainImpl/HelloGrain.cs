using Grains.GrainDef;

namespace Modules.Sample.GrainImpl;

public class HelloGrain : Grain, IHelloGrain
{
    private string HelloId => this.GetPrimaryKeyString();
    public Task<string> SayHello()
    {
        return Task.FromResult($"Hello from Grain {HelloId}");
    }
}
