using UnityEngine;
using FPSGame.Utilities;

namespace FPSGame.Weapons
{
    /// <summary>
    /// ScriptableObject that holds weapon configuration data.
    /// </summary>
    [CreateAssetMenu(fileName = "New Weapon Data", menuName = "FPS Game/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Basic Info")]
        public string weaponName;
        public WeaponType weaponType;
        
        [Header("Damage")]
        public float damage = 25f;
        public float headShotMultiplier = 2f;
        public float range = 100f;
        
        [Header("Fire Settings")]
        public FireMode fireMode;
        public float fireRate = 0.1f; // Time between shots
        public int burstCount = 3; // For burst mode
        public float burstDelay = 0.05f;
        
        [Header("Ammo")]
        public int magazineSize = 30;
        public int maxReserveAmmo = 120;
        public float reloadTime = 2f;
        
        [Header("Accuracy")]
        public float baseSpread = 0.1f;
        public float maxSpread = 0.5f;
        public float spreadIncreasePerShot = 0.05f;
        public float spreadDecreaseRate = 0.1f;
        
        [Header("Recoil")]
        public Vector3 recoilAmount = new Vector3(0.5f, 0.5f, 0);
        public float recoilSpeed = 6f;
        public float recoilReturnSpeed = 15f;
        
        [Header("ADS (Aim Down Sights)")]
        public float adsZoom = 0.5f;
        public float adsSpeed = 0.2f;
        public float adsSpreadMultiplier = 0.5f;
        
        [Header("Audio")]
        public AudioClip fireSound;
        public AudioClip reloadSound;
        public AudioClip emptySound;
        
        [Header("Effects")]
        public GameObject muzzleFlashPrefab;
        public GameObject bulletHolePrefab;
        public GameObject shellEjectionPrefab;
        public GameObject bloodEffectPrefab;
        
        [Header("Visual")]
        public GameObject weaponPrefab;
        public Vector3 weaponPositionOffset;
        public Vector3 aimPosition;
    }
}
