using System.Collections.Generic;
using UnityEngine;

public class WandController : MonoBehaviour
{
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;

    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObject.index); } }
    private SteamVR_TrackedObject trackedObject;

    private HashSet<InteractableItem> objectsHoveringOver = new HashSet<InteractableItem>();
    private InteractableItem closestItem;
    private InteractableItem interactingItem;

    // Use this for initialization
    void Start()
    {
        trackedObject = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller == null)
        {
            Debug.Log("Controller not yet initialized");
            return;
        }

        if (controller.GetPressDown(gripButton))
        {
            Debug.Log("gripButtonDown detected");
            float minDistance = float.MaxValue;

            float distance;
            foreach (InteractableItem item in objectsHoveringOver)
            {
                distance = (item.transform.position - transform.position).sqrMagnitude;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestItem = item;
                }
            }

            interactingItem = closestItem;
            closestItem = null;

            if (interactingItem)
            {
                if (interactingItem.IsInteracting())
                {
                    interactingItem.EndInteraction(this);
                }

                interactingItem.BeginInteraction(this);
            }
        }

        if (controller.GetPressUp(gripButton) && interactingItem != null)
        {
            Debug.Log("gripButtonUp detected");
            interactingItem.EndInteraction(this);
        }

        if (controller.GetPressDown(triggerButton))
        {
            Debug.Log("triggerButtonDown detected");
        }

        if (controller.GetPressUp(triggerButton))
        {
            Debug.Log("triggerButtonUp detected");
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("OnTriggerEnter detected");
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        if (collidedItem)
        {
            objectsHoveringOver.Add(collidedItem);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        Debug.Log("OnTriggerExit detected");
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        if (collidedItem)
        {
            objectsHoveringOver.Remove(collidedItem);
        }
    }
}
