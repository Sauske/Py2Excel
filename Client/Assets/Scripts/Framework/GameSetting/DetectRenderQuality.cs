using System;
using UnityEngine;

public class DetectRenderQuality 
{
    public static bool TryGetInt(ref int val, string str)
    {
        val = 0;
        bool result;
        try
        {
            val = Convert.ToInt32(str);
            result = true;
        }
        catch
        {
            result = false;
        }
        return result;
    }

    private static GameRenderQuality checkGPU_Adreno(string[] tokens)
    {
        int num = 0;
        for (int i = 1; i < tokens.Length; i++)
        {
            if (DetectRenderQuality.TryGetInt(ref num, tokens[i]))
            {
                if (num < 200)
                {
                    return GameRenderQuality.Low;
                }
                if (num < 300)
                {
                    if (num > 220)
                    {
                        return GameRenderQuality.Low;
                    }
                    return GameRenderQuality.Low;
                }
                else if (num < 400)
                {
                    if (num >= 330)
                    {
                        return GameRenderQuality.High;
                    }
                    if (num >= 320)
                    {
                        return GameRenderQuality.Medium;
                    }
                    return GameRenderQuality.Low;
                }
                else if (num >= 400)
                {
                    if (num < 420)
                    {
                        return GameRenderQuality.Medium;
                    }
                    return GameRenderQuality.High;
                }
            }
        }
        return GameRenderQuality.Low;
    }


    private static GameRenderQuality checkGPU_PowerVR(string[] tokens)
    {
        bool flag = false;
        bool flag2 = false;
        GameRenderQuality result = GameRenderQuality.Low;
        int num = 0;
        for (int i = 1; i < tokens.Length; i++)
        {
            string text = tokens[i];
            if (text == "sgx")
            {
                flag = true;
            }
            else
            {
                if (text == "rogue")
                {
                    flag2 = true;
                    break;
                }
                if (flag)
                {
                    bool flag3 = false;
                    int num2 = text.IndexOf("mp");
                    if (num2 > 0)
                    {
                        DetectRenderQuality.TryGetInt(ref num, text.Substring(0, num2));
                        flag3 = true;
                    }
                    else if (DetectRenderQuality.TryGetInt(ref num, text))
                    {
                        for (int j = i + 1; j < tokens.Length; j++)
                        {
                            text = tokens[j].ToLower();
                            if (text.IndexOf("mp") >= 0)
                            {
                                flag3 = true;
                                break;
                            }
                        }
                    }
                    if (num > 0)
                    {
                        if (num < 543)
                        {
                            result = GameRenderQuality.Low;
                        }
                        else if (num == 543)
                        {
                            result = GameRenderQuality.Low;
                        }
                        else if (num == 544)
                        {
                            result = GameRenderQuality.Low;
                            if (flag3)
                            {
                                result = GameRenderQuality.Medium;
                            }
                        }
                        else
                        {
                            result = GameRenderQuality.Medium;
                        }
                        break;
                    }
                }
                else if (text.Length > 4)
                {
                    char c = text[0];
                    char c2 = text[1];
                    if (c == 'g')
                    {
                        if (c2 >= '0' && c2 <= '9')
                        {
                            DetectRenderQuality.TryGetInt(ref num, text.Substring(1));
                        }
                        else
                        {
                            DetectRenderQuality.TryGetInt(ref num, text.Substring(2));
                        }
                        if (num > 0)
                        {
                            if (num >= 7000)
                            {
                                result = GameRenderQuality.High;
                            }
                            else if (num >= 6000)
                            {
                                if (num < 6100)
                                {
                                    result = GameRenderQuality.Low;
                                }
                                else if (num < 6400)
                                {
                                    result = GameRenderQuality.Medium;
                                }
                                else
                                {
                                    result = GameRenderQuality.High;
                                }
                            }
                            else
                            {
                                result = GameRenderQuality.Low;
                            }
                            break;
                        }
                    }
                }
            }
        }
        if (flag2)
        {
            result = GameRenderQuality.High;
        }
        return result;
    }

