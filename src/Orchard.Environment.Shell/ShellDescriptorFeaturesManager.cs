﻿using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Features;
using Orchard.Environment.Shell.Descriptor;
using Orchard.Environment.Shell.Descriptor.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orchard.Environment.Shell
{
    public class ShellDescriptorFeaturesManager : IShellDescriptorFeaturesManager
    {
        private readonly IExtensionManager _extensionManager;
        private readonly IShellDescriptorManager _shellDescriptorManager;

        private readonly ILogger<ShellFeaturesManager> _logger;

        public FeatureDependencyNotificationHandler FeatureDependencyNotification { get; set; }

        public ShellDescriptorFeaturesManager(IExtensionManager extensionManager,
            IShellDescriptorManager shellDescriptorManager,
            ILogger<ShellFeaturesManager> logger,
            IStringLocalizer<ShellFeaturesManager> localizer)
        {
            _extensionManager = extensionManager;
            _shellDescriptorManager = shellDescriptorManager;

            _logger = logger;
            T = localizer;
        }
        public IStringLocalizer T { get; set; }

        public Task<IEnumerable<IFeatureInfo>> EnableFeaturesAsync(ShellDescriptor shellDescriptor, IEnumerable<IFeatureInfo> features)
        {
            return EnableFeaturesAsync(shellDescriptor, features, false);
        }

        public async Task<IEnumerable<IFeatureInfo>> EnableFeaturesAsync(ShellDescriptor shellDescriptor, IEnumerable<IFeatureInfo> features, bool force)
        {
            var featuresToEnable = features
                .SelectMany(feature => GetFeaturesToEnable(feature, false))
                .Distinct()
                .ToList();

            if (featuresToEnable.Count > 0)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Enabling features {0}", string.Join(",", featuresToEnable.Select(x => x.Id)));
                }

                var enabledFeatures = await _extensionManager
                    .LoadFeaturesAsync(shellDescriptor.Features.Select(x => x.Id).ToArray());

                shellDescriptor.Features = enabledFeatures
                    .Select(x => x.FeatureInfo)
                    .Concat(featuresToEnable)
                    .Distinct()
                    .Select(x => new ShellFeature(x.Id))
                    .ToList();

                await _shellDescriptorManager.UpdateShellDescriptorAsync(
                    shellDescriptor.SerialNumber,
                    shellDescriptor.Features,
                    shellDescriptor.Parameters);
            }

            return featuresToEnable;
        }

        /// <summary>
        /// Disables a list of features.
        /// </summary>
        /// <param name="featureIds">The IDs for the features to be disabled.</param>
        /// <returns>An enumeration with the disabled feature IDs.</returns>
        public Task<IEnumerable<IFeatureInfo>> DisableFeaturesAsync(ShellDescriptor shellDescriptor, IEnumerable<IFeatureInfo> features)
        {
            return DisableFeaturesAsync(shellDescriptor, features, false);
        }

        /// <summary>
        /// Disables a list of features.
        /// </summary>
        /// <param name="features">The features to be disabled.</param>
        /// <param name="force">Boolean parameter indicating if the feature should disable the features which depend on it if required or fail otherwise.</param>
        /// <returns>An enumeration with the disabled feature IDs.</returns>
        public async Task<IEnumerable<IFeatureInfo>> DisableFeaturesAsync(ShellDescriptor shellDescriptor, IEnumerable<IFeatureInfo> features, bool force)
        {
            var featuresToDisable = features
                .SelectMany(feature => GetFeaturesToDisable(feature, force))
                .Distinct()
                .ToList();

            if (featuresToDisable.Count > 0)
            {
                var leadedFeatures = await _extensionManager
                    .LoadFeaturesAsync(shellDescriptor.Features.Select(x => x.Id).ToArray());

                var enabledFeatures = leadedFeatures.Select(x => x.FeatureInfo).ToList();

                foreach (var feature in featuresToDisable)
                {
                    enabledFeatures.Remove(feature);

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("{0} was disabled", feature.Id);
                    }
                }

                shellDescriptor.Features = enabledFeatures.Select(x => new ShellFeature(x.Id)).ToList();

                await _shellDescriptorManager.UpdateShellDescriptorAsync(
                    shellDescriptor.SerialNumber,
                    shellDescriptor.Features,
                    shellDescriptor.Parameters);
            }

            return featuresToDisable;
        }

        /// <summary>
        /// Enables a feature.
        /// </summary>
        /// <param name="featureId">The ID of the feature to be enabled.</param>
        /// <param name="availableFeatures">A dictionary of the available feature descriptors and their current state (enabled / disabled).</param>
        /// <param name="force">Boolean parameter indicating if the feature should enable it's dependencies if required or fail otherwise.</param>
        /// <returns>An enumeration of the enabled features.</returns>
        private IEnumerable<IFeatureInfo> GetFeaturesToEnable(
            IFeatureInfo featureInfo,
            bool force)
        {
            var featuresToEnable = _extensionManager
                .GetFeatureDependencies(featureInfo.Id)
                .ToList();

            if (featuresToEnable.Count > 1 && !force)
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning("Additional features need to be enabled.");
                }
                if (FeatureDependencyNotification != null)
                {
                    FeatureDependencyNotification("If {0} is enabled, then you'll also need to enable {1}.", featureInfo, featuresToEnable.Where(f => f.Id != featureInfo.Id));
                }
            }

            return featuresToEnable;
        }

        /// <summary>
        /// Disables a feature.
        /// </summary>
        /// <param name="featureId">The ID of the feature to be enabled.</param>
        /// <param name="force">Boolean parameter indicating if the feature should enable it's dependencies if required or fail otherwise.</param>
        /// <returns>An enumeration of the disabled features.</returns>
        private IEnumerable<IFeatureInfo> GetFeaturesToDisable(IFeatureInfo featureInfo, bool force)
        {
            var affectedFeatures = _extensionManager
                .GetDependentFeatures(featureInfo.Id)
                .ToList();

            if (affectedFeatures.Count > 1 && !force)
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning("Additional features need to be disabled.");
                }
                if (FeatureDependencyNotification != null)
                {
                    FeatureDependencyNotification("If {0} is disabled, then you'll also need to disable {1}.", featureInfo, affectedFeatures.Where(f => f.Id != featureInfo.Id));
                }
            }

            return affectedFeatures;
        }
    }
}
