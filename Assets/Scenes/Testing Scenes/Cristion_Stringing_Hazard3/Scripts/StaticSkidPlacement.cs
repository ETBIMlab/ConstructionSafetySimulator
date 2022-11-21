using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StaticSkidPlacement : MonoBehaviour
{
    [SerializeField] PlayableDirector _skidDirector;
    [SerializeField] Transform _skid;
    List<Transform> _planks = new List<Transform>();
    int _plankIndex;

    private void Awake()
    {
        for (int i = _skid.childCount - 1; i > 0; i--)
            _planks.Add(_skid.GetChild(i));
    }

    private void Start()
    {
        PlayTimeline();
    }

    public void PlayTimeline()
    {
        if (_plankIndex >= _planks.Count)
            return;

        _skidDirector.Play();
    }

    public void PlacePlank()
    {
        _planks[_plankIndex].gameObject.SetActive(true);
        _plankIndex++;
    }
}
