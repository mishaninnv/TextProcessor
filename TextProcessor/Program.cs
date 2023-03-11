using TextProcessor.Infrastructure.Enums;
using TextProcessor.Logic;

//var test = new string[] { "Add" };
//var test = new string[] { "Update" };
//var test = new string[] { "Delete" };
var test = new string[] { "Something" };

if (test.Length == 0)
{
    while (true)
    { 
        
    }
}
else if (test.Length > 2)
{
    Console.Error.WriteLine("Введено неверное количество параметров.\n Попробуйте снова.");
}

var controller = new MethodsController();

try
{
    var methodName = Enum.Parse(typeof(MethodEnumerator), test[0]).ToString();
    if (!string.IsNullOrWhiteSpace(methodName))
    {
        controller.CallMethod(methodName);
    }
}
catch (Exception e)
{
    Console.Error.WriteLine($"Error: {e.Message}");
}