using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameIntro : MonoBehaviour
{
    [SerializeField] GameObject howToPlayObject;
    [SerializeField] GameObject enrollNickObject;
    [SerializeField] GameObject init_PlayerInfoObject;
    [SerializeField] TMP_InputField _nickInput;
    string gameSceneName = "Main";

    /// <summary>
    /// 게임 시작
    /// </summary>
    public void StartGame()
    {
        if (SaveManager.Instance.Data.playerName==string.Empty)
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

        SaveManager.Instance.Data.playerName =_nickInput.text;
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
    /// <summary>
    /// 정보 초기화 창 열기
    /// </summary>
    public void ShowInitPlayer()
    {
        init_PlayerInfoObject.SetActive(true);
    }
    /// <summary>
    /// 캐릭터 정보 초기화
    /// </summary>
    public void Confirm_InitPlayerInfo()
    {
        //정보 초기화
        SaveData userData = SaveManager.Instance.Data;
        userData.playerName = string.Empty;
        userData.level = 1;
        userData.exp = 0;
        userData.playerFullExp = 300;
        userData.hp = 1000;
        userData.playerFullHp = 1000;
        userData.titleIdx = 0;

        userData.activeMissions.Clear();
        userData.dailyRecords.Clear();
        userData.whitelistdata = new Whitelistdata();

        userData.totalFocusSeconds = 0;
        userData.totalMissionsCompleted = 0;

        userData.todayConcentrateSeconds = 0;
        userData.countOtherAction = 0;
        userData.todayMissionCompleted = 0;
        userData.todayLossHP = 0f;

        //창 닫기
        CloseInitPlayer();
    }
    /// <summary>
    /// 정보 초기화 창 닫기
    /// </summary>
    public void CloseInitPlayer()
    {
        init_PlayerInfoObject.SetActive(false);
    }
}
