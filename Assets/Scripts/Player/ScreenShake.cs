using System.Collections;
using Events;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
        EventManager.Subscribe<DamageTakenEvent>(OnDamageTaken, this.Id());
    }

    private void OnDamageTaken(DamageTakenEvent e)
    {
        StartCoroutine(Shake(0.1f, 0.1f));
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        var originalPos = _camera.transform.localPosition;
        var elapsed = 0f;

        while (elapsed < duration)
        {
            var x = Random.Range(-1f, 1f) * magnitude;
            var y = Random.Range(-1f, 1f) * magnitude;

            _camera.transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        _camera.transform.localPosition = originalPos;
    }
}