using System.Collections.Generic;

namespace UnityFlow.DocumentationHelper.Library.Models
{

    public class DocumentationStructure
    {
        public string AssemblyName;
        public string ClassName;
        public List<DocumentationDescription> Descriptions;

        public DocumentationStructure(string assemblyName, string className)
        {
            AssemblyName = assemblyName;
            ClassName = className;
            Descriptions = new List<DocumentationDescription>();
        }

        public void AddDescription(DocumentationDescription description)
        {
            Descriptions.Add(description);
        }
    }

    public class DocumentationDescription
    {
        public string Title;
        public string Description;
        public string CodeExample = string.Empty;

        public DocumentationDescription(string title, string description, string[]? args, string codeExample)
        {
            Title = title;
            Description = string.Format(description, args);
            CodeExample = codeExample;
        }
    }
}