using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace reexmonkey.infrastructure.io.concretes
{
    public enum CodeDomLanguage
    { 
        csharp,
        visualbasic,
        jscript
    }

    public static class CodeDomExtensions
    {
        public static CodeDomLanguage ToCodeDomLanguage(this string language)
        {
            if (language.Equals("csharp", StringComparison.OrdinalIgnoreCase)) return CodeDomLanguage.csharp;
            else if(language.Equals("vsualbasic", StringComparison.OrdinalIgnoreCase))return CodeDomLanguage.visualbasic;
            else if (language.Equals("jscript", StringComparison.OrdinalIgnoreCase)) return CodeDomLanguage.jscript;
            else return CodeDomLanguage.csharp;
        }

        public static CompilerParameters GetDefaultCompilerParameters(this string language)
        {
            var linfo = CodeDomProvider.GetCompilerInfo(language);
            return (linfo != null) ? linfo.CreateDefaultCompilerParameters() : null;
        }

        public static IDictionary<string, CompilerParameters> GetAllCompilerParameters()
        {
            var langs = CodeDomExtensions.GetCompilerLanguages();
            var dict = new Dictionary<string, CompilerParameters>();

            foreach (var lang in langs)
            { 
                if(!dict.ContainsKey(lang))
                    dict.Add(lang, lang.GetDefaultCompilerParameters());
            }
            return dict;
        }

        public static IEnumerable<string> GetCompilerLanguages()
        {
            return CodeDomProvider.GetAllCompilerInfo().GetCompilerLanguages();
        }

        public static IEnumerable<string> GetCompilerLanguages(this IEnumerable<CompilerInfo> infos)
        {
            return infos.SelectMany(x => x.GetLanguages().Select(y => y));
        }

        public static CodeDomProvider ToCodeDomProvider(this CodeDomLanguage language)
        {

            CodeDomProvider cdp = null;
            switch (language)
            {
                case CodeDomLanguage.csharp: cdp = CodeDomProvider.CreateProvider("csharp"); break;
                case CodeDomLanguage.jscript: cdp = CodeDomProvider.CreateProvider("jscript");  break;
                case CodeDomLanguage.visualbasic: cdp = CodeDomProvider.CreateProvider("visualbasic"); break;
                default: cdp = CodeDomProvider.CreateProvider("csharp"); break;
            }

            return cdp;
        }

        public static CodeSnippetCompileUnit ToCodeSnippetCompileUnit(this string source)
        {
            return new CodeSnippetCompileUnit(source);
        }

        public static void GenerateCode(this CodeDomProvider provider, CodeCompileUnit unit, TextWriter tw, CodeGeneratorOptions options)
        {
            provider.GenerateCodeFromCompileUnit(unit, tw, options);
        }

        public static CompilerResults CompileCode(this CodeDomProvider provider, CompilerParameters options, params CodeCompileUnit[] units)
        {
            return provider.CompileAssemblyFromDom(options, units);
        }

        public static CompilerResults CompileCode(this CodeDomProvider provider, CompilerParameters options, params string[] sources)
        {
            return provider.CompileAssemblyFromSource(options, sources);
        }

        public static Expression<Func<T, TResult>> CompileToExpressionFunc<T, TResult>(this string expressionstring, CodeDomLanguage language, params string[] references )
        {
            Expression<Func<T, TResult>> expr = null;
            try
            {
                //Build code graph
                var coptions = new CompilerParameters(references);
                coptions.GenerateExecutable = false;
                coptions.GenerateInMemory = true;
                coptions.IncludeDebugInformation = true;
                var provider = language.ToCodeDomProvider();
                var ccu = new CodeCompileUnit();
                var gs = new CodeNamespace();
                gs.Imports.AddRange(new CodeNamespaceImport[]{
                    new CodeNamespaceImport("System"), 
                    new CodeNamespaceImport("System.Linq"),
                    new CodeNamespaceImport("System.Linq.Expressions")
                });

                var ns = new CodeNamespace("DecoratorNameSpace");               
                var decorator = new CodeTypeDeclaration("Decorator");
                decorator.IsClass = true;
                decorator.TypeAttributes = System.Reflection.TypeAttributes.Public ;
                ns.Types.Add(decorator);
                var func = new CodeMemberMethod();
                func.Name = "ToExpressionFunc";
                func.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                func.ReturnType = new CodeTypeReference(typeof(Expression<Func<T, TResult>>));
                var rstat = new CodeMethodReturnStatement();
                rstat.Expression = new CodeSnippetExpression(string.Format("({0})", expressionstring));
                func.Statements.Add(rstat);
                decorator.Members.Add(func);
                ccu.Namespaces.Add(gs);
                ccu.Namespaces.Add(ns);

                //Compile code
                var cresults = provider.CompileCode(coptions, ccu);
                if (!cresults.Errors.HasErrors)
                {
                    var type = cresults.CompiledAssembly.GetType("DecoratorNameSpace.Decorator");
                    var minfo = type.GetMethod("ToExpressionFunc");
                    if (minfo != null)
                    {
                        expr = (Expression<Func<T, TResult>>)minfo.Invoke(null, null);
                    }
                }

            }
            catch (NotImplementedException) { throw; }
            catch (System.Security.SecurityException) { throw; }
            catch (Exception) { throw; }
            
            return expr;
        }


    }
}
