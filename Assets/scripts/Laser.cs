 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] Vector2 _direction = Vector2.left;
    [SerializeField] float _distance = 10f;
    LineRenderer _lineRenderer;
    bool _isOn;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        Toggle(false);
    }

    public void Toggle(bool state)
    {
        _isOn = state;
        _lineRenderer.enabled = state;
    }

    private void Update()
    {
        if (_isOn == false)
            return;
        var endPoint = (Vector2)transform.position + (_direction * _distance);

        var firstThing = Physics2D.Raycast(transform.position, _direction, _distance);
        if (firstThing.collider)
        {
            endPoint = firstThing.point;
            var brick = firstThing.collider.GetComponent<BrickScript>();
            if (brick)
                brick.TakeLaserDamage();
        }
        _lineRenderer.SetPosition(1, endPoint);
    }
}
