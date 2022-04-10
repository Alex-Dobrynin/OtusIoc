namespace OtusIoc.Scopes
{
    public interface IScope : IDisposable
    {
        string Name { get; }
    }
}
