using CodeSniffer.Core.Sniffer;

namespace CodeSniffer.SnifferLib.VsProjects
{
    /// <summary>
    /// Represents a Visual Studio project file.
    /// </summary>
    public readonly struct VsProject
    {
        /// <summary>
        /// The project file path relative to the solution, for display purposes.
        /// </summary>
        public string ProjectName { get; }

        /// <summary>
        /// The full path to the project file.
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// The <see cref="CsReportBuilder.Asset"/> associated with this project.
        /// </summary>
        public CsReportBuilder.Asset Asset { get; }


        internal VsProject(string projectName, string filename, CsReportBuilder.Asset asset)
        {
            ProjectName = projectName;
            Filename = filename;
            Asset = asset;
        }
    }
}
