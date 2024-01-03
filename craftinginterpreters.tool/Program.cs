namespace craftinginterpreters.tool;
class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: generate_ast <output directory>");
            Environment.Exit(64);
        }
        string outputDir = args[0];
        //DefineAst(outputDir, "Expr", new List<string>
        //{
        //    "Binary   : Expr Left, Token Operator, Expr Right",
        //    "Grouping : Expr Expression",
        //    "Literal  : Object Value",
        //    "Unary    : Token Operator, Expr Right"
        //});
        DefineAst(outputDir, "Stmt", new List<string>() {
            "Expresion : Expr expression",
            "Print     : Expr expression" 
        });
    }

    private static void DefineAst(string outputDir, string baseName, List<string> types)
    {
        string path = Path.Combine(outputDir, baseName + ".cs");
        File.WriteAllText(path, @"namespace Lox;
                                  public abstract class " + baseName + "\n     {" + "\n        ");

        // The base accept method
        File.AppendAllText(path, "\n");
        File.AppendAllText(path, $" public abstract R Accept<R>(Visitor<R> visitor);\n");

        File.AppendAllText(path, "}");
        
        DefineVisitor(path, baseName, types);

        foreach (var type in types)
        {
            var splitedType = type.Split(':');
            string className = splitedType[0].Trim();
            string fields = splitedType[1].Trim();
            DefineType(path, baseName, className, fields);
        }

    }

    private static void DefineVisitor(string path, string baseName, List<string> types)
    {
        File.AppendAllText(path, "   public interface Visitor<R> {\n");
        foreach (var type in types)
        {
            var typeName = type.Split(":")[0].Trim();
            File.AppendAllText(path, $" R Visit{typeName}{baseName}({typeName} {baseName.ToLower()});\n");
        }

        File.AppendAllText(path, "}\n");
    }

    private static void DefineType(string path, string baseName, string className, string fields)
    {
        File.AppendAllText(path, $"public class {className} : {baseName}" +
            "\n    {"
            + $"   public  {className} ({fields}) " + "\n     {\n");

        string[] fieldsArray = fields.Split(", ");
        foreach (var field in fieldsArray)
        {
            string name = field.Split(" ")[1];
            File.AppendAllText(path, $"     this.{name} = {name};\n");
        }

        File.AppendAllText(path, "\n        }\n");

        File.AppendAllText(path, "public override R Accept<R>(Visitor<R> visitor) {return " + $"visitor.Visit{className}{baseName}(this);" + "}");

        foreach (var field in fieldsArray)
        {
            File.AppendAllText(path, $"    internal {field} " + "{ get; }");
        }

        File.AppendAllText(path, "\n        }\n");
    }
}
