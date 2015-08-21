using System;

namespace FantasySimulator.Interfaces
{
    public interface ITypeContainer : ITypeResolver
    {
        void SetInstance(object instance);
        void SetInstance(Type type, object instance);
        void SetInstance<T>(T instance);
    }
}
