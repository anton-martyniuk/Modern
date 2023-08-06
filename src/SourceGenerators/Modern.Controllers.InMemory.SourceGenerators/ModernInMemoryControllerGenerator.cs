using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Modern.Controllers.InMemory.SourceGenerators;

[Generator]
public class ModernInMemoryControllerGenerator : ISourceGenerator
{
    private const string ControllerAttributeName = nameof(ModernInMemoryControllerAttribute);
    private const string CreateRequestTypeMemberName = nameof(ModernInMemoryControllerAttribute.CreateRequestType);
    private const string UpdateRequestTypeMemberName = nameof(ModernInMemoryControllerAttribute.UpdateRequestType);
    private const string EntityDboTypeMemberName = nameof(ModernInMemoryControllerAttribute.EntityDboType);
    private const string ApiRouteMemberName = nameof(ModernInMemoryControllerAttribute.ApiRoute);
    private const string ControllerMemberName = nameof(ModernInMemoryControllerAttribute.ControllerName);

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

        foreach (var classDeclaration in receiver.ControllerClasses)
        {
            var model = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            if (model.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
            {
                continue;
            }

            var attributeData = classSymbol.GetAttributes().SingleOrDefault(a => a.AttributeClass?.Name == ControllerAttributeName);
            if (attributeData is null)
            {
                continue;
            }

            var createRequestType = attributeData.NamedArguments.SingleOrDefault(a => a.Key == CreateRequestTypeMemberName).Value.Value?.ToString()
                ?? attributeData.ConstructorArguments[0].Value?.ToString();
            if (createRequestType is null)
            {
                continue;
            }
            
            var updateRequestType = attributeData.NamedArguments.SingleOrDefault(a => a.Key == UpdateRequestTypeMemberName).Value.Value?.ToString()
                ?? attributeData.ConstructorArguments[1].Value?.ToString();
            if (updateRequestType is null)
            {
                continue;
            }
            
            var entityDboType = attributeData.NamedArguments.SingleOrDefault(a => a.Key == EntityDboTypeMemberName).Value.Value?.ToString()
                ?? attributeData.ConstructorArguments[2].Value?.ToString();
            if (entityDboType is null)
            {
                continue;
            }
            
            var apiRoute = attributeData.NamedArguments.SingleOrDefault(a => a.Key == ApiRouteMemberName).Value.Value?.ToString()
                ?? attributeData.ConstructorArguments[3].Value?.ToString();
            if (apiRoute is null)
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
            var className = apiRoute.Substring(apiRoute.LastIndexOf('.') + 1).Replace("Dbo", "").Replace("Dto", "");
            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
            
            // Get service name
            var controllerName = attributeData.NamedArguments.FirstOrDefault(a => a.Key == ControllerMemberName).Value.Value?.ToString()
                ?? $"{className}Controller";

            var source = GenerateServiceCode(namespaceName, entityDtoType, entityDboType, idType, 
                createRequestType, updateRequestType, controllerName, apiRoute);
            context.AddSource($"{controllerName}_controller_gen.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private static string GenerateServiceCode(string namespaceName, string entityDtoType, string entityDboType,
        string idType, string createRequestType, string updateRequestType, string controllerName, string apiRoute)
    {
        var sb = new StringBuilder();
        
        // Usings
        sb.AppendLine("using System.ComponentModel.DataAnnotations;");
        sb.AppendLine("using System.Net;");
        sb.AppendLine("using Mapster;");
        sb.AppendLine("using Microsoft.AspNetCore.JsonPatch;");
        sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
        sb.AppendLine("using Modern.Exceptions;");
        sb.AppendLine("using Modern.Services.DataStore.InMemory.Abstractions;");
        
        sb.AppendLine();
        sb.AppendLine($"namespace {namespaceName};");
        sb.AppendLine();

        sb.AppendLine();
        sb.AppendLine($"///<summary>The {entityDtoType} controller implementation</summary>");
        sb.AppendLine("[ApiController]");
        sb.AppendLine($"[Route(\"{apiRoute}\")]");
        sb.AppendLine($"public partial class {controllerName} : ModernInMemoryController<{createRequestType}, {updateRequestType}, {entityDtoType}, {entityDboType}, {idType}>");
        sb.AppendLine("{");
        sb.AppendLine("     ///<summary>Initializes a new instance of the class</summary>");
        sb.AppendLine($"    public {controllerName}(IModernInMemoryService<{entityDtoType}, {entityDboType}, {idType}> service)");
        sb.AppendLine("        : base(service)");
        sb.AppendLine("    {");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }
}

public class RepositorySyntaxReceiver : ISyntaxReceiver
{
    public List<ClassDeclarationSyntax> ControllerClasses { get; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclarationSyntax)
        {
            return;
        }
        
        if (classDeclarationSyntax.AttributeLists.Any(a => a.Attributes.Any(x => x.Name.ToString().Contains("ModernInMemoryController"))))
        {
            ControllerClasses.Add(classDeclarationSyntax);
        }
    }
}