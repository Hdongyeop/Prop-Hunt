using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class CreatureController : BaseController
{
    protected Rigidbody2D _rigid;
    protected BoxCollider2D _collider;
    
    protected override void Init()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
        _rigid = GetComponent<Rigidbody2D>();

        UpdateAnimation();
    }

    public virtual void OnDamaged()
    {

    }

    public virtual void OnDead()
    {
        GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        effect.transform.position = transform.position;
        effect.GetComponent<Animator>().Play("START");
        GameObject.Destroy(effect, 0.5f);

        State = CreatureState.Dead;
        transform.position = Vector3.zero;
    }

    public virtual void UseSkill(int skillId)
    {

    }

    public virtual void ChangeToProp(int propId)
    {
        
    }
}
