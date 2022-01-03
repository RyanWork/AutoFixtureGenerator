using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace AutoFixtureGenerator
{
    [Generator]
    public class AutoFixtureGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
// #if DEBUG
//             if (!Debugger.IsAttached)
//             {
//                 Debugger.Launch();
//             }
// #endif
        }

        static IEnumerable<ITypeSymbol> GetAllTypes(INamespaceSymbol root)
        {
            foreach (var namespaceOrTypeSymbol in root.GetMembers())
            {
                switch (namespaceOrTypeSymbol)
                {
                    case INamespaceSymbol @namespace:
                    {
                        foreach (var nested in GetAllTypes(@namespace))
                            yield return nested;
                        break;
                    }
                    case ITypeSymbol type:
                        yield return type;
                        break;
                }
            }
        }
        
        public void Execute(GeneratorExecutionContext context)
        {
            const string applicationNameSpace = "Application";
            var namespaceSymbol = context.Compilation
                .SourceModule
                .ReferencedAssemblySymbols
                .First(x => x.Name == applicationNameSpace)
                .GlobalNamespace
                .GetNamespaceMembers()
                .Single(x => x.Name == applicationNameSpace);
            
            var typesToMock = GetAllTypes(namespaceSymbol)
                .Where(x => x.Name.EndsWith("Store") || x.Name.EndsWith("Bridge"));
            
            ClassDeclarationSyntax? testBaseSyntax = context.Compilation
                .SyntaxTrees
                .SelectMany(syntaxTree => syntaxTree.GetRoot().DescendantNodes())
                .Where(x => x is ClassDeclarationSyntax)
                .Cast<ClassDeclarationSyntax>()
                .First(c => c.Identifier.ValueText == "TestBase");

            // NOTE: We cannot find the type FileScoped
            if (!SyntaxNodeHelper.TryGetParentSyntax(testBaseSyntax, out NamespaceDeclarationSyntax? namespaceDeclarationSyntax))
            {
                return; // or whatever you want to do in this scenario
            }

            StringBuilder sourceBuilder = new StringBuilder($@" 
// Auto-generated code
using AutoFixture;
using Moq;
using Application.Bridges;
using Application.Stores;

namespace {namespaceDeclarationSyntax?.Name.ToString()}
{{
    public partial class TestBase
    {{
        partial void PopulateFixture()
        {{"
            );

            // foreach (ITypeSymbol typeSymbol in typesToMock)
            // {
            //     sourceBuilder.AppendLine($"Fixture.Freeze<Mock<{typeSymbol.Name}>>();");
            // }

            sourceBuilder.AppendLine("}}}");
            context.AddSource($"TestBase.g.cs", sourceBuilder.ToString());
        }
    }
}