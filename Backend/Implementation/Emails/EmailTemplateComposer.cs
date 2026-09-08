using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace Implementation.Emails
{
    public class EmailTemplateComposer
    {
        public string GetTemplateContent(EmailTemplate template, object o)
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(dir, "Emails", "templates");

            var templateFile = template.ToString().ToLower() + ".html";

            filePath = Path.Combine(filePath, templateFile);

            var html = File.ReadAllText(filePath);

            var compiledTemplate = Handlebars.Compile(html);

            return compiledTemplate(o);
        }
    }
}
