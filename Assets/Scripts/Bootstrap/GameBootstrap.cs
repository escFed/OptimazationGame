using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private CustomUpdateManager updateManager;
    [SerializeField] private TransformView transformView;
    [SerializeField] private PlayerBaseStats playerStats;
    [SerializeField] private ProjectileData projectileData;
    [SerializeField] private PatternData weaponPattern;
    [SerializeField] private WaveData[] waves;
    [SerializeField] private UpgradeData[] availableUpgrades;
    [SerializeField] private int upgradeChoiceCount = 3;
    [SerializeField] private CameraFollowData cameraFollowData;
    [SerializeField] private GameUI gameUI;

    private ServiceLocator services;

    private void Awake()
    {
        services = new ServiceLocator();

        var timeService = new TimeService();
        var inputService = new PlayerInput();
        var poolService = new PoolService();
        var combatSystem = new CombatSystem();

        services.Register(timeService);
        services.Register(updateManager);
        services.Register(inputService);
        services.Register(poolService);
        services.Register(combatSystem);

        var player = new Player(1, transformView, playerStats);
        var mainCamera = Camera.main;
        var enemySystem = new EnemySystem(poolService);
        var movementSystem = new MovementSystem(player, inputService);
        var playerAimSystem = new PlayerAimSystem(player, inputService, mainCamera);
        var projectileSystem = new ProjectileSystem(poolService, enemySystem, combatSystem);
        var weaponSystem = new WeaponSystem(player, projectileSystem, projectileData, weaponPattern);
        var cameraFollowSystem = new CameraFollowSystem(mainCamera, player, cameraFollowData);
        var aiSystem = new AISystem(player, enemySystem);
        var spawnSystem = new SpawnSystem(enemySystem, player);
        var waveSystem = new WaveSystem(waves, spawnSystem, enemySystem);
        var gameStateSystem = new GameStateSystem(player, waveSystem);
        var enemyAttackSystem = new EnemyAttackSystem(player, enemySystem, combatSystem, gameStateSystem);
        var upgradeContext = new UpgradeContext(player, weaponSystem);
        var upgradeSystem = new UpgradeSystem(availableUpgrades, upgradeChoiceCount, upgradeContext, waveSystem);

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

        gameStateSystem.ShowMainMenu();
    }
}
