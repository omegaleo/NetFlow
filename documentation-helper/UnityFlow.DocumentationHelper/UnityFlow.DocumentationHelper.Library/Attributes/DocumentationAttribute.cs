using System;

namespace UnityFlow.DocumentationHelper.Library.Attributes
{
    [System.AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class DocumentationAttribute : Attribute
    {
        public string Title;
        public string Description;
        public string[]? Args;
        public string CodeExample = string.Empty;

        public DocumentationAttribute(string title, string description, string[]? args = null, string codeExample = "")
        {
            Title = title;
            Description = description;
            Args = args;
            CodeExample = codeExample;
        }
    }
}