using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class PlayerController : CreatureController
{
    protected Coroutine _coSkill;
    public string Name { get; set; }
    
    protected override void UpdateAnimation()
	{
		if (_animator == null || _sprite == null)
			return;
		
		if (State == CreatureState.Idle)
		{
			switch (Dir)
			{
				case MoveDir.Up:
					_animator.Play("IDLE_BACK");
					_sprite.flipX = false;
					break;
				case MoveDir.Down:
					// 현재 Animator의 LayerDefaultState가 IDLE_FRONT이기 때문에
					// 한번 더 같은 anim을 Play하려고 하면 먹히지 않는다
					// 똑같은 anim을 한번 더 재생하려고 할 땐 추가 인자로 -1, 0을 넣어주면 된다
					_animator.Play("IDLE_FRONT", -1, 0);
					_sprite.flipX = false;
					break;
				case MoveDir.Left:
					_animator.Play("IDLE_LEFT");
					_sprite.flipX = false;
					break;
				case MoveDir.Right:
					_animator.Play("IDLE_LEFT");
					_sprite.flipX = true;
					break;
			}
		}
		else if (State == CreatureState.Moving)
		{
			switch (Dir)
			{
				case MoveDir.Up:
					_animator.Play("WALK_BACK");
					_sprite.flipX = false;
					break;
				case MoveDir.Down:
					_animator.Play("WALK_FRONT");
					_sprite.flipX = false;
					break;
				case MoveDir.Left:
					_animator.Play("WALK_LEFT");
					_sprite.flipX = false;
					break;
				case MoveDir.Right:
					_animator.Play("WALK_LEFT");
					_sprite.flipX = true;
					break;
			}
		}
		else if (State == CreatureState.Skill)
		{
			switch (Dir)
			{
				case MoveDir.Up:
					_animator.Play("ATTACK_BACK");
					_sprite.flipX = false;
					break;
				case MoveDir.Down:
					_animator.Play("ATTACK_FRONT");
					_sprite.flipX = false;
					break;
				case MoveDir.Left:
					_animator.Play("ATTACK_LEFT");
					_sprite.flipX = false;
					break;
				case MoveDir.Right:
					_animator.Play("ATTACK_LEFT");
					_sprite.flipX = true;
					break;
			}
		}
		else
		{

		}
	}

	protected override void UpdateController()
	{		
		base.UpdateController();
	}

	public override void MoveToNextPos()
	{
		transform.position = new Vector3(PosInfo.PosX, PosInfo.PosY, 0);
	}

	public override void UseSkill(int skillId)
	{
		if (skillId == 1)
		{
			// 평타
			_coSkill = StartCoroutine(CoStartSkill());
		}
		else if (skillId == 2)
		{
			
		}
	}

	public override void ChangeToProp(int propId)
	{
		// 사람 -> 물건
		if (_animator.enabled && propId != -2)
		{
			_animator.enabled = false;
			Sprite sprite = Managers.Prop.GetPropSprite(propId);
			_sprite.sprite = sprite;

			// 충돌 Layer 변경
			string curLayerName = LayerMask.LayerToName(gameObject.layer);
			int curFloor = Int32.Parse(curLayerName[curLayerName.Length - 1].ToString());
			gameObject.layer = LayerMask.NameToLayer($"Prop{curFloor}");
			
			// Collider의 BodyType을 static으로 해서 안밀려나게 한다
			_rigid.bodyType = RigidbodyType2D.Static;
		}
		// 물건 -> 사람
		else
		{
			State = CreatureState.Idle;
			_animator.enabled = true;
			
			// 충돌 Layer 변경
			string curLayerName = LayerMask.LayerToName(gameObject.layer);
			int curFloor = Int32.Parse(curLayerName[curLayerName.Length - 1].ToString());
			gameObject.layer = LayerMask.NameToLayer($"CharLayer{curFloor}");
			
			// Collider의 BodyType을 dynamic으로 해서 원상복구
			_rigid.bodyType = RigidbodyType2D.Dynamic;
		}
		
	}

	protected virtual void CheckUpdatedFlag()
	{

	}

	IEnumerator CoStartSkill()
	{
		// 대기 시간
		State = CreatureState.Skill;
		yield return new WaitForSeconds(0.5f);
		State = CreatureState.Idle;
		_coSkill = null;
		CheckUpdatedFlag();
	}

	public override void OnDamaged()
	{
		Debug.Log("Player HIT !");
	}
}
