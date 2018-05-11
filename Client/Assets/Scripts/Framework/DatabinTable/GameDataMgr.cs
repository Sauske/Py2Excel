using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ProtoBuf;
using TableProto;

public class GameDataMgr : Singleton<GameDataMgr>
{
    public static DatabinTable<sprite_info_ARRAY, sprite_info> spriteInfoDatabin;
    public static DatabinTable<chapter_info_ARRAY, chapter_info> chapterInfoDatabin;
    public static DatabinTable<sound_info_ARRAY, sound_info> soundInfoDatabin;

    public override void Init()
    {
        base.Init();
    }

    public void UpdateFrame()
    {

    }

    public void LoadDataBin()
    {
        spriteInfoDatabin = new DatabinTable<sprite_info_ARRAY, sprite_info>("Databin/TableRes/sprite_info.bytes", "ID");
        chapterInfoDatabin = new DatabinTable<chapter_info_ARRAY, chapter_info>("Databin/TableRes/chapter_info.bytes", "ID");
        soundInfoDatabin = new DatabinTable<sound_info_ARRAY, sound_info>("Databin/TableRes/sound_info.bytes", "ID");

    }

    //public IEnumerator LoadDataBin()
    //{
    //    yield return null;        
    //}

    public void UnloadAllDataBin()
    {

    }
}
