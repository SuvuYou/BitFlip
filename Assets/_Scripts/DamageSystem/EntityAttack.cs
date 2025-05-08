using UnityEngine;

public enum EntityType { Player, Enemy }

public interface IEntityAttackComponent 
{
    public EntityAttack Attack { get; }
}

public class EntityAttack
{
    public enum AttackType 
    { 
        None = 0,
        EnemyBaseAttack = 1,
        PlayerBaseAttack = 2
    }

    public enum GuardType 
    { 
        None = 0,
        Absolute = 999,
    }

    public EntityAttackStats Stats;
    public EntityAttackState State;

    public EntityAttack(EntityAttackStats stats, EntityAttackState state = null)
    {
        Stats = stats;

        if (state != null) State = state;
        else State = new EntityAttackState();
    }

    public void HandleCollision(Collider2D other) 
    {
        var targetAttack = other.transform.root.GetComponentInChildren<IEntityAttackComponent>(); 
        var targetHealth = other.transform.root.GetComponentInChildren<IEntityHealthComponent>(); 

        if (targetHealth == null || targetHealth.Health.Stats.Entity != Stats.Target) return;

        bool targetHasHigherAttack = targetAttack != null && !CompareAttackType(State.CurrentAttackType, targetAttack.Attack.State.CurrentAttackType);
        bool targetBlocksThisAttack = targetAttack != null && !CompareAttackTypeToGuardType(State.CurrentAttackType, targetAttack.Attack.State.CurrentGuardType);

        if (targetHasHigherAttack || targetBlocksThisAttack) return;
     
        targetHealth.Damage(Stats.Damage);
    }

    private bool CompareAttackType(AttackType initiator, AttackType target) => initiator > target;
    private bool CompareAttackTypeToGuardType(AttackType initiator, GuardType target) => (int)initiator > (int)target;
}

public class EntityAttackStats
{
    public EntityType Target { get; private set; }
    public int Damage { get; private set; }

    public EntityAttackStats(EntityType target, int damage)
    {
        Target = target;
        Damage = damage;
    }
}

public class EntityAttackState
{   
    public EntityAttack.AttackType CurrentAttackType { get; private set; }
    public EntityAttack.GuardType CurrentGuardType { get; private set; }
    
    public void SetAttackType (EntityAttack.AttackType newAttackType) => CurrentAttackType = newAttackType;
    public void SetGuardType (EntityAttack.GuardType newGuardType) => CurrentGuardType = newGuardType;
}