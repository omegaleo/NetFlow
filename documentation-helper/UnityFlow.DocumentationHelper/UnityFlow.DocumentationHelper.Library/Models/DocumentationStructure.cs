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

        public DocumentationDescription(string title, string description, string[] args)
        {
            Title = title;
            Description = string.Format(description, args);
        }
    }
}