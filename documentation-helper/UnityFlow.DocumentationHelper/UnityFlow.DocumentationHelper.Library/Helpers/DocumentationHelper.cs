using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityFlow.DocumentationHelper.Library.Attributes;
using UnityFlow.DocumentationHelper.Library.Models;

namespace UnityFlow.DocumentationHelper.Library.Helpers
{

    public static class DocumentationHelperTool
    {
        [Documentation("GenerateDocumentation (bool generateForPackageAssembly)",
            @"Generates a List of objects of type {0} that contain the following fields:<br />
<b>AssemblyName</b>: Name of the main Assembly, used to identify the root namespace<br />
<b>ClassName</b>: Name of the class, used to identify the upper level object<br />
<b>Title</b>: Title what we're generating documentation for<br />
<b>Description</b>: Description of what we're generating documentation for, this can contain usage examples and can use the args array to pass names(e.g.: This method uses this methodology)<br>
<br>
Note: If generateForPackageAssembly is set to true, this will generate documentation for the library as well.<br>",
            new[] { nameof(DocumentationStructure) })]
        public static IEnumerable<DocumentationStructure> GenerateDocumentation(bool generateForPackageAssembly = false)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Select(a =>
                    new DocumentationAssembly()
                    {
                        AssemblyName = a.GetName(false)?.Name ?? nameof(a),
                        Types = GetTypesWithDocumentationAttribute(a)
                    }).Where(x => x.Types.Any()).ToList();

            if (!generateForPackageAssembly)
            {
                assemblies =
                    assemblies.Where(x => x.AssemblyName != typeof(DocumentationHelperTool).Assembly.GetName().Name)
                        .ToList();
            }

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.Types)
                {
                    var doc = GetDocumentation(type, assembly.AssemblyName);
                    yield return doc;
                }
            }
        }

        static DocumentationStructure GetDocumentation(Type type, string assemblyName)
        {
            var classDocs = type.GetCustomAttributes(typeof(DocumentationAttribute), true)
                .Select(x => (DocumentationAttribute)x);
            var fieldDocs = type.GetFields().SelectMany(f =>
                f.GetCustomAttributes(typeof(DocumentationAttribute), true).Select(x => (DocumentationAttribute)x));
            var propertyDocs = type.GetProperties().SelectMany(p =>
                p.GetCustomAttributes(typeof(DocumentationAttribute), true).Select(x => (DocumentationAttribute)x));
            var methodDocs = type.GetMethods().SelectMany(m =>
                m.GetCustomAttributes(typeof(DocumentationAttribute), true).Select(x => (DocumentationAttribute)x));

            var docStructure = new DocumentationStructure(assemblyName, type.Name);

            foreach (var doc in classDocs)
            {
                docStructure.AddDescription(new DocumentationDescription(doc.Title, doc.Description, doc.Args));
            }

            foreach (var doc in fieldDocs)
            {
                docStructure.AddDescription(new DocumentationDescription(doc.Title, doc.Description, doc.Args));
            }

            foreach (var doc in propertyDocs)
            {
                docStructure.AddDescription(new DocumentationDescription(doc.Title, doc.Description, doc.Args));
            }

            foreach (var doc in methodDocs)
            {
                docStructure.AddDescription(new DocumentationDescription(doc.Title, doc.Description, doc.Args));
            }

            return docStructure;
        }

        static IEnumerable<Type> GetTypesWithDocumentationAttribute(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                var classHasDoc = type.GetCustomAttributes(typeof(DocumentationAttribute), true).Any();
                var fieldsHaveDoc = type.GetFields()
                    .Any(f => f.GetCustomAttributes(typeof(DocumentationAttribute), true).Any());
                var typesHaveDoc = type.GetProperties()
                    .Any(p => p.GetCustomAttributes(typeof(DocumentationAttribute), true).Any());
                var methodsHaveDoc = type.GetMethods()
                    .Any(m => m.GetCustomAttributes(typeof(DocumentationAttribute), true).Any());

                if (classHasDoc || fieldsHaveDoc || typesHaveDoc || methodsHaveDoc)
                {
                    yield return type;
                }
            }
        }
    }
}