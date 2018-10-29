using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using BCL.Configuration;
using BCL.Entity;
using BCL.Interfaces;
using BCL.Resource;
using Rule = BCL.Entity.Rule;

namespace BCL
{
    public class Program
    {
        private static List<string> _directories;
        private static List<Rule> _rules;
        private static IFilesDistributor<FileModel> _distributor;

        static void Main(string[] args)
        {
            if (ConfigurationManager
                .GetSection("fileSystemSection") is FileSystemMonitorConfigSection config)
            {
                ReadConfiguration(config);
            }
            else
            {
                Console.Write(Strings.ConfigNotFounded);
                return;
            }

            Console.WriteLine(config.Culture.DisplayName);

            ILogger logger = new Logger();
            _distributor = new FilesDistributor(_rules, config.Rules.DefaultDirectory, logger);
            IFileWatcher<FileModel> watcher = new FileWatcher(_directories, logger);

            watcher.Created += OnCreated;

            Console.ReadKey();
        }

        private static async void OnCreated(object sender, ItemCreatedIventArgs<FileModel> args)
        {
            await _distributor.MoveAsync(args.CreatedItem);
        }

        private static void ReadConfiguration(FileSystemMonitorConfigSection config)
        {
            _directories = new List<string>(config.Directories.Count);
            _rules = new List<Rule>();

            foreach (DirectoryElement directory in config.Directories)
            {
                _directories.Add(directory.Path);
            }

            foreach (RuleElement rule in config.Rules)
            {
                _rules.Add(new Rule
                {
                    FilePattern = rule.FilePattern,
                    DestinationFolder = rule.DestinationFolder,
                    IsDateAppended = rule.IsDateAppended,
                    IsOrderAppended = rule.IsOrderAppended
                });
            }

            CultureInfo.DefaultThreadCurrentCulture = config.Culture;
            CultureInfo.DefaultThreadCurrentUICulture = config.Culture;
            CultureInfo.CurrentUICulture = config.Culture;
            CultureInfo.CurrentCulture = config.Culture;
        }
    }
}
