//using HotChocolate.Execution.Configuration;
//using HotChocolate.Types.Descriptors;
//using HotChocolate.Types.Descriptors.Definitions;
//using Microsoft.Extensions.DependencyInjection;
//using Modern.GraphQL.HotChocolate.DependencyInjection.Definitions.Configuration;
//using Modern.Services.DataStore.Abstractions;

//namespace Modern.GraphQL.HotChocolate.DataStore.DependencyInjection;

///// <summary>
///// The GraphQL dynamic type module
///// </summary>
//internal class DynamicTypeModule : ITypeModule
//{
//    private readonly List<ModernGraphQlSpecification> _specifications;
//    public event EventHandler<EventArgs>? TypesChanged;

//    /// <summary>
//    /// Initializes a new instance of the class
//    /// </summary>
//    /// <param name="specifications">List of specifications</param>
//    public DynamicTypeModule(List<ModernGraphQlSpecification> specifications)
//    {
//        _specifications = specifications;
//    }

//    public async ValueTask<IReadOnlyCollection<ITypeSystemMember>> CreateTypesAsync(IDescriptorContext context, CancellationToken cancellationToken)
//    {
//        var types = new List<ITypeSystemMember>();

//        foreach (var specification in _specifications)
//        {
//            var name = $"ModernGraphQlQuery.{specification.EntityDtoType.Name}";
//            var typeDefinition = new ObjectTypeDefinition(name);
//            var queryType = new ObjectTypeDefinition("Query");

//            //var schemaNamePascalCase = schema.Name!.ToPascalCase();
//            //var schemaNamePluralCamelCase = schema.PluralName!.ToCamelCase();
//            //var objectTypeDefinition = new ObjectTypeDefinition(schemaNamePascalCase);

//            await AddFieldsAsync(types, schema, objectTypeDefinition, schema.Properties);
//            queryType.Fields.Add(new ObjectFieldDefinition(schemaNamePluralCamelCase)
//                {
//                    Type = TypeReference.Parse($"[{schemaNamePascalCase}]"),
//                    Resolver = async (ctx) =>
//                    {
//                        var serviceType = typeof(IModernService<,,>).MakeGenericType(specification.EntityDtoType, specification.EntityDboType, specification.EntityIdType);
//                        var service = ctx.Services.GetRequiredService(serviceType);

//                        var entity = await service.TryGetByIdAsync(id).ConfigureAwait(false);
//                        return entity;
//                    }
//                }
//                .ToDescriptor(context)
//                .UseFiltering()
//                .ToDefinition());
//            types.Add(ObjectType.CreateUnsafe(objectTypeDefinition));
//        }

//        types.Add(ObjectType.CreateUnsafe(queryType));

//        await Task.CompletedTask;
//        return types;
//    }
//}
