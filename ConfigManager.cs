﻿using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using LethalConfig;

namespace Waterwraith
{
    class ConfigManager
    {
        public static void setupLethalConfig()
        {
            var sizeSlider = new FloatSliderConfigItem(Plugin.configSize, new FloatSliderOptions
            {
                Min = 75f,
                Max = 225f
            });

            var speedSlider = new FloatSliderConfigItem(Plugin.configSpeed, new FloatSliderOptions
            {
                Min = 0.1f,
                Max = 2.0f
            });

            var maxSnailsSlider = new IntSliderConfigItem(Plugin.configMaxSnails, new IntSliderOptions
            {
                Min = 0,
                Max = 10
            });

            var raritySlider = new IntSliderConfigItem(Plugin.configRarity, new IntSliderOptions
            {
                Min = 0,
                Max = 100
            });

            var goOutsideBox = new BoolCheckBoxConfigItem(Plugin.configGoOutside, requiresRestart: false);
            var enterShipBox = new BoolCheckBoxConfigItem(Plugin.configEnterShip, requiresRestart: false);
            var showTargetBox = new BoolCheckBoxConfigItem(Plugin.configShowTarget, requiresRestart: false);

            LethalConfigManager.AddConfigItem(sizeSlider);
            LethalConfigManager.AddConfigItem(speedSlider);
            LethalConfigManager.AddConfigItem(maxSnailsSlider);
            LethalConfigManager.AddConfigItem(raritySlider);
            LethalConfigManager.AddConfigItem(goOutsideBox);
            LethalConfigManager.AddConfigItem(enterShipBox);
            LethalConfigManager.AddConfigItem(showTargetBox);
        }
    }
}
