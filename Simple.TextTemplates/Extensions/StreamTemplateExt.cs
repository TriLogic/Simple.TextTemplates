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
        public static StringBuilder ReplaceTags(this Stream me, TagLookup lookup)
        {
            return new StreamReader(me).ReplaceTags(lookup);
        }

        public static TextTemplate CompileTemplate(this Stream me)
        {
            return TextTemplate.Compile(me.GetTextSource());
        }
    }
}
