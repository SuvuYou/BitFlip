using TMPro;
using UnityEngine;

public class RouteIndexUIRenderer : MonoBehaviour
{
    [SerializeField] private RectTransform _routeIndexUIParent;
    [SerializeField] private TextMeshProUGUI _routeIndexUI;

    public void RenderRouteAt(int routeIndex, int x, int y)
    {
        var routeIndexUI = Instantiate(_routeIndexUI, _routeIndexUIParent);
        routeIndexUI.transform.localPosition = new Vector3(x * 10, y * 10, 0);
        routeIndexUI.text = routeIndex.ToString();
    }
}