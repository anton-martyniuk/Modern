using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Modern.Services.DataStore.InMemory.SourceGenerators;

[Generator]
public class ModernInMemoryServiceGenerator : ISourceGenerator
{
    private const string ServiceAttributeName = nameof(ModernInMemoryServiceAttribute);
    private const string EntityDboTypeMemberName = nameof(ModernInMemoryServiceAttribute.EntityDboType);
    private const string ServiceMemberName = nameof(ModernInMemoryServiceAttribute.ServiceName);

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

        foreach (var classDeclaration in receiver.ServiceClasses)
        {
            var model = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            if (model.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
            {
                continue;
            }

            var attributeData = classSymbol.GetAttributes().SingleOrDefault(a => a.AttributeClass?.Name == ServiceAttributeName);
            if (attributeData is null)
            {
                continue;
            }

            var entityDboType = attributeData.NamedArguments.SingleOrDefault(a => a.Key == EntityDboTypeMemberName).Value.Value?.ToString()
                ?? attributeData.ConstructorArguments[0].Value?.ToString();
            if (entityDboType is null)
            {
                continue;
            }
            
            var idProperty = classSymbol.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(m => m.Name == "Id");
            if (idProperty == null)
            {
                continue;
            }
            
            var entityDtoType = classSymbol.ToDisplayString();
            var idType = idProperty.Type.ToDisplayString();

            // Get rid of Dbo and Dto suffixes
            var className = entityDboType.Substring(entityDboType.LastIndexOf('.') + 1).Replace("Dbo", "").Replace("Dto", "");
            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
            
            // Get service name
            var serviceName = attributeData.NamedArguments.FirstOrDefault(a => a.Key == ServiceMemberName).Value.Value?.ToString()
                ?? $"{className}Service";

            var source = GenerateServiceCode(namespaceName, entityDtoType, entityDboType, idType, serviceName);
            context.AddSource($"{serviceName}_inmemory_service_gen.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private static string GenerateServiceCode(string namespaceName, string entityDtoType, string entityDboType,
        string idType, string serviceName)
    {
        var sb = new StringBuilder();

        // Usings
        sb.AppendLine("using Microsoft.Extensions.Logging;");
        sb.AppendLine("using Microsoft.Extensions.Options;");
        sb.AppendLine("using Modern.Repositories.Abstractions;");
        sb.AppendLine("using Modern.Services.DataStore.InMemory.Abstractions;");
        sb.AppendLine("using Modern.Services.DataStore.InMemory.Abstractions.Cache;");
        sb.AppendLine("using Modern.Services.DataStore.InMemory.Configuration;");
        sb.AppendLine("using Modern.Services.DataStore.InMemory;");

        // Interface
        sb.AppendLine();
        sb.AppendLine($"namespace {namespaceName};");
        sb.AppendLine();
        sb.AppendLine($"///<summary>The {entityDtoType} service definition</summary>");
        sb.AppendLine($"public interface I{serviceName} : IModernInMemoryService<{entityDtoType}, {entityDboType}, {idType}>");
        sb.AppendLine("{");
        sb.AppendLine("}");

        // Class
        sb.AppendLine();
        sb.AppendLine($"///<summary>The {entityDtoType} service implementation</summary>");
        sb.AppendLine($"public partial class {serviceName} : ModernInMemoryService<{entityDtoType}, {entityDboType}, {idType}>, I{serviceName}");
        sb.AppendLine("{");
        sb.AppendLine("     ///<summary>Initializes a new instance of the class</summary>");
        sb.AppendLine($"    public {serviceName}(IModernRepository<{entityDboType}, {idType}> repository, IModernServiceCache<{entityDtoType}, {idType}> cache, IOptions<ModernInMemoryServiceConfiguration> options, ILogger<ModernInMemoryService<{entityDtoType}, {entityDboType}, {idType}>> logger)");
        sb.AppendLine("        : base(repository, cache, options, logger)");
        sb.AppendLine("    {");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }
}

public class RepositorySyntaxReceiver : ISyntaxReceiver
{
    public List<ClassDeclarationSyntax> ServiceClasses { get; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclarationSyntax)
        {
            return;
        }
        
        if (classDeclarationSyntax.AttributeLists.Any(a => a.Attributes.Any(x => x.Name.ToString().Contains("ModernInMemoryService"))))
        {
            ServiceClasses.Add(classDeclarationSyntax);
        }
    }
}