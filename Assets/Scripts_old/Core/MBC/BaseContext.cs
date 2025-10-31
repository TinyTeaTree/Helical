using UnityEngine;

public abstract class BaseContext : MonoBehaviour
{
    protected ContextGroup<IController> _controllerGroup = new();

    private void Awake()
    {
        CreateControllers();

        AwakeControllers(_controllerGroup);
    }

    private void Start()
    {
        StartControllers();
        PostStart();
    }

    private void StartControllers()
    {
        foreach (var controller in _controllerGroup.Group)
        {
            controller.Start();
        }
    }

    private void AwakeControllers(ContextGroup<IController> group)
    {
        foreach (var controller in _controllerGroup.Group)
        {
            controller.Awake(group);
        }
    }

    protected abstract void CreateControllers();

    protected virtual void PostStart()
    {

    }
}