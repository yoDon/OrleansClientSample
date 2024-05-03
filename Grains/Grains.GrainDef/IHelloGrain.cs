namespace Grains.GrainDef;

public interface IHelloGrain : IGrainWithStringKey
{
    public static IHelloGrain GetGrain(IGrainFactory grainFactory, string helloId)
    {
        return grainFactory.GetGrain<IHelloGrain>(helloId);
    }

    Task<string> SayHello();
}
