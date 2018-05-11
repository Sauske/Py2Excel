using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public enum enFileOperation
{
    ReadFile,
    WriteFile,
    DeleteFile,
    CreateDirectory,
    DeleteDirectory
}

public class FileManager
{
    public delegate void DelegateOnOperateFileFail(string fullPath, enFileOperation fileOperation);

    private static string s_cachePath = null;

    public static string s_ifsExtractFolder = "Resources";

    private static string s_ifsExtractPath = null;

    private static MD5CryptoServiceProvider s_md5Provider = new MD5CryptoServiceProvider();

    public static FileManager.DelegateOnOperateFileFail s_delegateOnOperateFileFail = delegate
    {
   
    };

    public static bool ClearDirectory(string fullPath)
    {
        bool result;
        try
        {
            string[] files = Directory.GetFiles(fullPath);
            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
            string[] directories = Directory.GetDirectories(fullPath);
            for (int j = 0; j < directories.Length; j++)
            {
                //Directory.Delete(directories[j], true);
            }
            result = true;
        }
        catch (Exception)
        {
            result = false;
        }
        return result;
    }

    public static bool ClearDirectory(string fullPath, string[] fileExtensionFilter, string[] folderFilter)
    {
        bool result;
        try
        {
            if (fileExtensionFilter != null)
            {
                string[] files = Directory.GetFiles(fullPath);
                for (int i = 0; i < files.Length; i++)
                {
                    if (fileExtensionFilter != null && fileExtensionFilter.Length > 0)
                    {
                        for (int j = 0; j < fileExtensionFilter.Length; j++)
                        {
                            if (files[i].Contains(fileExtensionFilter[j]))
                            {
                                FileManager.DeleteFile(files[i]);
                                break;
                            }
                        }
                    }
                }
            }
            if (folderFilter != null)
            {
                string[] directories = Directory.GetDirectories(fullPath);
                for (int k = 0; k < directories.Length; k++)
                {
                    if (folderFilter != null && folderFilter.Length > 0)
                    {
                        for (int l = 0; l < folderFilter.Length; l++)
                        {
                            if (directories[k].Contains(folderFilter[l]))
                            {
                                FileManager.DeleteDirectory(directories[k]);
                                break;
                            }
                        }
                    }
                }
            }
            result = true;
        }
        catch (Exception)
        {
            result = false;
        }
        return result;
    }

    public static string CombinePath(string path1, string path2)
    {
        if (path1.LastIndexOf('/') != path1.Length - 1)
        {
            path1 += "/";
        }
        if (path2.IndexOf('/') == 0)
        {
            path2 = path2.Substring(1);
        }
        return path1 + path2;
    }

    public static string CombinePaths(params string[] values)
    {
        if (values.Length <= 0)
        {
            return string.Empty;
        }
        if (values.Length == 1)
        {
            return FileManager.CombinePath(values[0], string.Empty);
        }
        if (values.Length > 1)
        {
            string text = FileManager.CombinePath(values[0], values[1]);
            for (int i = 2; i < values.Length; i++)
            {
                text = FileManager.CombinePath(text, values[i]);
            }
            return text;
        }
        return string.Empty;
    }

    public static void CopyFile(string srcFile, string dstFile)
    {
        File.Copy(srcFile, dstFile, true);
    }

    public static bool CreateDirectory(string directory)
    {
        if (FileManager.IsDirectoryExist(directory))
        {
            return true;
        }
        int num = 0;
        bool result;
        while (true)
        {
            try
            {
                Directory.CreateDirectory(directory);
                result = true;
                break;
            }
            catch (Exception ex)
            {
                num++;
                if (num >= 3)
                {
                    Debug.Log("Create Directory " + directory + " Error! Exception = " + ex.ToString());
                    FileManager.s_delegateOnOperateFileFail(directory, enFileOperation.CreateDirectory);
                    result = false;
                    break;
                }
            }
        }
        return result;
    }

    public static bool DeleteDirectory(string directory)
    {
        if (!FileManager.IsDirectoryExist(directory))
        {
            return true;
        }
        int num = 0;
        bool result;
        while (true)
        {
            try
            {
                //Directory.Delete(directory, true);
                result = true;
                break;
            }
            catch (Exception ex)
            {
                num++;
                if (num >= 3)
                {
                    Debug.Log("Delete Directory " + directory + " Error! Exception = " + ex.ToString());
                    FileManager.s_delegateOnOperateFileFail(directory, enFileOperation.DeleteDirectory);
                    result = false;
                    break;
                }
            }
        }
        return result;
    }

    public static bool DeleteFile(string filePath)
    {
        if (!FileManager.IsFileExist(filePath))
        {
            return true;
        }
        int num = 0;
        bool result;
        while (true)
        {
            try
            {
                File.Delete(filePath);
                result = true;
                break;
            }
            catch (Exception ex)
            {
                num++;
                if (num >= 3)
                {
                    Debug.Log("Delete File " + filePath + " Error! Exception = " + ex.ToString());
                    FileManager.s_delegateOnOperateFileFail(filePath, enFileOperation.DeleteFile);
                    result = false;
                    break;
                }
            }
        }
        return result;
    }

    public static string EraseExtension(string fullName)
    {
        if (fullName == null)
        {
            return null;
        }
        int num = fullName.LastIndexOf('.');
        if (num > 0)
        {
            return fullName.Substring(0, num);
        }
        return fullName;
    }

