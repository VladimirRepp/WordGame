using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordList : MonoBehaviour
{
    private static WordList S;

    [Header("Settings")]
    public TextAsset _wordListText;
    public int _numToParseBeforeYield = 10000;
    public int _wordLengthMin = 3;
    public int _wordLengthMax = 7;

    [Header("Parameters")]
    public int _currLine = 0;
    public int _totalLines;
    public int _longWordCount;
    public int _wordCount;

    private string[] _lines;
    private List<string> _longWords;
    private List<string> _words;

    private void Awake()
    {
        S = this;
    }

    public void Init()
    {
        _lines = _wordListText.text.Split('\n');
        _totalLines = _lines.Length;

        StartCoroutine(ParseLinesCoroutine());
    }

    static public void INIT()
    {
        S.Init();
    }

    public IEnumerator ParseLinesCoroutine()
    {
        string word;

        _longWords = new List<string>();
        _words = new List<string>();

        for(_currLine = 0; _currLine < _totalLines; _currLine++)
        {
            word = _lines[_currLine];

            if(word.Length == _wordLengthMax)
            {
                _longWords.Add(word);
            }

            if(word.Length >= _wordLengthMin && word.Length <= _wordLengthMax)
            {
                _words.Add(word);
            }

            if(_currLine % _numToParseBeforeYield == 0)
            {
                _longWordCount = _longWords.Count;
                _wordCount = _words.Count;

                yield return null;
            }
        }

        _longWordCount = _longWords.Count;
        _wordCount = _words.Count;

        gameObject.SendMessage("WordListParseComplete");
    }

    static public List<string> GET_WORDS()
    {
        return S._words;
    }

    static public string GET_WORD(int i)
    {
        return S._words[i];
    }

    static public List<string> GET_LONG_WORDS()
    {
        return S._longWords;
    }

    static public string GET_LONG_WORD(int i)
    {
        return S._longWords[i];
    }

    static public int WORD_COUNT
    {
        get
        {
            return S._wordCount;
        }
    }

    static public int LONG_WORD_COUNT
    {
        get
        {
            return S._longWordCount;
        }
    }

    static public int NUM_TO_PARSE_BEFORE_YIELD
    {
        get
        {
            return S._numToParseBeforeYield;
        }
    }

    static public int WORD_LENGTH_MIN
    {
        get
        {
            return S._wordLengthMin;
        }
    }

    static public int WORD_LENGTH_MAX
    {
        get
        {
            return S._wordLengthMin;
        }
    }
}
