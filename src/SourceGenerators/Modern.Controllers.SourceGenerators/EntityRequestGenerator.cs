using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Modern.Controllers.SourceGenerators;

[Generator]
public class EntityRequestGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new EntitySyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not EntitySyntaxReceiver receiver)
        {
            return;
        }
            
        foreach (var declaration in receiver.CandidateClasses)
        {
            var semanticModel = context.Compilation.GetSemanticModel(declaration.SyntaxTree);
            var classSymbol = semanticModel.GetDeclaredSymbol(declaration);
            GenerateRequestsCode(context, classSymbol);
        }
            
        foreach (var declaration in receiver.CandidateRecords)
        {
            var semanticModel = context.Compilation.GetSemanticModel(declaration.SyntaxTree);
            var classSymbol = semanticModel.GetDeclaredSymbol(declaration);
            GenerateRequestsCode(context, classSymbol);
        }
    }

    private static void GenerateRequestsCode(GeneratorExecutionContext context, ISymbol? classSymbol)
    {
        if (classSymbol is not INamedTypeSymbol namedTypeSymbol)
        {
            return;
        }

        var attributeData = classSymbol.GetAttributes().FirstOrDefault(ad => ad.AttributeClass?.ToDisplayString().Contains("EntityRequest") is true);
        if (attributeData is null)
        {
            return;
        }

        var source = GenerateRequestClasses(namedTypeSymbol, attributeData);
        context.AddSource($"{namedTypeSymbol.Name}_EntityRequests_gen.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static string GenerateRequestClasses(INamedTypeSymbol classSymbol, AttributeData attributeData)
    {
        var generateCreateRequest = (bool)(attributeData.NamedArguments.FirstOrDefault(arg => arg.Key == "GenerateCreateRequest").Value.Value ?? true);
        var generateUpdateRequest = (bool)(attributeData.NamedArguments.FirstOrDefault(arg => arg.Key == "GenerateUpdateRequest").Value.Value ?? true);

        var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
        var className = classSymbol.Name;
            
        var createRequestName = (string?)attributeData.NamedArguments.FirstOrDefault(arg => arg.Key == "CreateRequestName").Value.Value ?? $"Create{className}Request";
        var updateRequestName = (string?)attributeData.NamedArguments.FirstOrDefault(arg => arg.Key == "UpdateRequestName").Value.Value ?? $"Update{className}Request";
            
        var properties = classSymbol.GetMembers().OfType<IPropertySymbol>().ToList();
        // var createProperties = properties.Where(p => p.Name != "Id").ToList();
        // var updateProperties = properties.ToList();
        
        var createProperties = properties.Where(p => p.Name != "Id" && p.GetAttributes()
                .All(a => a.AttributeClass?.ToDisplayString().Contains("IgnoreCreateRequest") is false))
            .ToList();

        var updateProperties = properties.Where(p => p.GetAttributes()
                .All(a => a.AttributeClass?.ToDisplayString().Contains("IgnoreUpdateRequest") is false))
            .ToList();

        var sb = new StringBuilder();
        sb.AppendLine($"namespace {namespaceName}");
        sb.AppendLine("{");

        // Generate type for Create{ClassName}Request
        if (generateCreateRequest)
        {
            sb.AppendLine($"    ///<summary>The webapi create request for {className}</summary>");
            sb.AppendLine($"    public partial class {createRequestName}");
            sb.AppendLine("    {");
            foreach (var property in createProperties)
            {
                sb.AppendLine($"        ///<summary>Gets and sets a {property.Name} property</summary>");
                sb.AppendLine($"        public {property.Type} {property.Name} {{ get; set; }} = default!;");
                sb.AppendLine();
            }

            sb.AppendLine("    }");   
        }

        // Generate type for Update{ClassName}Request
        if (generateUpdateRequest)
        {
            sb.AppendLine();
            sb.AppendLine($"    ///<summary>The webapi update request for {className}</summary>");
            sb.AppendLine($"    public partial class {updateRequestName}");
            sb.AppendLine("    {");
            foreach (var property in updateProperties)
            {
                sb.AppendLine($"        ///<summary>Gets and sets a {property.Name} property</summary>");
                sb.AppendLine($"        public {property.Type} {property.Name} {{ get; set; }} = default!;");
                sb.AppendLine();
            }

            sb.AppendLine("    }");   
        }
            
        sb.AppendLine("}");

        return sb.ToString();
    }
}

public class EntitySyntaxReceiver : ISyntaxReceiver
{
    private const string EntityRequestAttributeName = "Modern.Controllers.DataStore.DependencyInjection.Attributes.EntityRequest";

    public List<ClassDeclarationSyntax> CandidateClasses { get; } = new();
        
    public List<RecordDeclarationSyntax> CandidateRecords { get; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        switch (syntaxNode)
        {
            case ClassDeclarationSyntax classDeclarationSyntax when HasEntityRequestAttribute(classDeclarationSyntax):

                CandidateClasses.Add(classDeclarationSyntax);
                break;
            case RecordDeclarationSyntax recordDeclarationSyntax when HasEntityRequestAttribute(recordDeclarationSyntax):
                CandidateRecords.Add(recordDeclarationSyntax);
                break;
        }
    }

    private static bool HasEntityRequestAttribute(MemberDeclarationSyntax classDeclarationSyntax)
        => classDeclarationSyntax.AttributeLists.Count > 0 && classDeclarationSyntax.AttributeLists.SelectMany(x => x.Attributes).Any(x => x.Name.ToString().Contains("EntityRequest"));
}