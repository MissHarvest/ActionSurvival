using Google;
using Firebase.Auth;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

// 2024. 03. 06 Byun Jeongmin
public class SignInOut : MonoBehaviour
{
    // Auth 용 instance
    FirebaseAuth auth = null;

    // 사용자 계정
    FirebaseUser user = null;

    // 기기 연동이 되어 있는 상태인지 체크한다.
    private bool signedIn = false;

    private void Awake()
    {
        // 초기화
        auth = FirebaseAuth.DefaultInstance;

        // 유저의 로그인 정보에 어떠한 변경점이 생기면 실행되게 이벤트를 걸어준다.
        auth.StateChanged += AuthStateChanged;
        //AuthStateChanged(this, null);
    }

    // 계정 로그인에 어떠한 변경점이 발생시 진입.
    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            // 연동된 계정과 기기의 계정이 같다면 true를 리턴한다. 
            signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }

            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    // 구글 로그인
    public static void GoogleLoginProcessing()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;

        if (GoogleSignIn.Configuration == null)
        {
            // 설정
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                RequestIdToken = true,
                RequestEmail = true,
                // google-service.json file에 있는 OAuth_client ID
                WebClientId = "850878432614-56hgi9dhhrehuqbalh4nerco0qgr2n5p.apps.googleusercontent.com"
            };
        }

        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();

        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();

        signIn.ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("Google Login task.IsCanceled");
            }
            else if (task.IsFaulted)
            {
                Debug.Log("Google Login task.IsFaulted");
            }
            else
            {
                Credential credential = GoogleAuthProvider.GetCredential(((Task<GoogleSignInUser>)task).Result.IdToken, null);
                auth.SignInWithCredentialAsync(credential).ContinueWith(authTask =>
                {
                    if (authTask.IsCanceled)
                    {
                        signInCompleted.SetCanceled();
                        Debug.Log("Google Login authTask.IsCanceled");
                        return;
                    }
                    if (authTask.IsFaulted)
                    {
                        signInCompleted.SetException(authTask.Exception);
                        Debug.Log("Google Login authTask.IsFaulted");
                        return;
                    }

                    user = authTask.Result;
                    Debug.LogFormat("Google User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
                    return;
                });
            }
        });
    }

    // 에디터 테스트용 익명 로그인
    public static void AnonyLogin()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            //// 익명 로그인 연동 결과
            //FirebaseUser newUser = task.Result;
            //Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
        });
    }

    // 연동 해제
    public void SignOut()
    {
        if (auth.CurrentUser != null)
            auth.SignOut();
    }

    // 연동 계정 삭제
    public void UserDelete()
    {
        if (auth.CurrentUser != null)
            auth.CurrentUser.DeleteAsync();
    }
}