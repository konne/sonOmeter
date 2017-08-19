using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using UKLib.Fireball.Syntax;

namespace UKLib.Fireball.Syntax
{
    public sealed class SynFileLoader
    {
        static LanguageList _LangList = null;

        private static string GetSyntaxFileName(SyntaxLanguage lang)
        {
            string file = Enum.GetName(typeof(SyntaxLanguage), lang);

            file += ".syn";

            return "UKLib.Fireball.SyntaxFiles.Syns." + file;
        }

        public static void SetSyntax(SyntaxDocument document, SyntaxLanguage language)
        {
            Stream xml = GetSyntaxStream(GetSyntaxFileName(language));

            document.Parser.Init(Language.FromSyntaxFile(xml));
        }

        private static Stream GetSyntaxStream(string file)
        {
            Stream strm = typeof(SynFileLoader).Assembly.GetManifestResourceStream(file);
            return strm;
        }

        public static void SetSyntax(SyntaxDocument document, string filename)
        {
            document.Parser.Init(SynFileLoader.LanguageList.GetLanguageFromFile(filename));
        }

        public static LanguageList LanguageList
        {
            get
            {
                if (_LangList == null)
                {
                    _LangList = new LanguageList();

                    SyntaxLanguage[] languages = (SyntaxLanguage[])Enum.GetValues(typeof(SyntaxLanguage));

                    foreach (SyntaxLanguage current in languages)
                    {
                        Stream strm = GetSyntaxStream(GetSyntaxFileName(current));

                        _LangList.Add(Language.FromSyntaxFile(strm));
                    }
                }

                return _LangList;
            }
        }
    }

    public enum SyntaxLanguage
    {
        Lang6502,
        ASP,
        CPP,
        Cobol,
        CSharp,
        CSS,
        DataFlex,
        Delphi,
        DOSBatch,
        Fortran90,
        FoxPro,
        Java,
        JavaScript,
        JSP,
        LotusScript,
        MSIL,
        MySql_SQL,
        NPath,
        Oracle_SQL,
        Perl,
        PHP,
        Povray,
        Python,
        Rtf,
        SmallTalk,
        SqlServer2K,
        SqlServer7,
        SystemPolicies,
        Template,
        Text,
        TurboPascal,
        VBNET,
        VB,
        VBScript,
        VRML97,
        XML
    }
}
