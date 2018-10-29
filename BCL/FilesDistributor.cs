using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BCL.Entity;
using BCL.Interfaces;
using BCL.Resource;

namespace BCL
{
    public class FilesDistributor : IFilesDistributor<FileModel>
    {
        private readonly ILogger _logger;
        private readonly List<Rule> _rules;
        private readonly string _defaultFolder;
        private const int FileCheckTimeoutMiliseconds = 1000;

        public FilesDistributor(IEnumerable<Rule> rules, string defaultFolder, ILogger logger)
        {
            _rules = rules.ToList();
            _logger = logger;
            _defaultFolder = defaultFolder;
        }

        public async Task MoveAsync(FileModel item)
        {
            var from = item.FullName;
            foreach (var rule in _rules)
            {
                var match = Regex.Match(item.Name, rule.FilePattern);

                if (!match.Success || match.Length != item.Name.Length) continue;

                rule.MatchesCount++;
                var to = FormDestinationPath(item, rule);
                _logger.Log(Strings.RuleMatch);

                await MoveFileAsync(@from, to);
                _logger.Log(string.Format(Strings.FileMovedTemplate, item.FullName, to));

                return;
            }

            var destination = Path.Combine(_defaultFolder, item.Name);
            _logger.Log(Strings.RuleNoMatch);

            await MoveFileAsync(from, destination);
            _logger.Log(string.Format(Strings.FileMovedTemplate, item.FullName, destination));
        }

        private async Task MoveFileAsync(string from, string to)
        {
            var dir = Path.GetDirectoryName(to);

            Directory.CreateDirectory(dir);

            var cannotAccessFile = true;

            do
            {
                try
                {
                    if (File.Exists(to))
                    {
                        File.Delete(to);
                    }
                    File.Move(from, to);
                    cannotAccessFile = false;
                }
                catch (FileNotFoundException)
                {
                    _logger.Log(Strings.CannotFindFile);
                    break;
                }
                catch (IOException ioex)
                {
                    var t = ioex.GetType();
                    await Task.Delay(FileCheckTimeoutMiliseconds);
                }
            } while (cannotAccessFile);
        }

        private static string FormDestinationPath(FileModel file, Rule rule)
        {
            var extension = Path.GetExtension(file.Name);
            var filename = Path.GetFileNameWithoutExtension(file.Name);
            var destination = new StringBuilder();
            destination.Append(Path.Combine(rule.DestinationFolder, filename));

            if (rule.IsDateAppended)
            {
                var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
                dateTimeFormat.DateSeparator = ".";
                destination.Append($"_{DateTime.Now.ToLocalTime().ToString(dateTimeFormat.ShortDatePattern)}");
            }

            if (rule.IsOrderAppended)
            {
                destination.Append($"_{rule.MatchesCount}");
            }

            destination.Append(extension);
            return destination.ToString();
        }
    }
}
