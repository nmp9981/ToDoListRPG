using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class GraphUtility
{
    /// <summary>
    /// 자료의 최댓, 최솟값 구하기
    /// </summary>
    public static (float mini, float maxi) MaxMinValue(List<float> list)
    {
        if (list == null || list.Count == 0) return (0, 0);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        foreach (var item in list)
        {
            if (item < minValue) minValue = item;
            if (item > maxValue) maxValue = item;
        }
        return (minValue, maxValue);
    }
    /// <summary>
    /// y축 최댓값 구하기
    /// </summary>
    public static float MaxYAxisValue(float maxValue)
    {
        float maxY = maxValue;

        //1시간 이하
        if (maxValue <= CalTimeUtility.hourUnit) maxY = CalTimeUtility.hourUnit;

        //1~3시간 이하, 15분단위
        if (maxValue > CalTimeUtility.hourUnit && maxValue <= 3 * CalTimeUtility.hourUnit)
        {
            float hour = (maxValue-1) / (CalTimeUtility.minutesUnit*15);
            maxY = (hour+1)* (CalTimeUtility.minutesUnit * 15);
        }
        //3~6시간 이하, 30분단위
        if (maxValue > 3*CalTimeUtility.hourUnit && maxValue <= 6 * CalTimeUtility.hourUnit)
        {
            float hour = (maxValue - 1) / (CalTimeUtility.minutesUnit * 30);
            maxY = (hour + 1) * (CalTimeUtility.minutesUnit * 30);
        }
        //6시간 이상, 1시간 단위
        if (maxValue > 6 * CalTimeUtility.hourUnit)
        {
            float hour = (maxValue-1) / CalTimeUtility.hourUnit;
            maxY = (hour+1) * CalTimeUtility.hourUnit;
        }
        return maxY;
    }
    /// <summary>
    /// x축 날짜 적기
    /// </summary>
    public static void XAxisDateText(List<TextMeshProUGUI> textList, int numCount)
    {
        DateTime today = DateTime.Now;
        for (int i = 0; i < numCount; i++)
        {
            DateTime date = today.AddDays(-i-1);
            textList[i].text = $"{date.Month}/{date.Day}";
        }
    }

    /// <summary>
    /// Y축 스케일 그리기
    /// </summary>
    /// <param name="textList"></param>
    /// <param name="maxValue"></param>
    /// <param name="minValue"></param>
    public static void DrawYAxisScale(List<TextMeshProUGUI> textList, float maxValue, float minValue)
    {
        //간격 개수
        int axisCount = textList.Count;

        //간격 
        float gap = (maxValue-minValue)/(axisCount-1);
        //간격은 깔끔하게 떨어져야함

        //나머지 스케일
        for (int i = 0; i < axisCount; i++)
        {
            float curValue = gap * i + minValue;
            string hourTime = CalTimeUtility.SecondToHour(curValue);
            textList[i].text = $"{hourTime} - ";
        }
    }
    /// <summary>
    /// X축 스케일 그리기
    /// </summary>
    /// <param name="textList"></param>
    /// <param name="maxValue"></param>
    /// <param name="minValue"></param>
    public static void DrawXAxisScale(List<Image> imageList, List<float> _data, float maxValue, float maxYValue)
    {
        //데이터 개수
        int numCount = _data.Count;

        //예외처리
        if (maxValue == 0) maxYValue = CalTimeUtility.hourUnit;

        //각 그래프 값
        for (int i = 0; i < numCount; i++)
        {
            float curValue = (_data[i]/maxYValue);
            imageList[i].fillAmount = curValue;
            imageList[i].color = Color.red;
        }
    }
}
