using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsApplier : MonoBehaviour
{
    [Header("Audio Mixer (рекомендовано)")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string musicParam = "MusicVolume";
    [SerializeField] private string sfxParam = "SfxVolume";

    [Header("Без мікшера (запасний варіант)")]
    [SerializeField] private AudioSource[] musicSources;
    [SerializeField] private AudioSource[] sfxSources;

    private void OnEnable()
    {
        GameSettings.Changed += Apply;
    }

    private void OnDisable()
    {
        GameSettings.Changed -= Apply;
    }

    private void Start()
    {
        Apply();
    }

    private void Apply()
    {
        float music = GameSettings.MusicEnabled ? GameSettings.MusicVolume : 0f;
        float sfx = GameSettings.SfxVolume;

        if (mixer != null)
        {
            mixer.SetFloat(musicParam, ToDecibels(music));
            mixer.SetFloat(sfxParam, ToDecibels(sfx));
        }

        if (musicSources != null)
        {
            foreach (AudioSource source in musicSources) if (source != null) source.volume = music;
        }

        if (sfxSources != null)
        {
            foreach (AudioSource source in sfxSources) if (source != null) source.volume = sfx;
        }
    }

    private static float ToDecibels(float value)
    {
        if (value <= 0.0001f) return -80f;
        return Mathf.Log10(value) * 20f;
    }
}
