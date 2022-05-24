using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// ����ɨ���㷨��Saito and Toriwaki, 1994��
/// </summary>
public class IndependentScanning
{
    public static float[,] CreateSDF(bool[,] color)
    {
        int width = color.GetLength(0);
        int height = color.GetLength(1);

        var positiveMaps = new bool[width, height];
        var negativeMaps = new bool[width, height];
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                // ��ɫ�ڵ�Ϊtrue����ɫ�ڵ�Ϊfalse
                if (color[i, j] == true)
                {
                    positiveMaps[i, j] = true;  // ����map�������ɫ�ڵ㣨Ŀ��㣩����ɫ�ڵ㣨��Ŀ��㣩�ľ���
                    negativeMaps[i, j] = false; // ����map�������ɫ�ڵ㣨Ŀ��㣩����ɫ�ڵ㣨��Ŀ��㣩�ľ���
                }
                else
                {
                    positiveMaps[i, j] = false;
                    negativeMaps[i, j] = true;
                }
            }
        }
        var d1 = Compute(positiveMaps);
        var d2 = Compute(negativeMaps);

        float[,] distance = new float[width, height];
        float max = float.MinValue;
        float min = float.MaxValue;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                float d = (float)(Math.Sqrt(d1[i, j]) - Math.Sqrt(d2[i, j]));
                distance[i, j] = d;
                max = Math.Max(d, max);
                min = Math.Min(d, min);
            }
        }
        // �������ֵ����Сֵ�Ĳ�ֵ
        float clamp = max - min;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                if (clamp <= 0)
                {
                    distance[i, j] = 0;
                }
                else
                {
                    // ������ӳ��Ϊ[0,1]
                    distance[i, j] = (distance[i, j] - min) / clamp;
                }
            }
        }
        return distance;
    }

    public static int[,] Compute(bool[,] maps)
    {
        int width = maps.GetLength(0);
        int height = maps.GetLength(1);

        int[,] distance = new int[width, height];
        int[] temp_width = new int[width];
        // 1. ��ÿһ�У�����Ŀ��㵽��Ŀ���ľ��룬���õ�ÿ����(i,j)��ˮƽ����
        for (int j = 0; j < height; j++)
        {
            // �Ƿ��з�Ŀ���
            bool hasTarget = false;
            for (int i = 0; i < width; i++)
            {
                if (maps[i, j] == true)
                {
                    if (i == 0)
                    {
                        temp_width[i] = int.MaxValue;
                    }
                    else
                    {
                        if (temp_width[i - 1] == int.MaxValue)
                        {
                            temp_width[i] = int.MaxValue;
                        }
                        else
                        {
                            temp_width[i] = temp_width[i - 1] + 1;
                        }
                    }
                }
                else
                {
                    hasTarget = true;
                    temp_width[i] = 0;
                }
            }
            for (int i = width - 1; i >= 0; i--)
            {
                if (hasTarget == false)
                {
                    temp_width[i] = width;
                }
                else
                {
                    if (maps[i, j] == true)
                    {
                        if (i < width - 1)
                        {
                            temp_width[i] = Math.Min(temp_width[i + 1] + 1, temp_width[i]);
                        }
                    }
                    else
                    {
                        temp_width[i] = 0;
                    }
                }
            }

            for (int i = 0; i < width; i++)
            {
                distance[i, j] = temp_width[i] * temp_width[i];
            }
        }
        // 2. ��ÿһ�У�����ÿ����(i,j)��ÿһ��(i,y)����ֱ���룬���õ���ǰ��(i, j)����ֱ��������
        // 3. ����ǰ��(i, j)����ֱ��������͵�ǰ�еĶ�Ӧ��(i, y)��ˮƽ������ӣ����е���Сֵ��Ϊ��ǰ��(i,j)�ľ���
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                int min = int.MaxValue;
                for (int y = 0; y < height; y++)
                {
                    var dis = distance[i, y] + (j - y) * (j - y);
                    min = Math.Min(dis, min);
                }
                temp_width[i] = min;
            }

            for (int i = 0; i < width; i++)
            {
                distance[i, j] = temp_width[i];
            }
        }
        return distance;
    }
}
