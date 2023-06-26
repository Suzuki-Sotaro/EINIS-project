using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using F23.StringSimilarity;
using PowerArgs;
using Testura.Code.Compilations;
using JaroWinkler = F23.StringSimilarity.JaroWinkler;

namespace SimiSharp.Core
{
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    public class SimiSharpArguments
    {
        [ArgDescription("Path to reference source file.")]
        [ArgShortcut("-r")]
        [ArgRequired(PromptIfMissing = true)]
        [ArgExistingFile]
        public FileInfo InputReferenceFilePath { get; set; }

        [ArgDescription("Path to analyzed source file.")]
        [ArgShortcut("-a")]
        [ArgRequired(PromptIfMissing = true)]
        [ArgExistingFile]
        public FileInfo InputAnalyzedFilePath { get; set; }

        private DirectoryInfo Workspace { get; set; }
        private DirectoryInfo CompiledDirectory { get; set; }
        private DirectoryInfo DecompiledDirectory { get; set; }
        private FileInfo ReferenceBinary { get; set; }
        private FileInfo AnalyzedBinary { get; set; }
        private FileInfo DecompiledReference { get; set; }
        private FileInfo DecompiledAnalyzed { get; set; }
        private FileInfo CleansedReference { get; set; }
        private FileInfo CleansedAnalyzed { get; set; }

        /// <summary>
        /// EntryPoint
        /// </summary>
        public void Main()
        {
            CreateWorkspace();
            CompileFiles();
            GenerateIL();
            CleanseIL();
            Compare();
        }

        private void Compare()
        {
            var reference = string.Join(separator: ' ', File.ReadAllLines(path: CleansedReference.FullName));
            var analyzed = string.Join(separator: ' ', File.ReadAllLines(path: CleansedAnalyzed.FullName));

            int kShing = 4;
            var jaroWinkler = new JaroWinkler(0.7).Similarity(s1: reference, s2: analyzed);
            var levenshtein = new NormalizedLevenshtein().Similarity(s1: reference, s2: analyzed);
            var cosine = new Cosine(k: kShing).Similarity(s1: reference, s2: analyzed);
            var jaccard = new Jaccard(k: kShing).Similarity(s1: reference, s2: analyzed);
            var soresenDice = new SorensenDice(k: kShing).Similarity(s1: reference, s2: analyzed);

            var document = new Document(
            new Span(text: "Analysis report"), "\n",
            new Grid
                {
                    Color = ConsoleColor.Gray,
                    Columns = { GridLength.Auto, GridLength.Auto },
                    AutoPosition = true,
                    Children =
                    {
                        new Cell("Metric") {Margin = new Thickness(1,0,1,0), Stroke = new LineThickness(LineWidth.Double, LineWidth.Single), Background = ConsoleColor.DarkBlue, Color = ConsoleColor.Cyan},
                        new Cell("Value") {Margin = new Thickness(1,0,1,0), Stroke = new LineThickness(LineWidth.Double, LineWidth.Single), Background = ConsoleColor.DarkBlue, Color = ConsoleColor.Cyan},

                        new Cell("Jaro-Winkler:") {Margin = new Thickness(1,0,1,0)}, new Cell((jaroWinkler*100).ToString("N2") + " %") {Margin = new Thickness(1,0,1,0)},
                        new Cell("Normalized Levenshtein:") {Margin = new Thickness(1,0,1,0)}, new Cell((levenshtein*100).ToString("N2") + " %") {Margin = new Thickness(1,0,1,0)},
                        new Cell($"Cosine ({kShing}):") { Color = ConsoleColor.Yellow, Margin = new Thickness(1,0,1,0)}, new Cell((cosine * 100).ToString("N2") + " %") {Margin = new Thickness(1,0,1,0)},
                        new Cell($"Jaccard Index ({kShing}):") { Color = ConsoleColor.Yellow, Margin = new Thickness(1,0,1,0)}, new Cell((jaccard * 100).ToString("N2") + " %") {Margin = new Thickness(1,0,1,0)},
                        new Cell($"Soresen Dice ({kShing}):") { Color = ConsoleColor.Yellow, Margin = new Thickness(1,0,1,0)}, new Cell((soresenDice * 100).ToString("N2") + " %") {Margin = new Thickness(1,0,1,0)},

                        new Cell("Average:") {Margin = new Thickness(1,0,1,0)}, new Cell(((jaroWinkler+levenshtein+cosine+jaccard+soresenDice)*100/5).ToString("N2") + " %") {Margin = new Thickness(1,0,1,0)},
                    }
                }
            );

            ConsoleRenderer.RenderDocument(document: document);

        }

        private void CleanseIL()
        {
            var selectedReferenceLines = File.ReadAllLines(path: DecompiledReference.FullName, Encoding.UTF8)
                .Where(x => x.Contains(value: "IL_", StringComparison.InvariantCulture))
                .Select(x =>
                {
                    x = x.Remove(startIndex: 0, count: 14);

                    if (x.Contains(' ', StringComparison.InvariantCulture))
                    {
                        var strs = x.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        x = strs[0];
                    }

                    return x;
                });

            var cleansedReferencePath = DecompiledReference.FullName.Replace(oldValue: ".il", newValue: "_cls.txt", StringComparison.InvariantCulture);
            File.WriteAllLines(path: cleansedReferencePath, selectedReferenceLines, Encoding.UTF8);
            CleansedReference = new FileInfo(fileName: cleansedReferencePath);

            var selectedRAnalyzedLines = File.ReadAllLines(path: DecompiledAnalyzed.FullName, Encoding.UTF8)
                .Where(x => x.Contains(value: "IL_", StringComparison.InvariantCulture))
                .Select(x =>
                {
                    x = x.Remove(startIndex: 0, count: 14);

                    if (x.Contains(' ', StringComparison.InvariantCulture))
                    {
                        var strs = x.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        x = strs[0];
                    }

                    return x;
                });
            var cleansedAnalyzedPath = DecompiledAnalyzed.FullName.Replace(oldValue: ".il", newValue: "_cls.txt", StringComparison.InvariantCulture);
            File.WriteAllLines(path: cleansedAnalyzedPath, selectedRAnalyzedLines, Encoding.UTF8);
            CleansedAnalyzed = new FileInfo(fileName: cleansedAnalyzedPath);
        }

        private void GenerateIL()
        {
            var ildasmPath = Path.Combine(path1: Directory.GetCurrentDirectory(), path2: "Tools", path3: "ildasm.exe");
            var decompiledReferencePath = Path.Combine(path1: DecompiledDirectory.FullName,
                path2: Path.GetFileNameWithoutExtension(ReferenceBinary.FullName) + ".il");
            var ildasm = new ProcessStartInfo(fileName: ildasmPath, arguments: string.Join(separator: ' ', ReferenceBinary.FullName , $"/out:\"{decompiledReferencePath}\"", "/utf8"));
            ildasm.WindowStyle = ProcessWindowStyle.Hidden;
            ildasm.CreateNoWindow = true;
            var process = Process.Start(ildasm);
            process.WaitForExit();
            DecompiledReference = new FileInfo(fileName: decompiledReferencePath);


            var decompiledAnalyzedPath = Path.Combine(path1: DecompiledDirectory.FullName,
                path2: Path.GetFileNameWithoutExtension(AnalyzedBinary.FullName) + ".il");

            ildasm = new ProcessStartInfo(fileName: ildasmPath, arguments: string.Join(separator: ' ', AnalyzedBinary.FullName, $"/out:\"{decompiledAnalyzedPath}\"", "/utf8"));
            process = Process.Start(ildasm);
            process.WaitForExit();
            DecompiledAnalyzed = new FileInfo(fileName: decompiledAnalyzedPath);
            process.Dispose();
        }

        private void CompileFiles()
        {
            var compiler = new Compiler();
            var referenceBinaryOutputPath = Path.Combine(path1: CompiledDirectory.FullName,
                path2: Path.GetFileNameWithoutExtension(InputReferenceFilePath.FullName)) + ".dll";
            var referenceCompileResult = compiler.CompileFilesAsync(
                outputPath: referenceBinaryOutputPath,
                pathsToCsFiles: InputReferenceFilePath.FullName).ConfigureAwait(false).GetAwaiter().GetResult();
            if (!referenceCompileResult.Success)
            {
                // TODO: Fail
                throw new Exception();
            }
            ReferenceBinary = new FileInfo(fileName: referenceCompileResult.PathToDll);

            var analyzedBinaryOutputPath = Path.Combine(path1: CompiledDirectory.FullName,
                path2: Path.GetFileNameWithoutExtension(InputAnalyzedFilePath.FullName)) + ".dll";
            var analyzedCompileResult = compiler.CompileFilesAsync(
                outputPath: analyzedBinaryOutputPath,
                pathsToCsFiles: InputAnalyzedFilePath.FullName).ConfigureAwait(false).GetAwaiter().GetResult();
            if (!analyzedCompileResult.Success)
            {
                // TODO: Fail
                throw new Exception();
            }
            AnalyzedBinary = new FileInfo(fileName: analyzedCompileResult.PathToDll);
        }

        private void CreateWorkspace()
        {
            var currentDir = new DirectoryInfo(path: Directory.GetCurrentDirectory());
            var workspacePath = Path.Combine(path1: currentDir.FullName, path2: "Workspace");

            if(Directory.Exists(path: workspacePath))
            {
                Directory.Delete(path: workspacePath, true);
            }

            Workspace = Directory.CreateDirectory(path: workspacePath);
            var compiledPath = Path.Combine(path1: Workspace.FullName, path2: "Compiled");
            var decompiledPath = Path.Combine(path1: Workspace.FullName, path2: "Decompiled");
            CompiledDirectory = Directory.CreateDirectory(path: compiledPath);
            DecompiledDirectory = Directory.CreateDirectory(path: decompiledPath);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Args.InvokeMain<SimiSharpArguments>(args);
        }
    }
}
