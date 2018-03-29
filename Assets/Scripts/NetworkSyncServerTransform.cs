using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkSyncServerTransform : NetworkBehaviour
{
    [SerializeField]
    protected float _posLerpRate = 15;
    [SerializeField]
    protected float _rotLerpRate = 15;
    [SerializeField]
    protected float _posThreshold = 0.1f;
    [SerializeField]
    protected float _rotThreshold = 1f;

    [SyncVar]
    protected Vector3 _lastPosition;

    [SyncVar]
    protected Vector3 _lastRotation;

    public virtual void Start()
    {
        
    }

    public virtual void Update()
    {
        if (isServer)
            return;
        
        InterpolatePosition();
        InterpolateRotation();
    }

    public virtual void FixedUpdate()
    {
        if (!isServer)
            return;

        var posChanged = IsPositionChanged();

        if (posChanged)
        {
            Rpc_SendPosition(transform.position);
            _lastPosition = transform.position;
        }

        var rotChanged = IsRotationChanged();

        if (rotChanged)
        {
            Rpc_SendRotation(transform.localEulerAngles);
            _lastRotation = transform.localEulerAngles;
        }
    }

    public void InterpolatePosition()
    {
        transform.position = Vector3.Lerp(transform.position, _lastPosition, Time.deltaTime * _posLerpRate);
    }

    protected void InterpolateRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_lastRotation), Time.deltaTime * _rotLerpRate);
    }

    [ClientRpc]
    public void Rpc_SendPosition(Vector3 pos)
    {
        _lastPosition = pos;
    }

    [ClientRpc]
    protected void Rpc_SendRotation(Vector3 rot)
    {
        _lastRotation = rot;
    }

    protected bool IsPositionChanged()
    {
        return Vector3.Distance(transform.position, _lastPosition) > _posThreshold;
    }

    protected bool IsRotationChanged()
    {
        return Vector3.Distance(transform.localEulerAngles, _lastRotation) > _rotThreshold;
    }

    public override int GetNetworkChannel()
    {
        return Channels.DefaultUnreliable;
    }

    public override float GetNetworkSendInterval()
    {
        return 0.01f;
    }
}