using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IkSetPosition : MonoBehaviour
{
    /// <summary>見るターゲット</summary>
    [SerializeField] Transform _target = default;
    /// <summary>どれくらい見るか</summary>
    [SerializeField, Range(0f, 1f)] float _weight = 0;
    /// <summary>身体をどれくらい向けるか</summary>
    [SerializeField, Range(0f, 1f)] float _bodyWeight = 0;
    /// <summary>頭をどれくらい向けるか</summary>
    [SerializeField, Range(0f, 1f)] float _headWeight = 0;
    /// <summary>目をどれくらい向けるか</summary>
    [SerializeField, Range(0f, 1f)] float _eyesWeight = 0;
    /// <summary>関節の動きをどれくらい制限するか</summary>
    [SerializeField, Range(0f, 1f)] float _clampWeight = 0;
    Animator _anim = default;
    GameObject enemy = default;
    bool _targetLost = true;

    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if(!_targetLost)
        _target = enemy.transform;
    }

    void OnAnimatorIK(int layerIndex)
    {
        // LookAt の重みとターゲットを指定する
        _anim.SetLookAtWeight(_weight, _bodyWeight, _headWeight, _eyesWeight, _clampWeight);
        if (!_targetLost)
            _anim.SetLookAtPosition(_target.position);
    }

    public void Target(GameObject _enemy) 
    {
        enemy = _enemy;
        _targetLost = false;
    }

    public void TargetLost() 
    {
        _targetLost = true;
    }

}