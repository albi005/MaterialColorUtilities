using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace MaterialColorUtilities.Schemes;

[Generator]
internal class SchemeConverterGenerator : IIncrementalGenerator
{ 
    const string SchemeDisplayString = "MaterialColorUtilities.Schemes.Scheme<TColor>";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var resultSourceFile = context.SyntaxProvider.CreateSyntaxProvider(
            (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax cds
            && cds.BaseList != null
            && cds.TypeParameterList != null
            && cds.Parent is not ClassDeclarationSyntax
            && cds.Modifiers.Any(SyntaxKind.PartialKeyword),
            CreateSourceFile);

        context.RegisterSourceOutput(resultSourceFile, static (SourceProductionContext context, (string Hint, SourceText Text) source) =>
        {
            if (source.Hint == null) return;
            context.AddSource(source.Hint, source.Text);
        });
    }

    public (string Hint, SourceText Source) CreateSourceFile(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        SemanticModel semanticModel = context.SemanticModel;
        INamedTypeSymbol classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax);

        if (!IsScheme(classSymbol)) return (null, null);

        StringBuilder builder = new();
        builder.AppendLine("using System;");
        builder.AppendLine();

        bool isGlobalNamespace = classSymbol.ContainingNamespace.IsGlobalNamespace;
        if (!isGlobalNamespace)
        {
            builder.AppendLine($"namespace {classSymbol.ContainingNamespace.ToDisplayString()}");
            builder.AppendLine("{");
        }

        AddClass(builder, classSymbol, classDeclarationSyntax);

        if (!isGlobalNamespace)
            builder.AppendLine("}");

        SourceText sourceText = SyntaxFactory.ParseCompilationUnit(builder.ToString())
            .NormalizeWhitespace()
            .GetText(Encoding.UTF8);

        string hint = "";
        if (!isGlobalNamespace) hint += $"{classSymbol.ContainingNamespace}.";
        hint += classDeclarationSyntax.Identifier.ToString();
        int typeParameterCount = classDeclarationSyntax.TypeParameterList.Parameters.Count;
        if (typeParameterCount > 1) hint += $"`{typeParameterCount}";
        hint += ".ConvertTo.g.cs";

        return (hint, sourceText);
    }

    bool IsScheme(INamedTypeSymbol symbol)
    {
        if (symbol.OriginalDefinition.ToDisplayString() == SchemeDisplayString)
            return true;
        if (symbol.BaseType == null)
            return false;
        return IsScheme(symbol.BaseType);
    }

    // "SCHEME" is replaced with the implementer's name
    // "TCOLOR" is defined by the implementer
    // public SCHEME<TResult> ConvertTo<TResult>(Func<TCOLOR, TResult> convert)
    // public SCHEME<TResult> ConvertTo<TResult>(Func<TCOLOR, TResult> convert, SCHEME<TResult> result)
    void AddClass(StringBuilder builder, INamedTypeSymbol symbol, ClassDeclarationSyntax declaration)
    {
        string tColor = GetTColor(symbol);
        string resultIdentifierAndTypeParameters = $"{declaration.Identifier}{declaration.TypeParameterList.ToString().Replace(tColor, "TResult")}";
        builder.AppendLine($"{declaration.Modifiers} class {declaration.Identifier}{declaration.TypeParameterList}");
        builder.AppendLine("{");
        builder.AppendLine("/// <summary>Converts the Scheme into a new one with a different color type</summary>");
        builder.AppendLine($"public new {resultIdentifierAndTypeParameters} ConvertTo<TResult>(Func<{tColor}, TResult> convert)");
        builder.AppendLine("{");
        builder.AppendLine($"return ConvertTo<TResult>(convert, new {resultIdentifierAndTypeParameters}());");
        builder.AppendLine("}");
        builder.AppendLine();
        builder.AppendLine("/// <summary>Maps the Scheme's colors onto an existing Scheme object with a different color type</summary>");
        builder.AppendLine($"public {resultIdentifierAndTypeParameters} ConvertTo<TResult>(Func<{tColor}, TResult> convert, {resultIdentifierAndTypeParameters} result)");
        builder.AppendLine("{");
        builder.AppendLine("base.ConvertTo(convert, result);");
        foreach (var member in symbol.GetMembers())
        {
            if (member.Kind == SymbolKind.Property)
            {
                var property = (IPropertySymbol)member;
                if (property.Type.Name == tColor)
                {
                    builder.AppendLine($"result.{property.Name} = convert({property.Name});");
                }
            }
        }
        builder.AppendLine();
        builder.AppendLine("return result;");
        builder.AppendLine("}");
        builder.AppendLine("}");
    }

    string GetTColor(INamedTypeSymbol symbol)
    {
        if (symbol.OriginalDefinition.ToDisplayString() == SchemeDisplayString)
            return symbol.TypeArguments[0].ToDisplayString();
        return GetTColor(symbol.BaseType);
    }
}
