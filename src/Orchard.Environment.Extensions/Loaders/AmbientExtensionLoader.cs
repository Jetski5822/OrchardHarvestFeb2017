﻿using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Orchard.Environment.Extensions.Loaders
{
    public class AmbientExtensionLoader : IExtensionLoader
    {
        private readonly ILogger _logger;

        public AmbientExtensionLoader(ILogger<AmbientExtensionLoader> logger)
        {
            _logger = logger;
        }

        public int Order => 20;

        public ExtensionEntry Load(IExtensionInfo extensionInfo)
        {
            var assembly = Assembly.Load(new AssemblyName(extensionInfo.Id));

            if (assembly == null)
            {
                return null;
            }

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Loaded referenced ambient extension \"{0}\": assembly name=\"{1}\"", extensionInfo.Id, assembly.FullName);
            }

            return new ExtensionEntry
            {
                ExtensionInfo = extensionInfo,
                Assembly = assembly,
                ExportedTypes = assembly.ExportedTypes
            };
        }
    }
}