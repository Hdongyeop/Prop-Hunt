using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    public int Id { get; set; }

	StatInfo _stat = new StatInfo();
	public virtual StatInfo Stat
	{
		get { return _stat; }
		set
		{
			if (_stat.Equals(value))
				return;
			
			_stat.Speed = value.Speed;
		}
	}

	public float Speed
	{
		get { return Stat.Speed; }
		set { Stat.Speed = value; }
	}

	public void SpeedChangeAndRollback(float changeSpeed, float normalSpeed)
	{
		StartCoroutine(CoSpeedChangeAndRollback(changeSpeed, normalSpeed));
	}
	
	private IEnumerator CoSpeedChangeAndRollback(float changeSpeed, float normalSpeed)
	{
		Speed = changeSpeed;
		yield return new WaitForSeconds(10f);
		Speed = normalSpeed;
	}
	
	protected bool _updated = false;

	PositionInfo _positionInfo = new PositionInfo();
	public PositionInfo PosInfo
	{
		get { return _positionInfo; }
		set
		{
			Pos = new Vector2(value.PosX, value.PosY);
			State = value.State;
			Dir = value.MoveDir;
		}
	}

	public Vector2 Pos
	{
		get => new Vector2(PosInfo.PosX, PosInfo.PosY);
		set
		{
			PosInfo.PosX = value.x;
			PosInfo.PosY = value.y;
			_updated = true;
		}
	}
	
	protected Animator _animator;
	
	protected SpriteRenderer _sprite;

	public CreatureState State
	{
		get { return PosInfo.State; }
		set
		{
			if (PosInfo.State == value)
				return;

			PosInfo.State = value;
			UpdateAnimation();
			_updated = true;
		}
	}

	public MoveDir Dir
	{
		get { return PosInfo.MoveDir; }
		set
		{
			if (PosInfo.MoveDir == value)
				return;

			PosInfo.MoveDir = value;

			UpdateAnimation();
			_updated = true;
		}
	}

	protected virtual void UpdateAnimation()
	{

	}

	private void Awake()
	{
		Init();
	}

	private void Update()
	{
		UpdateController();
	}

	protected virtual void Init()
	{
		
	}

	protected virtual void UpdateController()
	{
		switch (State)
		{
			case CreatureState.Idle:
				UpdateIdle();
				break;
			case CreatureState.Moving:
				UpdateMoving();
				break;
			case CreatureState.Skill:
				UpdateSkill();
				break;
			case CreatureState.Dead:
				UpdateDead();
				break;
		}
	}

	protected virtual void UpdateIdle()
	{
	}

	protected virtual void UpdateMoving()
	{
		MoveToNextPos();
	}

	public virtual void MoveToNextPos()
	{

	}

	protected virtual void UpdateSkill()
	{

	}
	
	protected virtual void UpdateDead()
	{

	}

	public void SortLayerChange(string sortLayerName)
	{
		gameObject.GetComponent<SpriteRenderer>().sortingLayerName = sortLayerName;
		SpriteRenderer[] srs = gameObject.GetComponentsInChildren<SpriteRenderer>();
		foreach ( SpriteRenderer sr in srs)
			sr.sortingLayerName = sortLayerName;
	}
}
