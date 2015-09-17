using System;
using System.Reflection;

namespace FantasySimulator.Interfaces
{
    /// <summary>
	/// This class exists to wrap an instance of Type when being passed as a parameter in Linq Expressions
	/// because some platforms don't support directly passing Type (I'm looking at you Windows 8.1)
	/// </summary>
	public class TypeWrapper
	{
		/// <summary>
		/// default constructor
		/// </summary>
		/// <param name="type">type to wrap</param>
		public TypeWrapper(Type type)
		{
			Type = type;
		}

		/// <summary>
		/// wrapped type
		/// </summary>
		public Type Type { get; private set; }

		/// <summary>
		/// Implicitly convert to a type
		/// </summary>
		/// <param name="wrapper">type wrapper</param>
		/// <returns>type</returns>
		public static implicit operator Type(TypeWrapper wrapper)
		{
			return wrapper.Type;
		}

		/// <summary>
		/// implicitly converts from type to type wrapper
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static implicit operator TypeWrapper(Type type)
		{
			return new TypeWrapper(type);
		}


        public static bool IsAssignableFrom(TypeWrapper a, TypeWrapper b)
        {
            var typeInfoA = a.Type.GetTypeInfo();
            var typeInfoB = b.Type.GetTypeInfo();
            var res = typeInfoA.IsAssignableFrom(typeInfoB);
            return res;
        }
    }
}
