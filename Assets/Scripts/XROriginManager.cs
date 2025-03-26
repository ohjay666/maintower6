using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;


public class XROriginManager : MonoBehaviour
{
    public XROrigin xrOrigin; // XR Origin (main tracking root)
    public string targetImageName; // The name of the reference image
    public GameObject virtualTargetObject; // The GameObject that represents the virtual position
    public OverlayText dbg;

    ARTrackedImage trackedImageActive;

    public void HandleTrackedImage()
    {
        // Run only when tracking state changes to "Tracking"
        if (trackedImageActive.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
        {
            Vector3 worldPosition = xrOrigin.transform.TransformPoint(trackedImageActive.transform.localPosition);
            l.g($"🔹 Tracked Image: {trackedImageActive.referenceImage.name}, Position: {worldPosition}");
            // Call your custom positioning logic here
            OnImageTracked();
        }
    }
    private void OnImageTracked()
    {
        if (true || trackedImageActive.referenceImage.name == targetImageName)
        {
            Vector3 detectedPosition = trackedImageActive.transform.position;
            Quaternion detectedRotation = trackedImageActive.transform.rotation;
            AddScan(detectedPosition, detectedRotation);
        }
    }
    private void setXROrigin()
    {
        Vector3 virtualTargetPosition = virtualTargetObject.transform.position;
        Quaternion virtualTargetRotation = virtualTargetObject.transform.rotation;
        // Position Offset
        Vector3 positionOffset = virtualTargetPosition - refinedAvgPos;
        xrOrigin.transform.position += positionOffset;

        // Rotation Offset
        Quaternion rotationOffset = virtualTargetRotation * Quaternion.Inverse(refinedAvgRot);
        xrOrigin.transform.rotation = rotationOffset * xrOrigin.transform.rotation;

        l.g($"XR Origin repositioned and rotated to match tracked image. Position Offset: {positionOffset}, Rotation Offset: {rotationOffset.eulerAngles}");
    }
    private List<(Vector3 position, Quaternion rotation)> scans = new List<(Vector3, Quaternion)>();

    public void AddScan(Vector3 scanPosition, Quaternion scanRotation)
    {
        // Collect up to 40 scans
        if (scans.Count < 40)
        {
            scans.Add((scanPosition, scanRotation));
            dbg.set($"scan No.#{scans.Count}");
            // Process once when we have 40 scans
            if (scans.Count == 40)
            {
                dbg.set($"scan complete -> setting origin");
                ProcessScans();
            }
        }
    }
    Vector3 refinedAvgPos;
    Quaternion refinedAvgRot;
    private void ProcessScans()
    {
        // Step 1: Compute the initial average position and rotation
        Vector3 initialAvgPos = ComputeAveragePosition(scans.Select(s => s.position).ToList());
        Quaternion initialAvgRot = ComputeAverageRotation(scans.Select(s => s.rotation).ToList());

        // Step 2: Rank scans by position + rotation difference
        var closestScans = scans
            .OrderBy(s => PositionRotationDifference(s.position, s.rotation, initialAvgPos, initialAvgRot))
            .Take(20) // Select the best 20
            .ToList();

        // Step 3: Compute refined averages
        refinedAvgPos = ComputeAveragePosition(closestScans.Select(s => s.position).ToList());
        refinedAvgRot = ComputeAverageRotation(closestScans.Select(s => s.rotation).ToList());

        Debug.Log($"Initial Avg Position: {initialAvgPos}, Rotation: {initialAvgRot.eulerAngles}");
        Debug.Log($"Refined Avg Position: {refinedAvgPos}, Rotation: {refinedAvgRot.eulerAngles}");

        // Clear scans for next batch
        Invoke(nameof(ResetScans), 3f);
        // scans.Clear();
        setXROrigin();
    }
    private void ResetScans()
    {
        scans.Clear();
        l.g("Scan list reset after 3 seconds.");
        dbg.set($"ready for next scan");
    }


    private Vector3 ComputeAveragePosition(List<Vector3> positions)
    {
        if (positions.Count == 0) return Vector3.zero;
        return new Vector3(
            positions.Average(p => p.x),
            positions.Average(p => p.y),
            positions.Average(p => p.z)
        );
    }

    private Quaternion ComputeAverageRotation(List<Quaternion> rotations)
    {
        if (rotations.Count == 0) return Quaternion.identity;

        Quaternion avg = rotations[0];
        for (int i = 1; i < rotations.Count; i++)
        {
            avg = Quaternion.Slerp(avg, rotations[i], 1f / (i + 1)); // Spherical averaging
        }
        return avg;
    }

    private float PositionRotationDifference(Vector3 pos, Quaternion rot, Vector3 avgPos, Quaternion avgRot)
    {
        float positionDifference = Vector3.Distance(pos, avgPos);
        float rotationDifference = Quaternion.Angle(rot, avgRot) / 180f; // Normalize to 0-1
        return positionDifference + rotationDifference;
    }

}
