using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordGame : MonoBehaviour
{
    static public WordGame S;

    [Header("Settings")]
    public GameObject _prefabLetter;
    public Rect _wordArea = new Rect(-24, 19, 48, 28);
    public float _letterSize = 1.5f;
    public bool _showAllWyrds = true;
    public float _bigLetterSize = 4f;
    public Color _bigColorDim = new Color(0.8f, 0.8f, 0.8f);
    public Color _bigColorSelected = new Color(1, 0.9f, 0.7f);
    public Vector3 _bigLetterCenter = new Vector3(0, -16, 0);
    public Color[] _wyrdPalette;

    [Header("Parameters")]
    public GameMode mode = GameMode.preGame;
    public WordLevel _currentLevel;
    public List<Wyrd> _wyrds;
    public List<Letter> _bigLetters;
    public List<Letter> _bigLettersActive;
    public string _testWord;

    private string _upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private Transform _letterAnchor, _bigLetterAnchor;

    private void Awake()
    {
        S = this;
        _letterAnchor = new GameObject("LetterAnchor").transform;
        _bigLetterAnchor = new GameObject("BigLetterAnchor").transform;
    }

    private void Start()
    {
        mode = GameMode.loading;
        WordList.INIT();
    }

    private void Update()
    {
        Letter ltr;
        char c;

        switch (mode)
        {
            case GameMode.inLevel:
                foreach(char cIt in Input.inputString)
                {
                    c = System.Char.ToUpperInvariant(cIt);

                    if (_upperCase.Contains(c))
                    {
                        ltr = FindNextLetterByChar(c);

                        if(ltr != null)
                        {
                            _testWord += c.ToString();
                            _bigLettersActive.Add(ltr);
                            _bigLetters.Remove(ltr);
                            ltr.color = _bigColorSelected;

                            ArrangeBigLetters();
                        }

                        if(c == '\b')
                        {
                            if (_bigLettersActive.Count == 0)
                                return;

                            if(_testWord.Length > 1)
                            {
                                _testWord = _testWord.Substring(0, _testWord.Length - 1);
                            }
                            else
                            {
                                _testWord = "";
                            }

                            ltr = _bigLettersActive[_bigLettersActive.Count - 1];
                            _bigLettersActive.Remove(ltr);
                            _bigLetters.Add(ltr);
                            ltr.color = _bigColorDim;
                            ArrangeBigLetters();
                        }

                        if(c == '\n' || c == '\r')
                        {
                            CheckWord();
                        }

                        if(c == ' ')
                        {
                            _bigLetters = ShuffleLetters(_bigLetters);
                            ArrangeBigLetters();
                        }
                    }
                }

                break;
        }
    }

    Letter FindNextLetterByChar(char c)
    {
        foreach(Letter l in _bigLetters)
        {
            if(l.c == c)
            {
                return l;
            }
        }

        return null;
    }

    public void CheckWord()
    {
        string subWord;
        bool foundTextWord = false;

        List<int> containedWords = new List<int>();
        for(int i = 0; i<_currentLevel._subWords.Count; i++)
        {
            if (_wyrds[i]._found)
                continue;

            subWord = _currentLevel._subWords[i];
            if(string.Equals(_testWord, subWord))
            {
                HighlightWyrd(i);
                ScoreManager.SCORE(_wyrds[i], 1);
                foundTextWord = true;
            }
            else if (_testWord.Contains(subWord))
            {
                containedWords.Add(i);
            }

            if (foundTextWord)
            {
                int numContained = containedWords.Count;
                int index;

                for(int j = 0; j<containedWords.Count; j++)
                {
                    index = numContained - j - 1;
                    HighlightWyrd(containedWords[index]);
                    ScoreManager.SCORE(_wyrds[containedWords[index]], i+2);
                }
            }
        }

        ClearBigLettersActive();
    }

    void HighlightWyrd(int index)
    {
        _wyrds[index]._found = true;
        _wyrds[index].color = (_wyrds[index].color + Color.white)/2f;
        _wyrds[index].visible = true;
    }

    void ClearBigLettersActive()
    {
        _testWord = "";
        foreach (Letter l in _bigLettersActive)
        {
            _bigLetters.Add(l);
            l.color = _bigColorDim;
        }

        _bigLettersActive.Clear();
        ArrangeBigLetters();
    }

    public void WordListParseComplete()
    {
        mode = GameMode.makeLevel;
        _currentLevel = MakeWordLevel();
    }

    public WordLevel MakeWordLevel(int levelNum = -1)
    {
        WordLevel level = new WordLevel();

        if (levelNum == -1)
        {
            level._longWordIndex = Random.Range(0, WordList.LONG_WORD_COUNT);
        }
        else
        {

        }

        level._levelNum = levelNum;
        level._word = WordList.GET_LONG_WORD(level._longWordIndex);
        level._charDict = WordLevel.MakeCharDict(level._word);

        StartCoroutine(FindSubWordsCoroutine(level));
        return level;
    }


    public void SubWordSearchComplete()
    {
        mode = GameMode.levelPrep;
        Layout();
    }

    private void Layout()
    {
        _wyrds = new List<Wyrd>();

        GameObject go;
        Letter lett;
        string word;
        Vector3 pos;
        float left = 0;
        float columnWidth = 3;
        char c;
        Color col;
        Wyrd wyrd;

        int numRows = Mathf.RoundToInt(_wordArea.height / _letterSize);

        for(int i = 0; i<_currentLevel._subWords.Count; i++)
        {
            wyrd = new Wyrd();
            word = _currentLevel._subWords[i];

            columnWidth = Mathf.Max(columnWidth, word.Length);

            for(int j = 0; j<word.Length; j++)
            {
                c = word[j];
                go = Instantiate<GameObject>(_prefabLetter);
                go.transform.SetParent(_letterAnchor);
                lett = go.GetComponent<Letter>();
                lett.c = c;

                pos = new Vector3(_wordArea.x + left + j * _letterSize, _wordArea.y, 0);
                pos.y -= (i % numRows) * _letterSize;

                lett.posImmediate = pos + Vector3.up * (20 + i % numRows);
                lett.pos = pos;
                lett._timeStart = Time.time + i * 0.05f;

                go.transform.localScale = Vector3.one * _letterSize;
                wyrd.Add(lett);
            }

            if (_showAllWyrds)
                wyrd.visible = true;

            wyrd.color = _wyrdPalette[word.Length - WordList.WORD_LENGTH_MIN];

            _wyrds.Add(wyrd);

            if(i % numRows == numRows - 1)
            {
                left += (columnWidth + 0.5f) * _letterSize;
            }
        }

        _bigLetters = new List<Letter>();
        _bigLettersActive = new List<Letter>();

        for(int i = 0; i<_currentLevel._word.Length; i++)
        {
            c = _currentLevel._word[i];
            go = Instantiate<GameObject>(_prefabLetter);
            go.transform.SetParent(_bigLetterAnchor);
            lett = go.GetComponent<Letter>();
            lett.c = c;
            go.transform.localScale = Vector3.one * _bigLetterSize;

            pos = new Vector3(0, -100, 0);
            lett.posImmediate = pos;
            lett.pos = pos;

            lett._timeStart = Time.time + _currentLevel._subWords.Count * 0.05f;
            lett._easingCuve = Easing.Sin + "-0.18";

            col = _bigColorDim;
            lett.color = col;
            lett.visible = true;
            lett._big = true;
            _bigLetters.Add(lett);
        }

        _bigLetters = ShuffleLetters(_bigLetters);

        ArrangeBigLetters();

        mode = GameMode.inLevel;
    }

    List<Letter> ShuffleLetters(List<Letter> letts)
    {
        List<Letter> newL = new List<Letter>();
        int index;

        while (letts.Count > 0)
        {
            index = Random.Range(0, letts.Count);
            newL.Add(letts[index]);
            letts.RemoveAt(index);
        }

        return newL;
    }

    void ArrangeBigLetters()
    {
        float halfWidth = ((float)_bigLetters.Count) / 2f - 0.5f;
        Vector3 pos; 

        for(int i = 0; i<_bigLetters.Count; i++)
        {
            pos = _bigLetterCenter;
            pos.x += (i - halfWidth) * _bigLetterSize;
            _bigLetters[i].pos = pos;
        }

        halfWidth = ((float)_bigLettersActive.Count) / 2f - 0.5f;
        for (int i = 0; i < _bigLettersActive.Count; i++)
        {
            pos = _bigLetterCenter;
            pos.x += (i - halfWidth) * _bigLetterSize;
            pos.y += _bigLetterSize * 1.25f;
            _bigLettersActive[i].pos = pos;
        }
    }

    public IEnumerator FindSubWordsCoroutine(WordLevel level)
    {
        level._subWords = new List<string>();
        string str;

        List<string> words = WordList.GET_WORDS();

        for (int i = 0; i < WordList.WORD_COUNT; i++)
        {
            str = words[i];

            if (WordLevel.CheckWordInLevel(str, level))
            {
                level._subWords.Add(str);
            }

            if (i % WordList.NUM_TO_PARSE_BEFORE_YIELD == 0)
            {
                yield return null;
            }
        }

        level._subWords.Sort();
        level._subWords = SortWordsByLength(level._subWords).ToList();

        SubWordSearchComplete();
    }

    public static IEnumerable<string> SortWordsByLength(IEnumerable<string> ws)
    {
        ws = ws.OrderBy(s => s.Length);
        return ws;
    }
}

public enum GameMode
{
    preGame,
    loading,
    makeLevel,
    levelPrep,
    inLevel
}