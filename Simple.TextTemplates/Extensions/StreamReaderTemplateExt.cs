using System.Text;
using System.IO;

namespace Simple.TextTemplates.Extensions
{
    public static class StreamReaderTemplateExt
    {
        public static ITextSource GetTextSource(this StreamReader me)
        {
            return new StringTextSource(me.ReadToEnd());
        }
        public static StringBuilder ReplaceTags(this StreamReader me, TagLookup lookup)
        {
            StringBuilder target = new StringBuilder();
            TextTemplate.Compile(new StringTextSource(me.ReadToEnd()))
                .ReplaceTags(target, lookup);
            return target;
        }
        public static TextTemplate CompileTextTemplate(this StreamReader me)
        {
            return TextTemplate.Compile(me.GetTextSource());
        }
    }
}
