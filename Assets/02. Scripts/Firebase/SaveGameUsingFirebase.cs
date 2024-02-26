using Firebase;
using Firebase.Database;
using Firebase.Extensions;
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

    public static void LoadFromFirebase<T>(string node, System.Action<T> callback)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference.Child(node);
        reference.GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted && !task.IsFaulted)
            {
                DataSnapshot snapshot = task.Result;
                T data = JsonUtility.FromJson<T>(snapshot.GetRawJsonValue());
                callback?.Invoke(data);
            }
        });
    }
}
