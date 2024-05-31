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
        /// <summary>
        /// A map of all classes with the <see cref="Constants.EasyEditorSettingsAttribute"/> and the fields for which Properties should be created
        /// </summary>
        public Dictionary<INamedTypeSymbol, List<IFieldSymbol>> ValidClasses { get; } = new Dictionary<INamedTypeSymbol, List<IFieldSymbol>>();
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax classDeclarationSyntax)
            {
                if (HasAttribute(classDeclarationSyntax, Constants.EasyEditorSettings))
                {

                    INamedTypeSymbol typeNodeSymbol = context.SemanticModel.Compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree).GetDeclaredSymbol(classDeclarationSyntax) as INamedTypeSymbol;

                    List<IFieldSymbol> potentialFields = typeNodeSymbol.GetMembers().OfType<IFieldSymbol>().ToList();


                    foreach (IFieldSymbol potentialField in potentialFields)
                    {

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
            var allAttributes = classDeclarationSyntax.AttributeLists.SelectMany(attrList => attrList.Attributes);

            return allAttributes.Any(attr => attr.Name.ToString() == easyEditorSettingsAttribute);

        }


    }
}
