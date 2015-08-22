using System;

namespace FantasySimulator.Interfaces
{
    public interface ITypeContainer : ITypeResolver
    {
        void SetInstance(object instance);
        void SetInstance(TypeWrapper type, object instance);
        void SetInstance<T>(T instance);
    }
}
