using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;
using System;

public class UnitGroupView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private LayerMask _stageLayerMask;
    private bool _isDragging = false;
    private Transform[] _units;
    private const float MAX_X_AXIS = 26;
    public void Init(Transform[] units)
    {
        _units = units;
        SetCenterPotision(_units);
        FixRectSize(units);
        foreach (var unit in units)
        {
            unit.SetParent(transform);
        }
        this.UpdateAsObservable().Where(_ => _isDragging).Subscribe(_ =>
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _stageLayerMask))
            {
                Vector3 worldPosition = hit.point;
                worldPosition.y = 0.1f;
                if (worldPosition.z > 5) worldPosition.z = 5;
                worldPosition.x = Mathf.Max(-MAX_X_AXIS, worldPosition.x - _rectTransform.sizeDelta.x / 10);
                worldPosition.x = Mathf.Min(MAX_X_AXIS, worldPosition.x + _rectTransform.sizeDelta.x / 10);
                transform.position = worldPosition;
            }
        }).AddTo(this);
    }

    private void FixRectSize(Transform[] units)
    {
        float xLength = units.Max(t => t.position.x) - units.Min(t => t.position.x);
        float yLength = units.Max(t => t.position.z) - units.Min(t => t.position.z);
        xLength = Mathf.Max(xLength * 3 + 10, 5);
        yLength = Mathf.Max(yLength * 3 + 10, 10);
        _rectTransform.sizeDelta = new Vector2(xLength, yLength);

        transform.position += Vector3.up / 10;
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

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _isDragging = false;
        }
    }
}
