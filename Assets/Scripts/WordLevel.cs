using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WordLevel
{
    public int _levelNum;
    public int _longWordIndex;
    public string _word;
    public Dictionary<char, int> _charDict;
    public List<string> _subWords;

    static public Dictionary<char, int> MakeCharDict(string w)
    {
        Dictionary<char, int> dict = new Dictionary<char, int>();
        char c;
        for(int i = 0; i < w.Length; i++)
        {
            c = w[i];

            if (dict.ContainsKey(c))
            {
                dict[c]++;
            }
            else
            {
                dict.Add(c, 1);
            }
        }

        return dict;
    }


    public static bool CheckWordInLevel(string str, WordLevel level)
    {
        Dictionary<char, int> counts = new Dictionary<char, int>();

        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];

            if (level._charDict.ContainsKey(c))
            {
                if (!counts.ContainsKey(c))
                {
                    counts.Add(c, 1);
                }
                else
                {
                    counts[c]++;
                }

                if (counts[c] > level._charDict[c])
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}
