using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class SettingsPage : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    
    [SerializeField] private List<Vector2Int> _availableResolutions;
    [SerializeField] private List<string> _availableResolutionsStrings;
    
    void Start()
    {
        _availableResolutionsStrings =
            _availableResolutions.Select(res => $"{res.x} x {res.y}").ToList();
        BindGeneral();
        BindAudio();
        BindRender();
    }

    private void BindGeneral()
    {
        var generalPage = _uiDocument.rootVisualElement.Q<VisualElement>("GeneralPage");
        var display = generalPage.Q<DropdownField>("DisplayDropdown");
        display.choices = _availableResolutionsStrings;
        display.RegisterValueChangedCallback(OnDisplayChange);
        var fullscreen = generalPage.Q<Toggle>("FullScreenToggle");
        fullscreen.value = Screen.fullScreen;
        fullscreen.RegisterCallback<ChangeEvent<bool>>(OnFullscreenChange);
    }

    private void OnFullscreenChange(ChangeEvent<bool> evt)
    {
        Screen.fullScreen = evt.newValue;
    }

    private void OnDisplayChange(ChangeEvent<string> evt)
    {
        var res = _availableResolutions[_availableResolutionsStrings.IndexOf(evt.newValue)];
        Screen.SetResolution(res.x, res.y, Screen.fullScreen);
    }

    private void BindAudio()
    {
        var audioPage = _uiDocument.rootVisualElement.Q<VisualElement>("AudioPage");
        var globalVolume = audioPage.Q<Slider>("GlobalVolume");
        var musicVolume = audioPage.Q<Slider>("MusicVolume");
        var sfxVolume = audioPage.Q<Slider>("SFXVolume");
        var voiceVolume = audioPage.Q<Slider>("VoiceVolume");
    }

    private void BindRender()
    {
        var renderPage = _uiDocument.rootVisualElement.Q<VisualElement>("RenderPage");
        var qualityPreset = renderPage.Q<DropdownField>("QualityPresetDropdown");
        var shadowQuality = renderPage.Q<DropdownField>("ShadowQualityDropdown");
        var antialiasing = renderPage.Q<DropdownField>("AntialiasingDropdown");
    }
}
