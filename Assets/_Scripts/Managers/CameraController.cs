using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _playerCamera;

    public void LockToTarget(Transform target)
    {
        _playerCamera.Follow = target;
        _playerCamera.LookAt = target;
    }

    public void UnlockFromTarget()
    {
        _playerCamera.Follow = null;
        _playerCamera.LookAt = null;
    }

    public IEnumerator ZoomToDungeon()
    {
        yield return CameraZoomCoroutine(zoomLevel: 5.5f);
    }

    public IEnumerator ZoomToPlayer()
    {
        yield return CameraZoomCoroutine(zoomLevel: 3f);
    }

    private IEnumerator CameraZoomCoroutine(float zoomLevel)
    {
        float duration = 0.5f;
        float start = _playerCamera.m_Lens.OrthographicSize;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            _playerCamera.m_Lens.OrthographicSize = Mathf.Lerp(start, zoomLevel, time / duration);
            yield return null;
        }

        _playerCamera.m_Lens.OrthographicSize = zoomLevel;
    }
}