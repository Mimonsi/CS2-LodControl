using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Input;
using Game.Modding;
using Game.SceneFlow;
using UnityEngine.InputSystem;

namespace LodControl
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(LodControl)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
        private Setting m_Setting;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));

            AssetDatabase.global.LoadSettings(nameof(LodControl), m_Setting, new Setting(this));

            m_Setting.RegisterKeyBindings();

            RegisterAction(Setting.kToggleDisableLodAction, OnToggleDisableLod);
            RegisterAction(Setting.kPreset1Action, (_, phase) => OnApplyPreset(phase, m_Setting.Preset1Value, "Preset 1"));
            RegisterAction(Setting.kPreset2Action, (_, phase) => OnApplyPreset(phase, m_Setting.Preset2Value, "Preset 2"));
            RegisterAction(Setting.kPreset3Action, (_, phase) => OnApplyPreset(phase, m_Setting.Preset3Value, "Preset 3"));
        }

        private void RegisterAction(string actionName, System.Action<ProxyAction, InputActionPhase> handler)
        {
            var action = m_Setting.GetAction(actionName);
            action.shouldBeEnabled = true;
            action.onInteraction += handler;
        }

        private void OnToggleDisableLod(ProxyAction action, InputActionPhase phase)
        {
            if (phase != InputActionPhase.Performed) return;
            m_Setting.DisableLodModels = !m_Setting.DisableLodModels;
            log.Info($"DisableLodModels toggled to {m_Setting.DisableLodModels}");
        }

        private void OnApplyPreset(InputActionPhase phase, float lodValue, string presetName)
        {
            if (phase != InputActionPhase.Performed) return;
            m_Setting.ApplyLodValue(lodValue);
            log.Info($"{presetName} applied with LevelOfDetail {lodValue}");
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }
    }
}
