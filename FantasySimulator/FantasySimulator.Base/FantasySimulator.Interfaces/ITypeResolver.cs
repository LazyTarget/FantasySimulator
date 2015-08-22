namespace FantasySimulator.Interfaces
{
    public interface ITypeResolver
    {
        T GetInstance<T>();
        object GetInstance(TypeWrapper type);
    }
}
