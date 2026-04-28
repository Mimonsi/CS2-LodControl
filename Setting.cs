using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Input;
using Game.Modding;
using Game.Settings;
using System.Collections.Generic;
using Game.Rendering;
using Unity.Entities;

namespace LodControl
{
    [FileLocation("ModsSettings/" + nameof(LodControl) + "/" + nameof(LodControl))]
    [SettingsUIGroupOrder(kGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";
        public const string kGroup = "Group";
        public const string kToggleDisableLodAction = nameof(ToggleDisableLodModelsBinding);
        private readonly RenderingSystem _renderingSystem;

        public Setting(IMod mod) : base(mod)
        {
            _renderingSystem = World.DefaultGameObjectInjectionWorld?.GetExistingSystemManaged<RenderingSystem>();
        }

        private float _levelOfDetail;

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

        public bool DisableLodModels
        {
            get => _renderingSystem.disableLodModels;
            set => _renderingSystem.disableLodModels = value;
        }

        [SettingsUIKeyboardBinding(BindingKeyboard.None, kToggleDisableLodAction)]
        public ProxyBinding ToggleDisableLodModelsBinding { get; set; }

        private void ChangeLodDistance(float value)
        {
            var lod = SharedSettings.instance.graphics
                .GetQualitySetting<LevelOfDetailQualitySettings>();
            lod.levelOfDetail = value;
            lod.Apply();
        }

        public override void SetDefaults()
        {
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
                { m_Setting.GetSettingsLocaleID(), "level of Detail Control" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kGroup), "Group" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LevelOfDetail)), "Level of Detail" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.LevelOfDetail)),
                    "Change distance where Level of Detail applies. Base game has values from 0-100%, this allows more freedom."
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DisableLodModels)), "Disable LOD Models" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.DisableLodModels)),
                    "Completely disable LOD Models. This renders everything at maximum quality. Not suitable for gameplay, but useful for screenshots"
                },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ToggleDisableLodModelsBinding)), "Toggle Disable LOD Models" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ToggleDisableLodModelsBinding)),
                    "Hotkey to toggle the 'Disable LOD Models' setting on or off."
                },
                { m_Setting.GetBindingMapLocaleID(), "LOD Control" },
                { m_Setting.GetBindingKeyLocaleID(Setting.kToggleDisableLodAction), "Toggle Disable LOD Models" },
            };
        }

        public void Unload()
        {
        }
    }
}