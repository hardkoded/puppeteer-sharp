#:package ICSharpCode.Decompiler@9.1.0.7988
#:property ImplicitUsings=false

using System;
using System.IO;
using System.Collections.Generic;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;

static bool EvaluateRelease(DirectoryInfo dir)
{
    var framework = dir.Name;
    var failed = false;

    var asssemblyPath = Path.Join(dir.FullName, "PuppeteerSharp.dll");
    if (!Path.Exists(asssemblyPath))
    {
        Console.Error.WriteLine($"[{framework}] No assembly found in release");
        return false;
    }

    var module = new PEFile(asssemblyPath);
    var references = module.Metadata.AssemblyReferences;
    foreach (var referenceHandle in references)
    {
        var reference = module.Metadata.GetAssemblyReference(referenceHandle);
        var name = reference.GetAssemblyName().Name;
        if (name == "WebDriverBiDi")
        {
            Console.Error.WriteLine($"[{framework}] WebDriverBiDi leaked into dependencies");
            failed = true;
            break;
        }
    }

    List<string> violatingTypes = new();
    var typeDefinitions = module.Metadata.TypeDefinitions;
    foreach (var typeDefinition in typeDefinitions)
    {
        var name = typeDefinition.GetFullTypeName(module.Metadata);
        if (typeDefinition.IsCompilerGeneratedOrIsInCompilerGeneratedClass(module.Metadata))
        {
            continue;
        }

        if (name.FullName.StartsWith("PuppeteerSharp.Bidi"))
        {
            violatingTypes.Add(name.FullName);
        }
    }

    failed = violatingTypes.Count > 0 || failed;

    foreach (var violation in violatingTypes)
    {
        Console.Error.WriteLine($"[{framework}] Leaked type {violation}");
    }

    if (!failed)
    {
        Console.WriteLine($"[{framework}] No WebDriverBidi leakage for release");
    }
    Console.WriteLine();

    return !failed;
}

var failed = false;

string releasePath;
if (args.Length < 1 || !Directory.Exists(releasePath = args[0]))
{
    Console.WriteLine("An existing release directory must be specified (e.g. bin/Release)");
    return 1;
}

foreach (var release in Directory.GetDirectories(releasePath))
{
    var dir = new DirectoryInfo(release);
    Console.WriteLine("Found release for framework " + dir.Name);
    var success = EvaluateRelease(dir);
    failed = !success || failed;
}

return failed ? 1 : 0;
