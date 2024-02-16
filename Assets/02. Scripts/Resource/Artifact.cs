using System;
using System.Collections;
using UnityEngine;

// 2024-02-14 WJY
public class Artifact : MonoBehaviour
{
    [SerializeField] private ItemDropTable _dropTable;
    [SerializeField] private ArtifactHitbox _hitbox;
    [SerializeField] private MeshFilter _model;
    private Season _season;
    private DayCycle _dayCycle;
    private float _influenceAmount;
    private Island _island;
    public string IslandName => _island.Property.name;
    public float RemainingHP => _hitbox.HP.currentValue;

    public event Action<Artifact> OnDestroy;

    public void SetInfo(Island island, Vector3 pos, Transform root, ArtifactData data, float? currentHP = null)
    {
        _island = island;
        transform.position = pos;
        transform.parent = root;
        _influenceAmount = data.InfluenceAmount;
        _hitbox.SetInfo(data.HPMax, data.HpRegen, currentHP);
        _season = Managers.Game.Season;
        _season.OnSeasonChanged += DestroyByNature;
        _dayCycle = Managers.Game.DayCycle;
        _dayCycle.OnEveningCame += SpawnOverflowedMonster;
        SetManagementedObject();
    }

    public void SetSharedMesh(Mesh mesh)
    {
        _model.sharedMesh = mesh;
    }

    public void SetDropTable(ItemDropTable table)
    {
        _dropTable = table;
    }

    public void RaiseInfluence()
    {
        _island.Influence += _influenceAmount;
    }

    public void ReduceInfluence()
    {
        _island.Influence -= _influenceAmount;
    }

    private void SetManagementedObject()
    {
        var manage = gameObject.GetOrAddComponent<ManagementedObject>();
        manage.Add(_hitbox.GetComponent<Collider>(), typeof(Collider));
        manage.Add(_model.GetComponent<Renderer>(), typeof(Renderer));
    }

    public void DestroyByAttack(IAttack attacker)
    {
        if (attacker is Behaviour behaviour)
        {
            var player = behaviour.GetComponentInParent<Player>();

            if (player != null)
                _dropTable.AddInventory(player.Inventory);
        }
        DestroyArtifact();
    }

    public void SpawnOverflowedMonster()
    {
        Managers.Game.MonsterWave.AddOverFlowedMonster(_island.Spawn());
    }

    public void DestroyByNature(Season.State state)
    {
        //// TODO: Call MonsterWave
        // See Also: 파괴될 때 생성이 아니라 매일 밤마다 생성 ?
        //Managers.Game.MonsterWave.AddOverFlowedMonster(_island.Spawn());
        DestroyArtifact();
    }

    public void DestroyArtifact()
    {
        _season.OnSeasonChanged -= DestroyByNature;
        _dayCycle.OnEveningCame -= SpawnOverflowedMonster;
        OnDestroy?.Invoke(this);
        ReduceInfluence();
        Destroy(gameObject);
    }
}