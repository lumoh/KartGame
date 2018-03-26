﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkSyncTransform : NetworkBehaviour
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

    void Start()
    {
        
    }

    void Update()
    {
        if (isLocalPlayer)
            return;
        
        InterpolatePosition();
        InterpolateRotation();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        var posChanged = IsPositionChanged();

        if (posChanged)
        {
            CmdSendPosition(transform.position);
            _lastPosition = transform.position;
        }

        var rotChanged = IsRotationChanged();

        if (rotChanged)
        {
            CmdSendRotation(transform.localEulerAngles);
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

    [Command(channel = Channels.DefaultUnreliable)]
    public void CmdSendPosition(Vector3 pos)
    {
        _lastPosition = pos;
    }

    [Command(channel = Channels.DefaultUnreliable)]
    protected void CmdSendRotation(Vector3 rot)
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