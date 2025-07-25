using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public static class CONSTANTS
{
    public static readonly string GAMEMANAGER_TAG = "GameManager";
    public static readonly string[] NAVISCENELIST = {"NaviTown", "NaviFind", "NaviDrill", "NaviStatus", "NaviSetting" };
    public static readonly string[] WORDSET = {"EnglishWordList", "jukugosmall", "NGSLS1_2en_en", "NGSL1_2en_en", "NAWL1_2en_en", "TSL1_2en_en", "ANC30000_20240811" }; // 名称の変更禁止
    public static readonly string[] WORDSETNAME = { "重要順英単語", "基礎熟語688", "The New General Service List - Spoken", "The New General Service List(英英)", "The New Academic WordList(英英)", "The TOEIC Service List(英英)", "ANC頻度順英単語" };
    public static readonly Color BUTTONCOLOR = new Color(8f / 255f, 148f / 255f, 247f / 255f);
}

public static class CONSTANTSFIND
{
    public static readonly int[] NEEDFAITH = {5, 50};
    public static readonly int[] GETKNOWLEDGE = {1, 10};
}

public static class CONSTANTSDATE
{
    public static readonly string FORMAT = "yyyy-MM-dd_HH:mm:ss.fff";
    public static readonly string DAYFORMAT = "yyyy-MM-dd";
    public static readonly string DAYFORMATNONHYPHEN = "yyyyMMdd";
}

public static class CONSTANTSRESOURCE
{
    public static readonly string[] NAME = { "Faith", "Knowledge", "SunPower", "MoonPower" };
}

public static class CONSTANTSMISSION
{
    public static readonly string[] TYPE = { "DailyBonusMission" };
    public static readonly string[,] NAME = { {"Daily Bonus"} }; // TYPEと1次元目は合わせる
}

public enum GameResource
{
    Faith,
    Knowledge,
    SunPower,
    MoonPower
}

/*
TSL1.2en-enにおいては
e-book
re^sume^
cafe^
entre^e
smartphone
by-law
に独自の説明を加えた

NGSLにおいては、?を;に変更するか取り除いた

NAWLにおいては
distribution に訳を追加
headquarters の訳にheadquarterを追加
descendent　に訳を追加
cheerを cheersに変更
 */
