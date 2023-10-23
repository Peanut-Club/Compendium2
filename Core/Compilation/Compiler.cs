using Compendium.Logging;
using Compendium.Utilities.Reflection;

using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Compendium.Compilation
{
    public static class Compiler
    {
        public static void Compile(string directoryPath, Action<CompilationResult> callback)
        {
            var provider = new CSharpCodeProvider(new Dictionary<string, string>()
            {
                ["CompilerVersion"] = "v4.0"
            });

            var output = $"{Path.GetTempPath()}/{Path.GetDirectoryName(directoryPath)}.dll";
            var parameters = new CompilerParameters();

            parameters.OutputAssembly = $"{Path.GetDirectoryName(directoryPath)}.dll";
            parameters.GenerateExecutable = false;
            parameters.CompilerOptions += $" /recurse:{directoryPath}{Path.DirectorySeparatorChar}*.cs";

            var result = provider.CompileAssemblyFromFile(parameters, "n.cs");

            foreach (var msg in result.Output)
                Log.Trace("Compiler", msg);

            if (result.Errors.HasErrors)
            {
                var msgArray = new CompilerError[result.Errors.Count];

                result.Errors.CopyTo(msgArray, 0);

                var errors = msgArray.Where(e => !e.IsWarning).ToArray();
                var warnings = msgArray.Where(e => e.IsWarning).ToArray();

                callback.SafeCall(new CompilationResult
                {
                    Errors = errors.Select(e => new CompilationMessage
                    {
                        Code = e.ErrorNumber,
                        File = e.FileName,
                        Message = e.ErrorText
                    }).ToArray(),

                    Warnings = warnings.Select(w => new CompilationMessage
                    {
                        Code = w.ErrorNumber,
                        File = w.FileName,
                        Message = w.ErrorText
                    }).ToArray(),

                    IsCompiled = false,

                    Raw = null,
                    Result = null
                });
            }
            else
            {
                var assembly = result.CompiledAssembly;
                var raw = IO.File.Read(result.PathToAssembly);

                File.Delete(result.PathToAssembly);

                if (result.Errors.HasWarnings)
                {
                    var msgArray = new CompilerError[result.Errors.Count];

                    result.Errors.CopyTo(msgArray, 0);

                    var warnings = msgArray.Where(e => e.IsWarning).ToArray();

                    callback.SafeCall(new CompilationResult
                    {
                        Errors = Array.Empty<CompilationMessage>(),

                        Warnings = warnings.Select(w => new CompilationMessage
                        {
                            Code = w.ErrorNumber,
                            File = w.FileName,
                            Message = w.ErrorText
                        }).ToArray(),

                        IsCompiled = true,

                        Raw = raw,
                        Result = assembly
                    });
                }
                else
                {
                    callback.SafeCall(new CompilationResult
                    {
                        Errors = Array.Empty<CompilationMessage>(),
                        Warnings = Array.Empty<CompilationMessage>(),

                        IsCompiled = true,

                        Raw = raw,
                        Result = assembly
                    });
                }
            }
        }
    }
}
