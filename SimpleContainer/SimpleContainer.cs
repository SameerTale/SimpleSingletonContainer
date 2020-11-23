using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleContainer
{
    public class SingletonContainer
    {
        public static SingletonContainer GetContainer()
        {
            if(_container == null)
            {
                _container = new SingletonContainer();
            }
            return _container;
        }

        private static SingletonContainer _container { get; set; }
        private SingletonContainer()
        {
            _dict = new Dictionary<Type, object>();
        }

        public void RegisterInstance<T>(T inst)
        {
            if(_dict.ContainsKey(inst.GetType()))
            {
                throw new InvalidOperationException($"cannot add more than one instance of a type {inst.GetType().Name}");
            }
            var t = typeof(T);
            _dict.Add(t, inst);
        }

        public void RegisterInstance<T>()
        {
            if (_dict.ContainsKey(typeof(T)))
            {
                throw new InvalidOperationException($"cannot add more than one instance of a type {typeof(T).Name}");
            }
            var t = typeof(T);
            _dict.Add(t, null);
        }

        public T GetInstance<T>()
        {
            var typ = typeof(T);
            return (T)GetInstance(typ);
        }

        public object GetInstance(Type typ)
        {
            object obj;
            _dict.TryGetValue(typ, out obj);
            if (obj != null)
            {
                return obj;
            }
            if (_dict.ContainsKey(typ))
            {
                if (typ.IsAbstract || typ.IsInterface)
                {
                    throw new InvalidOperationException($"Cannot infer the concrete type from {typ.Name}");
                }
                var constrs = typ.GetConstructors();
                if (constrs == null || constrs.Length != 1)
                {
                    throw new InvalidOperationException($"Cannot decide the constructor from {typ.Name}");
                }
                var constructor = constrs[0];
                var parameters = constructor.GetParameters();
                var parametersList = parameters.Select(s => GetInstance(s.ParameterType)).ToArray();
                return constructor.Invoke(parametersList);
            }
            return null;
        }

        Dictionary<Type, object> _dict;
    }
}
