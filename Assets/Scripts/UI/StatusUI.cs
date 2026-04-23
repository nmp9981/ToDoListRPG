using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusUI : MonoBehaviour
{ 
    [SerializeField] TextMeshProUGUI _nameText;

    [Header("총 집중")]
    [SerializeField] TextMeshProUGUI _totalConcentrateText;
    [SerializeField] TextMeshProUGUI _totalCountMissionText;

    [Header("주간 집중")]
    [SerializeField] List<TextMeshProUGUI> _weekConcentrateTextList = new();
    [SerializeField] TextMeshProUGUI _PrevWeekAverageConcentrateText;
    [SerializeField] TextMeshProUGUI _WeekAverageConcentrateText;

    /// <summary>
    /// 통계 창 세팅
    /// </summary>
    public void OpenSettingStatusUI()
    {
        var player = GameManager.Instance._player;
        _nameText.text = player._playerName;

        string totalTimeText = CalTimeUtility.SecondToDay(player.TotalConcentrateTime);
        _totalConcentrateText.text = $"총 집중 시간 : {totalTimeText}";
        _totalCountMissionText.text = $"총 완료 미션 개수 : {player.TotalCompleteMission}개";

        WeekGraph(player);
        WeekRecord(player);
    }
   
    /// <summary>
    /// 주간 그래프
    /// </summary>
    void WeekGraph(PlayerInfo player)
    {
        //최근 7일 정보(전날까지의 기록)
        List<float> dayResultList = new();
        //여기서는 stack의 복사본을 쓴다
        Stack<float> copyWeekConcentrateTimeStack = player._weekConcentrateTimeStack;
        for (int i = 0; i < 7; i++)
        {
            if (copyWeekConcentrateTimeStack.Count <= 0) break;

            float time = copyWeekConcentrateTimeStack.Pop();
            dayResultList.Add(time);
        }
        dayResultList.Reverse();
        //현재 정보
        var today = player.ConsumeConcentrateTime;

        //끝나면 stack의 복사본은 사라짐
        copyWeekConcentrateTimeStack.Clear();
    }
    /// <summary>
    /// 주간 기록, 월~일
    /// </summary>
    void WeekRecord(PlayerInfo player)
    {
        //최근 7+n일 정보(전날까지의 기록)
        int recentCount = CalTimeUtility.WeekCount(System.DateTime.Now);
        List<float> dayResultList = new();

        for(int i = 0; i < recentCount+7; i++)
        {
            if (player._weekConcentrateTimeStack.Count <= 0) break;

            float time = player._weekConcentrateTimeStack.Pop();
            dayResultList.Add(time);
        }
        dayResultList.Reverse();

        //지난주 기록(뒤 7개)
        float sumPrevTime = 0;
        float avgPrevTime = 0;
        float sumCurTime = 0;
        float avgCurTime = 0;
        if (dayResultList.Count >= 7)//지난주 정보가 있는 경우만
        {
            for (int i = 0; i < 7; i++)
            {
                sumPrevTime += dayResultList[7+i];
            }
            avgPrevTime = sumPrevTime / 7;
        }
        //이번주 기록(앞부분)
        if (dayResultList.Count > 0)
        {
            for (int i = 0; i < recentCount; i++)
            {
                sumCurTime += dayResultList[i];
            }
        }
        sumCurTime += player.ConsumeConcentrateTime;
        avgCurTime = sumCurTime / (recentCount + 1);

        string prevTimeText = CalTimeUtility.SecondToDay(avgPrevTime);
        string curTimeText = CalTimeUtility.SecondToDay(avgCurTime);

        _PrevWeekAverageConcentrateText.text = $"지난주 총 집중 시간 : {prevTimeText}";
        _WeekAverageConcentrateText.text = $"이번주 총 집중 시간 : {curTimeText}";
  
        //각 요일별 기록 - 월요일부터 기록
        if (dayResultList.Count > 0)
        {
            for (int i = 0; i < 7; i++)
            {
                if (i >= recentCount)//초과
                {
                    _weekConcentrateTextList[i].text = string.Empty;
                }
                else
                {
                    int idx = (dayResultList.Count >= 7) ? i + 7 : i;
                    float eachTime = dayResultList[idx];
                    string eachTimeText = CalTimeUtility.SecondToDay(eachTime);
                    _weekConcentrateTextList[i].text = $"{CalTimeUtility.NumToStringWeek(idx)}요일 : {eachTimeText}";
                }
            }
        }
       
        //오늘 날짜
        int curDayWeekIdx = ((int)DateTime.Now.DayOfWeek+6)% 7;
        _weekConcentrateTextList[curDayWeekIdx].text = 
            $"{CalTimeUtility.NumToStringWeek(curDayWeekIdx)}요일 : {CalTimeUtility.SecondToDay(sumCurTime)}";

        //다시 기록 넣기 - 꺼낸 만큼 되돌린다.
        for (int i = 0; i < dayResultList.Count; i++)
        {
            player._weekConcentrateTimeStack.Push(dayResultList[i]);
        }
    }
}
