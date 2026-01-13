using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FPSGame.Weapons;
using FPSGame.Player;
using FPSGame.Systems;

namespace FPSGame.UI
{
    /// <summary>
    /// Manages the heads-up display (HUD) for the player.
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        [Header("Health UI")]
        [SerializeField] private Image healthBar;
        [SerializeField] private TextMeshProUGUI healthText;
        
        [Header("Armor UI")]
        [SerializeField] private Image armorBar;
        [SerializeField] private TextMeshProUGUI armorText;
        
        [Header("Ammo UI")]
        [SerializeField] private TextMeshProUGUI currentAmmoText;
        [SerializeField] private TextMeshProUGUI reserveAmmoText;
        [SerializeField] private TextMeshProUGUI weaponNameText;
        
        [Header("Crosshair")]
        [SerializeField] private Image crosshair;
        [SerializeField] private float crosshairSpreadMultiplier = 50f;
        
        [Header("Hit Marker")]
        [SerializeField] private Image hitMarker;
        [SerializeField] private float hitMarkerDuration = 0.2f;
        
        [Header("Damage Indicator")]
        [SerializeField] private Image damageIndicator;
        [SerializeField] private float damageIndicatorFadeSpeed = 2f;
        
        [Header("Score UI")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI waveText;
        
        [Header("Kill Feed")]
        [SerializeField] private Transform killFeedContainer;
        [SerializeField] private GameObject killFeedItemPrefab;
        [SerializeField] private int maxKillFeedItems = 5;

        private PlayerHealth playerHealth;
        private WeaponManager weaponManager;
        private float hitMarkerTimer;
        private float damageIndicatorAlpha;

        private void Start()
        {
            InitializeReferences();
            SubscribeToEvents();
        }

        private void Update()
        {
            UpdateHitMarker();
            UpdateDamageIndicator();
        }

        /// <summary>
        /// Initializes component references.
        /// </summary>
        private void InitializeReferences()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            
            if (player != null)
            {
                playerHealth = player.GetComponent<PlayerHealth>();
                weaponManager = player.GetComponentInChildren<WeaponManager>();
            }
            
            if (hitMarker != null)
            {
                hitMarker.enabled = false;
            }
        }

        /// <summary>
        /// Subscribes to relevant game events.
        /// </summary>
        private void SubscribeToEvents()
        {
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged += UpdateHealthUI;
                playerHealth.OnArmorChanged += UpdateArmorUI;
                playerHealth.OnDamageReceived += ShowDamageIndicator;
            }

            if (weaponManager != null)
            {
                weaponManager.OnWeaponChanged += UpdateWeaponUI;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnScoreChanged += UpdateScoreUI;
                GameManager.Instance.OnWaveChanged += UpdateWaveUI;
            }
        }

        /// <summary>
        /// Updates the health UI.
        /// </summary>
        private void UpdateHealthUI(float current, float max)
        {
            if (healthBar != null)
            {
                healthBar.fillAmount = current / max;
            }

            if (healthText != null)
            {
                healthText.text = $"{Mathf.RoundToInt(current)}";
            }
        }

        /// <summary>
        /// Updates the armor UI.
        /// </summary>
        private void UpdateArmorUI(float current, float max)
        {
            if (armorBar != null)
            {
                armorBar.fillAmount = current / max;
            }

            if (armorText != null)
            {
                armorText.text = $"{Mathf.RoundToInt(current)}";
            }
        }

        /// <summary>
        /// Updates the weapon UI.
        /// </summary>
        private void UpdateWeaponUI(WeaponBase weapon, int weaponIndex)
        {
            if (weapon == null)
                return;

            if (weaponNameText != null)
            {
                weaponNameText.text = weapon.WeaponData.weaponName;
            }

            UpdateAmmoUI(weapon.CurrentAmmo, weapon.ReserveAmmo);
            
            // Subscribe to ammo changes
            weapon.OnAmmoChanged += UpdateAmmoUI;
            weapon.OnWeaponFired += ShowHitMarker;
        }

        /// <summary>
        /// Updates the ammo UI.
        /// </summary>
        private void UpdateAmmoUI(int current, int reserve)
        {
            if (currentAmmoText != null)
            {
                currentAmmoText.text = current.ToString();
            }

            if (reserveAmmoText != null)
            {
                reserveAmmoText.text = reserve.ToString();
            }
        }

        /// <summary>
        /// Updates the score UI.
        /// </summary>
        private void UpdateScoreUI(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score}";
            }
        }

        /// <summary>
        /// Updates the wave UI.
        /// </summary>
        private void UpdateWaveUI(int wave)
        {
            if (waveText != null)
            {
                waveText.text = $"Wave: {wave}";
            }
        }

        /// <summary>
        /// Shows the hit marker.
        /// </summary>
        private void ShowHitMarker()
        {
            if (hitMarker != null)
            {
                hitMarker.enabled = true;
                hitMarkerTimer = hitMarkerDuration;
            }
        }

        /// <summary>
        /// Updates the hit marker display.
        /// </summary>
        private void UpdateHitMarker()
        {
            if (hitMarker == null)
                return;

            if (hitMarkerTimer > 0)
            {
                hitMarkerTimer -= Time.deltaTime;
            }
            else
            {
                hitMarker.enabled = false;
            }
        }

        /// <summary>
        /// Shows damage indicator from a direction.
        /// </summary>
        private void ShowDamageIndicator(float damage, Utilities.DamageType damageType, Vector3 damageDirection)
        {
            damageIndicatorAlpha = 1f;
            
            // You can rotate the damage indicator based on damage direction here
        }

        /// <summary>
        /// Updates the damage indicator fade.
        /// </summary>
        private void UpdateDamageIndicator()
        {
            if (damageIndicator == null)
                return;

            if (damageIndicatorAlpha > 0)
            {
                damageIndicatorAlpha -= damageIndicatorFadeSpeed * Time.deltaTime;
                damageIndicatorAlpha = Mathf.Max(0, damageIndicatorAlpha);
                
                Color color = damageIndicator.color;
                color.a = damageIndicatorAlpha;
                damageIndicator.color = color;
            }
        }

        /// <summary>
        /// Updates crosshair spread based on weapon accuracy.
        /// </summary>
        public void UpdateCrosshairSpread(float spread)
        {
            if (crosshair == null)
                return;

            float size = 1f + (spread * crosshairSpreadMultiplier);
            crosshair.transform.localScale = Vector3.one * size;
        }

        /// <summary>
        /// Adds a kill to the kill feed.
        /// </summary>
        public void AddKillFeed(string killerName, string victimName)
        {
            if (killFeedContainer == null || killFeedItemPrefab == null)
                return;

            // Create kill feed item
            GameObject item = Instantiate(killFeedItemPrefab, killFeedContainer);
            TextMeshProUGUI text = item.GetComponent<TextMeshProUGUI>();
            
            if (text != null)
            {
                text.text = $"{killerName} killed {victimName}";
            }

            // Remove old items if too many
            if (killFeedContainer.childCount > maxKillFeedItems)
            {
                Destroy(killFeedContainer.GetChild(0).gameObject);
            }

            // Destroy after a few seconds
            Destroy(item, 5f);
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged -= UpdateHealthUI;
                playerHealth.OnArmorChanged -= UpdateArmorUI;
                playerHealth.OnDamageReceived -= ShowDamageIndicator;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnScoreChanged -= UpdateScoreUI;
                GameManager.Instance.OnWaveChanged -= UpdateWaveUI;
            }
        }
    }
}
