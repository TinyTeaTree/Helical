using Agents;
using Core;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;

namespace Game
{
    public class FloatingTextFeature : BaseFeature, IFloatingText, IAppLaunchAgent
    {
        [Inject] public IHud Hud { get; set; }
        [Inject] public ISummoningService Summoner { get; set; }

        private FloatingTextAssetPack _assetPack;

        public async UniTask AppLaunch()
        {
            _assetPack = await Summoner.LoadAssetPack<FloatingTextAssetPack>();
        }

        public void ShowDamageText(string damageText, Transform anchor)
        {
            ShowFloatingDamageText(damageText, anchor, FloatingTextPresetType.Damage);
        }

        public void ShowFloatingDamageText(string damageText, Transform anchor, FloatingTextPresetType presetType)
        {
            var preset = GetPreset(presetType);
            if (preset == null)
            {
                Notebook.NoteError($"No preset found for type: {presetType}");
                return;
            }

            var prefab = _assetPack?.GetPrefab(presetType);
            if (prefab == null)
            {
                Notebook.NoteError($"No prefab found for preset type: {presetType}");
                return;
            }

            if (!Hud.IsReady)
            {
                Notebook.NoteError("Cannot show floating text: Hud is not ready");
                return;
            }

            // Create the floating text widget using the Hud system
            var floatingTextWidget = Hud.CreateWidget(prefab, anchor);
            if (floatingTextWidget == null)
            {
                Notebook.NoteError("Failed to create floating text widget");
                return;
            }

            // Set the text and preset to start the animation, with completion callback
            var floatingTextComponent = floatingTextWidget.GetComponent<FloatingTextWidget>();
            floatingTextComponent.SetText(damageText, preset, () => Hud.DestroyWidget(floatingTextWidget));
        }

        public FloatingTextPresetSO GetPreset(FloatingTextPresetType presetType)
        {
            return _assetPack?.GetPreset(presetType);
        }
    }
}
