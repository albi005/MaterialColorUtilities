using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace MaterialColorUtilities.Schemes;

[Generator]
internal class SchemeConverterGenerator : ISourceGenerator
{
    const string SchemeDisplayString = "MaterialColorUtilities.Schemes.Scheme<TColor>";

    static readonly DiagnosticDescriptor NestedDescriptor = new(
        "MC0001",
        "Nested Scheme subclasses won't have converter methods generated",
        "Scheme subclass '{0}' won't have converter methods generated as it is nested inside another class",
        "",
        DiagnosticSeverity.Warning,
        true);

    static readonly DiagnosticDescriptor NonPartialDescriptor = new(
        "MC0002",
        "Non-partial Scheme subclasses won't have converter methods generated",
        "Scheme subclass '{0}' won't have converter methods generated as it is not marked partial",
        "",
        DiagnosticSeverity.Warning,
        true);

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        SyntaxReceiver syntaxReceiver = (SyntaxReceiver)context.SyntaxReceiver;
        foreach (ClassDeclarationSyntax classDeclarationSyntax in syntaxReceiver.Candidates)
        {
            SemanticModel semanticModel = context.Compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
            INamedTypeSymbol classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax);
            
            if (!IsScheme(classSymbol)) return;

            if (IsNested(classSymbol))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    NestedDescriptor,
                    classDeclarationSyntax.GetLocation(),
                    $"{classDeclarationSyntax.Identifier}{classDeclarationSyntax.TypeParameterList}"));
                break;
            }
            
            if (!IsPartial(classDeclarationSyntax))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    NonPartialDescriptor,
                    classDeclarationSyntax.GetLocation(),
                    $"{classDeclarationSyntax.Identifier}{classDeclarationSyntax.TypeParameterList}"));
                break;
            }
                
            StringBuilder builder = new();
            builder.AppendLine("using System;");
            builder.AppendLine();
            AddNamespace(builder, classSymbol, classDeclarationSyntax);

            SourceText sourceText = SyntaxFactory.ParseCompilationUnit(builder.ToString())
                .NormalizeWhitespace()
                .GetText(Encoding.UTF8);
            context.AddSource($"{classDeclarationSyntax.Identifier}.g.cs", sourceText);            
        }
    }

    bool IsScheme(INamedTypeSymbol symbol)
    {
        if (symbol.OriginalDefinition.ToDisplayString() == SchemeDisplayString)
            return true;
        if (symbol.BaseType == null)
            return false;
        return IsScheme(symbol.BaseType);
    }

    bool IsNested(INamedTypeSymbol symbol)
    {
        return symbol.ContainingType != null;
    }

    bool IsPartial(ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);
    }

    void AddNamespace(StringBuilder builder, INamedTypeSymbol classSymbol, ClassDeclarationSyntax classDeclarationSyntax)
    {
        bool isGlobalNamespace = classSymbol.ContainingNamespace.IsGlobalNamespace;
        if (!isGlobalNamespace)
        {
            builder.AppendLine($"namespace {classSymbol.ContainingNamespace.ToDisplayString()}");
            builder.AppendLine("{");
        }
        
        AddClass(builder, classSymbol, classDeclarationSyntax);

        if (!isGlobalNamespace)
            builder.AppendLine("}");
    }

    // "SCHEME" is replaced with the implementer's name
    // "TCOLOR" is defined by the implementer
    // public SCHEME<TResult> ConvertTo<TResult>(Func<TCOLOR, TResult> convert)
    // public SCHEME<TResult> ConvertTo<TResult>(Func<TCOLOR, TResult> convert, SCHEME<TResult> result)
    void AddClass(StringBuilder builder, INamedTypeSymbol symbol, ClassDeclarationSyntax declaration)
    {
        string tColor = GetTColor(symbol);
        builder.AppendLine($"{declaration.Modifiers} class {declaration.Identifier}{declaration.TypeParameterList}");
        builder.AppendLine("{");
        builder.AppendLine($"public new {declaration.Identifier}<TResult> ConvertTo<TResult>(Func<{tColor}, TResult> convert)");
        builder.AppendLine("{");
        builder.AppendLine($"return ConvertTo<TResult>(convert, new {declaration.Identifier}<TResult>());");
        builder.AppendLine("}");
        builder.AppendLine();
        builder.AppendLine($"public {declaration.Identifier}<TResult> ConvertTo<TResult>(Func<{tColor}, TResult> convert, {declaration.Identifier}<TResult> result)");
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

class SyntaxReceiver : ISyntaxReceiver
{
    public List<ClassDeclarationSyntax> Candidates { get; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax cds
            && cds.BaseList != null)
        {
            Candidates.Add(cds);
        }
    }
}
