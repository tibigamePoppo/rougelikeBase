using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitGroupView : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private bool _isDragging = false;
    private RectTransform _rectTransform;
    private Transform[] _units;
    public void Init(Transform[] units)
    {
        _units = units;
        SetCenterPotision(_units);
        FixRectSize(units);
        foreach (var unit in units)
        {
            unit.SetParent(transform);
        }
    }

    private void FixRectSize(Transform[] units)
    {
        _rectTransform = GetComponent<RectTransform>();
        float xLength = units.Max(t => t.position.x) - units.Min(t => t.position.x);
        float yLength = units.Max(t => t.position.z) - units.Min(t => t.position.z);
        xLength = Mathf.Max(xLength * 3 + 10, 5);
        yLength = Mathf.Max(yLength * 3 + 10, 10);
        _rectTransform.sizeDelta = new Vector2(xLength, yLength);

        transform.position += Vector3.up;
    }


    public void OnGameStart()
    {
        foreach (var unit in _units)
        {
            unit.transform.parent = null;
        }
        Destroy(gameObject);
    }

    public void UpdatePosition()
    {
        SetCenterPotision(_units);
        FixRectSize(_units);
    }

    private void SetCenterPotision(Transform[] units)
    {
        var center = units.Select(t => t.position).Aggregate(Vector3.zero, (sum, point) => sum + point) / units.Length;
        transform.position = center;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _isDragging = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isDragging)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            worldPosition.y = 0.1f;
            worldPosition.z += 10;
            if (worldPosition.z > 5) worldPosition.z = 5;
            transform.position = worldPosition;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _isDragging = false;
        }
    }
}
