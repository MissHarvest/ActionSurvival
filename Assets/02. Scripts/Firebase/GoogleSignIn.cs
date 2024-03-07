using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.UI;
using Firebase.Extensions;

// 2024. 03. 06 Byun Jeongmin
public class GoogleSignIn : MonoBehaviour
{
    private FirebaseAuth auth;

    private void Start()
    {
        // Firebase 초기화 및 인스턴스 생성
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;
        });
    }

    // Google 로그인 버튼이 클릭되었을 때 호출되는 메서드
    public static void SignInWithGoogle()
    {
        // Firebase 인증 객체 생성
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;

        // Google 로그인에 필요한 토큰 값들
        string googleIdToken = "YOUR_GOOGLE_ID_TOKEN";
        string googleAccessToken = "YOUR_GOOGLE_ACCESS_TOKEN";

        // GoogleAuthProvider를 사용하여 Credential 생성
        Credential credential = GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);

        // Credential을 사용하여 Google 계정으로 로그인 시도
        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            // 로그인 성공 시
            FirebaseUser newUser = task.Result;
            Debug.Log("Google 사용자 로그인 성공: " + newUser.DisplayName);
        });
    }
}