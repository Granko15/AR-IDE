using Antilatency.SDK;
using UnityEngine;

public class CustomAltTrackingUsbSocket : AltTrackingUsbSocket
{
    [SerializeField]
    protected Transform target;

    protected override void Update()
    {
        base.Update();

        Antilatency.Alt.Tracking.State trackingState;

        if (!GetTrackingState(out trackingState))
        {
            return;
        }

        target.localPosition = trackingState.pose.position;
        target.localRotation = trackingState.pose.rotation;
    }
}
