using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Modern.Repositories.EFCore.SourceGenerators;

[Generator]
public class ModernEfCoreRepositoryGenerator : ISourceGenerator
{
    private const string RepositoryAttributeName = nameof(ModernEfCoreRepositoryAttribute);
    private const string RepositoryWithFactoryAttributeName = nameof(ModernEfCoreRepositoryWithFactoryAttribute);
    private const string RepositoryForUnitOfWorkAttributeName = nameof(ModernEfCoreRepositoryForUnitOfWorkAttribute);
    private const string DbContextMemberName = nameof(ModernEfCoreRepositoryAttribute.DbContextType);

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

            var data = GetAttributeData(classSymbol);
            if (data is null)
            {
                continue;
            }

            var attributeData = data.Value.attributeData;

            var dbContextType = attributeData.NamedArguments.SingleOrDefault(a => a.Key == DbContextMemberName).Value.Value?.ToString()
                ?? attributeData.ConstructorArguments[0].Value?.ToString();
            if (dbContextType is null)
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
            
            // Get repository name
            var repositoryName = attributeData.NamedArguments.FirstOrDefault(a => a.Key == "RepositoryName").Value.Value?.ToString() 
                ?? $"{className}Repository";

            var source = data.Value.repositoryType switch
            {
                RepositoryType.Regular => GenerateRepositoryCode(dbContextType, entityType, idType, repositoryName),
                RepositoryType.WithFactory => GenerateRepositoryWithFactoryCode(dbContextType, entityType, idType, repositoryName),
                RepositoryType.UnitOfWork => GenerateRepositoryForUnitOfWorkCode(dbContextType, entityType, idType, repositoryName),
                _ => string.Empty
            };

            if (!string.IsNullOrWhiteSpace(source))
            {
                context.AddSource($"{repositoryName}_efcore_gen.cs", SourceText.From(source, Encoding.UTF8));
            }
        }
    }

    private static (AttributeData attributeData, RepositoryType repositoryType)? GetAttributeData(INamedTypeSymbol classSymbol)
    {
        var attributeData = classSymbol.GetAttributes().SingleOrDefault(a => a.AttributeClass?.Name == RepositoryAttributeName);
        if (attributeData is not null)
        {
            return (attributeData, RepositoryType.Regular);
        }
        
        attributeData = classSymbol.GetAttributes().SingleOrDefault(a => a.AttributeClass?.Name == RepositoryWithFactoryAttributeName);
        if (attributeData is not null)
        {
            return (attributeData, RepositoryType.WithFactory);
        }
        
        attributeData = classSymbol.GetAttributes().SingleOrDefault(a => a.AttributeClass?.Name == RepositoryForUnitOfWorkAttributeName);
        if (attributeData is not null)
        {
            return (attributeData, RepositoryType.UnitOfWork);
        }

        return null;
    }

    private static string GenerateRepositoryCode(string dbContextType, string entityType, string idType, string repositoryName)
    {
        var sb = new StringBuilder();

        // Usings
        sb.AppendLine("using Modern.Repositories.Abstractions;");
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine("using Microsoft.Extensions.Options;");
        sb.AppendLine("using Modern.Repositories.EFCore;");
        sb.AppendLine("using Modern.Repositories.EFCore.Configuration;");

        // Interface
        sb.AppendLine();
        sb.AppendLine($"///<summary>The {entityType} repository definition</summary>");
        sb.AppendLine($"public partial interface I{repositoryName} : IModernRepository<{entityType}, {idType}>");
        sb.AppendLine("{");
        sb.AppendLine("}");

        // Class
        sb.AppendLine();
        sb.AppendLine($"///<summary>The {entityType} repository implementation using EF Core</summary>");
        sb.AppendLine($"public partial class {repositoryName} : ModernEfCoreRepository<{dbContextType}, {entityType}, {idType}>, I{repositoryName}");
        sb.AppendLine("{");
        sb.AppendLine("     ///<summary>Initializes a new instance of the class</summary>");
        sb.AppendLine($"    public {repositoryName}({dbContextType} dbContext, IOptions<EfCoreRepositoryConfiguration> configuration)");
        sb.AppendLine("        : base(dbContext, configuration)");
        sb.AppendLine("    {");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }
    
    private static string GenerateRepositoryWithFactoryCode(string dbContextType, string entityType, string idType, string repositoryName)
    {
        var sb = new StringBuilder();

        // Usings
        sb.AppendLine("using Modern.Repositories.Abstractions;");
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine("using Microsoft.Extensions.Options;");
        sb.AppendLine("using Modern.Repositories.EFCore;");
        sb.AppendLine("using Modern.Repositories.EFCore.Configuration;");

        // Interface
        sb.AppendLine();
        sb.AppendLine($"///<summary>The {entityType} repository definition</summary>");
        sb.AppendLine($"public partial interface I{repositoryName} : IModernRepository<{entityType}, {idType}>");
        sb.AppendLine("{");
        sb.AppendLine("}");

        // Class
        sb.AppendLine();
        sb.AppendLine($"///<summary>The {entityType} repository implementation using EF Core</summary>");
        sb.AppendLine($"public partial class {repositoryName} : ModernEfCoreRepositoryWithFactory<{dbContextType}, {entityType}, {idType}>, I{repositoryName}");
        sb.AppendLine("{");
        sb.AppendLine("     ///<summary>Initializes a new instance of the class</summary>");
        sb.AppendLine($"    public {repositoryName}(IDbContextFactory<{dbContextType}> dbContextFactory, IOptions<EfCoreRepositoryConfiguration> configuration)");
        sb.AppendLine("        : base(dbContextFactory, configuration)");
        sb.AppendLine("    {"); 
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }
    
    private static string GenerateRepositoryForUnitOfWorkCode(string dbContextType, string entityType, string idType, string repositoryName)
    {
        var sb = new StringBuilder();

        // Usings
        sb.AppendLine("using Modern.Repositories.Abstractions;");
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine("using Modern.Repositories.EFCore;");

        // Interface
        sb.AppendLine();
        sb.AppendLine($"///<summary>The {entityType} repository definition</summary>");
        sb.AppendLine($"public partial interface I{repositoryName} : IModernRepository<{entityType}, {idType}>");
        sb.AppendLine("{");
        sb.AppendLine("}");

        // Class
        sb.AppendLine();
        sb.AppendLine($"///<summary>The {entityType} repository implementation using EF Core</summary>");
        sb.AppendLine($"public partial class {repositoryName} : ModernEfCoreRepositoryForUnitOfWork<{dbContextType}, {entityType}, {idType}>, I{repositoryName}");
        sb.AppendLine("{");
        sb.AppendLine("     ///<summary>Initializes a new instance of the class</summary>");
        sb.AppendLine($"    public {repositoryName}({dbContextType} dbContext)");
        sb.AppendLine("        : base(dbContext)");
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
        
        if (classDeclarationSyntax.AttributeLists.Any(a => a.Attributes.Any(x => x.Name.ToString().Contains("ModernEfCoreRepositoryForUnitOfWork"))))
        {
            RepositoryClasses.Add(classDeclarationSyntax);
        }
        else if (classDeclarationSyntax.AttributeLists.Any(a => a.Attributes.Any(x => x.Name.ToString().Contains("ModernEfCoreRepositoryWithFactory"))))
        {
            RepositoryClasses.Add(classDeclarationSyntax);
        }
        else if (classDeclarationSyntax.AttributeLists.Any(a => a.Attributes.Any(x => x.Name.ToString().Contains("ModernEfCoreRepository"))))
        {
            RepositoryClasses.Add(classDeclarationSyntax);
        }
    }
}