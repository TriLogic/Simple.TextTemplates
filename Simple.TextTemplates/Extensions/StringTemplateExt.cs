using System;
using System.Text;

namespace Simple.TextTemplates.Extensions
{
    public static class StringTemplateExt
    {
        public static ITextSource GetTextSource(this string me)
        {
            return new StringTextSource(me);
        }
        public static StringBuilder ReplaceTags(this string me, TagLookup lookup)
        {
            StringBuilder target = new StringBuilder();
            me.CompileTextTemplate()
                .ReplaceTags(target, lookup);
            return target;
        }

        public static TextTemplate CompileTextTemplate(this string me) 
        {
            return TextTemplate.Compile(me.GetTextSource());
        }
    }
}