    private static GameRenderQuality checkGPU_Mali(string[] tokens)
		{
			int num = 0;
			GameRenderQuality result = GameRenderQuality.Low;
			for (int i = 1; i < tokens.Length; i++)
			{
				string text = tokens[i];
				if (text.Length >= 3)
				{
					int num2 = text.LastIndexOf("mp");
					bool flag = text[0] == 't';
					if (num2 > 0)
					{
						int num3 = (!flag) ? 0 : 1;
						text = text.Substring(num3, num2 - num3);
						DetectRenderQuality.TryGetInt(ref num, text);
					}
					else
					{
						if (flag)
						{
							text = text.Substring(1);
						}
						if (DetectRenderQuality.TryGetInt(ref num, text))
						{
							for (int j = i + 1; j < tokens.Length; j++)
							{
								text = tokens[j];
								if (text.IndexOf("mp") >= 0)
								{
									break;
								}
							}
						}
					}
					if (num > 0)
					{
						if (num < 400)
						{
							result = GameRenderQuality.Low;
						}
						else if (num < 500)
						{
							if (num == 400)
							{
								result = GameRenderQuality.Low;
							}
							else if (num == 450)
							{
								result = GameRenderQuality.Medium;
							}
							else
							{
								result = GameRenderQuality.Low;
							}
						}
						else if (num < 700)
						{
							if (!flag)
							{
								result = GameRenderQuality.Low;
							}
							else if (num < 620)
							{
								result = GameRenderQuality.Low;
							}
							else if (num < 628)
							{
								result = GameRenderQuality.Medium;
							}
							else
							{
								result = GameRenderQuality.High;
							}
						}
						else if (!flag)
						{
							result = GameRenderQuality.Low;
						}
						else
						{
							result = GameRenderQuality.High;
						}
						break;
					}
				}
			}
			return result;
		}

		private static GameRenderQuality checkGPU_Tegra(string[] tokens)
		{
			bool flag = false;
			int num = 0;
			GameRenderQuality result = GameRenderQuality.Low;
			for (int i = 1; i < tokens.Length; i++)
			{
				if (DetectRenderQuality.TryGetInt(ref num, tokens[i]))
				{
					flag = true;
					if (num >= 4)
					{
						result = GameRenderQuality.High;
						break;
					}
					if (num == 3)
					{
						result = GameRenderQuality.Medium;
						break;
					}
				}
				else
				{
					string a = tokens[i];
					if (a == "k1")
					{
						result = GameRenderQuality.High;
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				result = GameRenderQuality.Medium;
			}
			return result;
		}

		private static GameRenderQuality checkGPU_Android(string gpuName)
		{
			GameRenderQuality result = GameRenderQuality.Low;
			int systemMemorySize = SystemInfo.systemMemorySize;
			if (systemMemorySize < 1500)
			{
				return GameRenderQuality.Low;
			}
			gpuName = gpuName.ToLower();
			char[] separator = new char[]
			{
				' ',
				'\t',
				'\r',
				'\n',
				'+',
				'-',
				':'
			};
			string[] array = gpuName.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			if (array == null || array.Length == 0)
			{
				return GameRenderQuality.Low;
			}
			if (array[0].Contains("vivante"))
			{
				result = GameRenderQuality.Low;
			}
			else if (array[0] == "adreno")
			{
				result = DetectRenderQuality.checkGPU_Adreno(array);
			}
			else if (array[0] == "powervr" || array[0] == "imagination" || array[0] == "sgx")
			{
				result = DetectRenderQuality.checkGPU_PowerVR(array);
			}
			else if (array[0] == "arm" || array[0] == "mali" || (array.Length > 1 && array[1] == "mali"))
			{
				result = DetectRenderQuality.checkGPU_Mali(array);
			}
			else if (array[0] == "tegra" || array[0] == "nvidia")
			{
				result = DetectRenderQuality.checkGPU_Tegra(array);
			}
			return result;
		}

		private static void checkDevice_Android(ref GameRenderQuality q)
		{
			string a = SystemInfo.deviceModel.ToLower();
			if (a == "samsung gt-s7568i")
			{
				q = GameRenderQuality.Low;
			}
			else if (a == "xiaomi 1s")
			{
				q = GameRenderQuality.Medium;
			}
			else if (a == "xiaomi 2013022")
			{
				q = GameRenderQuality.Medium;
			}
			else if (a == "samsung sch-i959")
			{
				q = GameRenderQuality.Medium;
			}
			else if (a == "xiaomi mi 3")
			{
				q = GameRenderQuality.High;
			}
			else if (a == "xiaomi mi 2a")
			{
				q = GameRenderQuality.Medium;
			}
			else if (a == "xiaomi hm 1sc")
			{
				q = GameRenderQuality.Low;
			}
		}

		public static GameRenderQuality check_Android()
		{
			GameRenderQuality result = DetectRenderQuality.checkGPU_Android(SystemInfo.graphicsDeviceName);
			DetectRenderQuality.checkDevice_Android(ref result);
			return result;
		}

}
