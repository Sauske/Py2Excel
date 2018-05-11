using System;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings
{
    public const string str_renderQuality = "gameSettings_RenderQuality";

    public const string str_particleQuality = "gameSettings_ParticleQuality";

    public const string str_gameSettingEnableHDMode = "GameSettingEnableHDMode_New2";

    private static bool m_enableHDMode;
    private static bool m_dynamicParticleLOD = true; 

    public static int defaultScreenWidth;
    public static int defaultScreenHeight;

    public static GameRenderQuality deviceLevel = GameRenderQuality.Low;
    public static GameRenderQuality renderQuality;
    public static GameRenderQuality particleQuality;

    public static bool EnableHDMode
    {
        get
        {
            return GameSettings.m_enableHDMode;
        }
        set
        {
            if (GameSettings.m_enableHDMode != value)
            {
                GameSettings.m_enableHDMode = value;
            }
        }
    }

    public static bool IsHighQuality
    {
        get
        {
            return (renderQuality == GameRenderQuality.High);
        }
    }

    public static float ModelLOD
    {
        get
        {
            return (int)renderQuality;
        }
        set
        {
            renderQuality = (GameRenderQuality)Mathf.Clamp(value, 0, 2);
        }
    }

    public static int ParticleLOD
    {
        get
        {
            return (int)particleQuality;
        }
        set
        {
            particleQuality = (GameRenderQuality)Mathf.Clamp(value, 0, 2);
        }
    }

    static GameSettings()
    {
        GameSettings.deviceLevel = GameRenderQuality.Low;
        GameSettings.m_dynamicParticleLOD = true;
    }

    public static void Init()
    {
        deviceLevel = GameRenderQuality.Low;
        deviceLevel = DetectRenderQuality.check_Android();
        if (PlayerPrefs.HasKey(str_renderQuality))
        {
            renderQuality = (GameRenderQuality)Mathf.Clamp(PlayerPrefs.GetInt(str_renderQuality, 0), 0, 2);
        }
        else
        {
            renderQuality = deviceLevel;
        }
        if (PlayerPrefs.HasKey(str_particleQuality))
        {
            particleQuality = (GameRenderQuality)Mathf.Clamp(PlayerPrefs.GetInt(str_particleQuality, 0), 0, 2);
        }
        else
        {
            particleQuality = renderQuality;
        }
    }

    public static void Save()
    {
        PlayerPrefs.SetInt(str_renderQuality, (int)GameSettings.renderQuality);
        PlayerPrefs.SetInt(str_particleQuality, (int)GameSettings.particleQuality);
    }

    public static void InitResolution()
    {
        if (GameSettings.defaultScreenWidth == 0 || GameSettings.defaultScreenHeight == 0)
        {
            int width = Screen.width;
            int height = Screen.height;
            GameSettings.defaultScreenWidth = Mathf.Max(width, height);
            GameSettings.defaultScreenHeight = Mathf.Min(width, height);
        }
    }

    public static bool SupportHDMode()
    {
        int num = (GameSettings.defaultScreenWidth <= GameSettings.defaultScreenHeight) ? GameSettings.defaultScreenHeight : GameSettings.defaultScreenWidth;
        int num2 = (GameSettings.defaultScreenWidth <= GameSettings.defaultScreenHeight) ? GameSettings.defaultScreenWidth : GameSettings.defaultScreenHeight;
        return num >= 1280 || num2 >= 720;
    }

    public static bool ShouldReduceResolution()
    {
        int num = (GameSettings.defaultScreenWidth <= GameSettings.defaultScreenHeight) ? GameSettings.defaultScreenHeight : GameSettings.defaultScreenWidth;
        int num2 = (GameSettings.defaultScreenWidth <= GameSettings.defaultScreenHeight) ? GameSettings.defaultScreenWidth : GameSettings.defaultScreenHeight;
        return num > 1280 || num2 > 720;
    }

    public static void SetHDMode(bool enable)
    {
        GameSettings.InitResolution();
        int num = GameSettings.defaultScreenWidth;
        int num2 = GameSettings.defaultScreenHeight;
        if (!enable)
        {
            num = 1280;
            num2 = num * GameSettings.defaultScreenHeight / GameSettings.defaultScreenWidth;
        }
        if (num != Screen.width || num2 != Screen.height)
        {
            Screen.SetResolution(num, num2, true);
        }
    }

    public static void RefreshResolution()
    {
        GameSettings.InitResolution();
        if (PlayerPrefs.HasKey(str_gameSettingEnableHDMode))
        {
            int @int = PlayerPrefs.GetInt(str_gameSettingEnableHDMode, 0);
            GameSettings.m_enableHDMode = (@int > 0);
        }
        else
        {
            bool flag = GameSettings.SupportHDMode();
            if (flag)
            {
                GameSettings.m_enableHDMode = false;
            }
            else
            {
                GameSettings.m_enableHDMode = !GameSettings.ShouldReduceResolution();
            }
        }
        GameSettings.SetHDMode(GameSettings.m_enableHDMode);
    }

    public static void DecideDynamicParticleLOD()
    {
        if ((DeviceCheckSys.GetAvailMemoryMegaBytes() > 300) || (DeviceCheckSys.GetTotalMemoryMegaBytes() > 1100))
        {
            m_dynamicParticleLOD = true;
        }
        else
        {
            m_dynamicParticleLOD = false;
        }
    }

    public static bool SupportOutline()
    {
        int num = (Screen.width <= Screen.height) ? Screen.height : Screen.width;
        int num2 = (Screen.width <= Screen.height) ? Screen.width : Screen.height;
        return (((num >= 960) && (num2 >= 540)) && (deviceLevel != GameRenderQuality.Low));
    }
}
