using System;
using System.Collections.Generic;
using System.Text;
using OSECore.Text;

namespace OSECoreExample
{
    internal static class TextSamples
    {
        internal static void Run()
        {
            Console.WriteLine();
            Console.WriteLine("Text examples");
            Console.WriteLine();
            UnicodeSupportExample();
            TextUtilitiesExample();
            TextParserExample();
        }

        private static void TextUtilitiesExample()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            List<char> chars = new List<char>();
            List<string> strings = new List<string>();
            for (int i = 0; i < 65536; ++i)
            {
                char c = Convert.ToChar(i);
                if (TextSupport.IsCharacterEntity(c))
                {
                    chars.Add(c);
                    strings.Add(TextSupport.PutCharacterEntity(c));
                }
            }
            Console.WriteLine(chars.Count + " total character entities:");
            string s = new string(chars.ToArray());
            int j = 0;
            do
            {
                int l = Math.Min(20, s.Length - j);
                Console.WriteLine("\t" + s.Substring(j, l));
                j += 20;
            } while (j < s.Length);

            Console.WriteLine();
            for (int k = 0; k < chars.Count; ++k)
            {
                Console.WriteLine("\t" + chars[k] + " = " + strings[k]);
            }
        }

        private static void TextParserExample()
        {
            string line = "A=1.23E6;";
            int i0 = 0;
            i0 = TextParser.ParseName(line, i0, out string name);
            i0 = TextParser.ParseOperator(line, i0, out string op, new string[] {"="});
            i0 = TextParser.ParseReal(line, i0, out double v);
            i0 = TextParser.ParseDelimiter(line, i0, out string delimiter);
            Console.WriteLine("Line \"" + line + "\" parses to name = " + name + ", operator = '" + op + "', value = " +
                              v + ", delimiter = '" + delimiter + "'.");

        }

        internal static void UnicodeSupportExample()
        {
            string normalText = "1234567890-+";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string superscriptText = UnicodeSupport.GetSuperscriptText(normalText);
            Console.WriteLine("Superscript version of '" + normalText + "' = '" + superscriptText + "'");
            // Result: "Superscript version of '1234567890-+' = '¹²³⁴⁵⁶⁷⁸⁹⁰⁻⁺'"

            Console.WriteLine("Superscript version of x2 = " + UnicodeSupport.GetSuperscriptText("x2"));
            // Result: "Superscript version of x2 = x²"

            string subscriptText = UnicodeSupport.GetSubscriptText(normalText);
            Console.WriteLine("Subscript version of '" + normalText + "' = '" + subscriptText + "'");
            // Result: "Subscript version of '1234567890-+' = '₁₂₃₄₅₆₇₈₉₀₋₊'"

            Console.WriteLine("Subscript version of H2O = " + UnicodeSupport.GetSubscriptText("H2O"));
            // Result: "Subscript version of H2O = H₂O"
        }

    }
}
