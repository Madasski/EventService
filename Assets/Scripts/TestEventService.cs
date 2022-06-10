using UnityEngine;

public class TestEventService : MonoBehaviour
{
    [SerializeField] private EventService _eventService;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _eventService.TrackEvent("keyPress", "Space");
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _eventService.TrackEvent("keyPress", "Number1");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _eventService.TrackEvent("keyPress", "Number2");
        }
    }
}