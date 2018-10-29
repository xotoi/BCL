using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BCL.Entity;
using BCL.Interfaces;
using BCL.Resource;

namespace BCL
{
    public class FileWatcher : IFileWatcher<FileModel>
    {
        private readonly List<FileSystemWatcher> _fileSystemWatchers;
        private readonly ILogger _logger;

        public FileWatcher(IEnumerable<string> directories, ILogger logger)
        {
            _logger = logger;
            _fileSystemWatchers = directories.Select(CreateWatcher).ToList();
        }

        public event EventHandler<ItemCreatedIventArgs<FileModel>> Created;

        private void OnCreated(FileModel file)
        {
            Created?.Invoke(this, new ItemCreatedIventArgs<FileModel> { CreatedItem = file });
        }

        private FileSystemWatcher CreateWatcher(string path)
        {
            var fileSystemWatcher = new FileSystemWatcher(path)
                {
                    NotifyFilter = NotifyFilters.FileName,
                    IncludeSubdirectories = false,
                    EnableRaisingEvents = true
                };

            fileSystemWatcher.Created += (sender, fileSystemEventArgs) =>
            {
                _logger.Log(string.Format(Strings.FileFoundedTemplate, fileSystemEventArgs.Name));
                OnCreated(new FileModel { FullName = fileSystemEventArgs.FullPath, Name = fileSystemEventArgs.Name });
            };

            return fileSystemWatcher;
        }
    }
}
