using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private CustomUpdateManager updateManager;
    [SerializeField] private GameUI gameUI;
    [SerializeField] private GameBootstrapData bootstrapData;

    private ServiceLocator services;
    private PlayerInput inputService;

    private void Awake()
    {
        services = new ServiceLocator();
        inputService = new PlayerInput();

        var timeService = new TimeService();
        var poolService = new PoolService();
        var combatSystem = new CombatSystem();

        services.Register(timeService);
        services.Register(updateManager);
        services.Register(inputService);
        services.Register(poolService);
        services.Register(combatSystem);

        var playerObject = Instantiate(bootstrapData.Player.Prefab, GetPlayerSpawnPosition(), Quaternion.identity);
        var transformView = playerObject.GetComponent<TransformView>();
        var player = new Player(1, transformView, bootstrapData.Player.Stats);
        var mainCamera = Camera.main;
        var enemySystem = new EnemySystem(poolService);
        var movementSystem = new MovementSystem(player, inputService);
        var playerAimSystem = new PlayerAimSystem(player, inputService, mainCamera);
        var projectileSystem = new ProjectileSystem(poolService, enemySystem, combatSystem);
        var weaponSystem = new WeaponSystem(player, projectileSystem, bootstrapData.Weapon.ProjectileData, bootstrapData.Weapon.InitialPattern);
        var cameraFollowSystem = new CameraFollowSystem(mainCamera, player, bootstrapData.CameraFollowData);
        var aiSystem = new AISystem(player, enemySystem);
        var spawnSystem = new SpawnSystem(enemySystem, player);
        var waveSystem = new WaveSystem(bootstrapData.Waves.Waves, spawnSystem, enemySystem);
        var gameStateSystem = new GameStateSystem(player, waveSystem);
        var enemyAttackSystem = new EnemyAttackSystem(player, enemySystem, combatSystem, gameStateSystem);
        var pauseSystem = new PauseSystem(inputService, gameStateSystem);
        var upgradeContext = new UpgradeContext(player, weaponSystem);
        var upgradeSystem = new UpgradeSystem(bootstrapData.Upgrades.AvailableUpgrades, bootstrapData.Upgrades.ChoiceCount, upgradeContext, waveSystem);

        gameUI.Initialize(gameStateSystem, upgradeSystem, player, waveSystem, enemySystem);

        services.Register(enemySystem);
        services.Register(weaponSystem);
        services.Register(waveSystem);
        services.Register(upgradeSystem);
        services.Register(gameStateSystem);
        services.Register(gameUI);

        updateManager.Register(movementSystem);
        updateManager.Register(playerAimSystem);
        updateManager.Register(waveSystem);
        updateManager.Register(spawnSystem);
        updateManager.Register(aiSystem);
        updateManager.Register(enemyAttackSystem);
        updateManager.Register(weaponSystem);
        updateManager.Register(projectileSystem);
        updateManager.Register(enemySystem);
        updateManager.RegisterLateUpdate(cameraFollowSystem);
        updateManager.Register(pauseSystem);

        gameStateSystem.ShowMainMenu();
    }
    private Vector3 GetPlayerSpawnPosition()
    {
        var terrain = bootstrapData.Player.Terrain;

        if (terrain == null)
        {
            return Vector3.up * bootstrapData.Player.SpawnHeightOffset;
        }

        var terrainPosition = terrain.transform.position;
        var terrainSize = terrain.terrainData.size;

        var center = terrainPosition + new Vector3(terrainSize.x * 0.5f, 0f, terrainSize.z * 0.5f);

        center.y = terrain.SampleHeight(center) + terrainPosition.y + bootstrapData.Player.SpawnHeightOffset;

        return center;
    }

    private void OnDestroy()
    {
        inputService?.Dispose();
    }
}
