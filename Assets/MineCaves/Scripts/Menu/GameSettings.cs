using System;
using UnityEngine;

//
public static class GameSettings
{
    private const string KeyMusicVolume = "settings.musicVolume";
    private const string KeySfxVolume = "settings.sfxVolume";
    private const string KeyMusicEnabled = "settings.musicEnabled";
    private const string KeyLanguage = "settings.language";
    private const string KeyShowJoystick = "settings.showJoystick";
    private const string KeyDebugOverlay = "settings.debugOverlay";
    private const string KeyChunkBorders = "settings.chunkBorders";

    public static readonly string[] LanguageCodes = { "en", "ru", "uk" };
    public static readonly string[] LanguageNames = { "English", "Russian", "Ukrainian" };

    public static event Action Changed;

    private static bool loaded;
    private static float musicVolume;
    private static float sfxVolume;
    private static bool musicEnabled;
    private static string language;
    private static bool showJoystick;
    private static bool showDebugOverlay;
    private static bool showChunkBorders;

    public static float MusicVolume { get { EnsureLoaded(); return musicVolume; } }
    public static float SfxVolume { get { EnsureLoaded(); return sfxVolume; } }
    public static bool MusicEnabled { get { EnsureLoaded(); return musicEnabled; } }
    public static string Language { get { EnsureLoaded(); return language; } }
    public static bool ShowJoystick { get { EnsureLoaded(); return showJoystick; } }
    public static bool ShowDebugOverlay { get { EnsureLoaded(); return showDebugOverlay; } }
    public static bool ShowChunkBorders { get { EnsureLoaded(); return showChunkBorders; } }

    public static int LanguageIndex
    {
        get
        {
            int index = Array.IndexOf(LanguageCodes, Language);
            return index < 0 ? 0 : index;
        }
    }

    private static void EnsureLoaded()
    {
        if (loaded) return;
        loaded = true;

        musicVolume = PlayerPrefs.GetFloat(KeyMusicVolume, 0.8f);
        sfxVolume = PlayerPrefs.GetFloat(KeySfxVolume, 1f);
        musicEnabled = PlayerPrefs.GetInt(KeyMusicEnabled, 1) == 1;
        language = PlayerPrefs.GetString(KeyLanguage, DetectSystemLanguage());
        showJoystick = PlayerPrefs.GetInt(KeyShowJoystick, 1) == 1;
        showDebugOverlay = PlayerPrefs.GetInt(KeyDebugOverlay, 0) == 1;
        showChunkBorders = PlayerPrefs.GetInt(KeyChunkBorders, 0) == 1;
    }

    private static string DetectSystemLanguage()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Russian: return "ru";
            case SystemLanguage.Ukrainian: return "uk";
            default: return "en";
        }
    }

    #region Сеттери

    public static void SetMusicVolume(float value)
    {
        EnsureLoaded();
        musicVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(KeyMusicVolume, musicVolume);
        Changed?.Invoke();
    }

    public static void SetSfxVolume(float value)
    {
        EnsureLoaded();
        sfxVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(KeySfxVolume, sfxVolume);
        Changed?.Invoke();
    }

    public static void SetMusicEnabled(bool value)
    {
        EnsureLoaded();
        musicEnabled = value;
        PlayerPrefs.SetInt(KeyMusicEnabled, value ? 1 : 0);
        Changed?.Invoke();
    }

    public static void SetLanguage(string code)
    {
        EnsureLoaded();
        language = code;
        PlayerPrefs.SetString(KeyLanguage, code);
        Changed?.Invoke();
    }

    public static void SetShowJoystick(bool value)
    {
        EnsureLoaded();
        showJoystick = value;
        PlayerPrefs.SetInt(KeyShowJoystick, value ? 1 : 0);
        Changed?.Invoke();
    }

    public static void SetShowDebugOverlay(bool value)
    {
        EnsureLoaded();
        showDebugOverlay = value;
        PlayerPrefs.SetInt(KeyDebugOverlay, value ? 1 : 0);
        Changed?.Invoke();
    }

    public static void SetShowChunkBorders(bool value)
    {
        EnsureLoaded();
        showChunkBorders = value;
        PlayerPrefs.SetInt(KeyChunkBorders, value ? 1 : 0);
        Changed?.Invoke();
    }

    #endregion

    public static void Save()
    {
        PlayerPrefs.Save();
    }
}
