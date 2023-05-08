using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [SerializeField] Sprite _Sprung;
    SpriteRenderer _SpriteRenderer;
    Sprite _defaultSprite;
    AudioSource _audioSource;

    void Start()
    {
        _SpriteRenderer = GetComponent<SpriteRenderer>();
        _defaultSprite = _SpriteRenderer.sprite;
        _audioSource = GetComponent<AudioSource>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (collision.collider.GetComponent<Player>().foot.transform.position.y > transform.position.y)
            {
                _audioSource.Play();
                _SpriteRenderer.sprite = _Sprung;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            _SpriteRenderer.sprite = _defaultSprite;
    }

}
