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
    public static readonly string[] WORDSET = {"EnglishWordList", "jukugosmall", "NGSLS1_2en_en", "NGSL1_2en_en", "NAWL1_2en_en", "TSL1_2en_en", "ANC30000_20240811" }; // –¼Ì‚Ì•ÏX‹Ö~
    public static readonly string[] WORDSETNAME = { "d—v‡‰p’PŒê", "Šî‘bnŒê688", "The New General Service List - Spoken", "The New General Service List(‰p‰p)", "The New Academic WordList(‰p‰p)", "The TOEIC Service List(‰p‰p)", "ANC•p“x‡‰p’PŒê" };
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
    public static readonly string[,] NAME = { {"Daily Bonus"} }; // TYPE‚Æ1ŸŒ³–Ú‚Í‡‚í‚¹‚é
}

public enum GameResource
{
    Faith,
    Knowledge,
    SunPower,
    MoonPower
}

/*
TSL1.2en-en‚É‚¨‚¢‚Ä‚Í
e-book
re^sume^
cafe^
entre^e
smartphone
by-law
‚É“Æ©‚Ìà–¾‚ğ‰Á‚¦‚½

NGSL‚É‚¨‚¢‚Ä‚ÍA?‚ğ;‚É•ÏX‚·‚é‚©æ‚èœ‚¢‚½

NAWL‚É‚¨‚¢‚Ä‚Í
distribution ‚É–ó‚ğ’Ç‰Á
headquarters ‚Ì–ó‚Éheadquarter‚ğ’Ç‰Á
descendent@‚É–ó‚ğ’Ç‰Á
cheer‚ğ cheers‚É•ÏX
 */
