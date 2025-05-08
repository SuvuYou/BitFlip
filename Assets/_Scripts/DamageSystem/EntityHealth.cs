using System;
using UnityEngine;

public interface IEntityHealthComponent
{
    public EntityHealth Health { get; }
    void Damage(int decrease);
}

public class EntityHealth
{
    public EntityHealthStats Stats { get; private set; }
    public EntityHealthState State { get; private set; }

    public EntityHealth(EntityHealthStats stats, EntityHealthState state = null)
    {
        Stats = stats;

        if (state != null) State = state;
        else State = new EntityHealthState();

        State.SetupTimer(Stats.DamageCooldown);
        ResetHealth();
    }

    public void ResetHealth() => State.SetCurrentHealth(Stats.MaxHealth);

    public void Damage(int decrease) 
    {
        if (State.IsImmuneToDamage) return;

        State.StartDamageImmunityTimer();

        State.SetCurrentHealth(State.CurrentHealth - decrease);

        State.TriggerOnGetHit(decrease);
    }

    public void TickDamageImmunityTimer() 
    {
        State.DamageImmunityTimer.Update(Time.deltaTime);

        if (State.DamageImmunityTimer.IsFinished) State.DamageImmunityTimer.Stop();
    } 
}

public class EntityHealthStats
{
    public EntityType Entity { get; private set; }
    public int MaxHealth { get; private set; }
    public float DamageCooldown { get; private set; }

    public EntityHealthStats(EntityType entity, int maxHealth, float damageCooldown)
    {
        Entity = entity;
        MaxHealth = maxHealth;
        DamageCooldown = damageCooldown;
    }
}

public class EntityHealthState
{   
    public Action<int> OnGetHit { get; set; }

    public int CurrentHealth { get; private set; }

    public void SetCurrentHealth(int health) => CurrentHealth = health;

    public void TriggerOnGetHit(int decrease) => OnGetHit?.Invoke(decrease);

    public Timer DamageImmunityTimer { get; private set; }
    public void SetupTimer(float damageCooldown) => DamageImmunityTimer = new Timer(damageCooldown);
    public void StartDamageImmunityTimer() 
    {
        DamageImmunityTimer.Reset();
        DamageImmunityTimer.Start();
    }

    public bool IsImmuneToDamage => DamageImmunityTimer.IsRunning && !DamageImmunityTimer.IsFinished;
}