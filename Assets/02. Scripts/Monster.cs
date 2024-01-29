// �ۼ� ��¥ : 2024. 01. 12
// �ۼ��� : Park Jun Uk 

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public abstract class Monster : MonoBehaviour, IAttack, IHit
{
    [SerializeField] protected string _name = string.Empty;
    [field: SerializeField] public MonsterAnimationData AnimationData { get; private set; }
    protected MonsterStateMachine _stateMachine;
    public Animator Animator { get; private set; }
    [field: SerializeField] public MonsterSO Data { get; private set; }

    public Vector3 RespawnPosition { get; private set; }

    public NavMeshAgent NavMeshAgent { get; private set; }

    public ItemDropTable looting;
    public bool Dead { get; private set; }

    public bool Berserk { get; private set; }

    [field: SerializeField] public Condition HP { get; private set; }

    private Island _habitat;

    public event Action<IAttack> OnHit;

    [Header("Attack")]
    public float attackTime;

    protected virtual void Awake()
    {
        gameObject.layer = 7;
        AnimationData.Initialize();
        Animator = GetComponentInChildren<Animator>();

        var name = _name == string.Empty ? this.GetType().Name : _name;
        Data = Managers.Resource.GetCache<MonsterSO>($"{name}.data");
        _stateMachine = new MonsterStateMachine(this);

        NavMeshAgent = Utility.GetOrAddComponent<NavMeshAgent>(gameObject);

        HP = new Condition(Data.MaxHP);
        HP.OnBelowedToZero += Die;
    }

    private void Start()
    {
        RespawnPosition = transform.position;
        _stateMachine.ChangeState(_stateMachine.IdleState);
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    public void SetIsland(Island island)
    {
        _habitat = island;
    }

    public void Respawn()
    {
        Dead = false;
        transform.position = RespawnPosition;
        _stateMachine.ChangeState(_stateMachine.IdleState);

        // [ Do ] HP 복구 //
        HP.Add(Data.MaxHP);
        gameObject.SetActive(true);
    }

    public void Die()
    {
        Dead = true;
        _stateMachine.ChangeState(_stateMachine.DieState);
        looting.AddInventory(Managers.Game.Player.Inventory);

        _habitat?.DiedMonsters.Add(this.gameObject);
    }

    public void Attack(IHit target)
    {        
        target.Hit(this, Data.AttackData.Atk);
    }

    public void Hit(IAttack attacker, float damage)
    {
        gameObject.layer = 14;
        HP.Subtract(damage);
        Debug.Log("피격");
        OnHit?.Invoke(attacker);
        StartCoroutine(Avoid());
    }

    IEnumerator Avoid()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.layer = 7;
    }

    public void SetBerserkMode()
    {
        Berserk = true;
        _stateMachine.DetectionDistModifier = 300;
    }

    public virtual void OffAttack()
    {

    }

    public abstract void TryAttack();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(RespawnPosition, Data.MovementData.PatrolRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _stateMachine.DetectionDist * _stateMachine.DetectionDistModifier);
    }
}
