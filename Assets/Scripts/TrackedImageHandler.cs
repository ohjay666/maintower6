using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TrackedImageHandler : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;

    private readonly Dictionary<TrackableId, ARTrackedImage> trackedImages = new();

    private void Update()
    {
        if (trackedImageManager == null) return;

        foreach (var trackedImage in trackedImageManager.trackables)
        {
            if (!trackedImages.ContainsKey(trackedImage.trackableId))
            {
                // This is a newly detected image
                Debug.Log($"Added: {trackedImage.referenceImage.name} at {trackedImage.transform.position}");
                trackedImages[trackedImage.trackableId] = trackedImage;
            }
            else
            {
                // Check if it has changed
                if (trackedImages[trackedImage.trackableId].transform.position != trackedImage.transform.position)
                {
                    Debug.Log($"Updated: {trackedImage.referenceImage.name} at {trackedImage.transform.position}");
                    trackedImages[trackedImage.trackableId] = trackedImage;
                }
            }
        }

        // Handle removed images
        List<TrackableId> toRemove = new();
        foreach (var id in trackedImages.Keys)
        {
            if (!trackedImageManager.trackables.Contains(id))
            {
                Debug.Log($"Removed: {trackedImages[id].referenceImage.name}");
                toRemove.Add(id);
            }
        }

        foreach (var id in toRemove)
        {
            trackedImages.Remove(id);
        }
    }
}
