using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public partial class Player : MonoBehaviour
{
    
    [SerializeField] float _maxHorizontalSpeed = 5;
    [SerializeField] float _jumpVelocity = 5; //[serializeField] = not public but can appear in the editor
    [SerializeField] float _jumpDuration = 0.5f;
    [SerializeField] Sprite _jumpSprite;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] float _footOffset;
    [SerializeField] float _groundAcceleration = 10;
    [SerializeField] float _snowAcceleration = 1;
    [SerializeField] AudioClip _coinSfx;
    [SerializeField] AudioClip _hurtSfx;
    [SerializeField] float _knockbackVelocity = 300;

    public Transform foot;
    private SpriteRenderer _spriteRenderer;
    private AudioSource _audioSource;
    PlayerInput _playerInput;
    public bool IsGrounded;
    public bool IsOnSnow;
    private Animator _animator;
    private Rigidbody2D _rb;
    
    private float _horizontal;
    int _jumpRemaining;
    float _jumpEndTime;

    PlayerData _playerData = new PlayerData();
    
    public event Action CoinChanged;
    public event Action HealthChanged;

    public int Coins { get => _playerData.Coins; private set => _playerData.Coins = value; }
    public int Health => _playerData.Health;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        _playerInput = GetComponent<PlayerInput>();
        _rb= GetComponent<Rigidbody2D>();

        FindObjectOfType<PlayerCanvas>().Bind(this);
    }
    void OnDrawGizmos()
    {
        SpriteRenderer spriteRenderer= GetComponent<SpriteRenderer>(); //---------------- DRAW GISMOZ SCRIPT
        Gizmos.color = Color.red;

        Vector2 origin = new Vector2(transform.position.x, transform.position.y - spriteRenderer.bounds.extents.y);
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.05f);

        //draw left foot
        origin = new Vector2(transform.position.x - _footOffset, transform.position.y - spriteRenderer.bounds.extents.y);
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.05f);
        
        //draw right foot
        origin = new Vector2(transform.position.x + _footOffset, transform.position.y - spriteRenderer.bounds.extents.y);
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.05f);
    }

    // Update is called once per frame
    void Update()
    {
        updateGrounding();


        //make variabel for rigidbody / rb(2d)
        

        var horizontalInput = _playerInput.actions["Move"].ReadValue<Vector2>().x; //------------------------ WALK SCRIPT

        var desiredHorizontal = horizontalInput * _maxHorizontalSpeed; // horizontal default is 1 (_horizontal), multiple by _maxHorizontalSpeed(editable)
        var acceleration = IsOnSnow? _snowAcceleration : _groundAcceleration;
        _horizontal = Mathf.Lerp(_horizontal, desiredHorizontal, Time.deltaTime * acceleration);

        

        var vertical = _rb.velocity.y;
        if (_playerInput.actions["Jump"].WasPerformedThisFrame() && _jumpRemaining > 0)//-------------- JUMP END TIME SCRIPT & TWICE JUMP SCRIPT
        {
            _jumpEndTime = Time.time + _jumpDuration;
            _jumpRemaining --; //  "--" same as -= 1

            _audioSource.pitch = _jumpRemaining > 0 ? 1 : 1.1f; //------------- pitch for first and second jump

            _audioSource.Play(); //--------------------- ADDED JUMP SOUND
        }

        if (_playerInput.actions["Jump"].ReadValue<float>() > 0 && _jumpEndTime > Time.time)
            vertical = _jumpVelocity;

        _rb.velocity = new Vector2(_horizontal, vertical); //-------------------- VELOCITY SCRIPT
        updateSprite();
    }

    private void updateGrounding()
    {
        IsGrounded = false;
        IsOnSnow = false;
        //------------------MAKE LASER (RAYCAST) SCRIPT

        //check center
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - _spriteRenderer.bounds.extents.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.05f, _layerMask); //(start point, laser direction, laser lenght ,layermask (list for ingnore collider))
        if (hit.collider)
        {
            IsGrounded = true;
            IsOnSnow = hit.collider.CompareTag("Snow");
        }
        //check left
        origin = new Vector2(transform.position.x - _footOffset, transform.position.y - _spriteRenderer.bounds.extents.y);
        hit = Physics2D.Raycast(origin, Vector2.down, 0.05f, _layerMask); //(start point, laser direction, laser lenght ,layermask (list for ingnore collider))
        if (hit.collider)
        {
            IsGrounded = true;
            IsOnSnow = hit.collider.CompareTag("Snow");
        }

        //check right
        origin = new Vector2(transform.position.x + _footOffset, transform.position.y - _spriteRenderer.bounds.extents.y);
        hit = Physics2D.Raycast(origin, Vector2.down, 0.05f, _layerMask); //(start point, laser direction, laser lenght ,layermask (list for ingnore collider))
        if (hit.collider)
        {
            IsGrounded = true;
            IsOnSnow = hit.collider.CompareTag("Snow");
        }

        //----------------- twice jump script
        if (IsGrounded && GetComponent<Rigidbody2D>().velocity.y <= 0)
            _jumpRemaining = 2;
    }

    private void updateSprite()
    {
        _animator.SetBool("IsGrounded", IsGrounded);
        _animator.SetFloat("HorizontalSpeed",Math.Abs(_horizontal)); //Math.abs = give absolute value (ex : -1 = 1 , -0.2 = 0.2)

        if (_horizontal > 0) //--------------------------- DIRECTION SCRIPT
            _spriteRenderer.flipX = false;
        else if (_horizontal < 0)
            _spriteRenderer.flipX = true;

        
    }

    public void AddPoint()
    {
        Coins++;
        _audioSource.PlayOneShot(_coinSfx);
        if(CoinChanged != null) //------v
        CoinChanged.Invoke();   //---------> same as CoinChanged?.Invoke();
        
    }


    public void Bind(PlayerData playerData)
    {
        _playerData= playerData;
    }

    public void TakeDamage(Vector2 hitNormal)
    {
        _playerData.Health--;
        if(_playerData.Health <= 0)
        {   
            SceneManager.LoadScene(0);
            return;
        }
        _rb.AddForce(-hitNormal * _knockbackVelocity);
        _audioSource.PlayOneShot(_hurtSfx);
        HealthChanged!.Invoke();
    }

    public void StopJump()
    {
        _jumpEndTime = Time.time;
    }
}
