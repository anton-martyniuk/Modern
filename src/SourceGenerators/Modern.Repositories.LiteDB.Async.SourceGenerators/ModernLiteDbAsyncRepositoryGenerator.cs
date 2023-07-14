using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Modern.Repositories.LiteDB.Async.SourceGenerators;

[Generator]
public class ModernLiteDbAsyncRepositoryGenerator : ISourceGenerator
{
    private const string RepositoryAttributeName = nameof(ModernLiteDbAsyncRepositoryAttribute);
    private const string ConnectionStringMemberName = nameof(ModernLiteDbAsyncRepositoryAttribute.ConnectionString);
    private const string CollectionMemberName = nameof(ModernLiteDbAsyncRepositoryAttribute.CollectionName);

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new RepositorySyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not RepositorySyntaxReceiver receiver)
        {
            return;
        }

        foreach (var classDeclaration in receiver.RepositoryClasses)
        {
            var model = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            if (model.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
            {
                continue;
            }

            var attributeData = classSymbol.GetAttributes().SingleOrDefault(a => a.AttributeClass?.Name == RepositoryAttributeName);
            if (attributeData is null)
            {
                continue;
            }
            
            var connectionString = attributeData.NamedArguments.SingleOrDefault(a => a.Key == ConnectionStringMemberName).Value.Value?.ToString()
                ?? attributeData.ConstructorArguments[0].Value?.ToString();
            if (connectionString is null)
            {
                continue;
            }
            
            var collectionName = attributeData.NamedArguments.SingleOrDefault(a => a.Key == CollectionMemberName).Value.Value?.ToString()
                ?? attributeData.ConstructorArguments[1].Value?.ToString();
            if (collectionName is null)
            {
                continue;
            }

            var idProperty = classSymbol.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(m => m.Name == "Id");
            if (idProperty == null)
            {
                continue;
            }

            var entityType = classSymbol.ToDisplayString();
            var idType = idProperty.Type.ToDisplayString();

            // Get rid of Dbo and Dto suffixes
            var className = classSymbol.Name.Replace("Dbo", "").Replace("Dto", "");
            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
            
            // Get repository name
            var repositoryName = attributeData.NamedArguments.FirstOrDefault(a => a.Key == "RepositoryName").Value.Value?.ToString() 
                ?? $"{className}Repository";

            var source = GenerateRepositoryCode(namespaceName, entityType, idType, connectionString, collectionName, repositoryName);
            context.AddSource($"{repositoryName}_litedb_gen.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private static string GenerateRepositoryCode(string namespaceName, string entityType, string idType, string connectionString,
        string collectionName, string repositoryName)
    {
        var sb = new StringBuilder();

        // Usings
        sb.AppendLine("using Modern.Repositories.Abstractions;");
        sb.AppendLine("using Modern.Repositories.LiteDB.Async;");

        // Interface
        sb.AppendLine();
        sb.AppendLine($"namespace {namespaceName};");
        sb.AppendLine();
        sb.AppendLine($"///<summary>The {entityType} repository definition</summary>");
        sb.AppendLine($"public interface I{repositoryName} : IModernRepository<{entityType}, {idType}>");
        sb.AppendLine("{");
        sb.AppendLine("}");

        // Class
        sb.AppendLine();
        sb.AppendLine($"///<summary>The {entityType} repository implementation using LiteDB Async</summary>");
        sb.AppendLine($"public partial class {repositoryName} : ModernLiteDbAsyncRepository<{entityType}, {idType}>, I{repositoryName}");
        sb.AppendLine("{");
        sb.AppendLine("     ///<summary>Initializes a new instance of the class</summary>");
        sb.AppendLine($"    public {repositoryName}()");
        sb.AppendLine($"        : base(\"{connectionString}\", \"{collectionName}\")");
        sb.AppendLine("    {");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }
}

public class RepositorySyntaxReceiver : ISyntaxReceiver
{
    public List<ClassDeclarationSyntax> RepositoryClasses { get; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclarationSyntax)
        {
            return;
        }
        
        if (classDeclarationSyntax.AttributeLists.Any(a => a.Attributes.Any(x => x.Name.ToString().Contains("ModernLiteDbAsyncRepository"))))
        {
            RepositoryClasses.Add(classDeclarationSyntax);
        }
    }
}