using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{ 
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _symbolText;
    [SerializeField] Image _symbolImage;

    [Header("총 집중")]
    [SerializeField] TextMeshProUGUI _totalConcentrateText;
    [SerializeField] TextMeshProUGUI _totalCountMissionText;

    [Header("주간 집중")]
    [SerializeField] List<TextMeshProUGUI> _weekConcentrateTextList = new();
    [SerializeField] TextMeshProUGUI _PrevWeekAverageConcentrateText;
    [SerializeField] TextMeshProUGUI _WeekAverageConcentrateText;

    [Header("그래프")]
    [SerializeField] List<TextMeshProUGUI> _dayTextList = new();
    [SerializeField] List<Image> _stickList = new();
    [SerializeField] List<TextMeshProUGUI> _yAxisScaleList = new();

    /// <summary>
    /// 통계 창 세팅
    /// </summary>
    public void OpenSettingStatusUI()
    {
        var player = GameManager.Instance._player;
        _nameText.text = SaveManager.Instance.Data.playerName;

        string fulltitle = GameManager.Instance._playerLvSymbolImage[player.PlayerSymbolIndex]._symbolName;
        int idx = fulltitle.IndexOf("-");
        _symbolText.text = fulltitle.Substring(0,idx-1);
        _symbolImage.sprite = GameManager.Instance._playerLvSymbolImage[player.PlayerSymbolIndex]._spriteImage;

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
        var _data = SaveManager.Instance.Data;

        //최근 7일 정보(전날까지의 기록)
        List<float> dayResultList = new();
        var oldestDay = DateTime.Parse(_data.dailyRecords[0].date);
        DateTime today = DateTime.Now.Date;
        int dayIdx = _data.dailyRecords.Count - 1;

        //뒤부터 7개를 뺀다.
        int totalDailyCount = player._dailyFocusRecordList.Count;
        for(int i = 0; i < 7; i++)
        {
            //오늘 날짜에서 1일씩 뺀다
            DateTime searchDay = today.AddDays(-i);

            //기록에 들은 날짜가 너무 적을 경우 반복문 탈출
            if (searchDay < oldestDay) break;

            //인덱스
            if (dayIdx < 0) break;

            //값이 있을 경우
            string customStr = searchDay.ToString("yyyy-MM-dd");
            if (customStr == _data.dailyRecords[dayIdx].date)
            {
                float time = _data.dailyRecords[dayIdx].focusSeconds;
                dayResultList.Add(time);
                dayIdx--;
            }
            else dayResultList.Add(0);//값이 없을 경우 이날 기록이 없다는 뜻이니 0을 넣는다.
        }

        //7개 채우기
        while (dayResultList.Count < 7)
        {
            dayResultList.Add(0);
        }

        if (dayResultList.Count>0) dayResultList.Reverse();//정렬
       
        //그래프의 최대 최솟값 구하기
        float minValue = GraphUtility.MaxMinValue(dayResultList).mini;
        float maxValue = GraphUtility.MaxMinValue(dayResultList).maxi;
        float maxYvalue = GraphUtility.MaxYAxisValue(maxValue);
    
        //Y축 그리기
        GraphUtility.DrawYAxisScale(_yAxisScaleList, maxYvalue, 0);

        //X축 정보
        GraphUtility.DrawXAxisScale(_stickList, dayResultList, maxValue, maxYvalue);

        //날짜 적기
        GraphUtility.XAxisDateText(_dayTextList, 7);
    }
    /// <summary>
    /// 주간 기록, 월~일
    /// </summary>
    void WeekRecord(PlayerInfo player)
    {
        var _data = SaveManager.Instance.Data;

        //최근 7+n일 정보(전날까지의 기록)
        int recentCount = CalTimeUtility.WeekCount(System.DateTime.Now);
        var oldestDay = DateTime.Parse(_data.dailyRecords[0].date);
        DateTime today = DateTime.Now.Date;

        List<float> dayResultList = new();
        int dayIdx = _data.dailyRecords.Count-1;

        for (int i = 0; i < recentCount + 7; i++)
        {
            //오늘 날짜에서 1일씩 뺀다
            DateTime searchDay = today.AddDays(-i);

            //기록에 들은 날짜가 너무 적을 경우 반복문 탈출
            if (searchDay < oldestDay) break;

            //인덱스
            if (dayIdx < 0) break;

            //값이 있을 경우
            string customStr = searchDay.ToString("yyyy-MM-dd");
            if (customStr == _data.dailyRecords[dayIdx].date)
            {
                float time = _data.dailyRecords[dayIdx].focusSeconds;
                dayResultList.Add(time);
                dayIdx--;
            }
            else dayResultList.Add(0);//값이 없을 경우 이날 기록이 없다는 뜻이니 0을 넣는다.
        }
        
        //지난주 기록(뒤 7개)
        float sumPrevTime = 0;
        float avgPrevTime = 0;
        float sumCurTime = 0;
        float avgCurTime = 0;
        if (dayResultList.Count > recentCount)//지난주 정보가 있는 경우만
        {
            for (int i = 0; i < 7; i++)
            {
                if (recentCount + i > dayResultList.Count) break;
                sumPrevTime += dayResultList[recentCount+i];
            }
            avgPrevTime = sumPrevTime / MathF.Min(dayResultList.Count-recentCount,7);
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

        string prevTimeText = CalTimeUtility.SecondToDay(sumPrevTime);
        string curTimeText = CalTimeUtility.SecondToDay(sumCurTime);

        _PrevWeekAverageConcentrateText.text = $"지난주 총 집중 시간 : {prevTimeText}";
        _WeekAverageConcentrateText.text = $"이번주 총 집중 시간 : {curTimeText}";

        //각 요일별 기록 - 월요일부터 기록
        dayResultList.Reverse();
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
                    int idx = (recentCount - i+6)%7;
                    float eachTime = dayResultList[idx];
                    string eachTimeText = CalTimeUtility.SecondToDay(eachTime);
                    _weekConcentrateTextList[i].text = $"{CalTimeUtility.NumToStringWeek(i)}요일 : {eachTimeText}";
                }
            }
        }

        //오늘 날짜
        int curDayWeekIdx = ((int)DateTime.Now.DayOfWeek+6)% 7;
        _weekConcentrateTextList[curDayWeekIdx].text = 
            $"{CalTimeUtility.NumToStringWeek(curDayWeekIdx)}요일 : {CalTimeUtility.SecondToDay(player.ConsumeConcentrateTime)}";

        //다시 기록 넣기 - 꺼낸 만큼 되돌린다.
        for (int i = 0; i < dayResultList.Count; i++)
        {
            player._weekConcentrateTimeStack.Push(dayResultList[i]);
        }
    }
}
