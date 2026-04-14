using UnityEngine;
using UnityEngine.SceneManagement;

public class GameIntro : MonoBehaviour
{
    [SerializeField] GameObject howToPlayObject;
    string gameSceneName = "Main";

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
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
