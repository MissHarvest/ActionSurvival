using System.Collections;
using UnityEngine;

public abstract class ResourceObject : MonoBehaviour, IInteractable
{
    protected ItemData _lootingItem;

    public float RemainRespawnTime { get; protected set; }

    // 테스트하실 때 씬 하이어라키에 미리 넣어놓고 진행하는 구조를 유지할 수 있도록 해봤습니다,,ㅋ
    public virtual IEnumerator Start()
    {
        yield return new WaitWhile(() => !Managers.Game.IsRunning);

        Initialize();
    }

    public virtual void Update()
    {
        if (RemainRespawnTime > 0)
            RemainRespawnTime -= Time.deltaTime;
        else
            Respawn();
    }

    // TODO: 드랍 테이블 만들어서 초기화하면 좋을 듯 ?
    protected ItemData InitLootingItem(string name)
    {
        return _lootingItem = Managers.Resource.GetCache<ItemData>(name);
    }

    public abstract void Initialize();

    public abstract void Respawn();

    public abstract void Interact(Player player);
}