using System;
using UnityEditor;
using System;
using System.IO;
using UnityEngine;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

public class TableToolEditor
{
    public const string ExcelPath = "/Tools/ExcelTable/Bat/0START.bat";

    public const string ActionPath = "/Tools/Action/ActionRun.bat";

    public const string ProtocalPath = "/Tools/Protocol/ProtoRun.bat";

    [MenuItem("Tools/打表Excel")]
    public static void ExecuteExcel()
    {
        string filePath = Application.dataPath.Remove(Application.dataPath.LastIndexOf("/")) + ExcelPath;
        string workDir = Path.GetDirectoryName(filePath);

        UnityEngine.Debug.Log(filePath + "  " + workDir);

        BatchingProcess(filePath, workDir);
        AssetDatabase.Refresh();
        return;
    }


    //[MenuItem("Tools/动作事件")]
    //public static void ExecuteAction()
    //{
    //    string filePath = Application.dataPath.Remove(Application.dataPath.LastIndexOf("/")) + ActionPath;
    //    string workDir = Path.GetDirectoryName(filePath);
    //    BatchingProcess(filePath, workDir);
    //    AssetDatabase.Refresh();
    //}

    [MenuItem("Tools/通信协议")]
    public static void ExecuteProtocol()
    {
        string filePath = Application.dataPath.Remove(Application.dataPath.LastIndexOf("/")) + ProtocalPath;
        string workDir = Path.GetDirectoryName(filePath);
        BatchingProcess(filePath, workDir);
        AssetDatabase.Refresh();
    }

    public static void BatchingProcess(string fileName, string workingDirectory)
    {
        ProcessStartInfo info = new ProcessStartInfo();
        info.FileName = fileName;
        info.WorkingDirectory = workingDirectory;
        info.UseShellExecute = true;
        info.Arguments = string.Format("10"); ;
        info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

        System.Diagnostics.Process task = null;
        bool rt = true;
        try
        {
            task = System.Diagnostics.Process.Start(info);
            if (task != null)
            {
                task.WaitForExit(100000);
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("ExecuteProgram:" + e.ToString());
            return;
        }
        finally
        {
            if (task != null && task.HasExited)
            {
                rt = (task.ExitCode == 0);
            }
        }
        return;
    }
}
