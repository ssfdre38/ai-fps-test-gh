using UnityEngine;
using UnityEngine.SceneManagement;
using FPSGame.Utilities;

namespace FPSGame.Systems
{
    /// <summary>
    /// Main game manager singleton that handles game state and flow.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        [Header("Game Settings")]
        [SerializeField] private GameState currentGameState = GameState.Playing;
        [SerializeField] private int currentScore = 0;
        [SerializeField] private int currentWave = 0;
        [SerializeField] private float scoreMultiplier = 1f;
        
        [Header("Player")]
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private GameObject playerPrefab;
        
        private GameObject playerInstance;

        // Events
        public delegate void GameStateChangedHandler(GameState newState);
        public event GameStateChangedHandler OnGameStateChanged;
        
        public delegate void ScoreChangedHandler(int newScore);
        public event ScoreChangedHandler OnScoreChanged;
        
        public delegate void WaveChangedHandler(int newWave);
        public event WaveChangedHandler OnWaveChanged;

        /// <summary>
        /// Gets the current game state.
        /// </summary>
        public GameState CurrentGameState => currentGameState;

        /// <summary>
        /// Gets the current score.
        /// </summary>
        public int CurrentScore => currentScore;

        /// <summary>
        /// Gets the current wave number.
        /// </summary>
        public int CurrentWave => currentWave;

        /// <summary>
        /// Gets the score multiplier.
        /// </summary>
        public float ScoreMultiplier => scoreMultiplier;

        /// <summary>
        /// Gets the player instance.
        /// </summary>
        public GameObject PlayerInstance => playerInstance;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            InitializeGame();
        }

        private void Update()
        {
            HandleInput();
        }

        /// <summary>
        /// Initializes the game.
        /// </summary>
        private void InitializeGame()
        {
            // Find or spawn player
            if (playerInstance == null)
            {
                playerInstance = GameObject.FindGameObjectWithTag("Player");
                
                if (playerInstance == null && playerPrefab != null && playerSpawnPoint != null)
                {
                    playerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
                }
            }
            
            ChangeGameState(GameState.Playing);
        }

        /// <summary>
        /// Handles global input.
        /// </summary>
        private void HandleInput()
        {
            // Pause/Unpause
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (currentGameState == GameState.Playing)
                {
                    PauseGame();
                }
                else if (currentGameState == GameState.Paused)
                {
                    ResumeGame();
                }
            }
        }

        /// <summary>
        /// Changes the game state.
        /// </summary>
        public void ChangeGameState(GameState newState)
        {
            currentGameState = newState;
            OnGameStateChanged?.Invoke(newState);

            switch (newState)
            {
                case GameState.Playing:
                    Time.timeScale = 1f;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    break;
                    
                case GameState.Paused:
                    Time.timeScale = 0f;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
                    
                case GameState.GameOver:
                    Time.timeScale = 0f;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
                    
                case GameState.Victory:
                    Time.timeScale = 0f;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
            }
        }

        /// <summary>
        /// Pauses the game.
        /// </summary>
        public void PauseGame()
        {
            ChangeGameState(GameState.Paused);
        }

        /// <summary>
        /// Resumes the game.
        /// </summary>
        public void ResumeGame()
        {
            ChangeGameState(GameState.Playing);
        }

        /// <summary>
        /// Adds score to the current total.
        /// </summary>
        public void AddScore(int points)
        {
            currentScore += Mathf.RoundToInt(points * scoreMultiplier);
            OnScoreChanged?.Invoke(currentScore);
        }

        /// <summary>
        /// Sets the score multiplier.
        /// </summary>
        public void SetScoreMultiplier(float multiplier)
        {
            scoreMultiplier = Mathf.Max(1f, multiplier);
        }

        /// <summary>
        /// Increments the wave counter.
        /// </summary>
        public void IncrementWave()
        {
            currentWave++;
            OnWaveChanged?.Invoke(currentWave);
        }

        /// <summary>
        /// Triggers game over.
        /// </summary>
        public void GameOver()
        {
            ChangeGameState(GameState.GameOver);
        }

        /// <summary>
        /// Triggers victory.
        /// </summary>
        public void Victory()
        {
            ChangeGameState(GameState.Victory);
        }

        /// <summary>
        /// Restarts the current level.
        /// </summary>
        public void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Loads the main menu.
        /// </summary>
        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        /// <summary>
        /// Loads a specific level.
        /// </summary>
        public void LoadLevel(string levelName)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(levelName);
        }

        /// <summary>
        /// Quits the game.
        /// </summary>
        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        /// <summary>
        /// Respawns the player.
        /// </summary>
        public void RespawnPlayer()
        {
            if (playerInstance != null && playerSpawnPoint != null)
            {
                var playerHealth = playerInstance.GetComponent<Player.PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.Respawn();
                }
                
                var characterController = playerInstance.GetComponent<CharacterController>();
                if (characterController != null)
                {
                    characterController.enabled = false;
                    playerInstance.transform.position = playerSpawnPoint.position;
                    playerInstance.transform.rotation = playerSpawnPoint.rotation;
                    characterController.enabled = true;
                }
            }
        }
    }
}
