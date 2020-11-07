﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Linux
{
    public class LinuxApplicationService : IApplicationService
    {
        public Task<IEnumerable<ApplicationModel>> GetAssociatedApplications(string fileExtension)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ApplicationModel>> GetInstalledApplications()
        {
            var installedSoftwares = new List<ApplicationModel>();

            foreach (var desktopFilePath in Directory.GetFiles("/usr/share/applications/", "*.desktop"))
            {
                await using var desktopFile = File.OpenRead(desktopFilePath);

                var desktopEntry = new IniFileReader().ReadFile(desktopFile);

                var desktopType = desktopEntry.GetValueOrDefault("Desktop Entry:Type");
                if (desktopType == null || !desktopType.Equals("Application", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var displayName = desktopEntry.GetValueOrDefault("Desktop Entry:Name");
                var startCommand = desktopEntry.GetValueOrDefault("Desktop Entry:Exec");

                if (string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(startCommand))
                {
                    continue;
                }

                var executePath = desktopEntry.GetValueOrDefault("Desktop Entry:Path");
                
                installedSoftwares.Add(new ApplicationModel
                {
                    DisplayName = displayName,
                    ExecutePath = executePath,
                    Arguments = startCommand
                });
            }

            return installedSoftwares;
        }

        private class IniFileReader
        {
            public Dictionary<string, string> ReadFile(Stream stream)
            {
                const string keyDelimiter = ":";

                var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                using var reader = new StreamReader(stream);
                var sectionPrefix = string.Empty;

                while (reader.Peek() != -1)
                {
                    var rawLine = reader.ReadLine();
                    var line = rawLine?.Trim();

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    switch (line[0])
                    {
                        case ';':
                        case '#':
                        case '/':
                            continue;
                        case '[' when line[^1] == ']':
                            sectionPrefix = line.Substring(1, line.Length - 2) + keyDelimiter;
                            continue;
                    }

                    var separator = line.IndexOf('=');
                    if (separator < 0)
                    {
                        continue;
                    }

                    var key = sectionPrefix + line.Substring(0, separator).Trim();
                    var value = line.Substring(separator + 1).Trim();

                    if (value.Length > 1 && value[0] == '"' && value[^1] == '"')
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    if (!data.ContainsKey(key))
                    {
                        data[key] = value;
                    }
                }

                return data;
            }
        }
    }
}