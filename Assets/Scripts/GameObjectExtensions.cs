using UnityEngine;

namespace BulletLike.GameObjectExtensions
{
  public static class GameObjectExtensions
  {
    public static int Id(this MonoBehaviour gameObject)
    {
      return gameObject.gameObject.GetInstanceID();
    }
  }
}