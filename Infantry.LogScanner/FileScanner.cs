namespace Infantry.LogScanner
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    public class FileScanner
    {
        public IEnumerable<string> GetFiles(string pattern)
        {
            var targets = new ConcurrentBag<string>();
            var directories = GetInitialDirectories();

            var tasks = new List<Task>();
            int maxTasks = Environment.ProcessorCount;

            do
            {
                if (directories.Count > tasks.Count && tasks.Count < maxTasks)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        while (directories.TryTake(out string currentDirectory))
                        {
                            try
                            {
                                var children = Directory.GetDirectories(currentDirectory);

                                foreach (var child in children)
                                {
                                    directories.Add(child);
                                }
                            }
                            catch
                            {
                                continue;
                            }

                            foreach (var fileName in SearchDirectory(currentDirectory, pattern))
                            {
                                targets.Add(fileName);
                            }
                        }
                    }));
                }

                var completedTasks = tasks.Where(t => t.IsCompleted).ToList();

                foreach (var task in completedTasks)
                {
                    tasks.Remove(task);
                }
            } while (tasks.Count > 0);

            return targets.ToArray();
        }

        private static ConcurrentBag<string> GetInitialDirectories()
        {
            var directories = new ConcurrentBag<string>();
            var drives = Directory.GetLogicalDrives();

            foreach (var drive in drives)
            {
                //directories.Add(drive);
            }

            directories.Add(@"C:\Users\joelm\Desktop\Infantry");

            return directories;
        }

        private static IEnumerable<string> SearchDirectory(string directory, string pattern)
        {
            var matches = new List<string>();

            foreach (var file in Directory.EnumerateFiles(directory))
            {
                string fileName = Path.GetFileName(file);

                if (Regex.IsMatch(Path.GetFileName(fileName), pattern))
                {
                    matches.Add(file);
                }
            }

            return matches;
        }
    }
}
