using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;
using Game.Rendering;
using Unity.Entities;
using UnityEngine;

namespace LodControl
{
    [FileLocation("ModsSettings/" + nameof(LodControl) + "/" + nameof(LodControl))]
    [SettingsUIGroupOrder(kGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";
        public const string kGroup = "Group";

        public Setting(IMod mod) : base(mod)
        {
        }



        private int _levelOfDetail;
        [SettingsUISlider(min = 1f, max = 1000f, step = 1f, unit = "percentage", scalarMultiplier = 100f)]
        [SettingsUISection(kSection, kGroup)]
        public int LevelOfDetail
        {
            get => _levelOfDetail;
            set
            {
                ChangeLodDistance(value);
                _levelOfDetail = value;
            }
        }

        public override void SetDefaults()
        {
            
        }

        [SettingsUIButton]
        public bool PrintLodSettingButton
        {
            set => PrintLodSetting();
        }

        public static void PrintLodSetting()
        {
            //SharedSettings.instance.graphics.levelOfDetail;
            RenderingSystem renderingSystem = World.DefaultGameObjectInjectionWorld?.GetExistingSystemManaged<RenderingSystem>();
            if (renderingSystem != null)
            {
                var distance = renderingSystem.levelOfDetail;
                Mod.log.Info("Level of Detail: " + distance);
            }
        }

        public void ChangeLodDistance(int value)
        {
            //SharedSettings.instance.graphics.levelOfDetail;
            /*RenderingSystem renderingSystem = World.DefaultGameObjectInjectionWorld?.GetExistingSystemManaged<RenderingSystem>();
            if (renderingSystem != null)
            {
                renderingSystem.levelOfDetail = _lodDistance;
            }*/
            var lod = SharedSettings.instance.graphics
                .GetQualitySetting<LevelOfDetailQualitySettings>();
            lod.levelOfDetail = value;
            lod.Apply();
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
                    $"Change distance where Level of Detail applies. Base game has values from 0-100%, this allows more freedom."
                },
            };
        }

        public void Unload()
        {
        }
    }
}