using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Microsoft.Build.Framework;
using SharpCompress.Common;
using SharpCompress.Readers;
using Task = Microsoft.Build.Utilities.Task;

namespace MsBuildUncompressTask;

public class Uncompress : Task
{
    private readonly List<ITaskItem> _files = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Unzip"/> class.
    /// </summary>
    public Uncompress()
    {
        Overwrite = true;
    }

    /// <summary>
    /// Gets or sets the name of the zip file.
    /// </summary>
    /// <value>The name of the zip file.</value>
    [Required]
    public string ArchivePath { get; set; }

    /// <summary>
    /// Gets or sets the target directory.
    /// </summary>
    /// <value>The target directory.</value>
    [Required]
    public string TargetDirectory { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to overwrite any existing files on extraction. Defaults to <c>true</c>.
    /// </summary>
    /// <value><c>true</c> to overwrite any existing files on extraction; otherwise, <c>false</c>.</value>
    [DefaultValue(true)]
    public bool Overwrite { get; set; }

    /// <summary>
    /// Gets the files extracted from the zip.
    /// </summary>
    /// <value>The files extracted from the zip.</value>
    [Output]
    public ITaskItem[] ExtractedFiles => _files.ToArray();

    /// <summary>
    /// When overridden in a derived class, executes the task.
    /// </summary>
    /// <returns>
    /// true if the task successfully executed; otherwise, false.
    /// </returns>
    public override bool Execute()
    {
        if (!File.Exists(ArchivePath))
        {
            Log.LogError("Archive not found", ArchivePath);
            return false;
        }

        if (!Directory.Exists(TargetDirectory))
            Directory.CreateDirectory(TargetDirectory);

        using var stream = File.OpenRead(ArchivePath);
        var reader = ReaderFactory.Open(stream);
        while (reader.MoveToNextEntry())
        {
            if (!reader.Entry.IsDirectory)
            {
                reader.WriteEntryToDirectory(TargetDirectory, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
            }
        }
        
        return true;
    }
}