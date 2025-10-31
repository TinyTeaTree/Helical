using System;
using System.Collections.Generic;
using System.Linq;

public class DataWarehouse : WagSingleton<DataWarehouse>
{
    private List<DataBox> _boxes = new();
    private Dictionary<string, DataBox> _warehouse = new();
    private Dictionary<Type, DataBox> _typeWarehouse = new();

    public void Init()
    {
        var dataBoxType = typeof(DataBox);

        var dataBoxAssymbly = dataBoxType.Assembly;
        var dataBoxAssemblyName = dataBoxAssymbly.GetName();

        var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach(var assembly in allAssemblies)
        {
            var assemblyName = assembly.GetName();
            var referencedAssemblies = assembly.GetReferencedAssemblies();
            if (referencedAssemblies.All(a => a.Name != dataBoxAssemblyName.Name ))
                continue;

            var assemblyTypes = assembly.GetTypes();

            var dataBoxTypes = assemblyTypes
                .Where(t => !t.IsAbstract && !t.IsGenericType)
                .Where(t => dataBoxType.IsAssignableFrom(t));

            foreach(var boxType in dataBoxTypes)
            {
                var boxInstance = Activator.CreateInstance(boxType) as DataBox;
                if (boxInstance.Id.IsNullOrEmpty())
                {
                    UnityEngine.Debug.LogWarning($"Dont Support null id for {boxType}");
                    continue;
                }

                if(boxInstance.Id == DataBox.Default_Id)
                {
                    UnityEngine.Debug.LogWarning($"Forgot to override a unique Box Id for {boxType}");
                    continue;
                }

                _boxes.Add(boxInstance);
            }
        }

        foreach(var box in _boxes)
        {
            _warehouse.Add(box.Id, box);
            _typeWarehouse.Add(box.GetType(), box);
        }

        foreach (var box in _boxes)
        {
            box.Initialize();
        }
    }

    public DataBox GetBox(string id)
    {
        if(!_warehouse.TryGetValue(id, out var box))
        {
            UnityEngine.Debug.LogWarning($"Did not find box for {id}");
            return null;
        }

        return _warehouse[id];
    }

    public T GetBox<T>()
        where T : DataBox
    {
        var boxType = typeof(T);
        if (!_typeWarehouse.TryGetValue(boxType, out var box))
        {
            UnityEngine.Debug.LogWarning($"Did not find box for {boxType}");
            return null;
        }

        return _typeWarehouse[boxType] as T;
    }
}