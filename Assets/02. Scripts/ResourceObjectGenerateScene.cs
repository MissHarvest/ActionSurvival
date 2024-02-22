using System.Collections;
using UnityEngine;

public class ResourceObjectGenerateScene : MonoBehaviour
{
    private void Start()
    {
        // 1. 리소스 로드
        Managers.Resource.LoadAllAsync<UnityEngine.Object>("MainScene", (key, count, total) =>
        {
            if (count == total)
            {
                // 2. 맵 생성
                GameManager.Instance.GenerateWorldAsync(null,
                () =>
                {
                    // 맵 생성 완료 시 콜백
                    // 3. 객체 생성, 초기화
                    GameManager.Instance.World.enabled = false;
                    foreach (var chunk in GameManager.Instance.World.ChunkMap.Values)
                        chunk.IsActive = true;
                });
            }
        });
    }
}