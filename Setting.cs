using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;

namespace LodControl
{
    [FileLocation("ModsSettings/" + nameof(LodControl) + "/" + nameof(LodControl))]
    [SettingsUIGroupOrder(kButtonGroup, kToggleGroup, kSliderGroup, kDropdownGroup)]
    [SettingsUIShowGroupName(kButtonGroup, kToggleGroup, kSliderGroup, kDropdownGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";

        public const string kButtonGroup = "Button";
        public const string kToggleGroup = "Toggle";
        public const string kSliderGroup = "Slider";
        public const string kDropdownGroup = "Dropdown";

        public Setting(IMod mod) : base(mod)
        {
        }



        private int _lodDistance;
        [SettingsUISlider(min = 10, max = 1000, step = 25, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(kSection, kSliderGroup)]
        public int LodDistance
        {
            get => _lodDistance;
            set
            {
                ChangeLodDistance(value);
                _lodDistance = value;
            }
        }

        public override void SetDefaults()
        {
            
        }

        public void ChangeLodDistance(int value)
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
                { m_Setting.GetSettingsLocaleID(), "LOD Control" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },
                
                { m_Setting.GetOptionGroupLocaleID(Setting.kSliderGroup), "Sliders" },
                
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LodDistance)), "Int slider" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.LodDistance)),
                    $"Use int property with getter and setter and [{nameof(SettingsUISliderAttribute)}] to get int slider"
                },
            };
        }

        public void Unload()
        {
        }
    }
}