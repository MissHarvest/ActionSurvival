// �ۼ� ��¥ : 2024. 01. 12
// �ۼ��� : Park Jun Uk 

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class Monster : MonoBehaviour, IAttack, IHit
{
    [SerializeField] protected string _name = string.Empty;
    [field: SerializeField] public MonsterAnimationData AnimationData { get; private set; }

    [field: SerializeField] public MonsterSound Sound {get; private set;}

    protected MonsterStateMachine _stateMachine;
    public Animator Animator { get; private set; }
    [field: SerializeField] public MonsterSO Data { get; private set; }

    public Vector3 RespawnPosition { get; private set; }

    public NavMeshAgent NavMeshAgent { get; private set; }

    public ItemDropTable looting;
    public bool Dead { get; private set; }

    public bool Berserk { get; private set; }

    [field: SerializeField] public Condition HP { get; private set; }

    public Island Habitat { get; private set; } = null;

    public event Action<IAttack> OnHit;
        
    public GameObject Target =>_stateMachine.Target;

    protected virtual void Awake()
    {
        gameObject.layer = 7;
        AnimationData.Initialize();
        Animator = GetComponentInChildren<Animator>();

        _name = gameObject.name.Split("(Clone)")[0];
        //var name = _name == string.Empty ? this.GetType().Name : _name;
        Data = Managers.Resource.GetCache<MonsterSO>($"{_name}.data");
        _stateMachine = new MonsterStateMachine(this);

        NavMeshAgent = Utility.GetOrAddComponent<NavMeshAgent>(gameObject);

        HP = new Condition(Data.MaxHP);
        HP.OnUpdated += OnHpUpdated;
        HP.OnBelowedToZero += Die;
    }

    private void Start()
    {
        RespawnPosition = transform.position;
        _stateMachine.ChangeState(_stateMachine.IdleState);
        SetManagementedObject();
    }

    private void Update()
    {
        HP.Update();
        _stateMachine.Update();
    }

    // 2024-01-29 WJY
    public void SetManagementedObject()
    {
        var managedObject = Utility.GetOrAddComponent<ManagementedObject>(gameObject);
        managedObject.Add(this, typeof(Behaviour));
        managedObject.AddRange(GetComponentsInChildren<Renderer>(true), typeof(Renderer));
        managedObject.AddRange(GetComponentsInChildren<Collider>(true), typeof(Collider));
    }

    public void SetIsland(Island island)
    {
        Habitat = island;
    }

    public void Respawn()
    {
        Dead = false;
        transform.position = RespawnPosition;
        gameObject.layer = 7;
        gameObject.SetActive(true);
        _stateMachine.ChangeState(_stateMachine.IdleState);

        // [ Do ] HP 복구 //
        HP.Add(Data.MaxHP);
        gameObject.SetActive(true);
    }

    public void Die()
    {
        gameObject.layer = 14;
        Dead = true;
        _stateMachine.ChangeState(_stateMachine.DieState);
        looting.AddInventory(GameManager.Instance.Player.Inventory);

        Habitat?.DiedMonsters.Add(this.gameObject);
    }

    public void Attack(AttackInfo attackData)
    {
        attackData.target.Hit(this, Data.AttackData.Atk);
    }

    public void Hit(IAttack attacker, float damage)
    {
        HP.Subtract(damage);
        OnHit?.Invoke(attacker);
        Managers.Sound.PlayEffectSound(transform.position, Sound.Hit, 1.0f, false);
    }

    public void SetBerserkMode()
    {
        Berserk = true;
        _stateMachine.DetectionDistModifier = 300;
    }

    private void OnHpUpdated(float amount)
    {
        if(amount >= 1.0f)
        {
            HP.regenRate = 0;
        }
    }

    public abstract void TryAttack();

    private void OnDrawGizmos()
    {
        if (RespawnPosition == null || Data == null || _stateMachine == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(RespawnPosition, Data.MovementData.PatrolRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _stateMachine.DetectionDist * _stateMachine.DetectionDistModifier);
    }
}
