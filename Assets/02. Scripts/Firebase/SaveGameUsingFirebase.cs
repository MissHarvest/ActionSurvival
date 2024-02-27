using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;
using UnityEngine;

// 2024. 02. 26 Byun Jeongmin
public class SaveGameUsingFirebase : MonoBehaviour
{
    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            FirebaseApp app = FirebaseApp.DefaultInstance;
        });
    }

    public static void SaveToFirebase(string node, string json)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child(node);
        reference.SetValueAsync(json);
    }

    public static void LoadFromFirebase<T>(string node, T existingData, Transform transform = null) // void 말고 T로 바꿔보기
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child(node);
        reference.GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted && !task.IsFaulted)
            {
                DataSnapshot snapshot = task.Result;

                //string jsonValue = snapshot.GetRawJsonValue();
                //Debug.Log($"로드된 JSON 값: {jsonValue}");
                string snapshotValue = snapshot.Value.ToString();
                Debug.Log($"JSON Value값: {snapshotValue}");
                if (transform != null)
                {
                    Vector3 pos = JsonUtility.FromJson<Vector3>(snapshotValue);
                    transform.position = pos;
                }
                //Debug.Log($"스냅샷 자식의 수: {snapshot.ChildrenCount}");

                JsonUtility.FromJsonOverwrite(snapshotValue, existingData);
            }
        });
    }

    // 특정 데이터만 지움
    public static void DeleteFirebaseNode(string node)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child(node);
        reference.RemoveValueAsync();
    }

    // 모든 데이터 지움
    public static void DeleteAllFirebaseData()
    {
        DatabaseReference rootReference = FirebaseDatabase.DefaultInstance.RootReference;

        rootReference.GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted && !task.IsFaulted)
            {
                DataSnapshot snapshot = task.Result;

                foreach (var childSnapshot in snapshot.Children)
                {
                    DatabaseReference childReference = rootReference.Child(childSnapshot.Key);
                    childReference.RemoveValueAsync();
                }
            }
        });
    }

    //public static bool CheckIfDataExists() // 무한루프 걸림
    //{
    //    DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
    //    DataSnapshot snapshot = reference.GetValueAsync().Result;

    //    return snapshot.Value != null;
    //}

    public static async Task<bool> CheckIfDataExists()
    {
        bool dataExists = false;

        DatabaseReference rootReference = FirebaseDatabase.DefaultInstance.RootReference;

        await rootReference.GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted && !task.IsFaulted)
            {
                DataSnapshot snapshot = task.Result;
                dataExists = snapshot.Exists;
            }
        });

        return dataExists;
    }
}
