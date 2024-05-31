using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace elhodel.EasyEditorSettings.SourceGenerators
{


    internal class EasyEditorSettingsSyntaxReceiver : ISyntaxContextReceiver
    {
        public Dictionary<INamedTypeSymbol, List<IFieldSymbol>> ValidClasses { get; } = new Dictionary<INamedTypeSymbol, List<IFieldSymbol>>();
        public List<string> DebugOutput = new List<string>();
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax classDeclarationSyntax)
            {
                if (HasAttribute(classDeclarationSyntax, Constants.EasyEditorSettings))//   HasInterface(classDeclarationSyntax, Constants.IValidatable))
                {
                    DebugOutput.Add("Im in!");
                    DebugOutput.Add($"Class: {classDeclarationSyntax.Identifier.Text}");
                    var typeNodeSymbol = context.SemanticModel.Compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree).GetDeclaredSymbol(classDeclarationSyntax) as INamedTypeSymbol;

                    var potentialFields = typeNodeSymbol.GetMembers().OfType<IFieldSymbol>().ToList();//  Where(s => s.Kind == SymbolKind.Field).ToList();


                    foreach (var potentialField in potentialFields)
                    {
                        DebugOutput.Add($"potentialField: {potentialField.Name.ToString()}");

                        foreach(var attr in potentialField.GetAttributes())
                        {
                            DebugOutput.Add($"Attribute Name {attr.AttributeClass.Name} DisplayString: {attr.AttributeClass.ToDisplayString()}");
                        }


                        if (potentialField.GetAttributes().Any(a => a.AttributeClass.Name == Constants.SerializeField))
                        {
                            if (!ValidClasses.ContainsKey(typeNodeSymbol))
                            {
                                ValidClasses.Add(typeNodeSymbol, new List<IFieldSymbol>());
                            }

                            ValidClasses[typeNodeSymbol].Add(potentialField as IFieldSymbol);
                        }
                    }

                }
            }
        }

        private bool HasAttribute(ClassDeclarationSyntax classDeclarationSyntax, string easyEditorSettingsAttribute)
        {
            var hasAttribute = classDeclarationSyntax.AttributeLists
                .SelectMany(attrList => attrList.Attributes);

            foreach (var attribute in hasAttribute)
            {
                DebugOutput.Add(attribute.Name.ToString());
            }
            return hasAttribute.Any(attr => attr.Name.ToString() == easyEditorSettingsAttribute);



            return classDeclarationSyntax.AttributeLists.Any(a => a.Attributes.Any(b => b.Name.ToString() == easyEditorSettingsAttribute));
        }

        /// <summary>Indicates whether or not the class has a specific interface.</summary>
        /// <returns>Whether or not the SyntaxList contains the attribute.</returns>
        public static bool HasInterface(ClassDeclarationSyntax source, string interfaceName)
        {
            IEnumerable<BaseTypeSyntax> baseTypes = source.BaseList.Types.Select(baseType => baseType);

            // To Do - cleaner interface finding.
            return baseTypes.Any(baseType => baseType.ToString() == interfaceName);
        }


        private bool IsDerivedFrom(INamedTypeSymbol baseType, string targetType)
        {
            while (baseType != null)
            {
                if (baseType.Name == targetType)
                    return true;

                baseType = baseType.BaseType;
            }

            return false;
        }
    }
}
