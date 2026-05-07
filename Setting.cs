using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Input;
using Game.Modding;
using Game.Rendering;
using Game.Settings;
using System.Collections.Generic;
using Unity.Entities;

namespace LodControl
{
    [FileLocation("ModsSettings/" + nameof(LodControl) + "/" + nameof(LodControl))]
    [SettingsUITabOrder(kSection, kPresetsSection)]
    [SettingsUIGroupOrder(kGroup, kPreset1Group, kPreset2Group, kPreset3Group)]
    [SettingsUIShowGroupName(kPreset1Group, kPreset2Group, kPreset3Group)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";
        public const string kPresetsSection = "Presets";
        public const string kGroup = "MainGroup";
        public const string kPreset1Group = "Preset1Group";
        public const string kPreset2Group = "Preset2Group";
        public const string kPreset3Group = "Preset3Group";
        public const string kVanillaPresetButtonGroup = "VanillaPresetButtonGroup";
        public const string kToggleDisableLodAction = nameof(ToggleDisableLodModelsBinding);
        public const string kPreset1Action = nameof(Preset1Binding);
        public const string kPreset2Action = nameof(Preset2Binding);
        public const string kPreset3Action = nameof(Preset3Binding);

        private readonly RenderingSystem _renderingSystem;
        private float _levelOfDetail;
        private float _preset1Value = 1f;
        private float _preset2Value = 1f;
        private float _preset3Value = 1f;

        public Setting(IMod mod) : base(mod)
        {
            _renderingSystem = World.DefaultGameObjectInjectionWorld?.GetExistingSystemManaged<RenderingSystem>();
        }

        [SettingsUISection(kSection, kGroup)]
        [SettingsUISlider(min = 0f, max = 1000f, step = 10f, unit = "percentage", scalarMultiplier = 100f)]
        public float LevelOfDetail
        {
            get => _levelOfDetail;
            set
            {
                ChangeLodDistance(value);
                _levelOfDetail = value;
            }
        }

        [SettingsUISection(kSection, kGroup)]
        [SettingsUIButton]
        [SettingsUIButtonGroup(kVanillaPresetButtonGroup)]
        public bool VeryLowPreset
        {
            set => ApplyLodValue(0.25f);
        }

        [SettingsUISection(kSection, kGroup)]
        [SettingsUIButton]
        [SettingsUIButtonGroup(kVanillaPresetButtonGroup)]
        public bool LowPreset
        {
            set => ApplyLodValue(0.35f);
        }

        [SettingsUISection(kSection, kGroup)]
        [SettingsUIButton]
        [SettingsUIButtonGroup(kVanillaPresetButtonGroup)]
        public bool MediumPreset
        {
            set => ApplyLodValue(0.5f);
        }

        [SettingsUISection(kSection, kGroup)]
        [SettingsUIButton]
        [SettingsUIButtonGroup(kVanillaPresetButtonGroup)]
        public bool HighPreset
        {
            set => ApplyLodValue(0.7f);
        }

        [SettingsUISection(kSection, kGroup)]
        public bool DisableLodModels
        {
            get => _renderingSystem.disableLodModels;
            set => _renderingSystem.disableLodModels = value;
        }

        [SettingsUISection(kSection, kGroup)]
        [SettingsUIKeyboardBinding(BindingKeyboard.None, kToggleDisableLodAction)]
        public ProxyBinding ToggleDisableLodModelsBinding { get; set; }

        [SettingsUISection(kPresetsSection, kPreset1Group)]
        [SettingsUISlider(min = 0f, max = 1000f, step = 10f, unit = "percentage", scalarMultiplier = 100f)]
        public float Preset1Value
        {
            get => _preset1Value;
            set => _preset1Value = value;
        }

        [SettingsUISection(kPresetsSection, kPreset1Group)]
        [SettingsUIKeyboardBinding(BindingKeyboard.None, kPreset1Action)]
        public ProxyBinding Preset1Binding { get; set; }

        [SettingsUISection(kPresetsSection, kPreset2Group)]
        [SettingsUISlider(min = 0f, max = 1000f, step = 10f, unit = "percentage", scalarMultiplier = 100f)]
        public float Preset2Value
        {
            get => _preset2Value;
            set => _preset2Value = value;
        }

        [SettingsUISection(kPresetsSection, kPreset2Group)]
        [SettingsUIKeyboardBinding(BindingKeyboard.None, kPreset2Action)]
        public ProxyBinding Preset2Binding { get; set; }

        [SettingsUISection(kPresetsSection, kPreset3Group)]
        [SettingsUISlider(min = 0f, max = 1000f, step = 10f, unit = "percentage", scalarMultiplier = 100f)]
        public float Preset3Value
        {
            get => _preset3Value;
            set => _preset3Value = value;
        }

        [SettingsUISection(kPresetsSection, kPreset3Group)]
        [SettingsUIKeyboardBinding(BindingKeyboard.None, kPreset3Action)]
        public ProxyBinding Preset3Binding { get; set; }

        public void ApplyLodValue(float value)
        {
            DisableLodModels = false;
            LevelOfDetail = value;
        }

        private void ChangeLodDistance(float value)
        {
            var lod = SharedSettings.instance.graphics
                .GetQualitySetting<LevelOfDetailQualitySettings>();
            lod.levelOfDetail = value;
            lod.Apply();
        }

        public override void SetDefaults()
        {
            LevelOfDetail = 1f;
            DisableLodModels = false;
            Preset1Value = 1f;
            Preset2Value = 1f;
            Preset3Value = 1f;
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "LOD Control" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },
                { m_Setting.GetOptionTabLocaleID(Setting.kPresetsSection), "Presets" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kGroup), "Main" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kPreset1Group), "Preset 1" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kPreset2Group), "Preset 2" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kPreset3Group), "Preset 3" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LevelOfDetail)), "Level of Detail" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.LevelOfDetail)),
                    "Change distance where Level of Detail applies. Base game has values from 0-100%, this allows more freedom."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VeryLowPreset)), "Very Low" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.VeryLowPreset)),
                    "The vanilla setting when graphics are set to 'Very Low'."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LowPreset)), "Low" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.LowPreset)),
                    "The vanilla setting when graphics are set to 'Low'."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MediumPreset)), "Medium" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.MediumPreset)),
                    "The vanilla setting when graphics are set to 'Medium'."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.HighPreset)), "High" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.HighPreset)),
                    "The vanilla setting when graphics are set to 'High'."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DisableLodModels)), "Disable LOD Models" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.DisableLodModels)),
                    "Completely disable LOD Models. This renders everything at maximum quality. Not suitable for gameplay, but useful for screenshots"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ToggleDisableLodModelsBinding)), "Hotkey" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ToggleDisableLodModelsBinding)),
                    "Hotkey to toggle the 'Disable LOD Models' setting on or off."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Preset1Value)), "Value" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.Preset1Value)),
                    "Value applied when Preset 1 is triggered."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Preset1Binding)), "Hotkey" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.Preset1Binding)),
                    "Hotkey that applies Preset 1."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Preset2Value)), "Value" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.Preset2Value)),
                    "Value applied when Preset 2 is triggered."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Preset2Binding)), "Hotkey" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.Preset2Binding)),
                    "Hotkey that applies Preset 2."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Preset3Value)), "Value" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.Preset3Value)),
                    "Value applied when Preset 3 is triggered."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Preset3Binding)), "Hotkey" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.Preset3Binding)),
                    "Hotkey that applies Preset 3."
                },

                { m_Setting.GetBindingMapLocaleID(), "LOD Control" },
                { m_Setting.GetBindingKeyLocaleID(Setting.kToggleDisableLodAction), "Toggle Disable LOD Models" },
                { m_Setting.GetBindingKeyLocaleID(Setting.kPreset1Action), "Preset 1" },
                { m_Setting.GetBindingKeyLocaleID(Setting.kPreset2Action), "Preset 2" },
                { m_Setting.GetBindingKeyLocaleID(Setting.kPreset3Action), "Preset 3" },
            };
        }

        public void Unload()
        {
        }
    }
}
