﻿namespace craftinginterpreters.tool;
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
        DefineAst(outputDir, "Lox", "Expr", new List<string>
        {
            "Assign   : Token Name, Expr Value",
            "Binary   : Expr Left, Token Operator, Expr Right",
            "Call     : Expr Callee, Token Paren, List<Expr> Arguments",
            "Function : List<Token> Parameters, List<Stmt> Body",
            "Grouping : Expr Expression",
            "Literal  : Object Value",
            "Logical  : Expr Left, Token Operator, Expr Right",
            "Unary    : Token Operator, Expr Right",
            "Variable : Token Name"
        });

        DefineAst(outputDir, "Statement", "Stmt", new List<string>() {
            "Block     : List<Stmt> statements",
            "Break     : ",
            "Expression: Expr expression",
            "Function  : Token name, Lox.Function function",
            "If        : Expr condition, Stmt thenBranch,"+
                       " Stmt elseBranch",
            "Print     : Expr expression",
            "Return    : Token name, Expr value",
            "Var       : Token name, Expr initializer",
            "While     : Expr condition, Stmt body",
        });
    }

    private static void DefineAst(string outputDir, string @namespace, string baseName, List<string> types)
    {
        string path = Path.Combine(outputDir, baseName + ".cs");
        File.WriteAllText(path, @$"namespace {@namespace};
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

        string[] fieldsArray = fields.Length == 0 ? new string[0] : fields.Split(", ");
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
