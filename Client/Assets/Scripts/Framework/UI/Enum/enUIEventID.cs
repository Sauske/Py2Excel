using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum enUIEventID 
{
    None,

    Common_SendMsgAlertOpen,
    Common_SendMsgAlertClose,

    UI_OnFormPriorityChanged = 45,
    UI_OnFormVisibleChanged,
    CheckDevice_Quit = 49,

    HomeSafe_Back = 100,
    HomeSafe_Menu = 101,
    HomeSafe_Start = 102,

    ChapterCommon_Back = 105,
    ChapterCommon_Pre = 106,
    ChapterCommon_Next = 107,
    ChapterCommon_Menu = 108,

    Chapter2_Back=103,
    Chapter2_CatEye =109,
    Chapter2_CallParent=110,
    Chapter2_CallPolice=111,
    Chapter2_OpenDoor=112,

    Chapter1_Qi = 113,
    Chapter1_LiLi = 114,
    Chapter1_DaHuiLang = 115,

    HomeChapterUI_Back = 203,
    HomeChapterUI_Pre = 204,
    HomeChapterUI_Next = 205,
    HomeChapterUI_Click = 206,

    Chapter10_Basin = 207,
    Chapter10_Extinguisher = 208,
    Chapter10_Car = 209,

    Chapter10_ComDragStart = 210,
    Chapter10_ComOnDrag = 211,
    Chapter10_ComDragEnd = 212,
    Chapter10_Return = 213,

    Chapter7_Click = 700,
    Chapter9_Click2 = 901,

    Chapter_Back=905,
    Chapter_Continue=906,
}
