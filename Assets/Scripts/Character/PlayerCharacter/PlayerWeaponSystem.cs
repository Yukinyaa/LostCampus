using UnityEngine;
using Mirror;
using System;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerWeaponSystem : NetworkBehaviour
{

	private StarterAssetsInputs _input;


	[SyncVar]
	[SerializeField]
	bool isWeaponFullyUndrawn = true;

	[Command]
	void CmdSetIsWeaponFullyUndrawn(bool value)
	{
		isWeaponFullyUndrawn = value;
	}



	[SerializeField]
	float attackCoolDown = 0;
	float timeSinceLastAttack = 0;
	[SerializeField]
	bool WeaponDrawn = false;
	[SerializeField]
	bool autoAttackOnDownEnabled = true; // 마우스 다운시 연속공격



	[Header("Weapon System")]
	[SerializeField] Transform weaponAnchor;
	[SerializeField] Weapon weapon;

	bool atkBtnLastState;
	bool wDrwnBtnLastState;

	private Animator _animator;
	private NetworkAnimator _nAnimator;

	[Tooltip("Character Status Class")]
	private Status Status;



	// animation IDs
	private int _animIDMotionSpeed;
	private int _animIDWeaponDrawn;
	private int _animIDAttack1;
	private int _animIDAttack2;
	private int _animIDAttack3;

	private void Awake()
	{
		_animator = GetComponentInChildren<Animator>();
		_nAnimator = GetComponent<NetworkAnimator>();
		Debug.Assert(_animator != null);

		if (gameObject.TryGetComponent<Status>(out Status Status))
			this.Status = Status;
	}


	private void Start()
	{
		_input = GetComponent<StarterAssetsInputs>();

		AssignAnimationIDs();

		Transform rightHand = _animator.GetBoneTransform(HumanBodyBones.RightHand);
		Transform rightIndex = _animator.GetBoneTransform(HumanBodyBones.RightIndexProximal);
		Transform rightPinky = _animator.GetBoneTransform(HumanBodyBones.RightLittleIntermediate);

		weaponAnchor.parent = rightHand;
		weaponAnchor.localPosition = Vector3.zero;
		weaponAnchor.localRotation = Quaternion.identity;
		weaponAnchor.forward = rightIndex.position - rightPinky.position;//새끼손가락 3번째 마디 > 검지 3번째 마디

		weaponAnchor.parent = rightHand;
		weaponAnchor.localPosition = Vector3.zero;
		weaponAnchor.localRotation = Quaternion.identity;
	}


	private void AssignAnimationIDs()
	{
		_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
		_animIDWeaponDrawn = Animator.StringToHash("WeaponDrawn");
		_animIDAttack1 = Animator.StringToHash("Attack1");
		_animIDAttack2 = Animator.StringToHash("Attack2");
		_animIDAttack3 = Animator.StringToHash("Attack3");
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
		WeaponSystem();
	}

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