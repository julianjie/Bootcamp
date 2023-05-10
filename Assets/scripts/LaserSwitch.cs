using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LaserSwitch : MonoBehaviour
{
    [SerializeField] Sprite _left;
    [SerializeField] Sprite _right;
    [SerializeField] UnityEvent _on;
    [SerializeField] UnityEvent _off;

    SpriteRenderer _spriteRenderer;
    bool _isOn;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player == null)
            return;

        var rigidBody = player.GetComponent<Rigidbody2D>();
        if (rigidBody.velocity.x > 0)
            TurnOn();
        else if (rigidBody.velocity.x < 0)
            TurnOff();
    }

    private void TurnOff()
    {
        if (_isOn)
        {
            _isOn = false;
            _spriteRenderer.sprite = _left;
            _off.Invoke();
        }
    }

    private void TurnOn()
    {
        if (_isOn == false)
        {
            _isOn = true;
            _spriteRenderer.sprite = _right;
            _on.Invoke();
        }
    }
}
