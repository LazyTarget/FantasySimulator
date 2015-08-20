using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FantasySimulator.Interfaces;

namespace FantasySimulator.Core
{
    public class TypeContainer : ITypeContainer
    {
        private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>(); 


        public void SetInstance(object instance)
        {
            var type = instance.GetType();
            SetInstance(type, instance);
        }

        public void SetInstance(Type type, object instance)
        {
            _instances[type] = instance;
        }

        public void SetInstance<T>(T instance)
        {
            var type = typeof (T);
            SetInstance(type, instance);
        }


        public T GetInstance<T>()
        {
            var type = typeof(T);
            object obj = null;
            if (_instances.ContainsKey(type))
            {
                obj = _instances[type];
            }
            else
            {
                foreach (var t in _instances.Keys)
                {
                    if (type.IsAssignableFrom(t))
                    {
                        obj = _instances[t];
                        break;
                    }
                }
            }
            var res = (T) obj;
            return res;
        }

        public object GetInstance(Type type)
        {
            object res = null;
            if (_instances.ContainsKey(type))
            {
                res = _instances[type];
            }
            else
            {
                foreach (var t in _instances.Keys)
                {
                    if (type.IsAssignableFrom(t))
                    {
                        res = _instances[t];
                        break;
                    }
                }
            }
            return res;
        }

    }
}
