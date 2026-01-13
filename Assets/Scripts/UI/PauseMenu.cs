using UnityEngine;
using UnityEngine.UI;
using FPSGame.Systems;
using FPSGame.Utilities;

namespace FPSGame.UI
{
    /// <summary>
    /// Manages the pause menu UI.
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [Header("Pause Menu")]
        [SerializeField] private GameObject pauseMenuPanel;
        
        [Header("Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;
        
        [Header("Sub Panels")]
        [SerializeField] private GameObject optionsPanel;

        private bool isPaused = false;

        private void Start()
        {
            SetupButtons();
            
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(false);
            
            if (optionsPanel != null)
                optionsPanel.SetActive(false);
        }

        private void Update()
        {
            // Listen for pause input
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        /// <summary>
        /// Sets up button listeners.
        /// </summary>
        private void SetupButtons()
        {
            if (resumeButton != null)
                resumeButton.onClick.AddListener(Resume);
            
            if (optionsButton != null)
                optionsButton.onClick.AddListener(ShowOptions);
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(LoadMainMenu);
            
            if (quitButton != null)
                quitButton.onClick.AddListener(QuitGame);
        }

        /// <summary>
        /// Pauses the game.
        /// </summary>
        public void Pause()
        {
            isPaused = true;
            
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(true);
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PauseGame();
            }
            else
            {
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        /// <summary>
        /// Resumes the game.
        /// </summary>
        public void Resume()
        {
            isPaused = false;
            
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(false);
            
            if (optionsPanel != null)
                optionsPanel.SetActive(false);
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
            }
            else
            {
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        /// <summary>
        /// Shows the options panel.
        /// </summary>
        public void ShowOptions()
        {
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(false);
            
            if (optionsPanel != null)
                optionsPanel.SetActive(true);
        }

        /// <summary>
        /// Hides the options panel.
        /// </summary>
        public void HideOptions()
        {
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(true);
            
            if (optionsPanel != null)
                optionsPanel.SetActive(false);
        }

        /// <summary>
        /// Loads the main menu.
        /// </summary>
        public void LoadMainMenu()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadMainMenu();
            }
            else
            {
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }

        /// <summary>
        /// Quits the game.
        /// </summary>
        public void QuitGame()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuitGame();
            }
            else
            {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
            }
        }
    }
}
