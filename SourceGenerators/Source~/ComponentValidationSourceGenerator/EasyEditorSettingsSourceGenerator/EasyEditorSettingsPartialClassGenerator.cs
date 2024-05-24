using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace elhodel.EasyEditorSettings.SourceGenerators
{
    [Generator]
    public class EasyEditorSettingsPartialClassGenerator : ISourceGenerator
    {


        public void Initialize(GeneratorInitializationContext context)
        {

            context.RegisterForSyntaxNotifications(() => new EasyEditorSettingsSyntaxReceiver());
        }



        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxContextReceiver is EasyEditorSettingsSyntaxReceiver receiver))
                return;

            GenerateSettingsProvider(context, receiver.ValidClasses.Keys);

            foreach (var pair in receiver.ValidClasses)
            {
                GenerateClassFile(context, pair.Key, pair.Value);
            }


        }


        private void GenerateSettingsProvider(GeneratorExecutionContext context, Dictionary<INamedTypeSymbol, List<IFieldSymbol>>.KeyCollection keys)
        {

            StringBuilder classBuilder = new StringBuilder();
            classBuilder.AppendLine($"using UnityEditor;");
            classBuilder.AppendLine($"namespace elhodel.EasyEditorSettings.SourceGenerators");
            classBuilder.AppendLine("{");

            classBuilder.AppendLine($"public class CustomSettingsProviders");
            classBuilder.AppendLine("{");

            foreach (INamedTypeSymbol classSymbol in keys)
            {

                AttributeData attribute = classSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass.Name == Constants.EasyEditorSettingsAttribute);

                string menuPath = attribute.ConstructorArguments[1].Value.ToString();
                string scope = (int)attribute.ConstructorArguments[2].Value == 0 ? "SettingsScope.User" : "SettingsScope.Project";
                classBuilder.AppendLine("[SettingsProvider]");
                classBuilder.AppendLine($"public static SettingsProvider Create{classSymbol.Name}SettingsProvider()");
                classBuilder.AppendLine("{");
                classBuilder.AppendLine($@"return new {Constants.ScriptableSingletonSettingsProvider}(new SerializedObject({classSymbol.ToDisplayString()}.instance), () => {classSymbol.ToDisplayString()}.instance.Save(), ""{menuPath}"", {scope});
");
                classBuilder.AppendLine("}");
            }


            classBuilder.AppendLine("}");
            classBuilder.AppendLine("}");

            context.AddSource($"CustomSettingsProviders_g.cs", SourceText.From(classBuilder.ToString(), Encoding.UTF8));

        }


        private void GenerateClassFile(GeneratorExecutionContext context, INamedTypeSymbol classSymbol, List<IFieldSymbol> fieldSymbols)
        {
            StringBuilder classBuilder = new StringBuilder();
            var namespaceSymbol = classSymbol.ContainingNamespace;

          
            AttributeData attribute = classSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass.Name == Constants.EasyEditorSettingsAttribute);

            classBuilder.AppendLine($"using UnityEngine;");
            classBuilder.AppendLine($"using UnityEditor;");
            classBuilder.AppendLine($"using System;");

            if (!(namespaceSymbol == null || namespaceSymbol.IsGlobalNamespace))
            {
                classBuilder.AppendLine($"namespace {namespaceSymbol.ToDisplayString()}");
                classBuilder.AppendLine("{");
            }


            classBuilder.AppendLine($"[FilePath(\"{attribute.ConstructorArguments[0].Value}\", ({attribute.ConstructorArguments[2].Type}){attribute.ConstructorArguments[2].Value})]\r\n");
            classBuilder.AppendLine($"public partial class {classSymbol.Name}");
            classBuilder.AppendLine("{");


            foreach (var field in fieldSymbols)
            {
                string prettyFieldName = NormalizePropertyName(field.Name);
                classBuilder.AppendLine($"public {field.Type.ToDisplayString(NullableFlowState.None)} {prettyFieldName}");
                classBuilder.AppendLine("{");
                classBuilder.AppendLine($"get=> {field.Name};");
                classBuilder.AppendLine($"set");
                classBuilder.AppendLine("{");
                classBuilder.AppendLine($"if({field.Name} != value) ");
                classBuilder.AppendLine("{");
                classBuilder.AppendLine($"{field.Name} = value;");
                classBuilder.AppendLine($"Save();");
                classBuilder.AppendLine("}");
                classBuilder.AppendLine("}");
                classBuilder.AppendLine("}");
            }

            classBuilder.AppendLine("public void Save()");
            classBuilder.AppendLine("{");
            classBuilder.AppendLine("Save(true);");
            classBuilder.AppendLine("}");


            classBuilder.AppendLine("}");
            if (!(namespaceSymbol == null || namespaceSymbol.IsGlobalNamespace))
            {
                classBuilder.AppendLine("}");
            }
            context.AddSource($"{classSymbol.Name}_g.cs", SourceText.From(classBuilder.ToString(), Encoding.UTF8));


        }
        private string NormalizePropertyName(string fieldName)
        {
            return Regex.Replace(fieldName, "_[a-z]", delegate (Match m)
            {
                return m.ToString().TrimStart('_').ToUpper();
            });
        }


    }
}


