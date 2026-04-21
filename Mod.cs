using Colossal.Logging;
using Game;
using Game.Input;
using Game.Modding;
using Game.SceneFlow;
using Colossal.IO.AssetDatabase;
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

            var toggleAction = m_Setting.GetAction(Setting.kToggleDisableLodAction);
            toggleAction.shouldBeEnabled = true;
            toggleAction.onInteraction += OnToggleDisableLod;
        }

        private void OnToggleDisableLod(ProxyAction action, InputActionPhase phase)
        {
            if (phase != InputActionPhase.Performed) return;
            m_Setting.DisableLodModels = !m_Setting.DisableLodModels;
            log.Info($"DisableLodModels toggled to {m_Setting.DisableLodModels}");
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