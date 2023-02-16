using System.Text;

namespace Simple.TextTemplates.Extensions
{
    public static class StringTemplateExt
    {
        public static StringBuilder ReplaceTags(this string me, TagLookup lookup)
        {
            StringBuilder target = new StringBuilder();
            TextTemplate.Compile(new StringTextSource(me))
                .ReplaceTags(target, lookup);
            return target;
        }
    }
}
