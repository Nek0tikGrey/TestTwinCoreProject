using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Linq;
using TestTwinCoreProject.Models;

namespace TestTwinCoreProject.TagHelpers
{
    public class FileTagHelper: TagHelper
    {
        public Guid Guid { get; set; }
        private readonly TwinCoreDbContext context;
        public FileTagHelper(TwinCoreDbContext context)
        {
            this.context = context;
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "post-content-img");
            output.TagMode = TagMode.StartTagAndEndTag;
            string content = "";
            foreach (var t in this.context.Files.Where(p => p.Guid == Guid))
            {
                content+=$"<img src=\"{t.Path}\" />";
            }
            output.Content.SetHtmlContent(content);
        }
    }
}
