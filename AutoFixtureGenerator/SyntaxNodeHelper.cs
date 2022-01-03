using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoFixtureGenerator;

public static class SyntaxNodeHelper
{
    public static bool TryGetParentSyntax<T>(SyntaxNode? syntaxNode, out T? result) where T : SyntaxNode
    {
        // set defaults
        result = null;

        if (syntaxNode == null)
        {
            return false;
        }

        try
        {
            syntaxNode = syntaxNode.Parent;

            switch (syntaxNode)
            {
                case null:
                    return false;
                case T node:
                    result = node;
                    return true;
                default:
                    return TryGetParentSyntax<T>(syntaxNode, out result);
            }
        }
        catch
        {
            return false;
        }
    }
}