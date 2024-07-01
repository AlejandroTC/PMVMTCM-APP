using UnityEngine;

public class DataSender : MonoBehaviour
{
    public void SendData(string data)
    {
        PersistentDataManager.Instance.SetReceivedData(data);
    }
}
