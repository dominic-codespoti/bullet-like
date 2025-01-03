namespace BulletLike.Events
{
  public class BaseEvent { }

  public class DamageTakenEvent : BaseEvent
  {
      public float Damage { get; private set; }

      public DamageTakenEvent(float damage)
      {
          Damage = damage;
      }
  }
}