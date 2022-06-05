using System.Text.RegularExpressions;
using System.Xml;

namespace Game {
    public static class XmlWriterExtensions {
        public static void SafeWriteAttributeString(this XmlWriter writer, string name, string value) {
            writer.WriteAttributeString(name, ReplaceHexadecimalSymbols(value));
        }

        // This is taken from https://stackoverflow.com/questions/21053138/c-sharp-hexadecimal-value-0x12-is-an-invalid-character
        private static string ReplaceHexadecimalSymbols(string txt) {
            const string regex = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";
            return Regex.Replace(txt, regex, "", RegexOptions.Compiled);
        }
    }
}