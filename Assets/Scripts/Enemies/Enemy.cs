using UnityEngine;

public class Enemy :
    IEntity,
    IDamageable,
    IPoolable
{
    private GameObject instance;
    private Animator[] animators;
    private float attackCooldownTimer;
    private int currentAnimationStateHash;

    public Enemy()
    {
        /*
         * Se crea durante el precalentamiento del pool.
         * Al asignar EnemyData se restablece al valor correcto.
         */
        Health = new Health(1f);
    }

    public int Id { get; private set; }
    public EnemyData Data { get; private set; }
    public Health Health { get; private set; }

    public bool IsDead =>
        Health == null ||
        Health.IsDead;

    public bool IsActive =>
        instance != null &&
        instance.activeInHierarchy;

    public bool CanAttack =>
        attackCooldownTimer <= 0f;

    public GameObject Instance =>
        instance;

    public Vector3 Position
    {
        get => instance.transform.position;
        set => instance.transform.position = value;
    }

    public void Initialize(
        int id,
        EnemyData data,
        GameObject instance,
        Vector3 position
    )
    {
        Id = id;
        Data = data;
        this.instance = instance;
        animators = instance.GetComponentsInChildren<Animator>(true);

        Health.Reset(data.Health);

        attackCooldownTimer = 0f;
        currentAnimationStateHash = 0;
        Position = position;

        SetMoving(false);
    }

    public void TakeDamage(
        float amount,
        IEntity source
    )
    {
        Health.TakeDamage(amount);
    }

    public void FaceDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude <= 0f)
        {
            return;
        }

        instance.transform.forward =
            direction.normalized;
    }


    public void SetMoving(bool isMoving)
    {
        if (
            Data == null ||
            animators == null ||
            animators.Length == 0
        )
        {
            return;
        }

        var stateName = isMoving
            ? Data.MoveAnimationState
            : Data.IdleAnimationState;

        if (string.IsNullOrWhiteSpace(stateName))
        {
            return;
        }

        var stateHash =
            Animator.StringToHash(stateName);

        if (stateHash == currentAnimationStateHash)
        {
            return;
        }

        var stateWasPlayed = false;

        for (var i = 0; i < animators.Length; i++)
        {
            var animator = animators[i];

            if (
                animator == null ||
                !animator.isActiveAndEnabled ||
                !animator.HasState(0, stateHash)
            )
            {
                continue;
            }

            animator.CrossFade(stateHash, 0.1f, 0);
            stateWasPlayed = true;
        }

        if (stateWasPlayed)
        {
            currentAnimationStateHash = stateHash;
        }
    }

    public void TickAttackCooldown(
        float deltaTime
    )
    {
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= deltaTime;
        }
    }

    public void ResetAttackCooldown()
    {
        attackCooldownTimer =
            Data.AttackCooldown;
    }

    public void OnSpawn()
    {
    }

    public void OnDespawn()
    {
        Id = 0;
        Data = null;
        instance = null;
        attackCooldownTimer = 0f;
        animators = null;
        currentAnimationStateHash = 0;
    }
}
