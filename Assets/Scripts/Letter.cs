using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour
{
    [Header("Settings")]
    public float _timeDuration = 0.5f;
    public string _easingCuve = Easing.InOut;

    [Header("Parameters")]
    public TextMesh _tMesh;
    public Renderer _tRend;
    public bool _big = false;

    public List<Vector3> _pts = null;
    public float _timeStart = -1;

    private char _c;
    private Renderer _rend;

    public char c
    {
        get
        {
            return _c;
        }
        set
        {
            _c = value;
            _tMesh.text = _c.ToString();
        }
    }

    public string str
    {
        get
        {
            return _c.ToString();
        }
        set
        {
            c = value[0];
        }
    }

    public bool visible
    {
        get
        {
            return _tRend.enabled;
        }
        set
        {
            _tRend.enabled = value;
        }
    }

    public Color color
    {
        get
        {
            return _rend.material.color;
        }
        set
        {
            _rend.material.color = value;
        }
    }

    public Vector3 pos
    {
        set
        {
            Vector3 mid = (transform.position + value) / 2f;
            float mag = (transform.position - value).magnitude;
            mid += Random.insideUnitSphere * mag * 0.25f;

            _pts = new List<Vector3>() { transform.position, mid, value };

            if (_timeStart == -1)
                _timeStart = Time.time;
        }
    }

    public Vector3 posImmediate
    {
        set
        {
            transform.position = value;
        }
    }

    private void Awake()
    {
        _tMesh = GetComponentInChildren<TextMesh>();
        _tRend = _tMesh.GetComponent<Renderer>();
        _rend = GetComponent<Renderer>();
        visible = false;
    }

    private void Update()
    {
        if (_timeStart == -1)
            return;

        float u = (Time.time - _timeStart) / _timeDuration;
        u = Mathf.Clamp01(u);
        float u1 = Easing.Ease(u, _easingCuve);
        Vector3 v = Utils.Bezier(u1, _pts);

        transform.position = v;

        if (u == 1)
            _timeStart = -1;
    }
}
