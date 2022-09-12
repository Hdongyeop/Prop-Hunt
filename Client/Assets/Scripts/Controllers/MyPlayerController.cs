using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class MyPlayerController : PlayerController
{
    bool _moveKeyPressed = false;
    private Vector2 destDir;
    
	protected override void Init()
	{
		base.Init();
	}

	protected override void UpdateController()
	{
		switch (State)
		{
			case CreatureState.Idle:
				GetDirInput();
				PressSpaceKey();
				break;
			case CreatureState.Moving:
				GetDirInput();
				PressSpaceKey();
				break;
		}

		base.UpdateController();
	}

	protected override void UpdateIdle()
	{
		// 이동 상태로 갈지 확인
		if (_moveKeyPressed)
		{
			State = CreatureState.Moving;
			return;
		}
	}

	void PressSpaceKey()
	{
		if (_coSkillCooltime == null && Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log("Skill !");

			C_Skill skill = new C_Skill() { Info = new SkillInfo() };
			// TODO UI를 통해서 3개의 선택지 중 1,2,3으로 고른 스프라이트의 번호를 보내줘야 함
			var randomProp = Managers.Prop.GetRandomProp();
			skill.Info.SkillId = randomProp;
			Managers.Network.Send(skill);

			_coSkillCooltime = StartCoroutine("CoInputCooltime", 1f); // 쿨타임
		}
	}
	
	Coroutine _coSkillCooltime;
	IEnumerator CoInputCooltime(float time)
	{
		yield return new WaitForSeconds(time);
		_coSkillCooltime = null;
	}

	void LateUpdate()
	{
		Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
	}

	// 키보드 입력
	void GetDirInput()
	{
		_moveKeyPressed = true;
		destDir = Vector2.zero;

		if (Input.GetKey(KeyCode.A))
			Dir = MoveDir.Left;
		else if (Input.GetKey(KeyCode.D))
			Dir = MoveDir.Right;
		else if (Input.GetKey(KeyCode.W))
			Dir = MoveDir.Up;
		else if (Input.GetKey(KeyCode.S))
			Dir = MoveDir.Down;

		if (Input.GetKey(KeyCode.A))
			destDir.x = -1;
		else if (Input.GetKey(KeyCode.D))
			destDir.x = 1;
		
		if (Input.GetKey(KeyCode.W))
			destDir.y = 1;
		else if (Input.GetKey(KeyCode.S))
			destDir.y = -1;

		if(destDir.x == 0 && destDir.y == 0)
			_moveKeyPressed = false;
		
		destDir.Normalize();
	}

	public override void MoveToNextPos()
	{
		if (_moveKeyPressed == false)
		{
			State = CreatureState.Idle;
			CheckUpdatedFlag();
			return;
		}

		_rigid.MovePosition(_rigid.position + destDir * Speed * Time.deltaTime);
		_rigid.velocity = Vector2.zero;
		Pos = new Vector2(transform.position.x, transform.position.y);
		CheckUpdatedFlag();
	}

	protected override void CheckUpdatedFlag()
	{
		if (_updated)
		{
			C_Move movePacket = new C_Move();
			movePacket.PosInfo = PosInfo;
			
			Managers.Network.Send(movePacket);
			_updated = false;
		}
	}
}
