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

        public void SetInstance(TypeWrapper type, object instance)
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
            var obj = GetInstance(type);
            var res = (T) obj;
            return res;
        }

        public object GetInstance(TypeWrapper type)
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
                    if (t == null)
                        continue;
                    if (TypeWrapper.IsAssignableFrom(type, t))
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
