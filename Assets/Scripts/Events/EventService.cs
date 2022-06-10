using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class EventService : MonoBehaviour
{
    private const string ServerUrl = "https://webhook.site/6d0ce2ea-88fd-4c71-a0e4-0837c9b09787";
    private const string BackupName = "Events";
    private const int SuccessCode = 200;
    private const float CooldownBeforeSend = 4f;

    private EventsWrapper _eventsToSend = new EventsWrapper();
    private bool _isCoroutineRunning;
    private WaitForSeconds _waitTime;

    private void Awake()
    {
        _waitTime = new WaitForSeconds(CooldownBeforeSend);
        LoadBackupFile();

        if (_eventsToSend.events.Count > 0)
        {
            StartCoroutine(SendEventsToServer());
        }
    }

    public void TrackEvent(string type, string data)
    {
        var eventStruct = new EventStruct()
        {
            type = type,
            data = data
        };

        _eventsToSend.events.Add(eventStruct);
        SaveBackupFile();

        if (_isCoroutineRunning == false)
        {
            StartCoroutine(SendEventsToServer());
        }
    }

    private IEnumerator SendEventsToServer()
    {
        _isCoroutineRunning = true;

        while (true)
        {
            yield return _waitTime;

            var form = new WWWForm();
            using var request = UnityWebRequest.Post(ServerUrl, form);

            string json = JsonUtility.ToJson(_eventsToSend);
            byte[] postBytes = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(postBytes);

            yield return request.SendWebRequest();

            if (request.responseCode == SuccessCode)
            {
                _eventsToSend.events.Clear();
                _isCoroutineRunning = false;
                ClearBackupFile();

                yield break;
            }
        }
    }

    private void LoadBackupFile()
    {
        string eventsJson = PlayerPrefs.GetString(BackupName);

        if (eventsJson.Length > 0)
        {
            _eventsToSend = JsonUtility.FromJson<EventsWrapper>(eventsJson);
            ClearBackupFile();
        }
        else
        {
            _eventsToSend = new EventsWrapper();
        }
    }

    private void SaveBackupFile()
    {
        var eventsJson = JsonUtility.ToJson(_eventsToSend);
        PlayerPrefs.SetString(BackupName, eventsJson);
        PlayerPrefs.Save();
    }

    private void ClearBackupFile()
    {
        PlayerPrefs.DeleteKey(BackupName);
    }
}