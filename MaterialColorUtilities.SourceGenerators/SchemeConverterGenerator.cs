using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace MaterialColorUtilities.SourceGenerators
{
    [Generator(LanguageNames.CSharp)]
    public sealed class SchemeConverterGenerator : IIncrementalGenerator
    {
        const string SchemeDisplayString = "MaterialColorUtilities.Schemes.Scheme<TColor>";

        public record struct ClassContext(ClassDeclarationSyntax Syntax, INamedTypeSymbol Symbol);
        
        public class ClassContextNameOnlyComparer : IEqualityComparer<ClassContext>
        {
            public bool Equals(ClassContext x, ClassContext y) => x.Symbol.ToDisplayString() == y.Symbol.ToDisplayString();
            public int GetHashCode(ClassContext obj) => obj.Symbol.ToDisplayString().GetHashCode();
            public static ClassContextNameOnlyComparer Default { get; } = new ClassContextNameOnlyComparer();
        }
        public record struct SourceCreationContext(
            string Identifier,
            string Modifiers,
            string Namespace,
            ImmutableArray<string> NewColors,
            string TColor,
            string TypeParameters) // includes <>
        {
            public bool Equals(SourceCreationContext other) =>
                Identifier == other.Identifier
                && Modifiers == other.Modifiers
                && Namespace == other.Namespace
                && NewColors.SequenceEqual(other.NewColors)
                && TColor == other.TColor
                && TypeParameters == other.TypeParameters;
            public override int GetHashCode() =>
                Identifier.GetHashCode()
                ^ Modifiers.GetHashCode()
                ^ Namespace.GetHashCode()
                ^ NewColors.GetHashCode()
                ^ TColor.GetHashCode()
                ^ TypeParameters.GetHashCode();
        }
        public record struct Result(string Hint, string SourceText);

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<ClassContext> classes = context.SyntaxProvider.CreateSyntaxProvider(
                static (syntaxNode, _) =>
                    syntaxNode is ClassDeclarationSyntax cds
                    && cds.BaseList != null
                    && cds.TypeParameterList != null
                    && cds.Parent is not ClassDeclarationSyntax
                    && cds.Modifiers.Any(SyntaxKind.PartialKeyword),
                static (context, _) =>
                {
                    ClassDeclarationSyntax syntax = (ClassDeclarationSyntax)context.Node;
                    INamedTypeSymbol symbol = context.SemanticModel.GetDeclaredSymbol(syntax);
                    return new ClassContext(syntax, symbol);
                });

            IncrementalValuesProvider<ClassContext> schemes = classes.Where(c => IsScheme(c.Symbol));

            IncrementalValuesProvider<ClassContext> schemesWithoutDuplicates = schemes
                .Collect()
                .SelectMany(static (schemes, _) => schemes.Distinct(ClassContextNameOnlyComparer.Default));

            // Collect necessary information about the Scheme
            IncrementalValuesProvider<SourceCreationContext> sourceCreationContexts = schemesWithoutDuplicates
                .Select(static (context, _) =>
                {
                    string tColor = GetTColor(context.Symbol);
                    return new SourceCreationContext(
                        context.Syntax.Identifier.ToString(),
                        context.Syntax.Modifiers.ToString(),
                        context.Symbol.ContainingNamespace.IsGlobalNamespace
                            ? null
                            : context.Symbol.ContainingNamespace.ToDisplayString(),
                        context.Symbol
                            .GetMembers()
                            .OfType<IPropertySymbol>()
                            .Where(p => p.Type.Name == tColor)
                            .Select(p => p.Name)
                            .ToImmutableArray(),
                        tColor,
                        context.Syntax.TypeParameterList.ToString());
                })
                .Where(static context => context.TypeParameters.Contains(context.TColor));

            // Build the SourceText
            IncrementalValuesProvider<Result> results = sourceCreationContexts.Select(static (context, _) =>
            {
                bool hasNamespace = context.Namespace != null;
                string typeParameters = Regex.Replace(context.TypeParameters, @$"\b{context.TColor}\b", "TResult");
                string resultType = $"{context.Identifier}{typeParameters}";

                StringBuilder builder = new();
                builder.AppendLine("using System;");
                builder.AppendLine();
                if (hasNamespace)
                {
                    builder.AppendLine($"namespace {context.Namespace}");
                    builder.AppendLine("{");
                }
                builder.AppendLine($"{context.Modifiers} class {context.Identifier}{context.TypeParameters}");
                builder.AppendLine("{");
                builder.AppendLine("/// <summary>Converts the Scheme into a new one with a different color type</summary>");
                builder.AppendLine($"public new {resultType} ConvertTo<TResult>(Func<{context.TColor}, TResult> convert)");
                builder.AppendLine("{");
                builder.AppendLine($"return ConvertTo<TResult>(convert, new {resultType}());");
                builder.AppendLine("}");
                builder.AppendLine();
                builder.AppendLine("/// <summary>Maps the Scheme's colors onto an existing Scheme object with a different color type</summary>");
                builder.AppendLine($"public {resultType} ConvertTo<TResult>(Func<{context.TColor}, TResult> convert, {resultType} result)");
                builder.AppendLine("{");
                builder.AppendLine("base.ConvertTo(convert, result);");
                foreach (var newColor in context.NewColors)
                {
                    builder.AppendLine($"result.{newColor} = convert({newColor});");
                }
                builder.AppendLine();
                builder.AppendLine("return result;");
                builder.AppendLine("}");
                builder.AppendLine("}");
                if (hasNamespace)
                    builder.AppendLine("}");

                string sourceText = SyntaxFactory.ParseCompilationUnit(builder.ToString())
                    .NormalizeWhitespace()
                    .GetText(Encoding.UTF8)
                    .ToString();

                string hint = "";
                if (hasNamespace) hint += $"{context.Namespace}.";
                hint += context.Identifier;
                hint += $"{{{context.TypeParameters.Substring(1, context.TypeParameters.Length - 2)}}}";
                hint += ".ConvertTo.sg.cs";

                return new Result(hint, sourceText);
            });

            context.RegisterSourceOutput(results, static (generator, result) =>
            {
#if DEBUG
                result.SourceText = $"// Generated at: {DateTime.Now}\n" + result.SourceText;
#endif
                generator.AddSource(result.Hint, result.SourceText);
            });
        }

        static bool IsScheme(INamedTypeSymbol symbol)
        {
            if (symbol.OriginalDefinition.ToDisplayString() == SchemeDisplayString)
                return true;
            if (symbol.BaseType == null)
                return false;
            return IsScheme(symbol.BaseType);
        }

        static string GetTColor(INamedTypeSymbol symbol)
        {
            if (symbol.OriginalDefinition.ToDisplayString() == SchemeDisplayString)
                return symbol.TypeArguments[0].ToDisplayString();
            return GetTColor(symbol.BaseType);
        }
    }
}

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Workaround for
    /// "Predefined type 'System.Runtime.CompilerServices.IsExternalInit' is not defined or imported"
    /// when declaring records
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit
    {
    }
}