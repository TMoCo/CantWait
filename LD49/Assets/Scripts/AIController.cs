using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class AIController : MonoBehaviour
{
    public Timer timer;
    public bool waiting = false;

    [Header("Route")]
    public Vector3[] targetPoints;
    public float startTime;
    public float waitTime;
    private bool returning;
    private int point;
    private float progress;

    [Space(10)]
    [Header("Our Settings")]
    public float _TipAngle = 45.0f;
    public float _TrayStartHeight = 1.0f;
    public float _TrayEndHeight = 2.0f;
    public bool _UsingCart = false;

    public GameObject[] foodItems;

    [Space(10)]
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;
    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;
    // Trays
    public GameObject _leftTray;
    public Transform _leftHandTrans;
    public Transform _leftTargetTrans;

    // player
    private float _speed;
    private float _animationBlend;
    private float _rotationVelocity;
    private Vector3 _targetRotation;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;
    private int _randomFoodItem;

    public Animator _animator;
    private CharacterController _controller;

    private const float _threshold = 0.01f;
    private bool _hasAnimator;

    void Awake()
    {
        _hasAnimator = gameObject.TryGetComponent(out _animator);

        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        timer = gameObject.GetComponent<Timer>();
        // set timer
        timer.onTimerFinished = ResetAI;
    }

    public void ResetAI()
    {
        _targetRotation = new Vector3(0.0f, 0.0f, 1.0f);
        point = 0;
        progress = 0.0f;
        returning = false;
        waiting = false;
        timer.ResetTimer();

        DisableHoldItems();
        PickRandomItem();
    }

    public void DisableHoldItems()
    {
        for(int i = 0; i < foodItems.Length; i++)
        {
            foodItems[i].SetActive(false);
        }
    }

    public void PickRandomItem()
    {
        if(!_UsingCart)
        {
            _randomFoodItem = Random.Range(0, foodItems.Length);
            foodItems[_randomFoodItem].SetActive(true);
        }
    }

    public void UpdateAI()
    {
        // Update is called once per frame by the AI manager
        _animator.SetBool(_animIDJump, false);
        _animator.SetBool(_animIDFreeFall, false);
        _animator.SetBool(_animIDGrounded, true);

        Move();
        UpdateTray();
    }

    private void Move()
    {
        int numPoints = targetPoints.Length;
        float windup = 1.0f;

        // Slower if finishing or starting
        if(point == 0)
            windup = Math.Max(0.05f, progress);
        if(point == targetPoints.Length - 2)
            windup = Math.Max(0.05f, 1.0f - progress);

        if(point == 0 || point == numPoints - 2)
        {
            if(!_UsingCart)
                _leftTargetTrans.position = new Vector3(_leftTargetTrans.position.x, _TrayStartHeight + windup * (_TrayEndHeight - _TrayStartHeight), _leftTargetTrans.position.z);
        }

        float currentSpeed = MoveSpeed * windup;
        _animationBlend = windup;

        Vector3 currPoint = (returning) ? targetPoints[numPoints - 1 - point] : targetPoints[point];
        Vector3 endPoint  = (returning) ? targetPoints[numPoints - 2 - point] : targetPoints[point + 1];
        _targetRotation = endPoint - currPoint;

        _animator.SetFloat(_animIDSpeed, Math.Min(currentSpeed / 5.0f, 2.5f));//_animationBlend);
        _animator.SetFloat(_animIDMotionSpeed, Math.Min(currentSpeed, 3.5f));//currentSpeed);

        transform.position = currPoint + (endPoint - currPoint) * progress;
        transform.rotation = Quaternion.LookRotation(_targetRotation, Vector3.up);

        // Move to next point
        progress += (currentSpeed * Time.deltaTime) / (endPoint - currPoint).magnitude;
        if(progress >= 1.0f)
        {
            point += 1;
            progress = 0.0f;
        }

        if(point == targetPoints.Length - 1)
        {
            if(returning)
            {
                timer.StartTimer();
                waiting = true; ; // mark waiter as in waiting state
            }
            else
            {
                DisableHoldItems();
                returning = true;
                point = 0;
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameObject player = other.gameObject;
            Vector3 toPlayer = player.transform.position - gameObject.transform.position;
            toPlayer = toPlayer.normalized;

            player.GetComponent<StarterAssets.ThirdPersonController>()._hitForce = toPlayer * 70.0f;
            Debug.Log("Collide with player");
        }
    }

    private void UpdateTray()
    {
        if(!_UsingCart)
        {
            Vector3 leftHandPos = _leftHandTrans.position;
            _leftTray.transform.position = leftHandPos;
            foodItems[_randomFoodItem].transform.position = new Vector3(leftHandPos.x, leftHandPos.y + 0.1f, leftHandPos.z);
        }
        else
        {
            Vector3 leftHandPos = _leftHandTrans.position;
            var matrix = transform.localToWorldMatrix;
            Vector4 inFront = new Vector4(-0.5f, -1.0f, 1.0f, 0.0f);
            inFront = matrix * inFront;
            _leftTargetTrans.position = leftHandPos + new Vector3(inFront.x, inFront.y, inFront.z);
        }
    }
}
