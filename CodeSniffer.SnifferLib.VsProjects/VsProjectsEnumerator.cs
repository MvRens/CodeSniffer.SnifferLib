using System.Diagnostics.CodeAnalysis;
using CodeSniffer.Core.Sniffer;
using CodeSniffer.SnifferLib.Matchers;
using JetBrains.Annotations;
using Microsoft.Build.Construction;
using Serilog;

namespace CodeSniffer.SnifferLib.VsProjects
{
    /// <summary>
    /// Provides a way to list Visual Studio projects in a folder, optionally only those belonging to a solution.
    /// </summary>
    [PublicAPI]
    public class VsProjectsEnumerator
    {
        private readonly ILogger logger;
        private readonly CsReportBuilder builder;
        private readonly IMatcher excludePathsMatcher;


        /// <inheritdoc cref="VsProjectsEnumerator"/>
        public VsProjectsEnumerator(ILogger logger, CsReportBuilder builder, IMatcher excludePathsMatcher)
        {
            this.logger = logger;
            this.builder = builder;
            this.excludePathsMatcher = excludePathsMatcher;
        }


        /// <summary>
        /// Searches the specified path for Visual Studio project files.
        /// </summary>
        /// <remarks>
        /// At the time of writing, only .csproj projects are supported.
        /// </remarks>
        /// <param name="path">The path to search recursively</param>
        /// <param name="solutionsOnly">If true, only project files referenced in a solution file are returned</param>
        /// <returns>A list of discovered project files</returns>
        public IEnumerable<VsProject> GetProjects(string path, bool solutionsOnly)
        {
            return solutionsOnly
                ? GetSolutionProjects(path)
                : GetAllProjectFiles(path);
        }



        private IEnumerable<VsProject> GetSolutionProjects(string path)
        {
            logger.Debug("Scanning path {path} for Visual Studio solutions", path);

            foreach (var solutionFile in Directory.GetFiles(path, @"*.sln", SearchOption.AllDirectories))
            {
                var solutionName = Path.GetRelativePath(path, solutionFile);
                if (excludePathsMatcher.Matches(solutionName))
                {
                    builder
                        .AddAsset(solutionName, solutionName)
                        .SetResult(CsReportResult.Skipped, Strings.ResultSkippedExcludePaths);
                    continue;
                }

                logger.Debug("Parsing solution file {filename}", solutionFile);

                var solution = SolutionFile.Parse(solutionFile);
                foreach (var project in solution.ProjectsInOrder.Where(p =>
                             p.ProjectType != SolutionProjectType.SolutionFolder))
                {
                    if (ValidVsProject(path, project.AbsolutePath, out var vsProject))
                        yield return vsProject.Value;
                }
            }
        }


        private IEnumerable<VsProject> GetAllProjectFiles(string path)
        {
            // I only use C#, so if you need support for other languages... it's open-source!
            logger.Debug("Scanning path {path} for C# projects", path);

            return Directory.GetFiles(path, @"*.csproj", SearchOption.AllDirectories)
                .Select(f => ValidVsProject(path, f, out var vsProject) ? vsProject : null)
                .Where(p => p != null)
                .Cast<VsProject>();
        }


        private bool ValidVsProject(string basePath, string absolutePath, [NotNullWhen(true)] out VsProject? project)
        {
            if (!File.Exists(absolutePath))
            {
                project = null;
                return false;
            }

            var projectName = Path.GetRelativePath(basePath, absolutePath);

            if (Path.DirectorySeparatorChar != '\\')
                projectName = projectName.Replace(Path.DirectorySeparatorChar, '\\');

            var asset = builder.AddAsset(projectName, projectName);

            if (excludePathsMatcher.Matches(projectName))
            {
                asset.SetResult(CsReportResult.Skipped, Strings.ResultSkippedExcludePaths);
                project = null;
                return false;
            }

            project = new VsProject(projectName, absolutePath, asset);
            return true;
        }
    }
}