using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
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

    public static void LoadFromFirebase<T>(string node, T existingData, Action callback = null) // void 말고 T로 바꿔보기
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
                //if (transform != null)
                //{
                //    Vector3 pos = JsonUtility.FromJson<Vector3>(snapshotValue);
                //    transform.position = pos;
                //    Debug.Log($"저장된 위치:{transform.position}");
                //}
                //Debug.Log($"스냅샷 자식의 수: {snapshot.ChildrenCount}");

                JsonUtility.FromJsonOverwrite(snapshotValue, existingData);
                callback?.Invoke();
            }
        });
    }

    public static void LoadFileFromFirebase(string node, object obj)
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

                JsonUtility.FromJsonOverwrite(snapshotValue, obj);
            }
        });
    }

    public static async Task<T> LoadFromFirebase<T>(string node)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child(node);
        var snapshot = await reference.GetValueAsync();

        if (snapshot != null && snapshot.Exists)
        {
            string snapshotValue = snapshot.Value.ToString();
            Debug.Log($"JSON Value값: {snapshotValue}");
            T loadedData = JsonUtility.FromJson<T>(snapshotValue);
            return loadedData;
        }
        else
        {
            return default(T);
        }
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

    public static async Task<bool> CheckIfNodeDataExists(string node)
    {
        bool dataExists = false;

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child(node);

        await reference.GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted && !task.IsFaulted)
            {
                DataSnapshot snapshot = task.Result;
                dataExists = snapshot.Exists;
            }
        });

        return dataExists;
    }
}
