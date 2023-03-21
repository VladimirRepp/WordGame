using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wyrd 
{
    public string _str;
    public List<Letter> _letters = new List<Letter>();
    public bool _found = false;

    public bool visible
    {
        get
        {
            if (_letters.Count == 0)
                return false;

            return _letters[0].visible;
        }
        set
        {
            foreach(Letter l in _letters)
            {
                l.visible = value;
            }
        }
    }

    public Color color
    {
        get
        {
            if (_letters.Count == 0)
                return Color.black;

            return _letters[0].color;
        }
        set
        {
            foreach(Letter l in _letters)
            {
                l.color = value;
            }
        }
    }

    public void Add(Letter l)
    {
        _letters.Add(l);
        _str += l.c.ToString();
    }
}
