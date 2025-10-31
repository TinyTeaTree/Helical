using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DebugFrameGraph : MonoBehaviour
{
    private static readonly Color32 BaseColor = new(50, 50, 50, 45);
    private static readonly Color32 SpikesColor = new(200, 100, 150, 200);

    [SerializeField] private RawImage image;
    [SerializeField] private TextMeshProUGUI framerate;

    private Texture2D _texture;
    private NativeArray<Color32> _graphData;

    private const int FramesToRecord = 100;
    private const int TargetFPS = 60;
    private const float FrameDuration = 1f / TargetFPS;
    private const int MaxFrameLag = 10;
    private const float DurationForFPSCalculation = 3f; // How many frames to average out for the medium fps

    private DateTime _lastUpdateTime;
    private DateTime _lastFramerateUpdate;

    private Queue<float> _framesRecord;
    private float _fullTime;

    void Awake()
    {
        _texture = new Texture2D(FramesToRecord, MaxFrameLag)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
        };

        _framesRecord = new();
        _fullTime = 0f;

        _graphData = _texture.GetPixelData<Color32>(0);

        for (int x = 0; x < FramesToRecord; x++)
        {
            for (int y = 0; y < MaxFrameLag; y++)
            {
                _graphData[y * FramesToRecord + x] = BaseColor;
            }
        }

        _texture.Apply();

        image.texture = _texture;

        _lastUpdateTime = DateTime.Now;
        gameObject.SetActive(PlayerPrefs.GetInt("FrameDebugGraphEnabled", 0) == 1);
    }

    private void Update()
    {
        float deltaTime = (float)(DateTime.Now - _lastUpdateTime).TotalSeconds;
        for (int y = 0; y < MaxFrameLag; y++)
        {
            int yOffset = y * FramesToRecord;

            for (int x = 0; x < FramesToRecord - 1; x++)
            {
                _graphData[yOffset + x] = _graphData[yOffset + x + 1];
            }
        }

        float spikeHeight = deltaTime / FrameDuration - 1f;

        for (int y = 0; y < MaxFrameLag; y++)
        {
            int x = FramesToRecord - 1;
            int yOffset = y * FramesToRecord;

            if (y + 1 < spikeHeight)
            {
                _graphData[yOffset + x] = SpikesColor;
            }
            else
            {
                float leftover = 1f - (y + 1 - spikeHeight);
                if (leftover > 0f)
                {
                    _graphData[yOffset + x] = Color32.Lerp(BaseColor, SpikesColor, leftover);
                }
                else
                {
                    _graphData[yOffset + x] = BaseColor;
                }
            }
        }

        _texture.Apply();

        _framesRecord.Enqueue(deltaTime);
        _fullTime += deltaTime;

        while (_fullTime > DurationForFPSCalculation && _framesRecord.Any())
        {
            var outTime = _framesRecord.Dequeue();
            _fullTime -= outTime;
        }

        framerate.text = "FPS: " + (_framesRecord.Count / (float)DurationForFPSCalculation).ToString("F1");
        _lastUpdateTime = DateTime.Now;
    }

    public void ToggleVisibility()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
        PlayerPrefs.SetInt("FrameDebugGraphEnabled", gameObject.activeInHierarchy ? 1 : 0);
        PlayerPrefs.Save();
        if (gameObject.activeInHierarchy)
        {
            _framesRecord.Clear();
            _fullTime = 0f;
            _lastUpdateTime = DateTime.Now;
        }
    }
}