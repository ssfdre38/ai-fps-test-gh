using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FPSGame.UI
{
    /// <summary>
    /// Manages the main menu UI.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [Header("Menu Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private GameObject creditsPanel;
        
        [Header("Buttons")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            ShowMainMenu();
            SetupButtons();
        }

        /// <summary>
        /// Sets up button listeners.
        /// </summary>
        private void SetupButtons()
        {
            if (startButton != null)
                startButton.onClick.AddListener(OnStartGame);
            
            if (optionsButton != null)
                optionsButton.onClick.AddListener(OnOptions);
            
            if (creditsButton != null)
                creditsButton.onClick.AddListener(OnCredits);
            
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuit);
        }

        /// <summary>
        /// Shows the main menu panel.
        /// </summary>
        private void ShowMainMenu()
        {
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(true);
            
            if (optionsPanel != null)
                optionsPanel.SetActive(false);
            
            if (creditsPanel != null)
                creditsPanel.SetActive(false);
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void OnStartGame()
        {
            // Load first level
            SceneManager.LoadScene("Level01");
        }

        /// <summary>
        /// Opens the options menu.
        /// </summary>
        public void OnOptions()
        {
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(false);
            
            if (optionsPanel != null)
                optionsPanel.SetActive(true);
        }

        /// <summary>
        /// Opens the credits panel.
        /// </summary>
        public void OnCredits()
        {
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(false);
            
            if (creditsPanel != null)
                creditsPanel.SetActive(true);
        }

        /// <summary>
        /// Quits the game.
        /// </summary>
        public void OnQuit()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        /// <summary>
        /// Returns to main menu from sub-panels.
        /// </summary>
        public void OnBack()
        {
            ShowMainMenu();
        }

        /// <summary>
        /// Loads a specific level.
        /// </summary>
        public void LoadLevel(string levelName)
        {
            SceneManager.LoadScene(levelName);
        }
    }
}
