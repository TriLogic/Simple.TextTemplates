using System.Text;
using System.IO;

namespace Simple.TextTemplates.Extensions
{
    public static class StreamTemplateExt
    {
        public static StringBuilder ReplaceTags(this Stream me, TagLookup lookup)
        {
            return new StreamReader(me).ReplaceTags(lookup);
        }
    }
}
