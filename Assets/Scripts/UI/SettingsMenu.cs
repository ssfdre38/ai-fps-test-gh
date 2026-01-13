using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FPSGame.Systems;
using FPSGame.Player;

namespace FPSGame.UI
{
    /// <summary>
    /// Manages game settings including audio, graphics, and controls.
    /// </summary>
    public class SettingsMenu : MonoBehaviour
    {
        [Header("Audio Settings")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        
        [Header("Graphics Settings")]
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private Toggle vsyncToggle;
        
        [Header("Control Settings")]
        [SerializeField] private Slider sensitivitySlider;
        [SerializeField] private TextMeshProUGUI sensitivityValueText;
        [SerializeField] private Toggle invertYToggle;
        
        [Header("Buttons")]
        [SerializeField] private Button applyButton;
        [SerializeField] private Button backButton;

        private Resolution[] resolutions;

        private void Start()
        {
            SetupResolutions();
            LoadSettings();
            SetupButtons();
        }

        /// <summary>
        /// Sets up available resolutions.
        /// </summary>
        private void SetupResolutions()
        {
            resolutions = Screen.resolutions;
            
            if (resolutionDropdown != null)
            {
                resolutionDropdown.ClearOptions();
                
                var options = new System.Collections.Generic.List<string>();
                int currentResolutionIndex = 0;
                
                for (int i = 0; i < resolutions.Length; i++)
                {
                    string option = $"{resolutions[i].width} x {resolutions[i].height}";
                    options.Add(option);
                    
                    if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                    {
                        currentResolutionIndex = i;
                    }
                }
                
                resolutionDropdown.AddOptions(options);
                resolutionDropdown.value = currentResolutionIndex;
                resolutionDropdown.RefreshShownValue();
            }
        }

        /// <summary>
        /// Sets up button listeners.
        /// </summary>
        private void SetupButtons()
        {
            if (applyButton != null)
                applyButton.onClick.AddListener(ApplySettings);
            
            if (backButton != null)
                backButton.onClick.AddListener(Back);
            
            // Add listeners for sliders
            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            
            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            
            if (sensitivitySlider != null)
                sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }

        /// <summary>
        /// Loads saved settings.
        /// </summary>
        private void LoadSettings()
        {
            // Audio settings
            if (masterVolumeSlider != null)
                masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
            
            if (musicVolumeSlider != null)
                musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            
            // Graphics settings
            if (qualityDropdown != null)
                qualityDropdown.value = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
            
            if (fullscreenToggle != null)
                fullscreenToggle.isOn = Screen.fullScreen;
            
            if (vsyncToggle != null)
                vsyncToggle.isOn = QualitySettings.vSyncCount > 0;
            
            // Control settings
            if (sensitivitySlider != null)
            {
                float sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 2f);
                sensitivitySlider.value = sensitivity;
                UpdateSensitivityText(sensitivity);
            }
            
            if (invertYToggle != null)
                invertYToggle.isOn = PlayerPrefs.GetInt("InvertY", 0) == 1;
        }

        /// <summary>
        /// Applies all settings.
        /// </summary>
        public void ApplySettings()
        {
            // Audio
            if (AudioManager.Instance != null)
            {
                if (masterVolumeSlider != null)
                    AudioManager.Instance.MasterVolume = masterVolumeSlider.value;
                
                if (musicVolumeSlider != null)
                    AudioManager.Instance.MusicVolume = musicVolumeSlider.value;
                
                if (sfxVolumeSlider != null)
                    AudioManager.Instance.SFXVolume = sfxVolumeSlider.value;
            }
            
            // Graphics
            if (qualityDropdown != null)
                QualitySettings.SetQualityLevel(qualityDropdown.value);
            
            if (resolutionDropdown != null)
            {
                Resolution resolution = resolutions[resolutionDropdown.value];
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            }
            
            if (fullscreenToggle != null)
                Screen.fullScreen = fullscreenToggle.isOn;
            
            if (vsyncToggle != null)
                QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
            
            // Controls
            if (sensitivitySlider != null)
            {
                // Find camera controller and update sensitivity
                var cameraController = FindObjectOfType<CameraController>();
                if (cameraController != null)
                {
                    cameraController.MouseSensitivity = sensitivitySlider.value;
                }
            }
            
            SaveSettings();
        }

        /// <summary>
        /// Saves settings to PlayerPrefs.
        /// </summary>
        private void SaveSettings()
        {
            // Audio
            if (masterVolumeSlider != null)
                PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
            
            if (musicVolumeSlider != null)
                PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
            
            if (sfxVolumeSlider != null)
                PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
            
            // Graphics
            if (qualityDropdown != null)
                PlayerPrefs.SetInt("QualityLevel", qualityDropdown.value);
            
            // Controls
            if (sensitivitySlider != null)
                PlayerPrefs.SetFloat("MouseSensitivity", sensitivitySlider.value);
            
            if (invertYToggle != null)
                PlayerPrefs.SetInt("InvertY", invertYToggle.isOn ? 1 : 0);
            
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Called when master volume changes.
        /// </summary>
        private void OnMasterVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.MasterVolume = value;
            }
        }

        /// <summary>
        /// Called when music volume changes.
        /// </summary>
        private void OnMusicVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.MusicVolume = value;
            }
        }

        /// <summary>
        /// Called when SFX volume changes.
        /// </summary>
        private void OnSFXVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SFXVolume = value;
            }
        }

        /// <summary>
        /// Called when sensitivity changes.
        /// </summary>
        private void OnSensitivityChanged(float value)
        {
            UpdateSensitivityText(value);
        }

        /// <summary>
        /// Updates the sensitivity display text.
        /// </summary>
        private void UpdateSensitivityText(float value)
        {
            if (sensitivityValueText != null)
            {
                sensitivityValueText.text = value.ToString("F1");
            }
        }

        /// <summary>
        /// Returns to previous menu.
        /// </summary>
        public void Back()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Resets all settings to default.
        /// </summary>
        public void ResetToDefault()
        {
            if (masterVolumeSlider != null)
                masterVolumeSlider.value = 1f;
            
            if (musicVolumeSlider != null)
                musicVolumeSlider.value = 1f;
            
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.value = 1f;
            
            if (qualityDropdown != null)
                qualityDropdown.value = 2; // Medium quality
            
            if (fullscreenToggle != null)
                fullscreenToggle.isOn = true;
            
            if (vsyncToggle != null)
                vsyncToggle.isOn = true;
            
            if (sensitivitySlider != null)
                sensitivitySlider.value = 2f;
            
            if (invertYToggle != null)
                invertYToggle.isOn = false;
            
            ApplySettings();
        }
    }
}
