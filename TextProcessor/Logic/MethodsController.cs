using TextProcessor.Infrastructure.Enums;

namespace TextProcessor.Logic;

internal class MethodsController
{
    private Dictionary<string, Action> _methods;

    public MethodsController()
    {
        _methods = new Dictionary<string, Action>
        {
            { nameof(MethodEnumerator.Add), Add },
            { nameof(MethodEnumerator.Update), Update },
            { nameof(MethodEnumerator.Delete), Delete }
        };
    }

    internal void CallMethod(string methodName)
    {
        var callMethod = _methods[methodName];
        callMethod?.Invoke();
    }

    private void Add()
    {
        Console.WriteLine("Add method");
    }
    private void Update()
    {
        Console.WriteLine("Update method");
    }
    private void Delete()
    {
        Console.WriteLine("Delete method");
    }
}
