using UnityEngine;
using UnityEditor;
using Services;

[CustomEditor(typeof(BaseSoundDesign), true)]
public class BaseSoundDesignEditor : Editor
{
    private GameObject _previewObject;
    private AudioSource _previewSource;
    private bool _isPlaying = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BaseSoundDesign soundDesign = (BaseSoundDesign)target;

        // Add some spacing
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Audio Preview", EditorStyles.boldLabel);

        // Display clip info
        if (soundDesign.Clip != null)
        {
            EditorGUILayout.LabelField("Clip:", soundDesign.Clip.name);
            EditorGUILayout.LabelField("Length:", $"{soundDesign.Clip.length:F2} seconds");
            EditorGUILayout.LabelField("Channels:", soundDesign.Clip.channels == 1 ? "Mono" : "Stereo");
            EditorGUILayout.LabelField("Sample Rate:", $"{soundDesign.Clip.frequency} Hz");
        }
        else
        {
            EditorGUILayout.HelpBox("No audio clip assigned!", MessageType.Warning);
        }

        EditorGUILayout.Space();

        // Play button
        EditorGUI.BeginDisabledGroup(soundDesign.Clip == null);
        if (GUILayout.Button(_isPlaying ? "Stop Preview" : "Play Preview"))
        {
            if (_isPlaying)
            {
                StopPreview();
            }
            else
            {
                PlayPreview(soundDesign);
            }
        }
        EditorGUI.EndDisabledGroup();

        // Volume and loop info
        if (soundDesign.Clip != null)
        {
            EditorGUILayout.LabelField($"Volume: {soundDesign.Volume:F2}");
            EditorGUILayout.LabelField($"Loop: {soundDesign.Loop}");
            if (soundDesign is SoundDesign sd && sd.PitchRange.x != sd.PitchRange.y)
            {
                EditorGUILayout.LabelField($"Pitch Range: {sd.PitchRange.x:F2} - {sd.PitchRange.y:F2}");
            }
            else
            {
                EditorGUILayout.LabelField($"Pitch: {soundDesign.Pitch:F2}");
            }
        }

        EditorGUILayout.HelpBox("Click 'Play Preview' to audition the sound in the editor. This works without entering play mode!", MessageType.Info);
    }

    private void PlayPreview(BaseSoundDesign soundDesign)
    {
        if (soundDesign.Clip == null) return;

        // Stop any currently playing preview
        StopPreview();

        // Create temporary GameObject with AudioSource for preview
        _previewObject = new GameObject("AudioPreview");
        _previewObject.hideFlags = HideFlags.HideAndDontSave;
        _previewSource = _previewObject.AddComponent<AudioSource>();

        // Configure the AudioSource with the sound design settings
        _previewSource.clip = soundDesign.Clip;
        _previewSource.volume = soundDesign.Volume;
        _previewSource.pitch = soundDesign.Pitch;
        _previewSource.loop = soundDesign.Loop;

        // Play the clip
        _previewSource.Play();
        _isPlaying = true;

        // Schedule the cleanup when clip ends (with a small buffer for non-looping sounds)
        if (!soundDesign.Loop)
        {
            double stopTime = EditorApplication.timeSinceStartup + soundDesign.Clip.length / soundDesign.Pitch + 0.1f;
            EditorApplication.CallbackFunction cleanupCallback = null;
            cleanupCallback = () => {
                if (EditorApplication.timeSinceStartup >= stopTime && _isPlaying)
                {
                    StopPreview();
                    Repaint();
                }
                else if (_isPlaying)
                {
                    EditorApplication.delayCall += cleanupCallback;
                }
            };
            EditorApplication.delayCall += cleanupCallback;
        }
    }

    private void StopPreview()
    {
        if (_previewSource != null)
        {
            _previewSource.Stop();
            _previewSource = null;
        }

        if (_previewObject != null)
        {
            Object.DestroyImmediate(_previewObject);
            _previewObject = null;
        }

        _isPlaying = false;
    }

    private void OnDisable()
    {
        // Make sure to stop any playing preview when the editor is disabled
        StopPreview();
    }
}
