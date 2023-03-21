using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    static private ScoreManager S;

    [Header("Settings")]
    public List<float> _scoreFontSizes = new List<float> { 36, 64, 64, 1 };
    public Vector3 _scoreMidPoin = new Vector3(1, 1, 0);
    public float _scoreTravelTime = 3f;
    public float _scoreComboDelay = 0.5f;

    private RectTransform _rectTrans;

    private void Awake()
    {
        S = this;
        _rectTrans = GetComponent<RectTransform>();
    }

    static public void SCORE(Wyrd wyrd, int combo)
    {
        S.Score(wyrd, combo);
    }

    void Score(Wyrd wyrd, int combo)
    {
        List<Vector2> pts = new List<Vector2>();

        Vector3 pt = wyrd._letters[0].transform.position;
        pt = Camera.main.WorldToViewportPoint(pt);

        pts.Add(pt);
        pts.Add(_scoreMidPoin);
        pts.Add(_rectTrans.anchorMax);

        int value = wyrd._letters.Count * combo;
        FloatingScore fs = Scoreboard.S.CreateFloatingScore(value, pts);

        fs.timeDuration = _scoreTravelTime;
        fs.timeStart = Time.time + combo * _scoreComboDelay;
        fs.fontSizes = _scoreFontSizes;

        fs.easingCurve = Easing.InOut + Easing.InOut;

        string s = wyrd._letters.Count.ToString();
        if(combo > 1)
        {
            s += " x " + combo;
        }
        fs.GetComponent<Text>().text = s;
    }
}
