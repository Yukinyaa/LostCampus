﻿using UnityEngine;
using Mirror;
using System;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class ThirdPersonController : NetworkBehaviour
{
    #region SerializedFields
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
	[Tooltip("Health")]
	public float Health=100;

	[Space(10)]
	[Tooltip("The height the player can jump")]
	public float JumpHeight = 1.2f;
	[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
	public float Gravity = -15.0f;

	[Space(10)]
	[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
	public float JumpTimeout = 0.50f;
	[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
	public float FallTimeout = 0.15f;

	[Header("Player Grounded")]
	[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
	public bool Grounded = true;
	[Tooltip("Useful for rough ground")]
	public float GroundedOffset = -0.14f;
	[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
	public float GroundedRadius = 0.28f;
	[Tooltip("What layers the character uses as ground")]
	public LayerMask GroundLayers;

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

	[Header("Weapon System")]
	public Transform weaponAnchor;

	public Weapon weapon;
	#endregion

	[Header("Debugging")]

	[SyncVar][SerializeField]
	bool isWeaponFullyUndrawn = true;

	[Command]
	void CmdSetIsWeaponFullyUndrawn(bool value)
	{
		isWeaponFullyUndrawn = value;
	}

	#region localValues
	// cinemachine
	private float _cinemachineTargetYaw;
	private float _cinemachineTargetPitch;

	// player
	private float _speed;
	private float _animationBlend;
	private float _targetRotation = 0.0f;
	private float _rotationVelocity;
	private float _verticalVelocity;
	private float _terminalVelocity = 53.0f;

	// timeout deltatime
	private float _jumpTimeoutDelta;
	private float _fallTimeoutDelta;

	// animation IDs
	private int _animIDSpeed;
	private int _animIDGrounded;
	private int _animIDJump;
	private int _animIDFreeFall;
	private int _animIDMotionSpeed;
	private int _animIDWeaponDrawn;
	private int _animIDAttack1;
	private int _animIDAttack2;
	private int _animIDAttack3;

	private Animator _animator;
	private NetworkAnimator _nAnimator;
	private CharacterController _controller;
	private StarterAssetsInputs _input;
	private GameObject _mainCamera;

	private const float _threshold = 0.01f;

	[Tooltip("Character Status Class")]
	private Status Status;
	#endregion

	private bool _hasAnimator;

	private void Awake()
	{
		_animator = GetComponentInChildren<Animator>();
		_nAnimator = GetComponent<NetworkAnimator>();
		Debug.Assert(_animator != null);
		_hasAnimator = true;
		GetComponent<NetworkAnimator>().animator = _animator;
		// get a reference to our main camera
		if (_mainCamera == null)
		{
			_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		}
		if(gameObject.TryGetComponent<Status>(out Status Status))
			this.Status = Status;
	}


	private void Start()
	{
		_controller = GetComponent<CharacterController>();
		_input = GetComponent<StarterAssetsInputs>();

		_controller.detectCollisions = false;

		AssignAnimationIDs();

		Transform righthand = _animator.GetBoneTransform(HumanBodyBones.RightHand);
		weaponAnchor.parent = righthand;
		weaponAnchor.localPosition = Vector3.zero;
		weaponAnchor.localRotation = Quaternion.identity;

		// reset our timeouts on start
		_jumpTimeoutDelta = JumpTimeout;
		_fallTimeoutDelta = FallTimeout;
	}

	public override void OnStartClient()
	{
		if (isLocalPlayer)
		{
			GameObject.Find("PlayerFollowCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow
				= this.CinemachineCameraTarget.transform;
		}
		else
		{
			GetComponent<PlayerInput>().enabled = false;
		}
	}

	private void Update()
	{
		if (isWeaponFullyUndrawn)
		{
			weaponAnchor.gameObject.SetActive(false);
		}
		else
		{
			weaponAnchor.gameObject.SetActive(true);
		}
		if (!isLocalPlayer) return;
		JumpAndGravity();
		WeaponSystem();
		GroundedCheck();
		Move();
	}

	[SerializeField]
	float attackCoolDown = 0;
	float timeSinceLastAttack = 0;
	[SerializeField]
	bool WeaponDrawn = false;
	[SerializeField]
	bool autoAttackOnDownEnabled = true; // 마우스 다운시 연속공격

	bool atkBtnLastState;
	bool wDrwnBtnLastState;

	private void WeaponSystem()
    {
		attackCoolDown -= Time.deltaTime;
		timeSinceLastAttack += Time.deltaTime;

		if (_input.drawWeapon && wDrwnBtnLastState == false && attackCoolDown < 0)
		{
			WeaponDrawn = !WeaponDrawn;
			if (WeaponDrawn == true) CmdSetIsWeaponFullyUndrawn(false);
			_animator.SetBool(_animIDWeaponDrawn, WeaponDrawn);
			attackCoolDown = 1f;
		}

		else if (_input.attack && (autoAttackOnDownEnabled || atkBtnLastState == false))
		{
			if (WeaponDrawn == false)
			{
				WeaponDrawn = true;
				_animator.SetBool(_animIDWeaponDrawn, true);
			}
			if (attackCoolDown < 0)
			{
				attackCoolDown = 1f;
				timeSinceLastAttack = 0f;

				_animator.SetTrigger(_animIDAttack1);
				_nAnimator.SetTrigger(_animIDAttack1);
				StartCoroutine(AttackCollider());

			}
		}
		else if (attackCoolDown < 0.5f)
		{
			if (isWeaponFullyUndrawn == WeaponDrawn)
				CmdSetIsWeaponFullyUndrawn(!WeaponDrawn);
		}

		atkBtnLastState = _input.attack;
		wDrwnBtnLastState = _input.drawWeapon;
	}

    private void LateUpdate()
	{
		if (isLocalPlayer)
		{
			CameraRotation();
		}
	}

	private void AssignAnimationIDs()
	{
		_animIDSpeed = Animator.StringToHash("Speed");
		_animIDGrounded = Animator.StringToHash("Grounded");
		_animIDJump = Animator.StringToHash("Jump");
		_animIDFreeFall = Animator.StringToHash("FreeFall");
		_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
		_animIDWeaponDrawn = Animator.StringToHash("WeaponDrawn");
		_animIDAttack1 = Animator.StringToHash("Attack1");
		_animIDAttack2 = Animator.StringToHash("Attack2");
		_animIDAttack3 = Animator.StringToHash("Attack3");
	}

	private void GroundedCheck()
	{
		// set sphere position, with offset
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
		Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

		// update animator if using character

		_animator.SetBool(_animIDGrounded, Grounded);
	}

	private void CameraRotation()
	{
		// if there is an input and camera position is not fixed
		if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
		{
			_cinemachineTargetYaw += _input.look.x * Time.deltaTime;
			_cinemachineTargetPitch += _input.look.y * Time.deltaTime;
		}

		// clamp our rotations so our values are limited 360 degrees
		_cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
		_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

		// Cinemachine will follow this target
		CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
	}

	private void Move()
	{
		if (_input.sprint&&isWeaponFullyUndrawn)
        {
			if (this.Status.AP <= 0)
			{
				_input.sprint = false;
			}
			this.Status.AP-=1*Time.deltaTime;
			if (this.Status.AP <= 0)
				this.Status.AP = 0;
		}
        else
        {
			this.Status.AP+=2*Time.deltaTime;
			if (this.Status.AP >= this.Status.MaxAP)
				this.Status.AP = this.Status.MaxAP;
		}
		// set target speed based on move speed, sprint speed and if sprint is pressed
		float targetSpeed = isWeaponFullyUndrawn ? (_input.sprint ? SprintSpeed : MoveSpeed) : MoveSpeed / 2;

		// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

		// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is no input, set the target speed to 0
		if (_input.move == Vector2.zero) targetSpeed = 0.0f;

		// a reference to the players current horizontal velocity
		float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

		float speedOffset = 0.1f;
		float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

		// accelerate or decelerate to target speed
		if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
		{
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

			// round speed to 3 decimal places
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		}
		else
		{
			_speed = targetSpeed;
		}
		_animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);

		// normalise input direction
		Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is a move input rotate player when the player is moving
		if (_input.move != Vector2.zero)
		{
			_targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
			float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

			// rotate to face input direction relative to camera position
			transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
		}


		Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

		// move the player
		_controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

		// update animator if using character
		_animator.SetFloat(_animIDSpeed, _animationBlend);
		_animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
			
	}

	private void JumpAndGravity()
	{
		if (Grounded)
		{
			// reset the fall timeout timer
			_fallTimeoutDelta = FallTimeout;

			// update animator if using character
			_animator.SetBool(_animIDJump, false);
			_animator.SetBool(_animIDFreeFall, false);

			// stop our velocity dropping infinitely when grounded
			if (_verticalVelocity < 0.0f)
			{
				_verticalVelocity = -2f;
			}

			// Jump
			if (_input.jump && _jumpTimeoutDelta <= 0.0f)
			{
				// the square root of H * -2 * G = how much velocity needed to reach desired height
				_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

				// update animator if using character
				_animator.SetBool(_animIDJump, true);
			}

			// jump timeout
			if (_jumpTimeoutDelta >= 0.0f)
			{
				_jumpTimeoutDelta -= Time.deltaTime;
			}
		}
		else
		{
			// reset the jump timeout timer
			_jumpTimeoutDelta = JumpTimeout;

			// fall timeout
			if (_fallTimeoutDelta >= 0.0f)
			{
				_fallTimeoutDelta -= Time.deltaTime;
			}
			else
			{
				// update animator if using character
				_animator.SetBool(_animIDFreeFall, true);
			}

			// if we are not grounded, do not jump
			_input.jump = false;
		}

		// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		if (_verticalVelocity < _terminalVelocity)
		{
			_verticalVelocity += Gravity * Time.deltaTime;
		}
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

		if (Grounded) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;
			
		// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
		Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
	}

	IEnumerator AttackCollider()
    {
		while (!_animator.GetCurrentAnimatorStateInfo(1).IsTag("Attacking"))
		{
			//전환 중일 때 실행되는 부분
			yield return null;
		}
		weapon.Set(true);
		while (_animator.GetCurrentAnimatorStateInfo(1).IsTag("Attacking"))
		{
			//애니메이션 재생 중 실행되는 부분
			yield return null;
		}
		weapon.Set(false);
		
		//애니메이션 완료 후 실행되는 부분

	}
}