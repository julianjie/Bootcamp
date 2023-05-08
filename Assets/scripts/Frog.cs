using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Frog : MonoBehaviour
{
    Rigidbody2D _rb;
    SpriteRenderer _spriteRenderer;
    Sprite _defaultSprite;
    int _jumpRemaining;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] int _jumps = 2;
    [SerializeField] float _jumpDelay = 3;
    [SerializeField] Vector2 _jumpForce;
    [SerializeField] Sprite _jumpSprite;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _defaultSprite = _spriteRenderer.sprite;
        InvokeRepeating("Jump", _jumpDelay, _jumpDelay);
        _jumpRemaining = _jumps;
        _audioSource = GetComponent<AudioSource>();
    }

    void Jump()
    {
        if (_jumpRemaining <= 0)
        {
            _jumpForce *= new Vector2(-1, 1); //--------------- change jump direction
            _jumpRemaining = _jumps;
        }
        _jumpRemaining--;//---------- jump remaining -1 after jump

        _rb.AddForce(_jumpForce); //------------add jump

        //_spriteRenderer.flipX = !_spriteRenderer.flipX; //-------------------- flip the character(short way)
        _spriteRenderer.flipX = _jumpForce.x > 0; //----------------new script flip char
        _spriteRenderer.sprite = _jumpSprite; //frog jump animation
        
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        _spriteRenderer.sprite = _defaultSprite; //frog stand when touch the ground collider
        _audioSource.Play(); //frog play sound on hit with other collider
    }

}


