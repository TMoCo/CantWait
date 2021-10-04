using UnityEngine;
using System;
using Random = UnityEngine.Random;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

 public struct Forces
 {
	public float force;
	float speed;
	float maxAcc;
	float damping;
	float velocity;
	float acceleration;
	float maxTip;
	float resetSpeed;

 	public Forces(float speedIn, float maxAccIn, float dampingIn, float maxTipIn, float resetSpeedIn)
 	{
		speed = speedIn;
		maxAcc = maxAccIn;
		damping = dampingIn;
		maxTip = maxTipIn;
		resetSpeed = resetSpeedIn;
 		force = 0;
 		velocity = 0;
 		acceleration = 0;
 	}
	public bool Update(float val, float dTime)
	{
		acceleration += dTime * val * speed;
		velocity += acceleration;
		force += velocity;

		// limiter
		if(acceleration > maxAcc)
			acceleration = maxAcc;
		if(acceleration < -maxAcc)
			acceleration = -maxAcc;

		// Temp limiter
		if(force > maxTip)
		{
			return true;
		}
		if(force < -maxTip)
		{
			return true;
		}
		// dampingIn
		if(val == 0)
		{
			acceleration = Damp(acceleration, dTime);
			velocity = Damp(velocity, dTime);
			force = Damp(force, dTime);
		}
		return false;
	}

	private float Damp(float val, float dTime)
	{
		float absVal = Math.Abs(val);
		float adjust = Math.Max(absVal * damping * dTime, resetSpeed * dTime);
		adjust = Math.Min(absVal, adjust);
		if(val > 0)
			return val - adjust;
		else if(val < 0)
			return val + adjust;
		return 0.0f;
	}
 }

public struct TrayPhysics
{
	public Forces x;
	public Forces y;
	public TrayPhysics(float speedIn, float maxAccIn, float dampingIn, float maxTipIn, float resetSpeedIn)
	{
		x = new Forces(speedIn, maxAccIn, dampingIn, maxTipIn, resetSpeedIn);
		y = new Forces(speedIn, maxAccIn, dampingIn, maxTipIn, resetSpeedIn);
	}
	public bool Update(float xVal, float yVal, float dTime)
	{
		bool died = false;

		died = x.Update(xVal, dTime);
		died |= y.Update(yVal, dTime);

		return died;
	}
}



namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class ThirdPersonController : MonoBehaviour
	{

		// reference to the UI
		public LevelManager levelManager;

		[Header("Our Settings")]
		public float _MovePower = 0.5f;
		public float _TipAngle = 45.0f;
		public float _TrayStartHeight;
		public float _TrayEndHeight;
		public float _ResetSpeed = 0.5f;
		public float _CamDist = 6.0f;
		public float _CamHeight = 2.5f;
		public bool _LostGame = false;
		public float _DeathTime = 0.0f;

		// Trays
		public GameObject _leftTray;
		public GameObject _rightTray;
		public Transform _leftHandTrans;
		public Transform _rightHandTrans;
		public Transform _leftTargetTrans;
		public Transform _rightTargetTrans;

		[Space(10)]
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 2.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 5.335f;
		[Tooltip("How fast the character turns to face movement direction")]
		[Range(0.0f, 0.3f)]
		public float RotationSmoothTime = 0.12f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Space(10)]
		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 70.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -30.0f;
		[Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
		public float CameraAngleOverride = 0.0f;
		[Tooltip("For locking the camera position on all axis")]
		public bool LockCameraPosition = false;

		// cinemachine
		private float _cinemachineTargetYaw;
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _animationBlend;
		private float _rotationVelocity;
		private Vector3 _targetRotation;
		public Vector3 _hitForce;

		// animation IDs
		private int _animIDSpeed;
		private int _animIDGrounded;
		private int _animIDJump;
		private int _animIDFreeFall;
		private int _animIDMotionSpeed;

		private Animator _animator;
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;

		// Tray Physics
		public float _plateSpeed;
		public float _plateMaxAcc;
		public float _plateDamping;
		private TrayPhysics _trayPhysics;



		private const float _threshold = 0.01f;

		private bool _hasAnimator;

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			_hasAnimator = TryGetComponent(out _animator);
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
			_hasAnimator = TryGetComponent(out _animator);

			_trayPhysics = new TrayPhysics(_plateSpeed, _plateMaxAcc, _plateDamping, _TipAngle, _ResetSpeed);
			_targetRotation = new Vector3(0.0f, 0.0f, 1.0f);
			_leftTargetTrans.position = new Vector3(_leftTargetTrans.position.x, _TrayStartHeight, _leftTargetTrans.position.z);
			_rightTargetTrans.position = new Vector3(_rightTargetTrans.position.x, _TrayStartHeight, _rightTargetTrans.position.z);
			AssignAnimationIDs();
			_hitForce = new Vector3(0.0f, 0.0f, 0.0f);
		}

		private void Update()
		{
			if(!_LostGame)
			{
				if(_trayPhysics.Update(_input.move.x, _input.move.y, Time.deltaTime))
				{
					_LostGame = true;
					_DeathTime = Time.timeSinceLevelLoad;
					GetComponent<Rigidbody>().constraints = 0;
					_leftTray.GetComponent<BoxCollider>().enabled = false;
					_rightTray.GetComponent<BoxCollider>().enabled = false;


					foreach(Transform child in _leftTray.transform)
					{
					   child.GetComponent<Rigidbody>().isKinematic = false;
					   child.GetComponent<BoxCollider>().enabled = true;
					   child.transform.parent = null;
					}
					foreach(Transform child in _rightTray.transform)
					{
					   child.GetComponent<Rigidbody>().isKinematic = false;
					   child.GetComponent<BoxCollider>().enabled = true;
					   child.transform.parent = null;
					}
				}

				JumpAndGravity();
				GroundedCheck();
				Move();
				UpdateTray();
			}
			else
			{
				float cTime = Time.timeSinceLevelLoad;
				float sinceDeath = cTime - _DeathTime;
				float deathAnim = 6.0f;
				_speed = Math.Max(deathAnim - sinceDeath, 0.0f);
				_animator.SetFloat(_animIDSpeed, Math.Max(deathAnim - sinceDeath, 0.0f));
				_animator.SetFloat(_animIDMotionSpeed, Math.Max((deathAnim * 0.5f) - sinceDeath, 0.0f));
				_controller.Move(_targetRotation * _speed * Time.deltaTime);
				if(sinceDeath > 0.5f)
					GetComponent<CapsuleCollider>().enabled = false;
				if (sinceDeath > 2.0f)
					levelManager.PlayerLost();
			}
		}

		private void UpdateTray()
		{
			_leftTray.transform.eulerAngles = new Vector3(_trayPhysics.y.force, 0.0f, -_trayPhysics.x.force);
			_rightTray.transform.eulerAngles = new Vector3(_trayPhysics.y.force, 0.0f, -_trayPhysics.x.force);


			Vector3 leftHandPos = _leftHandTrans.position;
			Vector3 rightHandPos = _rightHandTrans.position;

			if(_leftTargetTrans.position.y < _TrayEndHeight)
			{
				_leftTargetTrans.position = new Vector3(_leftTargetTrans.position.x, _leftTargetTrans.position.y + 3.0f * Time.deltaTime, _leftTargetTrans.position.z);
				_rightTargetTrans.position = new Vector3(_rightTargetTrans.position.x, _rightTargetTrans.position.y + 3.0f * Time.deltaTime, _rightTargetTrans.position.z);
			}

			_leftTray.transform.position = leftHandPos;
			_rightTray.transform.position = rightHandPos;

			// lower hands if reached _finalGoal
			Vector3 goal = levelManager.GetComponent<BoxCollider>().center;
			float distToGoal = Vector3.Distance(transform.position, goal);
			float minGoalDist = 10.0f;

			if(distToGoal < minGoalDist)
			{
				float ratio = distToGoal / minGoalDist;
				float flex = Math.Abs(_TrayEndHeight - _TrayStartHeight) * ratio * 0.75f;

	            _leftTargetTrans.position = new Vector3(_leftTargetTrans.position.x, _TrayStartHeight +  flex, _leftTargetTrans.position.z);
				_rightTargetTrans.position = new Vector3(_rightTargetTrans.position.x, _TrayStartHeight + flex, _rightTargetTrans.position.z);
            };

		}

		private void LateUpdate()
		{
			CameraRotation();
		}

		private void AssignAnimationIDs()
		{
			_animIDSpeed = Animator.StringToHash("Speed");
			_animIDGrounded = Animator.StringToHash("Grounded");
			_animIDJump = Animator.StringToHash("Jump");
			_animIDFreeFall = Animator.StringToHash("FreeFall");
			_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
		}

		private void GroundedCheck()
		{
			_animator.SetBool(_animIDGrounded, true);
		}

		private void CameraRotation()
		{
			Vector3 camPos = CinemachineCameraTarget.transform.position;
			Vector3 playerPos = transform.position;
			Vector3 targetPos = new Vector3(playerPos.x, playerPos.y + _CamHeight, playerPos.z - _CamDist);

			// Lerp towards
			CinemachineCameraTarget.transform.position = Vector3.Lerp(camPos, targetPos, 0.03f);
		}

		private void Move()
		{
			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

			float xSpeed = _trayPhysics.x.force;
			float ySpeed = _trayPhysics.y.force;

			// been hit by a waiter
			if(_hitForce.magnitude > 0.01f)
			{
				xSpeed += _hitForce.x;
				ySpeed += _hitForce.z;
				_hitForce = _hitForce * 0.95f;
			}

			// a reference to the players current horizontal velocity
			_speed = new Vector3(xSpeed, 0.0f, ySpeed).magnitude;
			_speed *= 0.4f;
			//float motionSpeed = 0.45f * _speed;

			_animationBlend = _speed * 0.3f;

			if(_speed > 0.01)
			{
				 targetSpeed = _speed;
				_targetRotation.x = xSpeed;
				_targetRotation.z = ySpeed;
				_targetRotation = _targetRotation.normalized;
			}
			else
			{
				targetSpeed = 0.0f;
			}

			// move the player
			//motionSpeed = Math.Min((float)Math.Pow(targetSpeed, _MovePower), _speed) * _input.uiState;
			_controller.Move(_targetRotation * _speed * Time.deltaTime);
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_targetRotation, Vector3.up), 0.04f * _input.uiState);

			// update animator if using character
			if (_hasAnimator)
			{
				_animator.SetFloat(_animIDSpeed, _animationBlend);
				_animator.SetFloat(_animIDMotionSpeed, _speed * 0.25f + Math.Min(_speed, 1.0f) + Math.Min(_speed, 0.5f));
			}
		}

		private void JumpAndGravity()
		{
			_animator.SetBool(_animIDJump, false);
			_animator.SetBool(_animIDFreeFall, false);
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y, transform.position.z), 0.5f);
		}
	}
}
