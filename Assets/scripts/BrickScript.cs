using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickScript : MonoBehaviour
{

    [SerializeField] ParticleSystem _breakParticle;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<Player>();
        if (player == null) 
            return;
        Vector2 normal = collision.contacts[0].normal;
        float dot = Vector2.Dot(normal, Vector2.up);
        Debug.Log(dot);

        if (dot > 0.5)
        {
            player.StopJump();
            Instantiate(_breakParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
            
        }

    }
}