    public static string GetCachePath(string fileName)
    {
        return FileManager.CombinePath(FileManager.GetCachePath(), fileName);
    }

    public static string GetCachePath()
    {
        if (FileManager.s_cachePath == null)
        {
            FileManager.s_cachePath = Application.persistentDataPath;
        }
        return FileManager.s_cachePath;
    }

    public static string GetCachePathWithHeader(string fileName)
    {
        return FileManager.GetLocalPathHeader() + FileManager.GetCachePath(fileName);
    }

    public static string GetExtension(string fullName)
    {
        int num = fullName.LastIndexOf('.');
        if (num > 0 && num + 1 < fullName.Length)
        {
            return fullName.Substring(num);
        }
        return string.Empty;
    }

    public static int GetFileLength(string filePath)
    {
        if (!FileManager.IsFileExist(filePath))
        {
            return 0;
        }
        int num = 0;
        int result = 0;
        while (true)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                //result = (int)fileInfo.Length;
                break;
            }
            catch (Exception ex)
            {
                num++;
                if (num >= 3)
                {
                    Debug.Log("Get FileLength of " + filePath + " Error! Exception = " + ex.ToString());
                    result = 0;
                    break;
                }
            }
        }
        return result;
    }

    public static string GetFileMd5(string filePath)
    {
        if (!FileManager.IsFileExist(filePath))
        {
            return string.Empty;
        }
        return BitConverter.ToString(FileManager.s_md5Provider.ComputeHash(FileManager.ReadFile(filePath))).Replace("-", string.Empty);
    }

    public static string GetFullDirectory(string fullPath)
    {
        return Path.GetDirectoryName(fullPath);
    }

    public static string GetFullName(string fullPath)
    {
        if (fullPath == null)
        {
            return null;
        }
        int num = fullPath.LastIndexOf("/");
        if (num > 0)
        {
            return fullPath.Substring(num + 1, fullPath.Length - num - 1);
        }
        return fullPath;
    }

    public static string GetIFSExtractPath()
    {
        if (FileManager.s_ifsExtractPath == null)
        {
            FileManager.s_ifsExtractPath = FileManager.CombinePath(FileManager.GetCachePath(), FileManager.s_ifsExtractFolder);
        }
        return FileManager.s_ifsExtractPath;
    }

    private static string GetLocalPathHeader()
    {
        return "file://";
    }

    public static string GetMd5(byte[] data)
    {
        return BitConverter.ToString(FileManager.s_md5Provider.ComputeHash(data)).Replace("-", string.Empty);
    }

    public static string GetMd5(string str)
    {
        return BitConverter.ToString(FileManager.s_md5Provider.ComputeHash(Encoding.UTF8.GetBytes(str))).Replace("-", string.Empty);
    }

    public static string GetStreamingAssetsPathWithHeader(string fileName)
    {
        return Path.Combine(Application.streamingAssetsPath, fileName);
    }

    public static bool IsDirectoryExist(string directory)
    {
        return Directory.Exists(directory);
    }

    public static bool IsFileExist(string filePath)
    {
        return File.Exists(filePath);
    }

    public static byte[] ReadFile(string filePath)
    {
        if (!FileManager.IsFileExist(filePath))
        {
            return null;
        }
        byte[] array = null;
        int num = 0;
        do
        {
            try
            {
                //array = File.ReadAllBytes(filePath);
            }
            catch (Exception ex)
            {
                Debug.Log(string.Concat(new object[] {
					"Read File ",
					filePath,
					" Error! Exception = ",
					ex.ToString (),
					", TryCount = ",
					num
				}));
                array = null;
            }
            if (array != null && array.Length > 0)
            {
                return array;
            }
            num++;
        }
        while (num < 3);
        Debug.Log(string.Concat(new object[] {
			"Read File ",
			filePath,
			" Fail!, TryCount = ",
			num
		}));
        FileManager.s_delegateOnOperateFileFail(filePath, enFileOperation.ReadFile);
        return null;
    }

    public static bool WriteFile(string filePath, byte[] data, int offset, int length)
    {
        FileStream fileStream = null;
        int num = 0;
        bool result;
        while (true)
        {
            try
            {
                fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                fileStream.Write(data, offset, length);
                fileStream.Close();
                result = true;
                break;
            }
            catch (Exception ex)
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
                num++;
                if (num >= 3)
                {
                    Debug.Log("Write File " + filePath + " Error! Exception = " + ex.ToString());
                    FileManager.DeleteFile(filePath);
                    FileManager.s_delegateOnOperateFileFail(filePath, enFileOperation.WriteFile);
                    result = false;
                    break;
                }
            }
        }
        return result;
    }

    public static bool WriteFile(string filePath, byte[] data)
    {
        int num = 0;
        bool result;
        while (true)
        {
            try
            {
                //File.WriteAllBytes(filePath, data);
                result = true;
                break;
            }
            catch (Exception ex)
            {
                num++;
                if (num >= 3)
                {
                    Debug.Log("Write File " + filePath + " Error! Exception = " + ex.ToString());
                    FileManager.DeleteFile(filePath);
                    FileManager.s_delegateOnOperateFileFail(filePath, enFileOperation.WriteFile);
                    result = false;
                    break;
                }
            }
        }
        return result;
    }
}
