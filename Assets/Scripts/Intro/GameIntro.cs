using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameIntro : MonoBehaviour
{
    [SerializeField] GameObject howToPlayObject;
    [SerializeField] GameObject enrollNickObject;
    [SerializeField] TMP_InputField _nickInput;
    string gameSceneName = "Main";

    /// <summary>
    /// 게임 시작
    /// </summary>
    public void StartGame()
    {
        if (!PlayerPrefs.HasKey("UserName"))
        {
            OpenEnrollNick();
        }
        else SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    public void GameExit()
    {
#if UNITY_EDITOR //에디터에서
        UnityEditor.EditorApplication.isPlaying = false;
#else //나머지
        Application.Quit(); // 어플리케이션 종료
#endif
    }
    /// <summary>
    /// 닉네임 등록 창 열기
    /// </summary>
    public void OpenEnrollNick()
    {
        enrollNickObject.SetActive(true);
    }
    /// <summary>
    /// 닉네임 확정
    /// </summary>
    public void ConfirmNickName()
    {
        if (_nickInput.text == string.Empty) return;

        PlayerPrefs.SetString("UserName",_nickInput.text);
        CloseEnrollNick();
    }
    /// <summary>
    /// 닉네임 등록창 닫기
    /// </summary>
    public void CloseEnrollNick()
    {
        enrollNickObject.SetActive(false);
    }

    /// <summary>
    /// 게임 방법 소개
    /// </summary>
    public void ShowHowToPlay()
    {
        howToPlayObject.SetActive(true);
    }
    /// <summary>
    /// 게임 방법 소개 닫기
    /// </summary>
    public void CloseHowToPlay()
    {
        howToPlayObject.SetActive(false);
    }
}
