using OneCanRun.Game.Share;
using UnityEngine;
namespace OneCanRun.Game
{
    public static class Events
    {
        public static ObjectiveUpdateEvent ObjectiveUpdateEvent = new ObjectiveUpdateEvent();
        public static AllObjectivesCompletedEvent AllObjectivesCompletedEvent = new AllObjectivesCompletedEvent();
        public static GameOverEvent GameOverEvent = new GameOverEvent();
        public static PlayerDeathEvent PlayerDeathEvent = new PlayerDeathEvent();
        public static EnemyKillEvent EnemyKillEvent = new EnemyKillEvent();
        public static PickupEvent PickupEvent = new PickupEvent();
        public static AmmoPickupEvent AmmoPickupEvent = new AmmoPickupEvent();
        public static DamageEvent DamageEvent = new DamageEvent();
        public static DisplayMessageEvent DisplayMessageEvent = new DisplayMessageEvent();
    }

    public class ObjectiveUpdateEvent : Event
    {
        public Objective Objective;
        public string DescriptionText;
        public string CounterText;
        public bool IsComplete;
        public string NotificationText;
    }

    public class AllObjectivesCompletedEvent : Event { }

    public class GameOverEvent : Event
    {
        public bool Win;
    }

    public class PlayerDeathEvent : Event { }

    public class EnemyKillEvent : Event
    {
        public GameObject Enemy;
        public int RemainingEnemyCount;
    }

    public class PickupEvent : Event
    {
        public GameObject Pickup;
    }

    public class AmmoPickupEvent : Event
    {
        public WeaponController Weapon;
    }

    public class DamageEvent : Event
    {
        public GameObject Sender;
        public float DamageValue;
    }

    public class DisplayMessageEvent : Event
    {
        public string Message;
        public float DelayBeforeDisplay;
    }
}