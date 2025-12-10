using System.Text;
using System.IO;

namespace Simple.TextTemplates.Extensions
{
    public static class StreamTemplateExt
    {
        public static ITextSource GetTextSource(this Stream me)
        {
            return new StreamReader(me)
                .GetTextSource();
        }
        public static StringBuilder ReplaceTags(this Stream me, TagLookup lookup, TagStyle style)
        {
            return new StreamReader(me).ReplaceTags(lookup, style);
        }

        public static TextTemplate CompileTemplate(this Stream me, TagStyle style)
        {
            return TextTemplate.Compile(me.GetTextSource(), style);
        }
    }
}
