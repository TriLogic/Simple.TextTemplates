using System.Text;
using System.IO;

namespace Simple.TextTemplates.Extensions
{
    public static class TextReaderTemplateExt
    {
        public static StringBuilder ReplaceTags(this TextReader me, TagLookup lookup)
        {
            StringBuilder target = new StringBuilder();
            TextTemplate.Compile(new StringTextSource(me.ReadToEnd()))
                .ReplaceTags(target, lookup);
            return target;
        }
    }
}
