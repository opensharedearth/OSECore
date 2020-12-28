using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECore.Text
{
    /// <summary>
    /// This class contains static methods that implement basic methods for handling text.
    /// </summary>
    public static class TextSupport
    {

        /// <summary>   The default tab size. This is the number of space
        ///             characters in a tab character.  The default is 8.</summary>
        public const int DefaultTabSize = 8;
        /// <summary>
        /// Remove comment from line of text.
        /// </summary>
        /// <param name="line">The line of text potentially contains the comment.</param>
        /// <param name="comment">The character that delimits the comment on the left.</param>
        /// <param name="allowInline">If true, a comment may exist on a line with other elements.  if false, then only white space
        /// is allowed on the line with the comment.</param>
        /// <returns>The text of text with the comment removed.  Any trailing white space is also removed.</returns>
        /// <remarks>The term comment as used here implies a sequence of text on a line that is delimited on the left by a specific
        /// character and on the right by the end of line.</remarks>
        public static string RemoveComment(string line, char comment, bool allowInline = true)
        {
            if (allowInline)
            {
                int pos = line.IndexOf(comment);
                if (pos >= 0)
                {
                    return line.Substring(0, pos).TrimEnd();
                }
                else
                {
                    return line.TrimEnd();
                }
            }
            else
            {
                foreach (var c in line)
                {
                    if (!char.IsWhiteSpace(c))
                    {
                        if (c == comment)
                        {
                            return "";
                        }
                        else
                        {
                            return line;
                        }
                    }
                }

                return "";
            }
        }
        /// <summary>
        /// Convert character entities in the form &amp;aaa; to the special characters they represent, where aaa is the name of the special character.
        /// </summary>
        /// <param name="line">The line containing character entities.</param>
        /// <returns>The line with character entities converted into the special characters they represent.</returns>
        public static string EvaluateCharacterEntities(string line)
        {
            int p = line.IndexOf('&');
            if (p >= 0)
            {
                StringBuilder sb = new StringBuilder();
                int i = 0;
                int p0 = p;
                while (p >= 0)
                {
                    sb.Append(line.Substring(i, p - i));
                    int q = line.IndexOf(';', p + 1);
                    if (q >= 0)
                    {
                        char c = GetCharacterEntity(line.Substring(p, q - p + 1));
                        if (c != '\0')
                        {
                            sb.Append(c);
                            i = q + 1;
                        }
                        else
                        {
                            sb.Append(line[p]);
                            i = p + 1;
                        }
                        p = line.IndexOf('&', i);
                    }
                }
                sb.Append(line.Substring(i));
                return sb.ToString();
            }
            return line;
        }
        /// <summary>
        /// Convert a line of text to a tagged block.
        /// </summary>
        /// <param name="tag">The tag to use.  This is in the form &lt;tag&gt;</param>
        /// <param name="line">The line to put in the tagged block.</param>
        /// <param name="startColumn">Starting column for any additional lines that are required</param>
        /// <param name="endColumn">Ending column for tagged block.  If the block extend beyond this
        ///                         column, the text is wrapped onto additional lines.</param>
        /// <returns>The tagged block</returns>
        /// <remarks>A tagged block is a series of text surrounded by a start tag and an end tag.  The start tag is in the form &lt;aaa&gt;, where aaa is the name of the tag.
        /// The end tag is in the form &lt;/aaa&gt;</remarks>
        public static string PutTaggedBlock(string tag, string line, int startColumn = 8, int endColumn = 80)
        {
            StringBuilder sb = new StringBuilder();
            if (IsTag(tag))
            {
                sb.Append(tag);
                sb.Append(line);
                sb.Append(GetEndTag(tag));
                return GetWrappedText(sb.ToString(), startColumn, endColumn);
            }
            else
            {
                return line;
            }
        }
        /// <summary>
        /// Format provided text into a series of lines that start and end in specified columns.
        /// </summary>
        /// <param name="text">The text to be formatted.</param>
        /// <param name="colstart">The 1-based start column.</param>
        /// <param name="colend">The 1-basd end column.</param>
        /// <returns>The text formatted in to a block of text starting and ending in the specified columns.</returns>
        public static string GetWrappedText(string text, int colstart, int colend)
        {
            if (colstart <= 0 || colstart >= colend)
            {
                return text;
            }
            else if (!String.IsNullOrEmpty(text))
            {
                string[] words = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                int loff = colstart - 1;
                int llength = colend - colstart;
                StringBuilder sb = new StringBuilder();
                int p = 0;
                foreach (string word in words)
                {
                    if (p < loff)
                    {
                        sb.Append(new string(' ', loff - p));
                        p = loff;
                    }
                    if (p == loff)
                    {
                        sb.Append(word);
                        p += word.Length;
                    }
                    else if (p + 1 + word.Length <= llength)
                    {
                        sb.Append(' ' + word);
                        p += word.Length + 1;
                    }
                    else
                    {
                        sb.AppendLine();
                        sb.Append(new string(' ', loff));
                        sb.Append(word);
                        p = loff + word.Length;
                    }
                }
                sb.AppendLine();
                return sb.ToString();
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// Find and extract a tagged block from a line and optionally reading
        /// a stream to find the end tag.
        /// </summary>
        /// <param name="line">The initial line of text containing the tag.</param>
        /// <param name="sr">An optional text stream to read to find the end tag if it is not contained in the initial line.</param>
        /// <returns>The contents of the tagged block.  The tags are removed.</returns>
        /// <remarks>In many cases the tagged block will</remarks>
        /// <exception cref="ApplicationException">If the end tag is not found in the provided stream then
        ///                                        an application exception is thrown.</exception>
        public static string GetTaggedBlock(string line, StreamReader sr = null)
        {
            StringBuilder sb = new StringBuilder();
            if (line != null)
            {
                string tag = GetTag(line);
                int p = FindTag(line, tag);
                if (p >= 0)
                {
                    p += tag.Length;
                    int q = FindEndTag(line, tag);
                    if (q >= p)
                    {
                        return line.Substring(p, q - p);
                    }
                    else
                    {
                        sb.Append(line.Substring(p).Trim());
                        if (sr != null)
                        {
                            string line1 = sr.ReadLine();
                            while (line1 != null)
                            {
                                line1 = line1.Trim();
                                q = FindEndTag(line1, tag);
                                if (q == 0)
                                {
                                    return sb.ToString();
                                }
                                else if (q > 0)
                                {
                                    sb.Append(" " + line1.Substring(0, q));
                                    return sb.ToString();
                                }
                                else
                                {
                                    sb.Append(" " + line1);
                                }
                                line1 = sr.ReadLine();
                            }
                            throw new ApplicationException("Found end of stream while searching for ending tag for " + tag + ".");
                        }
                        else
                        {
                            return "";
                        }
                    }
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Convert special characters to their character entity equivalents.
        /// </summary>
        /// <param name="s">The text string containing the special characters</param>
        /// <returns>The text string with the special characters converted to character entities.</returns>
        public static string PutCharacterEntities(string s)
        {
            StringBuilder sb = new StringBuilder();
            int p = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (IsCharacterEntity(s[i]))
                {
                    if (i > p)
                    {
                        sb.Append(s.Substring(p, i - p));
                    }
                    sb.Append(PutCharacterEntity(s[i]));
                    p = i + 1;
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Convert a single character entity into the special character it represents.
        /// </summary>
        /// <param name="entity">The character entity.  The first character must be '&amp;' and the last character must be ';'.</param>
        /// <returns>the special character.</returns>
        public static char GetCharacterEntity(string entity)
        {
            if (entity != null && entity.Length > 2 && entity.First() == '&' && entity.Last() == ';')
            {
                switch (entity.Substring(1, entity.Length - 2))
                {
                    case "quot": return '\x0022';   // (34) HTML 2.0 HTMLspecial ISOnum quotation mark (APL quote)
                    case "amp": return '\x0026';    // (38) HTML 2.0 HTMLspecial ISOnum ampersand
                    case "apos": return '\x0027';   // (39) XHTML 1.0 HTMLspecial ISOnum apostrophe (apostrophe-quote); see below
                    case "lt": return '\x003C'; // (60) HTML 2.0 HTMLspecial ISOnum less-than sign
                    case "gt": return '\x003E'; // (62) HTML 2.0 HTMLspecial ISOnum greater-than sign
                    case "nbsp": return '\x00A0';    // (160) HTML 3.2 HTMLlat1 ISOnum no-break space (non-breaking space)[d]
                    case "iexcl": return '\x00A1';  // (161) HTML 3.2 HTMLlat1 ISOnum inverted exclamation mark
                    case "cent": return '\x00A2';   // (162) HTML 3.2 HTMLlat1 ISOnum cent sign
                    case "pound": return '\x00A3';  // (163) HTML 3.2 HTMLlat1 ISOnum pound sign
                    case "curren": return '\x00A4'; // (164) HTML 3.2 HTMLlat1 ISOnum currency sign
                    case "yen": return '\x00A5';    // (165) HTML 3.2 HTMLlat1 ISOnum yen sign (yuan sign)
                    case "brvbar": return '\x00A6'; // (166) HTML 3.2 HTMLlat1 ISOnum broken bar (broken vertical bar)
                    case "sect": return '\x00A7';   // (167) HTML 3.2 HTMLlat1 ISOnum section sign
                    case "uml": return '\x00A8';    // (168) HTML 3.2 HTMLlat1 ISOdia diaeresis (spacing diaeresis); see Germanic umlaut
                    case "copy": return '\x00A9';   // (169) HTML 3.2 HTMLlat1 ISOnum copyright symbol
                    case "ordf": return '\x00AA';   // (170) HTML 3.2 HTMLlat1 ISOnum feminine ordinal indicator
                    case "laquo": return '\x00AB';  // (171) HTML 3.2 HTMLlat1 ISOnum left-pointing double angle quotation mark (left pointing guillemet)
                    case "not": return '\x00AC';    // (172) HTML 3.2 HTMLlat1 ISOnum not sign
                    case "shy": return '\x00AD';    // (173) HTML 3.2 HTMLlat1 ISOnum soft hyphen (discretionary hyphen)
                    case "reg": return '\x00AE';    // (174) HTML 3.2 HTMLlat1 ISOnum registered sign (registered trademark symbol)
                    case "macr": return '\x00AF';   // (175) HTML 3.2 HTMLlat1 ISOdia macron (spacing macron, overline, APL overbar)
                    case "deg": return '\x00B0';    // (176) HTML 3.2 HTMLlat1 ISOnum degree symbol
                    case "plusmn": return '\x00B1'; // (177) HTML 3.2 HTMLlat1 ISOnum plus-minus sign (plus-or-minus sign)
                    case "sup2": return '\x00B2';   // (178) HTML 3.2 HTMLlat1 ISOnum superscript two (superscript digit two, squared)
                    case "sup3": return '\x00B3';   // (179) HTML 3.2 HTMLlat1 ISOnum superscript three (superscript digit three, cubed)
                    case "acute": return '\x00B4';  // (180) HTML 3.2 HTMLlat1 ISOdia acute accent (spacing acute)
                    case "micro": return '\x00B5';  // (181) HTML 3.2 HTMLlat1 ISOnum micro sign
                    case "para": return '\x00B6';   // (182) HTML 3.2 HTMLlat1 ISOnum pilcrow sign (paragraph sign)
                    case "middot": return '\x00B7'; // (183) HTML 3.2 HTMLlat1 ISOnum middle dot (Georgian comma, Greek middle dot)
                    case "cedil": return '\x00B8';  // (184) HTML 3.2 HTMLlat1 ISOdia cedilla (spacing cedilla)
                    case "sup1": return '\x00B9';   // (185) HTML 3.2 HTMLlat1 ISOnum superscript one (superscript digit one)
                    case "ordm": return '\x00BA';   // (186) HTML 3.2 HTMLlat1 ISOnum masculine ordinal indicator
                    case "raquo": return '\x00BB';  // (187) HTML 3.2 HTMLlat1 ISOnum right-pointing double angle quotation mark (right pointing guillemet)
                    case "frac14": return '\x00BC'; // (188) HTML 3.2 HTMLlat1 ISOnum vulgar fraction one quarter (fraction one quarter)
                    case "frac12": return '\x00BD'; // (189) HTML 3.2 HTMLlat1 ISOnum vulgar fraction one half (fraction one half)
                    case "frac34": return '\x00BE'; // (190) HTML 3.2 HTMLlat1 ISOnum vulgar fraction three quarters (fraction three quarters)
                    case "iquest": return '\x00BF'; // (191) HTML 3.2 HTMLlat1 ISOnum inverted question mark (turned question mark)
                    case "Agrave": return '\x00C0'; // (192) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter A with grave accent (Latin capital letter A grave)
                    case "Aacute": return '\x00C1'; // (193) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter A with acute accent
                    case "Acirc": return '\x00C2';  // (194) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter A with circumflex
                    case "Atilde": return '\x00C3'; // (195) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter A with tilde
                    case "Auml": return '\x00C4';   // (196) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter A with diaeresis
                    case "Aring": return '\x00C5';  // (197) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter A with ring above (Latin capital letter A ring)
                    case "AElig": return '\x00C6';  // (198) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter AE (Latin capital ligature AE)
                    case "Ccedil": return '\x00C7'; // (199) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter C with cedilla
                    case "Egrave": return '\x00C8'; // (200) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter E with grave accent
                    case "Eacute": return '\x00C9'; // (201) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter E with acute accent
                    case "Ecirc": return '\x00CA';  // (202) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter E with circumflex
                    case "Euml": return '\x00CB';   // (203) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter E with diaeresis
                    case "Igrave": return '\x00CC'; // (204) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter I with grave accent
                    case "Iacute": return '\x00CD'; // (205) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter I with acute accent
                    case "Icirc": return '\x00CE';  // (206) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter I with circumflex
                    case "Iuml": return '\x00CF';   // (207) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter I with diaeresis
                    case "ETH": return '\x00D0';    // (208) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter Eth
                    case "Ntilde": return '\x00D1'; // (209) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter N with tilde
                    case "Ograve": return '\x00D2'; // (210) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter O with grave accent
                    case "Oacute": return '\x00D3'; // (211) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter O with acute accent
                    case "Ocirc": return '\x00D4';  // (212) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter O with circumflex
                    case "Otilde": return '\x00D5'; // (213) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter O with tilde
                    case "Ouml": return '\x00D6';   // (214) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter O with diaeresis
                    case "times": return '\x00D7';  // (215) HTML 3.2 HTMLlat1 ISOnum multiplication sign
                    case "Oslash": return '\x00D8'; // (216) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter O with stroke (Latin capital letter O slash)
                    case "Ugrave": return '\x00D9'; // (217) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter U with grave accent
                    case "Uacute": return '\x00DA'; // (218) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter U with acute accent
                    case "Ucirc": return '\x00DB';  // (219) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter U with circumflex
                    case "Uuml": return '\x00DC';   // (220) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter U with diaeresis
                    case "Yacute": return '\x00DD'; // (221) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter Y with acute accent
                    case "THORN": return '\x00DE';  // (222) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter THORN
                    case "szlig": return '\x00DF';  // (223) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter sharp s (ess-zed); see German Eszett
                    case "agrave": return '\x00E0'; // (224) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter a with grave accent
                    case "aacute": return '\x00E1'; // (225) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter a with acute accent
                    case "acirc": return '\x00E2';  // (226) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter a with circumflex
                    case "atilde": return '\x00E3'; // (227) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter a with tilde
                    case "auml": return '\x00E4';   // (228) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter a with diaeresis
                    case "aring": return '\x00E5';  // (229) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter a with ring above
                    case "aelig": return '\x00E6';  // (230) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter ae (Latin small ligature ae)
                    case "ccedil": return '\x00E7'; // (231) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter c with cedilla
                    case "egrave": return '\x00E8'; // (232) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter e with grave accent
                    case "eacute": return '\x00E9'; // (233) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter e with acute accent
                    case "ecirc": return '\x00EA';  // (234) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter e with circumflex
                    case "euml": return '\x00EB';   // (235) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter e with diaeresis
                    case "igrave": return '\x00EC'; // (236) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter i with grave accent
                    case "iacute": return '\x00ED'; // (237) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter i with acute accent
                    case "icirc": return '\x00EE';  // (238) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter i with circumflex
                    case "iuml": return '\x00EF';   // (239) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter i with diaeresis
                    case "eth": return '\x00F0';    // (240) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter eth
                    case "ntilde": return '\x00F1'; // (241) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter n with tilde
                    case "ograve": return '\x00F2'; // (242) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter o with grave accent
                    case "oacute": return '\x00F3'; // (243) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter o with acute accent
                    case "ocirc": return '\x00F4';  // (244) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter o with circumflex
                    case "otilde": return '\x00F5'; // (245) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter o with tilde
                    case "ouml": return '\x00F6';   // (246) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter o with diaeresis
                    case "divide": return '\x00F7'; // (247) HTML 3.2 HTMLlat1 ISOnum division sign (obelus)
                    case "oslash": return '\x00F8'; // (248) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter o with stroke (Latin small letter o slash)
                    case "ugrave": return '\x00F9'; // (249) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter u with grave accent
                    case "uacute": return '\x00FA'; // (250) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter u with acute accent
                    case "ucirc": return '\x00FB';  // (251) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter u with circumflex
                    case "uuml": return '\x00FC';   // (252) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter u with diaeresis
                    case "yacute": return '\x00FD'; // (253) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter y with acute accent
                    case "thorn": return '\x00FE';  // (254) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter thorn
                    case "yuml": return '\x00FF';   // (255) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter y with diaeresis
                    case "OElig": return '\x0152';  // (338) HTML 4.0 HTMLspecial ISOlat2 Latin capital ligature oe[e]
                    case "oelig": return '\x0153';  // (339) HTML 4.0 HTMLspecial ISOlat2 Latin small ligature oe[e]
                    case "Scaron": return '\x0160'; // (352) HTML 4.0 HTMLspecial ISOlat2 Latin capital letter s with caron
                    case "scaron": return '\x0161'; // (353) HTML 4.0 HTMLspecial ISOlat2 Latin small letter s with caron
                    case "Yuml": return '\x0178';   // (376) HTML 4.0 HTMLspecial ISOlat2 Latin capital letter y with diaeresis
                    case "fnof": return '\x0192';   // (402) HTML 4.0 HTMLsymbol ISOtech Latin small letter f with hook (function, florin)
                    case "circ": return '\x02C6';   // (710) HTML 4.0 HTMLspecial ISOpub modifier letter circumflex accent
                    case "tilde": return '\x02DC';  // (732) HTML 4.0 HTMLspecial ISOdia small tilde
                    case "Alpha": return '\x0391';  // (913) HTML 4.0 HTMLsymbol  Greek capital letter Alpha
                    case "Beta": return '\x0392';   // (914) HTML 4.0 HTMLsymbol  Greek capital letter Beta
                    case "Gamma": return '\x0393';  // (915) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Gamma
                    case "Delta": return '\x0394';  // (916) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Delta
                    case "Epsilon": return '\x0395';    // (917) HTML 4.0 HTMLsymbol  Greek capital letter Epsilon
                    case "Zeta": return '\x0396';   // (918) HTML 4.0 HTMLsymbol  Greek capital letter Zeta
                    case "Eta": return '\x0397';    // (919) HTML 4.0 HTMLsymbol  Greek capital letter Eta
                    case "Theta": return '\x0398';  // (920) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Theta
                    case "Iota": return '\x0399';   // (921) HTML 4.0 HTMLsymbol  Greek capital letter Iota
                    case "Kappa": return '\x039A';  // (922) HTML 4.0 HTMLsymbol  Greek capital letter Kappa
                    case "Lambda": return '\x039B'; // (923) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Lambda
                    case "Mu": return '\x039C'; // (924) HTML 4.0 HTMLsymbol  Greek capital letter Mu
                    case "Nu": return '\x039D'; // (925) HTML 4.0 HTMLsymbol  Greek capital letter Nu
                    case "Xi": return '\x039E'; // (926) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Xi
                    case "Omicron": return '\x039F';    // (927) HTML 4.0 HTMLsymbol  Greek capital letter Omicron
                    case "Pi": return '\x03A0'; // (928) HTML 4.0 HTMLsymbol  Greek capital letter Pi
                    case "Rho": return '\x03A1';    // (929) HTML 4.0 HTMLsymbol  Greek capital letter Rho
                    case "Sigma": return '\x03A3';  // (931) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Sigma
                    case "Tau": return '\x03A4';    // (932) HTML 4.0 HTMLsymbol  Greek capital letter Tau
                    case "Upsilon": return '\x03A5';    // (933) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Upsilon
                    case "Phi": return '\x03A6';    // (934) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Phi
                    case "Chi": return '\x03A7';    // (935) HTML 4.0 HTMLsymbol  Greek capital letter Chi
                    case "Psi": return '\x03A8';    // (936) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Psi
                    case "Omega": return '\x03A9';  // (937) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Omega
                    case "alpha": return '\x03B1';  // (945) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter alpha
                    case "beta": return '\x03B2';   // (946) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter beta
                    case "gamma": return '\x03B3';  // (947) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter gamma
                    case "delta": return '\x03B4';  // (948) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter delta
                    case "epsilon": return '\x03B5';    // (949) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter epsilon
                    case "zeta": return '\x03B6';   // (950) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter zeta
                    case "eta": return '\x03B7';    // (951) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter eta
                    case "theta": return '\x03B8';  // (952) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter theta
                    case "iota": return '\x03B9';   // (953) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter iota
                    case "kappa": return '\x03BA';  // (954) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter kappa
                    case "lambda": return '\x03BB'; // (955) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter lambda
                    case "mu": return '\x03BC'; // (956) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter mu
                    case "nu": return '\x03BD'; // (957) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter nu
                    case "xi": return '\x03BE'; // (958) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter xi
                    case "omicron": return '\x03BF';    // (959) HTML 4.0 HTMLsymbol NEW Greek small letter omicron
                    case "pi": return '\x03C0'; // (960) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter pi
                    case "rho": return '\x03C1';    // (961) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter rho
                    case "sigmaf": return '\x03C2'; // (962) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter final sigma
                    case "sigma": return '\x03C3';  // (963) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter sigma
                    case "tau": return '\x03C4';    // (964) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter tau
                    case "upsilon": return '\x03C5';    // (965) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter upsilon
                    case "phi": return '\x03C6';    // (966) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter phi
                    case "chi": return '\x03C7';    // (967) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter chi
                    case "psi": return '\x03C8';    // (968) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter psi
                    case "omega": return '\x03C9';  // (969) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter omega
                    case "thetasym": return '\x03D1';   // (977) HTML 4.0 HTMLsymbol NEW Greek theta symbol
                    case "upsih": return '\x03D2';  // (978) HTML 4.0 HTMLsymbol NEW Greek Upsilon with hook symbol
                    case "piv": return '\x03D6';    // (982) HTML 4.0 HTMLsymbol ISOgrk3 Greek pi symbol
                    case "ensp": return '\x2002';    // (8194) HTML 4.0 HTMLspecial ISOpub en space[d]
                    case "emsp": return '\x2003';    // (8195) HTML 4.0 HTMLspecial ISOpub em space[d]
                    case "thinsp": return '\x2009';    // (8201) HTML 4.0 HTMLspecial ISOpub thin space[d]
                    case "zwnj": return '\x200C';    // (8204) HTML 4.0 HTMLspecial NEW RFC 2070 zero-width non-joiner
                    case "zwj": return '\x200D';    // (8205) HTML 4.0 HTMLspecial NEW RFC 2070 zero-width joiner
                    case "lrm": return '\x200E';    // (8206) HTML 4.0 HTMLspecial NEW RFC 2070 left-to-right mark
                    case "rlm": return '\x200F';    // (8207) HTML 4.0 HTMLspecial NEW RFC 2070 right-to-left mark
                    case "ndash": return '\x2013';  // (8211) HTML 4.0 HTMLspecial ISOpub en dash
                    case "mdash": return '\x2014';  // (8212) HTML 4.0 HTMLspecial ISOpub em dash
                    case "lsquo": return '\x2018';  // (8216) HTML 4.0 HTMLspecial ISOnum left single quotation mark
                    case "rsquo": return '\x2019';  // (8217) HTML 4.0 HTMLspecial ISOnum right single quotation mark
                    case "sbquo": return '\x201A';  // (8218) HTML 4.0 HTMLspecial NEW single low-9 quotation mark
                    case "ldquo": return '\x201C';  // (8220) HTML 4.0 HTMLspecial ISOnum left double quotation mark
                    case "rdquo": return '\x201D';  // (8221) HTML 4.0 HTMLspecial ISOnum right double quotation mark
                    case "bdquo": return '\x201E';  // (8222) HTML 4.0 HTMLspecial NEW double low-9 quotation mark
                    case "dagger": return '\x2020'; // (8224) HTML 4.0 HTMLspecial ISOpub dagger, obelisk
                    case "Dagger": return '\x2021'; // (8225) HTML 4.0 HTMLspecial ISOpub double dagger, double obelisk
                    case "bull": return '\x2022';   // (8226) HTML 4.0 HTMLspecial ISOpub bullet (black small circle)[f]
                    case "hellip": return '\x2026'; // (8230) HTML 4.0 HTMLsymbol ISOpub horizontal ellipsis (three dot leader)
                    case "permil": return '\x2030'; // (8240) HTML 4.0 HTMLspecial ISOtech per mille sign
                    case "prime": return '\x2032';  // (8242) HTML 4.0 HTMLsymbol ISOtech prime (minutes, feet)
                    case "Prime": return '\x2033';  // (8243) HTML 4.0 HTMLsymbol ISOtech double prime (seconds, inches)
                    case "lsaquo": return '\x2039'; // (8249) HTML 4.0 HTMLspecial ISO proposed single left-pointing angle quotation mark[g]
                    case "rsaquo": return '\x203A'; // (8250) HTML 4.0 HTMLspecial ISO proposed single right-pointing angle quotation mark[g]
                    case "oline": return '\x203E';  // (8254) HTML 4.0 HTMLsymbol NEW overline (spacing overscore)
                    case "frasl": return '\x2044';  // (8260) HTML 4.0 HTMLsymbol NEW fraction slash (solidus)
                    case "euro": return '\x20AC';   // (8364) HTML 4.0 HTMLspecial NEW euro sign
                    case "image": return '\x2111';  // (8465) HTML 4.0 HTMLsymbol ISOamso black-letter capital I (imaginary part)
                    case "weierp": return '\x2118'; // (8472) HTML 4.0 HTMLsymbol ISOamso script capital P (power set, Weierstrass p)
                    case "real": return '\x211C';   // (8476) HTML 4.0 HTMLsymbol ISOamso black-letter capital R (real part symbol)
                    case "trade": return '\x2122';  // (8482) HTML 4.0 HTMLsymbol ISOnum trademark symbol
                    case "alefsym": return '\x2135';    // (8501) HTML 4.0 HTMLsymbol NEW alef symbol (first transfinite cardinal)[h]
                    case "larr": return '\x2190';   // (8592) HTML 4.0 HTMLsymbol ISOnum leftwards arrow
                    case "uarr": return '\x2191';   // (8593) HTML 4.0 HTMLsymbol ISOnum upwards arrow
                    case "rarr": return '\x2192';   // (8594) HTML 4.0 HTMLsymbol ISOnum rightwards arrow
                    case "darr": return '\x2193';   // (8595) HTML 4.0 HTMLsymbol ISOnum downwards arrow
                    case "harr": return '\x2194';   // (8596) HTML 4.0 HTMLsymbol ISOamsa left right arrow
                    case "crarr": return '\x21B5';  // (8629) HTML 4.0 HTMLsymbol NEW downwards arrow with corner leftwards (carriage return)
                    case "lArr": return '\x21D0';   // (8656) HTML 4.0 HTMLsymbol ISOtech leftwards double arrow[i]
                    case "uArr": return '\x21D1';   // (8657) HTML 4.0 HTMLsymbol ISOamsa upwards double arrow
                    case "rArr": return '\x21D2';   // (8658) HTML 4.0 HTMLsymbol ISOnum rightwards double arrow[j]
                    case "dArr": return '\x21D3';   // (8659) HTML 4.0 HTMLsymbol ISOamsa downwards double arrow
                    case "hArr": return '\x21D4';   // (8660) HTML 4.0 HTMLsymbol ISOamsa left right double arrow
                    case "forall": return '\x2200'; // (8704) HTML 4.0 HTMLsymbol ISOtech for all
                    case "part": return '\x2202';   // (8706) HTML 4.0 HTMLsymbol ISOtech partial differential
                    case "exist": return '\x2203';  // (8707) HTML 4.0 HTMLsymbol ISOtech there exists
                    case "empty": return '\x2205';  // (8709) HTML 4.0 HTMLsymbol ISOamso empty set (null set); see also U+8960, ⌀
                    case "nabla": return '\x2207';  // (8711) HTML 4.0 HTMLsymbol ISOtech del or nabla (vector differential operator)
                    case "isin": return '\x2208';   // (8712) HTML 4.0 HTMLsymbol ISOtech element of
                    case "notin": return '\x2209';  // (8713) HTML 4.0 HTMLsymbol ISOtech not an element of
                    case "ni": return '\x220B'; // (8715) HTML 4.0 HTMLsymbol ISOtech contains as member
                    case "prod": return '\x220F';   // (8719) HTML 4.0 HTMLsymbol ISOamsb n-ary product (product sign)[k]
                    case "sum": return '\x2211';    // (8721) HTML 4.0 HTMLsymbol ISOamsb n-ary summation[l]
                    case "minus": return '\x2212';  // (8722) HTML 4.0 HTMLsymbol ISOtech minus sign
                    case "lowast": return '\x2217'; // (8727) HTML 4.0 HTMLsymbol ISOtech asterisk operator
                    case "radic": return '\x221A';  // (8730) HTML 4.0 HTMLsymbol ISOtech square root (radical sign)
                    case "prop": return '\x221D';   // (8733) HTML 4.0 HTMLsymbol ISOtech proportional to
                    case "infin": return '\x221E';  // (8734) HTML 4.0 HTMLsymbol ISOtech infinity
                    case "ang": return '\x2220';    // (8736) HTML 4.0 HTMLsymbol ISOamso angle
                    case "and": return '\x2227';    // (8743) HTML 4.0 HTMLsymbol ISOtech logical and (wedge)
                    case "or": return '\x2228'; // (8744) HTML 4.0 HTMLsymbol ISOtech logical or (vee)
                    case "cap": return '\x2229';    // (8745) HTML 4.0 HTMLsymbol ISOtech intersection (cap)
                    case "cup": return '\x222A';    // (8746) HTML 4.0 HTMLsymbol ISOtech union (cup)
                    case "int": return '\x222B';    // (8747) HTML 4.0 HTMLsymbol ISOtech integral
                    case "there4": return '\x2234'; // (8756) HTML 4.0 HTMLsymbol ISOtech therefore sign
                    case "sim": return '\x223C';    // (8764) HTML 4.0 HTMLsymbol ISOtech tilde operator (varies with, similar to)[m]
                    case "cong": return '\x2245';   // (8773) HTML 4.0 HTMLsymbol ISOtech congruent to
                    case "asymp": return '\x2248';  // (8776) HTML 4.0 HTMLsymbol ISOamsr almost equal to (asymptotic to)
                    case "ne": return '\x2260'; // (8800) HTML 4.0 HTMLsymbol ISOtech not equal to
                    case "equiv": return '\x2261';  // (8801) HTML 4.0 HTMLsymbol ISOtech identical to; sometimes used for 'equivalent to'
                    case "le": return '\x2264'; // (8804) HTML 4.0 HTMLsymbol ISOtech less-than or equal to
                    case "ge": return '\x2265'; // (8805) HTML 4.0 HTMLsymbol ISOtech greater-than or equal to
                    case "sub": return '\x2282';    // (8834) HTML 4.0 HTMLsymbol ISOtech subset of
                    case "sup": return '\x2283';    // (8835) HTML 4.0 HTMLsymbol ISOtech superset of[n]
                    case "nsub": return '\x2284';   // (8836) HTML 4.0 HTMLsymbol ISOamsn not a subset of
                    case "sube": return '\x2286';   // (8838) HTML 4.0 HTMLsymbol ISOtech subset of or equal to
                    case "supe": return '\x2287';   // (8839) HTML 4.0 HTMLsymbol ISOtech superset of or equal to
                    case "oplus": return '\x2295';  // (8853) HTML 4.0 HTMLsymbol ISOamsb circled plus (direct sum)
                    case "otimes": return '\x2297'; // (8855) HTML 4.0 HTMLsymbol ISOamsb circled times (vector product)
                    case "perp": return '\x22A5';   // (8869) HTML 4.0 HTMLsymbol ISOtech up tack (orthogonal to, perpendicular)[o]
                    case "sdot": return '\x22C5';   // (8901) HTML 4.0 HTMLsymbol ISOamsb dot operator[p]
                    case "lceil": return '\x2308';  // (8968) HTML 4.0 HTMLsymbol ISOamsc left ceiling (APL upstile)
                    case "rceil": return '\x2309';  // (8969) HTML 4.0 HTMLsymbol ISOamsc right ceiling
                    case "lfloor": return '\x230A'; // (8970) HTML 4.0 HTMLsymbol ISOamsc left floor (APL downstile)
                    case "rfloor": return '\x230B'; // (8971) HTML 4.0 HTMLsymbol ISOamsc right floor
                    case "lang": return '\x2329';   // (9001) HTML 4.0 HTMLsymbol ISOtech left-pointing angle bracket (bra)[q]
                    case "rang": return '\x232A';   // (9002) HTML 4.0 HTMLsymbol ISOtech right-pointing angle bracket (ket)[r]
                    case "loz": return '\x25CA';    // (9674) HTML 4.0 HTMLsymbol ISOpub lozenge
                    case "spades": return '\x2660'; // (9824) HTML 4.0 HTMLsymbol ISOpub black spade suit[f]
                    case "clubs": return '\x2663';  // (9827) HTML 4.0 HTMLsymbol ISOpub black club suit (shamrock)[f]
                    case "hearts": return '\x2665'; // (9829) HTML 4.0 HTMLsymbol ISOpub black heart suit (valentine)[f]
                    case "diams": return '\x2666'; // (9830) HTML 4.0 HTMLsymbol ISOpub black diamond suit[f]
                }
            }
            return '\0';
        }
        /// <summary>
        /// Convert a special character into its equivalent character entity.
        /// </summary>
        /// <param name="entity">The special character</param>
        /// <returns>The string representing the character entity. The first character will be '&amp;' and the last character will be ';'</returns>
        public static string PutCharacterEntity(char entity)
        {
                switch (entity)
                {
                case '\x0022': return "&quot;";   // (34) HTML 2.0 HTMLspecial ISOnum quotation mark (APL quote)
                case '\x0026': return "&amp;";    // (38) HTML 2.0 HTMLspecial ISOnum ampersand
                case '\x0027': return "&apos;";   // (39) XHTML 1.0 HTMLspecial ISOnum apostrophe (apostrophe-quote); see below
                case '\x003C': return "&lt;"; // (60) HTML 2.0 HTMLspecial ISOnum less-than sign
                case '\x003E': return "&gt;"; // (62) HTML 2.0 HTMLspecial ISOnum greater-than sign
                case '\x00A0': return "&nbsp;";    // (160) HTML 3.2 HTMLlat1 ISOnum no-break space (non-breaking space)[d]
                case '\x00A1': return "&iexcl;";  // (161) HTML 3.2 HTMLlat1 ISOnum inverted exclamation mark
                case '\x00A2': return "&cent;";   // (162) HTML 3.2 HTMLlat1 ISOnum cent sign
                case '\x00A3': return "&pound;";  // (163) HTML 3.2 HTMLlat1 ISOnum pound sign
                case '\x00A4': return "&curren;"; // (164) HTML 3.2 HTMLlat1 ISOnum currency sign
                case '\x00A5': return "&yen;";    // (165) HTML 3.2 HTMLlat1 ISOnum yen sign (yuan sign)
                case '\x00A6': return "&brvbar;"; // (166) HTML 3.2 HTMLlat1 ISOnum broken bar (broken vertical bar)
                case '\x00A7': return "&sect;";   // (167) HTML 3.2 HTMLlat1 ISOnum section sign
                case '\x00A8': return "&uml;";    // (168) HTML 3.2 HTMLlat1 ISOdia diaeresis (spacing diaeresis); see Germanic umlaut
                case '\x00A9': return "&copy;";   // (169) HTML 3.2 HTMLlat1 ISOnum copyright symbol
                case '\x00AA': return "&ordf;";   // (170) HTML 3.2 HTMLlat1 ISOnum feminine ordinal indicator
                case '\x00AB': return "&laquo;";  // (171) HTML 3.2 HTMLlat1 ISOnum left-pointing double angle quotation mark (left pointing guillemet)
                case '\x00AC': return "&not;";    // (172) HTML 3.2 HTMLlat1 ISOnum not sign
                case '\x00AD': return "&shy;";    // (173) HTML 3.2 HTMLlat1 ISOnum soft hyphen (discretionary hyphen)
                case '\x00AE': return "&reg;";    // (174) HTML 3.2 HTMLlat1 ISOnum registered sign (registered trademark symbol)
                case '\x00AF': return "&macr;";   // (175) HTML 3.2 HTMLlat1 ISOdia macron (spacing macron, overline, APL overbar)
                case '\x00B0': return "&deg;";    // (176) HTML 3.2 HTMLlat1 ISOnum degree symbol
                case '\x00B1': return "&plusmn;"; // (177) HTML 3.2 HTMLlat1 ISOnum plus-minus sign (plus-or-minus sign)
                case '\x00B2': return "&sup2;";   // (178) HTML 3.2 HTMLlat1 ISOnum superscript two (superscript digit two, squared)
                case '\x00B3': return "&sup3;";   // (179) HTML 3.2 HTMLlat1 ISOnum superscript three (superscript digit three, cubed)
                case '\x00B4': return "&acute;";  // (180) HTML 3.2 HTMLlat1 ISOdia acute accent (spacing acute)
                case '\x00B5': return "&micro;";  // (181) HTML 3.2 HTMLlat1 ISOnum micro sign
                case '\x00B6': return "&para;";   // (182) HTML 3.2 HTMLlat1 ISOnum pilcrow sign (paragraph sign)
                case '\x00B7': return "&middot;"; // (183) HTML 3.2 HTMLlat1 ISOnum middle dot (Georgian comma, Greek middle dot)
                case '\x00B8': return "&cedil;";  // (184) HTML 3.2 HTMLlat1 ISOdia cedilla (spacing cedilla)
                case '\x00B9': return "&sup1;";   // (185) HTML 3.2 HTMLlat1 ISOnum superscript one (superscript digit one)
                case '\x00BA': return "&ordm;";   // (186) HTML 3.2 HTMLlat1 ISOnum masculine ordinal indicator
                case '\x00BB': return "&raquo;";  // (187) HTML 3.2 HTMLlat1 ISOnum right-pointing double angle quotation mark (right pointing guillemet)
                case '\x00BC': return "&frac14;"; // (188) HTML 3.2 HTMLlat1 ISOnum vulgar fraction one quarter (fraction one quarter)
                case '\x00BD': return "&frac12;"; // (189) HTML 3.2 HTMLlat1 ISOnum vulgar fraction one half (fraction one half)
                case '\x00BE': return "&frac34;"; // (190) HTML 3.2 HTMLlat1 ISOnum vulgar fraction three quarters (fraction three quarters)
                case '\x00BF': return "&iquest;"; // (191) HTML 3.2 HTMLlat1 ISOnum inverted question mark (turned question mark)
                case '\x00C0': return "&Agrave;"; // (192) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter A with grave accent (Latin capital letter A grave)
                case '\x00C1': return "&Aacute;"; // (193) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter A with acute accent
                case '\x00C2': return "&Acirc;";  // (194) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter A with circumflex
                case '\x00C3': return "&Atilde;"; // (195) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter A with tilde
                case '\x00C4': return "&Auml;";   // (196) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter A with diaeresis
                case '\x00C5': return "&Aring;";  // (197) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter A with ring above (Latin capital letter A ring)
                case '\x00C6': return "&AElig;";  // (198) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter AE (Latin capital ligature AE)
                case '\x00C7': return "&Ccedil;"; // (199) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter C with cedilla
                case '\x00C8': return "&Egrave;"; // (200) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter E with grave accent
                case '\x00C9': return "&Eacute;"; // (201) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter E with acute accent
                case '\x00CA': return "&Ecirc;";  // (202) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter E with circumflex
                case '\x00CB': return "&Euml;";   // (203) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter E with diaeresis
                case '\x00CC': return "&Igrave;"; // (204) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter I with grave accent
                case '\x00CD': return "&Iacute;"; // (205) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter I with acute accent
                case '\x00CE': return "&Icirc;";  // (206) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter I with circumflex
                case '\x00CF': return "&Iuml;";   // (207) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter I with diaeresis
                case '\x00D0': return "&ETH;";    // (208) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter Eth
                case '\x00D1': return "&Ntilde;"; // (209) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter N with tilde
                case '\x00D2': return "&Ograve;"; // (210) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter O with grave accent
                case '\x00D3': return "&Oacute;"; // (211) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter O with acute accent
                case '\x00D4': return "&Ocirc;";  // (212) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter O with circumflex
                case '\x00D5': return "&Otilde;"; // (213) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter O with tilde
                case '\x00D6': return "&Ouml;";   // (214) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter O with diaeresis
                case '\x00D7': return "&times;";  // (215) HTML 3.2 HTMLlat1 ISOnum multiplication sign
                case '\x00D8': return "&Oslash;"; // (216) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter O with stroke (Latin capital letter O slash)
                case '\x00D9': return "&Ugrave;"; // (217) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter U with grave accent
                case '\x00DA': return "&Uacute;"; // (218) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter U with acute accent
                case '\x00DB': return "&Ucirc;";  // (219) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter U with circumflex
                case '\x00DC': return "&Uuml;";   // (220) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter U with diaeresis
                case '\x00DD': return "&Yacute;"; // (221) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter Y with acute accent
                case '\x00DE': return "&THORN;";  // (222) HTML 2.0 HTMLlat1 ISOlat1 Latin capital letter THORN
                case '\x00DF': return "&szlig;";  // (223) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter sharp s (ess-zed); see German Eszett
                case '\x00E0': return "&agrave;"; // (224) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter a with grave accent
                case '\x00E1': return "&aacute;"; // (225) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter a with acute accent
                case '\x00E2': return "&acirc;";  // (226) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter a with circumflex
                case '\x00E3': return "&atilde;"; // (227) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter a with tilde
                case '\x00E4': return "&auml;";   // (228) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter a with diaeresis
                case '\x00E5': return "&aring;";  // (229) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter a with ring above
                case '\x00E6': return "&aelig;";  // (230) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter ae (Latin small ligature ae)
                case '\x00E7': return "&ccedil;"; // (231) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter c with cedilla
                case '\x00E8': return "&egrave;"; // (232) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter e with grave accent
                case '\x00E9': return "&eacute;"; // (233) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter e with acute accent
                case '\x00EA': return "&ecirc;";  // (234) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter e with circumflex
                case '\x00EB': return "&euml;";   // (235) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter e with diaeresis
                case '\x00EC': return "&igrave;"; // (236) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter i with grave accent
                case '\x00ED': return "&iacute;"; // (237) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter i with acute accent
                case '\x00EE': return "&icirc;";  // (238) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter i with circumflex
                case '\x00EF': return "&iuml;";   // (239) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter i with diaeresis
                case '\x00F0': return "&eth;";    // (240) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter eth
                case '\x00F1': return "&ntilde;"; // (241) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter n with tilde
                case '\x00F2': return "&ograve;"; // (242) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter o with grave accent
                case '\x00F3': return "&oacute;"; // (243) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter o with acute accent
                case '\x00F4': return "&ocirc;";  // (244) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter o with circumflex
                case '\x00F5': return "&otilde;"; // (245) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter o with tilde
                case '\x00F6': return "&ouml;";   // (246) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter o with diaeresis
                case '\x00F7': return "&divide;"; // (247) HTML 3.2 HTMLlat1 ISOnum division sign (obelus)
                case '\x00F8': return "&oslash;"; // (248) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter o with stroke (Latin small letter o slash)
                case '\x00F9': return "&ugrave;"; // (249) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter u with grave accent
                case '\x00FA': return "&uacute;"; // (250) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter u with acute accent
                case '\x00FB': return "&ucirc;";  // (251) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter u with circumflex
                case '\x00FC': return "&uuml;";   // (252) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter u with diaeresis
                case '\x00FD': return "&yacute;"; // (253) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter y with acute accent
                case '\x00FE': return "&thorn;";  // (254) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter thorn
                case '\x00FF': return "&yuml;";   // (255) HTML 2.0 HTMLlat1 ISOlat1 Latin small letter y with diaeresis
                case '\x0152': return "&OElig;";  // (338) HTML 4.0 HTMLspecial ISOlat2 Latin capital ligature oe[e]
                case '\x0153': return "&oelig;";  // (339) HTML 4.0 HTMLspecial ISOlat2 Latin small ligature oe[e]
                case '\x0160': return "&Scaron;"; // (352) HTML 4.0 HTMLspecial ISOlat2 Latin capital letter s with caron
                case '\x0161': return "&scaron;"; // (353) HTML 4.0 HTMLspecial ISOlat2 Latin small letter s with caron
                case '\x0178': return "&Yuml;";   // (376) HTML 4.0 HTMLspecial ISOlat2 Latin capital letter y with diaeresis
                case '\x0192': return "&fnof;";   // (402) HTML 4.0 HTMLsymbol ISOtech Latin small letter f with hook (function, florin)
                case '\x02C6': return "&circ;";   // (710) HTML 4.0 HTMLspecial ISOpub modifier letter circumflex accent
                case '\x02DC': return "&tilde;";  // (732) HTML 4.0 HTMLspecial ISOdia small tilde
                case '\x0391': return "&Alpha;";  // (913) HTML 4.0 HTMLsymbol  Greek capital letter Alpha
                case '\x0392': return "&Beta;";   // (914) HTML 4.0 HTMLsymbol  Greek capital letter Beta
                case '\x0393': return "&Gamma;";  // (915) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Gamma
                case '\x0394': return "&Delta;";  // (916) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Delta
                case '\x0395': return "&Epsilon;";    // (917) HTML 4.0 HTMLsymbol  Greek capital letter Epsilon
                case '\x0396': return "&Zeta;";   // (918) HTML 4.0 HTMLsymbol  Greek capital letter Zeta
                case '\x0397': return "&Eta;";    // (919) HTML 4.0 HTMLsymbol  Greek capital letter Eta
                case '\x0398': return "&Theta;";  // (920) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Theta
                case '\x0399': return "&Iota;";   // (921) HTML 4.0 HTMLsymbol  Greek capital letter Iota
                case '\x039A': return "&Kappa;";  // (922) HTML 4.0 HTMLsymbol  Greek capital letter Kappa
                case '\x039B': return "&Lambda;"; // (923) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Lambda
                case '\x039C': return "&Mu;"; // (924) HTML 4.0 HTMLsymbol  Greek capital letter Mu
                case '\x039D': return "&Nu;"; // (925) HTML 4.0 HTMLsymbol  Greek capital letter Nu
                case '\x039E': return "&Xi;"; // (926) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Xi
                case '\x039F': return "&Omicron;";    // (927) HTML 4.0 HTMLsymbol  Greek capital letter Omicron
                case '\x03A0': return "&Pi;"; // (928) HTML 4.0 HTMLsymbol  Greek capital letter Pi
                case '\x03A1': return "&Rho;";    // (929) HTML 4.0 HTMLsymbol  Greek capital letter Rho
                case '\x03A3': return "&Sigma;";  // (931) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Sigma
                case '\x03A4': return "&Tau;";    // (932) HTML 4.0 HTMLsymbol  Greek capital letter Tau
                case '\x03A5': return "&Upsilon;";    // (933) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Upsilon
                case '\x03A6': return "&Phi;";    // (934) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Phi
                case '\x03A7': return "&Chi;";    // (935) HTML 4.0 HTMLsymbol  Greek capital letter Chi
                case '\x03A8': return "&Psi;";    // (936) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Psi
                case '\x03A9': return "&Omega;";  // (937) HTML 4.0 HTMLsymbol ISOgrk3 Greek capital letter Omega
                case '\x03B1': return "&alpha;";  // (945) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter alpha
                case '\x03B2': return "&beta;";   // (946) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter beta
                case '\x03B3': return "&gamma;";  // (947) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter gamma
                case '\x03B4': return "&delta;";  // (948) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter delta
                case '\x03B5': return "&epsilon;";    // (949) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter epsilon
                case '\x03B6': return "&zeta;";   // (950) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter zeta
                case '\x03B7': return "&eta;";    // (951) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter eta
                case '\x03B8': return "&theta;";  // (952) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter theta
                case '\x03B9': return "&iota;";   // (953) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter iota
                case '\x03BA': return "&kappa;";  // (954) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter kappa
                case '\x03BB': return "&lambda;"; // (955) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter lambda
                case '\x03BC': return "&mu;"; // (956) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter mu
                case '\x03BD': return "&nu;"; // (957) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter nu
                case '\x03BE': return "&xi;"; // (958) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter xi
                case '\x03BF': return "&omicron;";    // (959) HTML 4.0 HTMLsymbol NEW Greek small letter omicron
                case '\x03C0': return "&pi;"; // (960) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter pi
                case '\x03C1': return "&rho;";    // (961) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter rho
                case '\x03C2': return "&sigmaf;"; // (962) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter final sigma
                case '\x03C3': return "&sigma;";  // (963) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter sigma
                case '\x03C4': return "&tau;";    // (964) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter tau
                case '\x03C5': return "&upsilon;";    // (965) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter upsilon
                case '\x03C6': return "&phi;";    // (966) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter phi
                case '\x03C7': return "&chi;";    // (967) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter chi
                case '\x03C8': return "&psi;";    // (968) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter psi
                case '\x03C9': return "&omega;";  // (969) HTML 4.0 HTMLsymbol ISOgrk3 Greek small letter omega
                case '\x03D1': return "&thetasym;";   // (977) HTML 4.0 HTMLsymbol NEW Greek theta symbol
                case '\x03D2': return "&upsih;";  // (978) HTML 4.0 HTMLsymbol NEW Greek Upsilon with hook symbol
                case '\x03D6': return "&piv;";    // (982) HTML 4.0 HTMLsymbol ISOgrk3 Greek pi symbol
                case '\x2002': return "&ensp;";    // (8194) HTML 4.0 HTMLspecial ISOpub en space[d]
                case '\x2003': return "&emsp;";    // (8195) HTML 4.0 HTMLspecial ISOpub em space[d]
                case '\x2009': return "&thinsp;";    // (8201) HTML 4.0 HTMLspecial ISOpub thin space[d]
                case '\x200C': return "&zwnj;";    // (8204) HTML 4.0 HTMLspecial NEW RFC 2070 zero-width non-joiner
                case '\x200D': return "&zwj;";    // (8205) HTML 4.0 HTMLspecial NEW RFC 2070 zero-width joiner
                case '\x200E': return "&lrm;";    // (8206) HTML 4.0 HTMLspecial NEW RFC 2070 left-to-right mark
                case '\x200F': return "&rlm;";    // (8207) HTML 4.0 HTMLspecial NEW RFC 2070 right-to-left mark
                case '\x2013': return "&ndash;";  // (8211) HTML 4.0 HTMLspecial ISOpub en dash
                case '\x2014': return "&mdash;";  // (8212) HTML 4.0 HTMLspecial ISOpub em dash
                case '\x2018': return "&lsquo;";  // (8216) HTML 4.0 HTMLspecial ISOnum left single quotation mark
                case '\x2019': return "&rsquo;";  // (8217) HTML 4.0 HTMLspecial ISOnum right single quotation mark
                case '\x201A': return "&sbquo;";  // (8218) HTML 4.0 HTMLspecial NEW single low-9 quotation mark
                case '\x201C': return "&ldquo;";  // (8220) HTML 4.0 HTMLspecial ISOnum left double quotation mark
                case '\x201D': return "&rdquo;";  // (8221) HTML 4.0 HTMLspecial ISOnum right double quotation mark
                case '\x201E': return "&bdquo;";  // (8222) HTML 4.0 HTMLspecial NEW double low-9 quotation mark
                case '\x2020': return "&dagger;"; // (8224) HTML 4.0 HTMLspecial ISOpub dagger, obelisk
                case '\x2021': return "&Dagger;"; // (8225) HTML 4.0 HTMLspecial ISOpub double dagger, double obelisk
                case '\x2022': return "&bull;";   // (8226) HTML 4.0 HTMLspecial ISOpub bullet (black small circle)[f]
                case '\x2026': return "&hellip;"; // (8230) HTML 4.0 HTMLsymbol ISOpub horizontal ellipsis (three dot leader)
                case '\x2030': return "&permil;"; // (8240) HTML 4.0 HTMLspecial ISOtech per mille sign
                case '\x2032': return "&prime;";  // (8242) HTML 4.0 HTMLsymbol ISOtech prime (minutes, feet)
                case '\x2033': return "&Prime;";  // (8243) HTML 4.0 HTMLsymbol ISOtech double prime (seconds, inches)
                case '\x2039': return "&lsaquo;"; // (8249) HTML 4.0 HTMLspecial ISO proposed single left-pointing angle quotation mark[g]
                case '\x203A': return "&rsaquo;"; // (8250) HTML 4.0 HTMLspecial ISO proposed single right-pointing angle quotation mark[g]
                case '\x203E': return "&oline;";  // (8254) HTML 4.0 HTMLsymbol NEW overline (spacing overscore)
                case '\x2044': return "&frasl;";  // (8260) HTML 4.0 HTMLsymbol NEW fraction slash (solidus)
                case '\x20AC': return "&euro;";   // (8364) HTML 4.0 HTMLspecial NEW euro sign
                case '\x2111': return "&image;";  // (8465) HTML 4.0 HTMLsymbol ISOamso black-letter capital I (imaginary part)
                case '\x2118': return "&weierp;"; // (8472) HTML 4.0 HTMLsymbol ISOamso script capital P (power set, Weierstrass p)
                case '\x211C': return "&real;";   // (8476) HTML 4.0 HTMLsymbol ISOamso black-letter capital R (real part symbol)
                case '\x2122': return "&trade;";  // (8482) HTML 4.0 HTMLsymbol ISOnum trademark symbol
                case '\x2135': return "&alefsym;";    // (8501) HTML 4.0 HTMLsymbol NEW alef symbol (first transfinite cardinal)[h]
                case '\x2190': return "&larr;";   // (8592) HTML 4.0 HTMLsymbol ISOnum leftwards arrow
                case '\x2191': return "&uarr;";   // (8593) HTML 4.0 HTMLsymbol ISOnum upwards arrow
                case '\x2192': return "&rarr;";   // (8594) HTML 4.0 HTMLsymbol ISOnum rightwards arrow
                case '\x2193': return "&darr;";   // (8595) HTML 4.0 HTMLsymbol ISOnum downwards arrow
                case '\x2194': return "&harr;";   // (8596) HTML 4.0 HTMLsymbol ISOamsa left right arrow
                case '\x21B5': return "&crarr;";  // (8629) HTML 4.0 HTMLsymbol NEW downwards arrow with corner leftwards (carriage return)
                case '\x21D0': return "&lArr;";   // (8656) HTML 4.0 HTMLsymbol ISOtech leftwards double arrow[i]
                case '\x21D1': return "&uArr;";   // (8657) HTML 4.0 HTMLsymbol ISOamsa upwards double arrow
                case '\x21D2': return "&rArr;";   // (8658) HTML 4.0 HTMLsymbol ISOnum rightwards double arrow[j]
                case '\x21D3': return "&dArr;";   // (8659) HTML 4.0 HTMLsymbol ISOamsa downwards double arrow
                case '\x21D4': return "&hArr;";   // (8660) HTML 4.0 HTMLsymbol ISOamsa left right double arrow
                case '\x2200': return "&forall;"; // (8704) HTML 4.0 HTMLsymbol ISOtech for all
                case '\x2202': return "&part;";   // (8706) HTML 4.0 HTMLsymbol ISOtech partial differential
                case '\x2203': return "&exist;";  // (8707) HTML 4.0 HTMLsymbol ISOtech there exists
                case '\x2205': return "&empty;";  // (8709) HTML 4.0 HTMLsymbol ISOamso empty set (null set); see also U+8960, ⌀
                case '\x2207': return "&nabla;";  // (8711) HTML 4.0 HTMLsymbol ISOtech del or nabla (vector differential operator)
                case '\x2208': return "&isin;";   // (8712) HTML 4.0 HTMLsymbol ISOtech element of
                case '\x2209': return "&notin;";  // (8713) HTML 4.0 HTMLsymbol ISOtech not an element of
                case '\x220B': return "&ni;"; // (8715) HTML 4.0 HTMLsymbol ISOtech contains as member
                case '\x220F': return "&prod;";   // (8719) HTML 4.0 HTMLsymbol ISOamsb n-ary product (product sign)[k]
                case '\x2211': return "&sum;";    // (8721) HTML 4.0 HTMLsymbol ISOamsb n-ary summation[l]
                case '\x2212': return "&minus;";  // (8722) HTML 4.0 HTMLsymbol ISOtech minus sign
                case '\x2217': return "&lowast;"; // (8727) HTML 4.0 HTMLsymbol ISOtech asterisk operator
                case '\x221A': return "&radic;";  // (8730) HTML 4.0 HTMLsymbol ISOtech square root (radical sign)
                case '\x221D': return "&prop;";   // (8733) HTML 4.0 HTMLsymbol ISOtech proportional to
                case '\x221E': return "&infin;";  // (8734) HTML 4.0 HTMLsymbol ISOtech infinity
                case '\x2220': return "&ang;";    // (8736) HTML 4.0 HTMLsymbol ISOamso angle
                case '\x2227': return "&and;";    // (8743) HTML 4.0 HTMLsymbol ISOtech logical and (wedge)
                case '\x2228': return "&or;"; // (8744) HTML 4.0 HTMLsymbol ISOtech logical or (vee)
                case '\x2229': return "&cap;";    // (8745) HTML 4.0 HTMLsymbol ISOtech intersection (cap)
                case '\x222A': return "&cup;";    // (8746) HTML 4.0 HTMLsymbol ISOtech union (cup)
                case '\x222B': return "&int;";    // (8747) HTML 4.0 HTMLsymbol ISOtech integral
                case '\x2234': return "&there4;"; // (8756) HTML 4.0 HTMLsymbol ISOtech therefore sign
                case '\x223C': return "&sim;";    // (8764) HTML 4.0 HTMLsymbol ISOtech tilde operator (varies with, similar to)[m]
                case '\x2245': return "&cong;";   // (8773) HTML 4.0 HTMLsymbol ISOtech congruent to
                case '\x2248': return "&asymp;";  // (8776) HTML 4.0 HTMLsymbol ISOamsr almost equal to (asymptotic to)
                case '\x2260': return "&ne;"; // (8800) HTML 4.0 HTMLsymbol ISOtech not equal to
                case '\x2261': return "&equiv;";  // (8801) HTML 4.0 HTMLsymbol ISOtech identical to; sometimes used for 'equivalent to'
                case '\x2264': return "&le;"; // (8804) HTML 4.0 HTMLsymbol ISOtech less-than or equal to
                case '\x2265': return "&ge;"; // (8805) HTML 4.0 HTMLsymbol ISOtech greater-than or equal to
                case '\x2282': return "&sub;";    // (8834) HTML 4.0 HTMLsymbol ISOtech subset of
                case '\x2283': return "&sup;";    // (8835) HTML 4.0 HTMLsymbol ISOtech superset of[n]
                case '\x2284': return "&nsub;";   // (8836) HTML 4.0 HTMLsymbol ISOamsn not a subset of
                case '\x2286': return "&sube;";   // (8838) HTML 4.0 HTMLsymbol ISOtech subset of or equal to
                case '\x2287': return "&supe;";   // (8839) HTML 4.0 HTMLsymbol ISOtech superset of or equal to
                case '\x2295': return "&oplus;";  // (8853) HTML 4.0 HTMLsymbol ISOamsb circled plus (direct sum)
                case '\x2297': return "&otimes;"; // (8855) HTML 4.0 HTMLsymbol ISOamsb circled times (vector product)
                case '\x22A5': return "&perp;";   // (8869) HTML 4.0 HTMLsymbol ISOtech up tack (orthogonal to, perpendicular)[o]
                case '\x22C5': return "&sdot;";   // (8901) HTML 4.0 HTMLsymbol ISOamsb dot operator[p]
                case '\x2308': return "&lceil;";  // (8968) HTML 4.0 HTMLsymbol ISOamsc left ceiling (APL upstile)
                case '\x2309': return "&rceil;";  // (8969) HTML 4.0 HTMLsymbol ISOamsc right ceiling
                case '\x230A': return "&lfloor;"; // (8970) HTML 4.0 HTMLsymbol ISOamsc left floor (APL downstile)
                case '\x230B': return "&rfloor;"; // (8971) HTML 4.0 HTMLsymbol ISOamsc right floor
                case '\x2329': return "&lang;";   // (9001) HTML 4.0 HTMLsymbol ISOtech left-pointing angle bracket (bra)[q]
                case '\x232A': return "&rang;";   // (9002) HTML 4.0 HTMLsymbol ISOtech right-pointing angle bracket (ket)[r]
                case '\x25CA': return "&loz;";    // (9674) HTML 4.0 HTMLsymbol ISOpub lozenge
                case '\x2660': return "&spades;"; // (9824) HTML 4.0 HTMLsymbol ISOpub black spade suit[f]
                case '\x2663': return "&clubs;";  // (9827) HTML 4.0 HTMLsymbol ISOpub black club suit (shamrock)[f]
                case '\x2665': return "&hearts;"; // (9829) HTML 4.0 HTMLsymbol ISOpub black heart suit (valentine)[f]
                case '\x2666': return "&diams;";    // (9830) HTML 4.0 HTMLsymbol ISOpub black diamond suit[f]
            }
            return "";
        }
        /// <summary>
        /// Determines whether the indicated special character has a character entity equivalent.
        /// </summary>
        /// <param name="c">The special character.</param>
        /// <returns>True if a character entity exists for the special character.</returns>
        public static bool IsCharacterEntity(char c)
        {
            switch (c)
            {
                case '\x0022':
                case '\x0026':
                case '\x0027':
                case '\x003C':
                case '\x003E':
                case '\x00A0':
                case '\x00A1':
                case '\x00A2':
                case '\x00A3':
                case '\x00A4':
                case '\x00A5':
                case '\x00A6':
                case '\x00A7':
                case '\x00A8':
                case '\x00A9':
                case '\x00AA':
                case '\x00AB':
                case '\x00AC':
                case '\x00AD':
                case '\x00AE':
                case '\x00AF':
                case '\x00B0':
                case '\x00B1':
                case '\x00B2':
                case '\x00B3':
                case '\x00B4':
                case '\x00B5':
                case '\x00B6':
                case '\x00B7':
                case '\x00B8':
                case '\x00B9':
                case '\x00BA':
                case '\x00BB':
                case '\x00BC':
                case '\x00BD':
                case '\x00BE':
                case '\x00BF':
                case '\x00C0':
                case '\x00C1':
                case '\x00C2':
                case '\x00C3':
                case '\x00C4':
                case '\x00C5':
                case '\x00C6':
                case '\x00C7':
                case '\x00C8':
                case '\x00C9':
                case '\x00CA':
                case '\x00CB':
                case '\x00CC':
                case '\x00CD':
                case '\x00CE':
                case '\x00CF':
                case '\x00D0':
                case '\x00D1':
                case '\x00D2':
                case '\x00D3':
                case '\x00D4':
                case '\x00D5':
                case '\x00D6':
                case '\x00D7':
                case '\x00D8':
                case '\x00D9':
                case '\x00DA':
                case '\x00DB':
                case '\x00DC':
                case '\x00DD':
                case '\x00DE':
                case '\x00DF':
                case '\x00E0':
                case '\x00E1':
                case '\x00E2':
                case '\x00E3':
                case '\x00E4':
                case '\x00E5':
                case '\x00E6':
                case '\x00E7':
                case '\x00E8':
                case '\x00E9':
                case '\x00EA':
                case '\x00EB':
                case '\x00EC':
                case '\x00ED':
                case '\x00EE':
                case '\x00EF':
                case '\x00F0':
                case '\x00F1':
                case '\x00F2':
                case '\x00F3':
                case '\x00F4':
                case '\x00F5':
                case '\x00F6':
                case '\x00F7':
                case '\x00F8':
                case '\x00F9':
                case '\x00FA':
                case '\x00FB':
                case '\x00FC':
                case '\x00FD':
                case '\x00FE':
                case '\x00FF':
                case '\x0152':
                case '\x0153':
                case '\x0160':
                case '\x0161':
                case '\x0178':
                case '\x0192':
                case '\x02C6':
                case '\x02DC':
                case '\x0391':
                case '\x0392':
                case '\x0393':
                case '\x0394':
                case '\x0395':
                case '\x0396':
                case '\x0397':
                case '\x0398':
                case '\x0399':
                case '\x039A':
                case '\x039B':
                case '\x039C':
                case '\x039D':
                case '\x039E':
                case '\x039F':
                case '\x03A0':
                case '\x03A1':
                case '\x03A3':
                case '\x03A4':
                case '\x03A5':
                case '\x03A6':
                case '\x03A7':
                case '\x03A8':
                case '\x03A9':
                case '\x03B1':
                case '\x03B2':
                case '\x03B3':
                case '\x03B4':
                case '\x03B5':
                case '\x03B6':
                case '\x03B7':
                case '\x03B8':
                case '\x03B9':
                case '\x03BA':
                case '\x03BB':
                case '\x03BC':
                case '\x03BD':
                case '\x03BE':
                case '\x03BF':
                case '\x03C0':
                case '\x03C1':
                case '\x03C2':
                case '\x03C3':
                case '\x03C4':
                case '\x03C5':
                case '\x03C6':
                case '\x03C7':
                case '\x03C8':
                case '\x03C9':
                case '\x03D1':
                case '\x03D2':
                case '\x03D6':
                case '\x2002':
                case '\x2003':
                case '\x2009':
                case '\x200C':
                case '\x200D':
                case '\x200E':
                case '\x200F':
                case '\x2013':
                case '\x2014':
                case '\x2018':
                case '\x2019':
                case '\x201A':
                case '\x201C':
                case '\x201D':
                case '\x201E':
                case '\x2020':
                case '\x2021':
                case '\x2022':
                case '\x2026':
                case '\x2030':
                case '\x2032':
                case '\x2033':
                case '\x2039':
                case '\x203A':
                case '\x203E':
                case '\x2044':
                case '\x20AC':
                case '\x2111':
                case '\x2118':
                case '\x211C':
                case '\x2122':
                case '\x2135':
                case '\x2190':
                case '\x2191':
                case '\x2192':
                case '\x2193':
                case '\x2194':
                case '\x21B5':
                case '\x21D0':
                case '\x21D1':
                case '\x21D2':
                case '\x21D3':
                case '\x21D4':
                case '\x2200':
                case '\x2202':
                case '\x2203':
                case '\x2205':
                case '\x2207':
                case '\x2208':
                case '\x2209':
                case '\x220B':
                case '\x220F':
                case '\x2211':
                case '\x2212':
                case '\x2217':
                case '\x221A':
                case '\x221D':
                case '\x221E':
                case '\x2220':
                case '\x2227':
                case '\x2228':
                case '\x2229':
                case '\x222A':
                case '\x222B':
                case '\x2234':
                case '\x223C':
                case '\x2245':
                case '\x2248':
                case '\x2260':
                case '\x2261':
                case '\x2264':
                case '\x2265':
                case '\x2282':
                case '\x2283':
                case '\x2284':
                case '\x2286':
                case '\x2287':
                case '\x2295':
                case '\x2297':
                case '\x22A5':
                case '\x22C5':
                case '\x2308':
                case '\x2309':
                case '\x230A':
                case '\x230B':
                case '\x2329':
                case '\x232A':
                case '\x25CA':
                case '\x2660':
                case '\x2663':
                case '\x2665':
                case '\x2666':
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Options for text utilities.  This is a bitfield.
        /// </summary>
        [Flags]
        public enum Options
        {
            /// <summary>
            /// No option.
            /// </summary>
            None = 0,
            /// <summary>
            /// Trim white space.
            /// </summary>
            TrimWhiteSpace = 1,
            /// <summary>
            /// Evaluate character entities.
            /// </summary>
            EvaluateCharacterEntities = 2
        };
        enum Markdown { Normal, Sub, Sup };
        enum MarkdownTag { None, StartSub, EndSub, StartSup, EndSup };
        /// <summary>
        /// Convert text containing markdown elements into text where the recognized markdowns are implemented.
        /// </summary>
        /// <param name="line">The line of text containing markdown elements.</param>
        /// <param name="options">The options to use when interpreting the markdown.</param>
        /// <returns></returns>
        public static string InterpretMarkdown(string line, Options options = Options.None)
        {
            if ((options & Options.TrimWhiteSpace) == Options.TrimWhiteSpace)
            {
                line = line.Trim();
            }
            if ((options & Options.EvaluateCharacterEntities) == Options.EvaluateCharacterEntities)
            {
                line = EvaluateCharacterEntities(line);
            }
            Markdown stage = Markdown.Normal;
            List<string> elements = new List<string>();
            int p = line.IndexOf('<');
            if (p >= 0)
            {
                StringBuilder sb = new StringBuilder();
                int i = 0;
                int p0 = p;
                while (p >= 0)
                {
                    sb.Append(InterpretMarkdown(line.Substring(i, p - i), stage));
                    int q = line.IndexOf('>', p + 1);
                    if (q >= 0)
                    {
                        string tag = line.Substring(p, q - p + 1);
                        switch (GetMarkdownTag(tag))
                        {
                            case MarkdownTag.StartSub:
                                if (stage == Markdown.Normal)
                                {
                                    stage = Markdown.Sub;
                                }
                                else
                                {
                                    sb.Append(tag);
                                }
                                i = q + 1;
                                break;
                            case MarkdownTag.StartSup:
                                if (stage == Markdown.Normal)
                                {
                                    stage = Markdown.Sup;
                                }
                                else
                                {
                                    sb.Append(tag);
                                }
                                i = q + 1;
                                break;
                            case MarkdownTag.EndSub:
                                if (stage != Markdown.Sub)
                                {
                                    sb.Append(line.Substring(p0, q - p0 + 1));
                                }
                                stage = Markdown.Normal;
                                i = q + 1;
                                break;
                            case MarkdownTag.EndSup:
                                if (stage != Markdown.Sup)
                                {
                                    sb.Append(line.Substring(p0, q - p0 + 1));
                                }
                                stage = Markdown.Normal;
                                i = q + 1;
                                break;
                            default:
                                sb.Append(line.Substring(p, q - p + 1));
                                i = q + 1;
                                break;

                        }
                    }
                    else
                    {
                        i = p + 1;
                    }
                    p0 = p;
                    p = line.IndexOf('<', i);
                }
                if (i < line.Length)
                {
                    sb.Append(line.Substring(i));
                }
                return sb.ToString();
            }
            else
            {
                return line;
            }
        }
        private static MarkdownTag GetMarkdownTag(string s)
        {
            switch (s)
            {
                case "<sub>": return MarkdownTag.StartSub;
                case "</sub>": return MarkdownTag.EndSub;
                case "<sup>": return MarkdownTag.StartSup;
                case "</sup>": return MarkdownTag.EndSup;
                default: return MarkdownTag.None;
            }
        }
        private static string GetMarkdownTag(MarkdownTag tag)
        {
            switch (tag)
            {
                case MarkdownTag.EndSub: return "</sub>";
                case MarkdownTag.EndSup: return "</sup>";
                case MarkdownTag.StartSub: return "<sub>";
                case MarkdownTag.StartSup: return "<sup>";
                default: return "";
            }
        }
        private static string InterpretMarkdown(string s, Markdown stage)
        {
            switch (stage)
            {
                case Markdown.Normal: return s;
                case Markdown.Sub: return UnicodeSupport.GetSubscriptText(s);
                case Markdown.Sup: return UnicodeSupport.GetSuperscriptText(s);
                default: return s;
            }
        }
        /// <summary>
        /// Convert text to text containing markdown elements.
        /// </summary>
        /// <param name="s">The input string</param>
        /// <returns>The input string containing markdown elements.</returns>
        public static string WriteMarkdown(string s)
        {
            Markdown stage = Markdown.Normal;
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                if (UnicodeSupport.IsSuperscriptChar(c))
                {
                    if (stage != Markdown.Sup)
                    {
                        if (stage == Markdown.Sub)
                        {
                            sb.Append(GetMarkdownTag(MarkdownTag.EndSub));
                        }
                        sb.Append(GetMarkdownTag(MarkdownTag.StartSup));
                    }
                    sb.Append(UnicodeSupport.GetNormalChar(c));
                    stage = Markdown.Sup;
                }
                else if (UnicodeSupport.IsSubscriptChar(c))
                {
                    if (stage != Markdown.Sub)
                    {
                        if (stage == Markdown.Sup)
                        {
                            sb.Append(GetMarkdownTag(MarkdownTag.EndSup));
                        }
                        sb.Append(GetMarkdownTag(MarkdownTag.StartSub));
                    }
                    sb.Append(UnicodeSupport.GetNormalChar(c));
                    stage = Markdown.Sub;
                }
                else if (stage == Markdown.Sup)
                {
                    sb.Append(GetMarkdownTag(MarkdownTag.EndSup));
                    sb.Append(c);
                    stage = Markdown.Normal;
                }
                else if (stage == Markdown.Sub)
                {
                    sb.Append(GetMarkdownTag(MarkdownTag.EndSub));
                    sb.Append(c);
                    stage = Markdown.Normal;
                }
                else
                {
                    sb.Append(c);
                }
            }
            if (stage == Markdown.Sup)
            {
                sb.Append(GetMarkdownTag(MarkdownTag.EndSup));
            }
            else if (stage == Markdown.Sub)
            {
                sb.Append(GetMarkdownTag(MarkdownTag.EndSub));
            }
            return sb.ToString();
        }
        private static string GetTag(string s)
        {
            if (s != null)
            {
                int p = s.IndexOf('<');
                if (p >= 0)
                {
                    int q = s.IndexOf('>', p + 1);
                    if (q >= 0)
                    {
                        return s.Substring(p, q - p + 1);
                    }
                }
            }
            return "";
        }
        private static int FindTag(string line, string tag)
        {
            if (IsTag(tag))
            {
                return line.IndexOf(tag);
            }
            else
            {
                return -1;
            }
        }
        private static int FindTag(string line)
        {
            string tag = GetTag(line);
            return FindTag(line, tag);
        }
        private static int FindEndTag(string line, string tag)
        {
            string endTag = GetEndTag(tag);
            return line.IndexOf(endTag);
        }

        private static string GetEndTag(string tag)
        {
            if (IsTag(tag))
            {
                if (IsEndTag(tag))
                {
                    return tag;
                }
                else
                {
                    return tag.Insert(1, "/");
                }
            }
            return "";
        }

        private static bool IsTag(string tag)
        {
            return tag != null && tag.Length > 2 && tag.First() == '<' && tag.Last() == '>';
        }
        private static bool IsEndTag(string tag)
        {
            return IsTag(tag) && tag[1] == '/';
        }
        /// <summary>
        /// Options for escaping quotes contained within quoted strings.
        /// </summary>
        public enum QuoteOptions
        {
            /// <summary>
            /// Simply ignore internal quotes.
            /// </summary>
            None,
            /// <summary>
            /// Each internal quote is doubled, thus appears as 2 sequential quote characters.
            /// </summary>
            Double,
            /// <summary>
            /// Each internal quote is escaped by inserting a backslash character ('\\') immediately in front of it.
            /// </summary>
            Escape
        }
        /// <summary>
        /// Place a field in quotes while addressing any quote characters that are internal to the string.
        /// </summary>
        /// <param name="field">The text to enclose in quotes.</param>
        /// <param name="quote">The quote character, by default '"'.</param>
        /// <param name="qo">The option which indicates how to address internal quote characters.  By default ignore them.</param>
        /// <returns>The quoted text.</returns>
        public static string Quote(string field, char quote = '"', QuoteOptions qo = QuoteOptions.None)
        {
            if (qo != QuoteOptions.None)
            {
                string insert = new string(qo == QuoteOptions.Double ? quote : '\\', 1);
                int i = field.IndexOf(quote);
                while (i >= 0)
                {
                    field = field.Insert(i++, insert);
                    if (i < field.Length - 1)
                        i = field.IndexOf(quote, i + 1);
                    else
                        break;
                }

            }
            return quote + field + quote;
        }
        /// <summary>
        /// Remove quotes from quoted strings.
        /// </summary>
        /// <param name="field">The quoted text</param>
        /// <param name="quote">The quoted character used to enclose the string, by default '"'.</param>
        /// <returns>The unquoted text.</returns>
        public static string Unquote(string field, char quote = '"', QuoteOptions qo = QuoteOptions.None)
        {
            if (field.Length >= 2 && field[0] == quote && field[field.Length - 1] == quote)
            {
                field = field.Substring(1, field.Length - 2);
                if (qo != QuoteOptions.None)
                {
                    int p = 0;
                    while ((p = field.IndexOf(quote,p)) >= 0)
                    {
                        if (qo == QuoteOptions.Double)
                        {
                            if (++p < field.Length && field[p] == quote) field = field.Remove(p, 1);
                        }
                        else if (qo == QuoteOptions.Escape)
                        {
                            if (p > 0 && field[p - 1] == '\\') field = field.Remove(p - 1, 1);
                            else ++p;
                        }
                    }

                }
            }

            return field;
        }
        /// <summary>   Remove tab characters from line.  Replaces all
        ///             tab charactres with spaces. </summary>
        ///
        /// <param name="line">     The line of text potentially containing tabs. </param>
        /// <param name="tabSize">  (Optional) Size of the tab in spaces.  By default 8. </param>
        ///
        /// <returns>   The line with tabs removed. </returns>
        public static string Detab(string line, int tabSize = DefaultTabSize)
        {
            if (!String.IsNullOrEmpty(line) && tabSize >= 2)
            {
                string[] fields = line.Split('\t');
                if (fields.Length == 1)
                {
                    return line;
                }
                StringBuilder sb = new StringBuilder();
                int p = 0;
                for (int i = 0; i < fields.Length - 1; ++i)
                {
                    string s = fields[i];
                    sb.Append(s);
                    p += s.Length;
                    sb.Append(new string(' ', tabSize - (p % tabSize)));
                }

                sb.Append(fields.Last());
                return sb.ToString();
            }
            return line;
        }
        /// <summary>   Add tabs to line.  Multiple spaces are replaced with
        ///             tab character whereever possible. </summary>
        ///
        /// <param name="line">     The line of text to which tabs are added. </param>
        /// <param name="tabSize">  (Optional) Size of the tab in spaces.  By default 8. </param>
        ///
        /// <returns>   The line with spaces replaces with tab characters. </returns>
        /// <remarks>The line is assumed to begin at column 1.  Tab stops are therefore
        ///          columns 9, 17, etc with a tab width of 8.  Tabs replace spaces at the beginning
        ///          of the line and also within the line.  There must be at least 2 consequtive spaces
        ///          for a tab to be added.</remarks>
        public static string Entab(string line, int tabSize = DefaultTabSize)
        {
            const int addTabThreadhold = 2;
            if (!String.IsNullOrEmpty(line) && tabSize >= 2)
            {
                StringBuilder sb = new StringBuilder();
                int spaces = 0;
                int tabs = 0;
                int p = 0;
                int j = 0;
                for (int i = 0; i < line.Length; ++i)
                {
                    switch (line[i])
                    {
                        case ' ':
                            ++spaces;
                            ++p;
                            break;
                        case '\t':
                            int padding = 0;
                            padding = tabSize - p % tabSize;
                            p += padding;
                            spaces += padding;
                            ++tabs;
                            break;
                        default:
                            j = EntabCore(line, i, tabSize, spaces, addTabThreadhold, tabs, j, sb, p);
                            tabs = 0;
                            spaces = 0;
                            break;
                    }
                }

                j = EntabCore(line, line.Length, tabSize, spaces, addTabThreadhold, tabs, j, sb, p);
                if (j < line.Length) sb.Append(line.Substring(j));
                return sb.ToString();
            }

            return line;
        }
        /// <summary>   Entab core. Internal method to support converting spaces into tabs.</summary>
        ///
        /// <param name="line">             The line of text to entab. </param>
        /// <param name="i">                Zero-based index to start scanning for tab placement. </param>
        /// <param name="tabSize">          Size of the tab in spaces.  By default 8. </param>
        /// <param name="spaces">           The number of spaces found. </param>
        /// <param name="addTabThreadhold"> The add tab threadhold. Number of spaces needed before tabs are added.</param>
        /// <param name="tabs">             The number of tabs found. </param>
        /// <param name="j">                The position of the first non-white space character after whitespace. </param>
        /// <param name="sb">               The string buffer. </param>
        /// <param name="p">                The current zero-based column position. </param>
        ///
        /// <returns>   Updated position of first non-white space character after whitespace. </returns>
        private static int EntabCore(string line, int i, int tabSize, int spaces, int addTabThreadhold, int tabs, int j,
            StringBuilder sb, int p)
        {
            if (spaces > addTabThreadhold || tabs > 0)
            {
                int l = i - spaces;
                int p0 = p - spaces;
                int firstStop = p0 + tabSize - (p0 % tabSize);
                int nStop = (spaces - firstStop) / tabSize + 1;
                int adj = p % tabSize;
                if (nStop > 0) sb.Append(new string('\t', nStop));
                if (adj > 0) sb.Append(new string(' ', adj));
                j = i;
            }

            return j;
        }
        /// <summary>   Gets offset from column. The offset is always the column number less one
        ///             except for lines that contain tabs.  In this situation the offset is
        ///             computed by evaluating the tabs in the line. </summary>
        ///
        /// <param name="line">     The line of text to scan. </param>
        /// <param name="column">   The 1-based column. </param>
        /// <param name="tabSize">  (Optional) Size of the tab in spaces.  By default 8. </param>
        ///
        /// <returns>   The zero-based offset from column. </returns>
        public static int GetOffsetFromColumn(string line, int column, int tabSize = DefaultTabSize)
        {
            int column0 = 1;
            int offset = 0;
            while (column0 < column)
            {
                if (line[offset++] == '\t')
                {
                    column0 += tabSize - (column0 - 1) % tabSize;
                }
                else
                {
                    ++column0;
                }
            }

            return offset;
        }
        /// <summary>   Gets column from offset. The column is always the offset plus
        ///             one except for lines that contain tabs.  In this situation the column is
        ///             calculated by scanning the line to evaluate tabs.</summary>
        ///
        /// <param name="line">     The line of text to scan. </param>
        /// <param name="offset">   The zero-based offset. </param>
        /// <param name="tabSize">  (Optional) Size of the tab in spaces.  By default 8. </param>
        ///
        /// <returns>   The 1-based column from the zero-based offset. </returns>
        public static int GetColumnFromOffset(string line, int offset, int tabSize = DefaultTabSize)
        {
            int column = 1;
            int offset0 = 0;
            while (offset0 < offset)
            {
                if (line[offset0++] == '\t')
                {
                    column += tabSize - (column - 1) % tabSize;
                }
                else
                {
                    ++column;
                }
            }

            return column;
        }
        /// <summary>   Query if 'name' is valid name. A name in this
        ///             context is a character string that is suitable for use as an indentifier
        ///             of some type.</summary>
        ///
        /// <param name="name">             The name candidate to test </param>
        /// <param name="firstCharacters">  (Optional) The set of characters allowed for the first character. Upper and lower case Unicode characters are assumed.
        ///                                 By default contains only the underscore.</param>
        /// <param name="nextCharacters">   (Optional) The set of characters allowed for characters other than the first.  Upper and lower case
        ///                                 Unicode characters and digits are assumed.  By default contains only the underscore character. </param>
        ///
        /// <returns>   True if valid name, false if not. </returns>
        /// <remarks>Nominally identifiers do not contain white space and have alphabetic first characters allow digits
        ///          for characters other than the first.  However there are many exceptions to this simple rule which
        ///          the first and next characters arguments attempt to enable.</remarks>
        public static bool IsValidName(string name, string firstCharacters = "_", string nextCharacters = "_")
        {
            if (!String.IsNullOrEmpty(name) && firstCharacters != null && nextCharacters != null)
            {
                char c0 = name[0];
                if (char.IsLetter(c0) || firstCharacters.IndexOf(c0) >= 0)
                {
                    for (int i = 1; i < name.Length; ++i)
                    {
                        char c1 = name[i];
                        if (char.IsLetterOrDigit(c1) || nextCharacters.IndexOf(c1) >= 0)
                        {
                            continue;
                        }
                        return false;
                    }

                    return true;
                }
            }
            return false;
        }

        public static char GetEscapedCharacter(string s)
        {
            if (!String.IsNullOrEmpty(s))
            {
                char c = s[0];
                if (c == '\\' && s.Length > 1)
                {
                    switch (s[1])
                    {
                        case '0': return '\0';
                        case 'a': return '\a';
                        case 'b': return '\b';
                        case 'f': return '\f';
                        case 'n': return '\n';
                        case 'r': return '\r';
                        case 't': return '\t';
                        case 'v': return '\v';
                        case '\\': return '\\';
                        case 'u':
                        case 'x':
                        {
                            if(int.TryParse(s.Substring(2), NumberStyles.HexNumber, null, out int v))
                            {
                                return Convert.ToChar(v);
                            }
                        }
                            break;
                        default:
                            return c;
                    }
                }

                return c;
            }

            return '\0';
        }

        public static string PutEscapedCharacter(char c)
        {
            if (char.IsControl(c))
            {
                switch (c)
                {
                    case '0': return "\\0";
                    case 'a': return "\\a";
                    case 'b': return "\\b";
                    case 'f': return "\\f";
                    case 'n': return "\\n";
                    case 'r': return "\\r";
                    case 't': return "\\t";
                    case 'v': return "\\v";
                    case '\\': return "\\\\";
                    default:
                        return String.Format("\\x{0:x}", (int)c);

                }
            }

            return new string(c, 1);
        }
        /// <summary>   Puts hexadecimal character that the value represents.</summary>
        ///
        /// <param name="v">    An int to converter to a hexidecimal character. </param>
        ///
        /// <returns>   A hexidecimal character.  If the paracter is not in the range 0-15, the null character '\0' is returned.</returns>
        public static char PutHexChar(int v)
        {
            switch (v)
            {
                case 0: return '0';
                case 1: return '1';
                case 2: return '2';
                case 3: return '3';
                case 4: return '4';
                case 5: return '5';
                case 6: return '6';
                case 7: return '7';
                case 8: return '8';
                case 9: return '9';
                case 10: return 'A';
                case 11: return 'B';
                case 12: return 'C';
                case 13: return 'D';
                case 14: return 'E';
                case 15: return 'F';
                default:
                    return '\0';
            }
        }
        /// <summary>   Gets the value that a character in a hexidecimal string represents. </summary>
        ///
        /// <param name="c">    The hexidecimal character. One of 0-9, a-f or A-F.</param>
        ///
        /// <returns>   The value of the hexidecimal character.  If the character is not 
        ///             one of the valid character, zero is returned. </returns>
        public static int GetHexValue(char c)
        {
            switch (c)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'a':
                case 'A':
                    return 10;
                case 'b':
                case 'B':
                    return 11;
                case 'c':
                case 'C':
                    return 12;
                case 'd':
                case 'D':
                    return 13;
                case 'e':
                case 'E':
                    return 14;
                case 'f':
                case 'F':
                    return 15;
                default:
                    return 0;
            }
        }
    }
}
