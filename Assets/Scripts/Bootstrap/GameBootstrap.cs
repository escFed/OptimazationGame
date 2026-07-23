using System.Collections.Generic;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private CustomUpdateManager updateManager;
    [SerializeField] private GameUI gameUI;
    [SerializeField] private GameBootstrapData bootstrapData;

    [Header("Pool Prewarm")]
    [SerializeField, Min(0)]
    private int projectilePrewarmCount = 64;

    [SerializeField, Min(0)]
    private int enemyPrewarmCountPerType = 20;

    [SerializeField, Min(0)]
    private int effectPrewarmCount = 32;

    private ServiceLocator services;
    private PlayerInput inputService;

    private Player player;
    private EnemySystem enemySystem;
    private ProjectileSystem projectileSystem;
    private EffectSystem effectSystem;
    private WeaponSystem weaponSystem;
    private WaveSystem waveSystem;
    private UpgradeSystem upgradeSystem;
    private GameStateSystem gameStateSystem;

    private bool applicationHasFocus = true;
    private bool applicationIsPaused;
    private bool applicationIsActive = true;

    private void Awake()
    {
        Application.runInBackground = true;

        services = new ServiceLocator();
        inputService = new PlayerInput();

        var timeService =
            new TimeService();

        var poolService =
            new PoolService();

        var combatSystem =
            new CombatSystem();

        effectSystem =
            new EffectSystem(poolService);

        services.Register(timeService);
        services.Register(updateManager);
        services.Register(inputService);
        services.Register(poolService);
        services.Register(combatSystem);
        services.Register(effectSystem);

        var playerObject = Instantiate(
            bootstrapData.Player.Prefab,
            GetPlayerSpawnPosition(),
            Quaternion.identity
        );

        var transformView =
            playerObject.GetComponent<TransformView>();

        player = new Player(
            1,
            transformView,
            bootstrapData.Player.Stats
        );

        var mainCamera = Camera.main;

        enemySystem =
            new EnemySystem(poolService);

        var movementSystem =
            new MovementSystem(
                player,
                inputService
            );

        var playerAimSystem =
            new PlayerAimSystem(
                player,
                inputService,
                mainCamera
            );

        projectileSystem =
            new ProjectileSystem(
                poolService,
                enemySystem,
                combatSystem,
                effectSystem
            );

        weaponSystem =
            new WeaponSystem(
                player,
                projectileSystem,
                bootstrapData.Weapon.ProjectileData,
                bootstrapData.Weapon.InitialPattern
            );

        var cameraFollowSystem =
            new CameraFollowSystem(
                mainCamera,
                player,
                bootstrapData.CameraFollowData
            );

        var aiSystem =
            new AISystem(
                player,
                enemySystem
            );

        var spawnSystem =
            new SpawnSystem(
                enemySystem,
                player
            );

        waveSystem =
            new WaveSystem(
                bootstrapData.Waves.Waves,
                spawnSystem,
                enemySystem
            );

        gameStateSystem =
            new GameStateSystem(
                player,
                waveSystem
            );

        var enemyAttackSystem =
            new EnemyAttackSystem(
                player,
                enemySystem,
                combatSystem,
                gameStateSystem
            );

        var pauseSystem =
            new PauseSystem(
                inputService,
                gameStateSystem
            );

        var upgradeContext =
            new UpgradeContext(
                player,
                weaponSystem
            );

        upgradeSystem =
            new UpgradeSystem(
                bootstrapData.Upgrades.AvailableUpgrades,
                bootstrapData.Upgrades.ChoiceCount,
                upgradeContext,
                waveSystem
            );

        gameUI.Initialize(
            gameStateSystem,
            upgradeSystem,
            player,
            waveSystem,
            enemySystem,
            RestartGame,
            ReturnToMainMenu
        );

        services.Register(enemySystem);
        services.Register(weaponSystem);
        services.Register(waveSystem);
        services.Register(upgradeSystem);
        services.Register(gameStateSystem);
        services.Register(gameUI);

        updateManager.Register(
            movementSystem
        );

        updateManager.Register(
            playerAimSystem
        );

        updateManager.Register(
            waveSystem
        );

        updateManager.Register(
            spawnSystem
        );

        updateManager.Register(
            aiSystem
        );

        updateManager.Register(
            enemyAttackSystem
        );

        updateManager.Register(
            weaponSystem
        );

        updateManager.Register(
            projectileSystem
        );

        updateManager.Register(
            effectSystem
        );

        updateManager.Register(
            enemySystem
        );

        updateManager.RegisterLateUpdate(
            cameraFollowSystem
        );

        updateManager.Register(
            pauseSystem
        );

        /*
         * Toda la creaci?n inicial se realiza antes de que el
         * usuario pueda comenzar la partida.
         */
        PrewarmPools(poolService);

        gameStateSystem.ShowMainMenu();
    }

    private void PrewarmPools(
        PoolService poolService
    )
    {
        PrewarmProjectilePool(
            poolService
        );

        PrewarmEffectPools(
            poolService
        );

        PrewarmEnemyPools(
            poolService
        );
    }

    private void PrewarmProjectilePool(
        PoolService poolService
    )
    {
        var projectileData =
            bootstrapData.Weapon.ProjectileData;

        if (
            projectileData == null ||
            projectileData.Prefab == null ||
            projectilePrewarmCount <= 0
        )
        {
            return;
        }

        poolService.Prewarm(
            projectileData.Prefab,
            projectilePrewarmCount
        );

        projectileSystem.Prewarm(
            projectilePrewarmCount
        );
    }

    private void PrewarmEffectPools(
        PoolService poolService
    )
    {
        var projectileData =
            bootstrapData.Weapon.ProjectileData;

        if (
            projectileData == null ||
            effectPrewarmCount <= 0
        )
        {
            return;
        }

        var spawnEffect =
            projectileData.SpawnEffectPrefab;

        var impactEffect =
            projectileData.ImpactEffectPrefab;

        if (spawnEffect != null)
        {
            poolService.Prewarm(
                spawnEffect,
                effectPrewarmCount
            );
        }

        if (
            impactEffect != null &&
            impactEffect != spawnEffect
        )
        {
            poolService.Prewarm(
                impactEffect,
                effectPrewarmCount
            );
        }
    }

    private void PrewarmEnemyPools(
        PoolService poolService
    )
    {
        var waves =
            bootstrapData.Waves.Waves;

        if (
            waves == null ||
            waves.Length == 0
        )
        {
            return;
        }

        var uniquePrefabs =
            new HashSet<GameObject>();

        var maximumConcurrentEnemies = 0;

        for (var i = 0; i < waves.Length; i++)
        {
            var wave = waves[i];

            if (wave == null)
            {
                continue;
            }

            if (
                wave.MaxEnemies >
                maximumConcurrentEnemies
            )
            {
                maximumConcurrentEnemies =
                    wave.MaxEnemies;
            }

            var enemyTypes =
                wave.EnemyTypes;

            if (enemyTypes == null)
            {
                continue;
            }

            for (
                var enemyIndex = 0;
                enemyIndex < enemyTypes.Length;
                enemyIndex++
            )
            {
                var enemyData =
                    enemyTypes[enemyIndex];

                if (
                    enemyData == null ||
                    enemyData.Prefab == null
                )
                {
                    continue;
                }

                if (
                    !uniquePrefabs.Add(
                        enemyData.Prefab
                    )
                )
                {
                    continue;
                }

                poolService.Prewarm(
                    enemyData.Prefab,
                    enemyPrewarmCountPerType
                );
            }
        }

        enemySystem.Prewarm(
            maximumConcurrentEnemies
        );
    }

    private void RestartGame()
    {
        ResetSession();
        gameStateSystem.StartGame();
    }

    private void ReturnToMainMenu()
    {
        ResetSession();
        gameStateSystem.ShowMainMenu();
    }

    private void ResetSession()
    {
        Time.timeScale = 0f;

        waveSystem.ResetSession();
        projectileSystem.ResetSession();
        enemySystem.ResetSession();
        upgradeSystem.ResetSession();
        effectSystem.ResetSession();
        weaponSystem.ResetSession();
        player.ResetSession(
            GetPlayerSpawnPosition()
        );
        gameUI.ResetSession();
    }

    private void OnApplicationFocus(
        bool hasFocus
    )
    {
        applicationHasFocus = hasFocus;
        RefreshApplicationState();
    }

    private void OnApplicationPause(
        bool pauseStatus
    )
    {
        applicationIsPaused = pauseStatus;
        RefreshApplicationState();
    }

    private void RefreshApplicationState()
    {
        var shouldBeActive =
            applicationHasFocus &&
            !applicationIsPaused;

        if (
            applicationIsActive ==
            shouldBeActive
        )
        {
            return;
        }

        applicationIsActive =
            shouldBeActive;

        if (shouldBeActive)
        {
            inputService?.Enable();

            if (updateManager != null)
            {
                updateManager.enabled = true;
            }

            return;
        }

        inputService?.Disable();

        if (
            gameStateSystem != null &&
            gameStateSystem.IsPlaying &&
            Time.timeScale > 0f
        )
        {
            gameStateSystem.PauseGame();
        }

        if (updateManager != null)
        {
            updateManager.enabled = false;
        }
    }

    private Vector3 GetPlayerSpawnPosition()
    {
        var terrain =
            bootstrapData.Player.Terrain;

        if (terrain == null)
        {
            return Vector3.up *
                   bootstrapData.Player
                       .SpawnHeightOffset;
        }

        var terrainPosition =
            terrain.transform.position;

        var terrainSize =
            terrain.terrainData.size;

        var center =
            terrainPosition +
            new Vector3(
                terrainSize.x * 0.5f,
                0f,
                terrainSize.z * 0.5f
            );

        center.y =
            terrain.SampleHeight(center) +
            terrainPosition.y +
            bootstrapData.Player
                .SpawnHeightOffset;

        return center;
    }

    private void OnDestroy()
    {
        inputService?.Dispose();
    }
}