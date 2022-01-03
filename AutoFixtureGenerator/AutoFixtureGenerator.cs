using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                .Where(type => type.Name.EndsWith("Store") || type.Name.EndsWith("Bridge"))
                .OrderByDescending(type => type.IsAbstract);
            
            ClassDeclarationSyntax? testBaseSyntax = context.Compilation
                .SyntaxTrees
                .SelectMany(syntaxTree => syntaxTree.GetRoot().DescendantNodes())
                .Where(x => x is ClassDeclarationSyntax)
                .Cast<ClassDeclarationSyntax>()
                .First(c => c.Identifier.ValueText == "TestBase");
            
            if (!SyntaxNodeHelper.TryGetParentSyntax(testBaseSyntax, out BaseNamespaceDeclarationSyntax? namespaceDeclarationSyntax))
            {
                return;
            }

            StringBuilder sourceBuilder = new StringBuilder($@" 
// Auto-generated code
using System;
using AutoFixture;
using Moq;
using Application.Bridges;
using Application.Stores;

namespace {namespaceDeclarationSyntax?.Name.ToString()}
{{
    public partial class TestBase
    {{
        partial void PopulateFixture()
        {{
"
            );

            foreach (ITypeSymbol typeSymbol in typesToMock)
            {
                sourceBuilder.AppendLine($"Fixture.Freeze<Mock<{typeSymbol.Name}>>();");
                sourceBuilder.AppendLine($"Fixture.Freeze<Lazy<{typeSymbol.Name}>>();");
            }

            sourceBuilder.AppendLine(@"
        }
    }
}");
            context.AddSource($"TestBase.g.cs", sourceBuilder.ToString());
        }

        private static IEnumerable<ITypeSymbol> GetAllTypes(INamespaceSymbol root)
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
    }
}