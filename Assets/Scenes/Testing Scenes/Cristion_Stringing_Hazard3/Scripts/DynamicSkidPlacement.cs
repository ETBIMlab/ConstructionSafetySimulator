using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum SkidPlacementState
{
    Idle,
    Pickup,
    WalkToSkid,
    Drop,
    WalkToTruck
}

struct Truck
{
    public Transform transform;
    public List<Transform> planks;
    public List<Transform> pickupSpots;
    public int plankIndex;
}

struct Skid
{
    public int index;
    public Transform transform;
    public List<Transform> planks;
    public int plankIndex;
    public Transform dropSpot;
}

public class DynamicSkidPlacement : MonoBehaviour
{
    [SerializeField] Animator _workerAnimator;
    [SerializeField] Animator _heldPlankAnimator;
    [SerializeField] Transform _flatbedTruck;
    [SerializeField] List<Transform> _skids;

    Truck _truck;
    Skid _currentSkid;
    NavMeshAgent _agent;
    SkidPlacementState _state;
    float maxForgivenessDistance = 0.01f;
    WaitForFixedUpdate _fixedWait = new WaitForFixedUpdate();

    private void Awake()
    {
        Transform container = _flatbedTruck.GetChild(1);
        foreach (Transform plank in container)
            _truck.planks.Add(plank);

        container = _flatbedTruck.GetChild(2);
        foreach (Transform spot in container)
            _truck.pickupSpots.Add(spot);

        ResetTruck();
        FocusSkid(0);

        _agent = GetComponent<NavMeshAgent>();
        _state = SkidPlacementState.Idle;
    }

    public void Play()
    {
        MoveToTruck();
    }

    void MoveToTruck()
    {
        _state = SkidPlacementState.WalkToTruck;
        //_agent.SetDestination(_truck.pickupSpots)
    }

    void PickupSkid()
    {

    }

    void MoveToSkid()
    {

    }

    void DropSkid()
    {

    }

    IEnumerator CheckDistance(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > maxForgivenessDistance)
            yield return _fixedWait;
    }

    void ResetTruck()
    {
        _truck.plankIndex = 0;

        foreach (Transform plank in _truck.planks)
            plank.gameObject.SetActive(true);
    }

    void FocusSkid(int index)
    {
        if (index >= _skids.Count)
            return;

        _currentSkid.index = index;
        _currentSkid.plankIndex = 0;

        Transform skid = _skids[index];
        _currentSkid.planks.Clear();
        for (int i = skid.childCount - 1; i > 1; i--)
            _currentSkid.planks.Add(_skids[i]);

        _currentSkid.dropSpot = skid.GetChild(0);
    }
}
