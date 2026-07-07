using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixer audioMixer;

    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static AudioManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    void Start()
    {
        SetupVolumeControl(masterSlider, "Master", "MasterVolume");
        SetupVolumeControl(bgmSlider, "BGM", "BGMVolume");
        SetupVolumeControl(sfxSlider, "SFX", "SFXVolume");
    }

    void SetupVolumeControl(Slider slider, string mixerParam, string prefsKey)
    {
        float savedVol = PlayerPrefs.GetFloat(prefsKey, 0.75f);
        slider.value = savedVol;
        ApplyVolume(mixerParam, prefsKey, savedVol);

        slider.onValueChanged.AddListener(val => ApplyVolume(mixerParam, prefsKey, val));
    }

    public void ApplyVolume(string mixerParam, string prefsKey, float value)
    {
        float dB = value <= 0 ? -80f : Mathf.Log10(value) * 20f;
        audioMixer.SetFloat(mixerParam, dB);
        PlayerPrefs.SetFloat(prefsKey, value);
    }
}