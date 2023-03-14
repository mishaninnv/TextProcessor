using TextProcessor.DataBase;
using TextProcessor.Logic;

var manager = new MethodsManager();
var initDb = new ManagerDb();
initDb.InitDb();

try
{
    switch (args.Length)
    {
        case 0:
            manager.GetData();
            break;
        case 1:
            manager.CallMethod(args[0]);
            break;
        case 2:
            manager.CallMethod(args[0], args[1]);
            break;
        default:
            Console.WriteLine("Вы ввели более 2х параметров. Попробуйте снова.");
            break;
    }
}
catch (Exception e)
{ 
    Console.Error.WriteLine(e.Message);
}