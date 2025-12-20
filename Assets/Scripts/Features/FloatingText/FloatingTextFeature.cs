using Core;
using Services;
using UnityEngine;

namespace Game
{
    public class FloatingTextFeature : BaseFeature, IFloatingText
    {
        [Inject] public IHud Hud { get; set; }
        [Inject] public ISummoningService Summoner { get; set; }

        private FloatingTextWidget _floatingTextPrefab;

        public void ShowDamageText(string damageText, Transform anchor, FloatingTextPresetSO preset)
        {
            if (!Hud.IsReady)
            {
                Notebook.NoteError("Cannot show floating text: Hud is not ready");
                return;
            }

            if (anchor == null)
            {
                Notebook.NoteError("Cannot show floating text: anchor transform is null");
                return;
            }

            if (preset == null)
            {
                Notebook.NoteError("Cannot show floating text: preset is null");
                return;
            }

            // Load the prefab if not already loaded
            if (_floatingTextPrefab == null)
            {
                _floatingTextPrefab = Summoner.LoadResource<FloatingTextWidget>(Addresses.FloatingText);
                if (_floatingTextPrefab == null)
                {
                    Notebook.NoteError("Failed to load FloatingTextWidget prefab");
                    return;
                }
            }

            // Create the floating text widget using the Hud system
            var floatingTextWidget = Hud.CreateWidget(_floatingTextPrefab, anchor);
            if (floatingTextWidget == null)
            {
                Notebook.NoteError("Failed to create floating text widget");
                return;
            }

            // Set the text and preset to start the animation, with completion callback
            var floatingTextComponent = floatingTextWidget.GetComponent<FloatingTextWidget>();
            floatingTextComponent.SetText(damageText, preset, () => Hud.DestroyWidget(anchor));
        }
    }
}
