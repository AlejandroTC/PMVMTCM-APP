using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using TMPro;

public class ESP32BLEManager : MonoBehaviour
{
    public string DeviceName = "ESP32";
    public string ServiceUUID = "12345678-1234-1234-1234-123456789abc"; // Cambiar por el UUID de servicio de tu ESP32
    public string Characteristic = "87654321-4321-4321-4321-abcdefabcdef"; // Cambiar por el UUID de característica de tu ESP32

    public TMP_Text BluetoothStatus;
    public GameObject PanelMiddle;
    public TMP_Text TextToSend;
    public Button scanButton;
    public Button connectButton;
    public Button disconnectButton;
    public Transform devicesListParent;
    public GameObject deviceListItemPrefab;

    private List<string> deviceAddresses = new List<string>();
    private string selectedDeviceAddress;

    enum States
    {
        None,
        Scan,
        Connect,
        RequestMTU,
        Subscribe,
        Unsubscribe,
        Disconnect,
        Communication,
    }

    private bool _connected = false;
    private float _timeout = 0f;
    private States _state = States.None;

    void Start()
    {
        scanButton.onClick.AddListener(StartScan);
        connectButton.onClick.AddListener(ConnectToDevice);
        disconnectButton.onClick.AddListener(DisconnectFromDevice);

        BluetoothLEHardwareInterface.Initialize(true, false, () =>
        {
            BluetoothStatus.text = "Bluetooth Inicializado";
        }, (error) =>
        {
            BluetoothLEHardwareInterface.Log("Error: " + error);
        });
    }

    void Update()
    {
        if (_timeout > 0f)
        {
            _timeout -= Time.deltaTime;
            if (_timeout <= 0f)
            {
                _timeout = 0f;

                switch (_state)
                {
                    case States.None:
                        break;

                    case States.Scan:
                        BluetoothStatus.text = "Buscando dispositivos...";

                        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) =>
                        {
                            // if (name.Contains(DeviceName))
                            // {
                            if (!deviceAddresses.Contains(address))
                            {
                                AddDeviceToList(address, name);
                            }
                            // }
                        }, null, false, false);
                        break;

                    case States.Connect:
                        BluetoothLEHardwareInterface.ConnectToPeripheral(selectedDeviceAddress, null, null, (address, serviceUUID, characteristicUUID) =>
                        {
                            if (IsEqual(serviceUUID, ServiceUUID))
                            {
                                if (IsEqual(characteristicUUID, Characteristic))
                                {
                                    _connected = true;
                                    SetState(States.RequestMTU, 2f);
                                    BluetoothStatus.text = "Conectado";
                                }
                            }
                        }, (disconnectedAddress) =>
                        {
                            BluetoothLEHardwareInterface.Log("Device disconnected: " + disconnectedAddress);
                            BluetoothStatus.text = "Desconectado";
                            _connected = false;
                        });
                        break;

                    case States.RequestMTU:
                        BluetoothLEHardwareInterface.RequestMtu(selectedDeviceAddress, 185, (address, newMTU) =>
                        {
                            BluetoothStatus.text = "MTU set to " + newMTU.ToString();
                            SetState(States.Subscribe, 0.1f);
                        });
                        break;

                    case States.Subscribe:
                        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(selectedDeviceAddress, ServiceUUID, Characteristic, null, (address, characteristicUUID, bytes) =>
                        {
                            BluetoothStatus.text = "Recibido: " + Encoding.UTF8.GetString(bytes);
                        });
                        _state = States.None;
                        break;

                    case States.Unsubscribe:
                        BluetoothLEHardwareInterface.UnSubscribeCharacteristic(selectedDeviceAddress, ServiceUUID, Characteristic, null);
                        SetState(States.Disconnect, 4f);
                        break;

                    case States.Disconnect:
                        if (_connected)
                        {
                            BluetoothLEHardwareInterface.DisconnectPeripheral(selectedDeviceAddress, (address) =>
                            {
                                BluetoothLEHardwareInterface.DeInitialize(() =>
                                {
                                    _connected = false;
                                    _state = States.None;
                                });
                            });
                        }
                        else
                        {
                            BluetoothLEHardwareInterface.DeInitialize(() =>
                            {
                                _state = States.None;
                            });
                        }
                        break;
                }
            }
        }
    }

    void StartScan()
    {
        ClearDevicesList();
        SetState(States.Scan, 0.1f);
    }

    void ConnectToDevice()
    {
        if (!string.IsNullOrEmpty(selectedDeviceAddress))
        {
            SetState(States.Connect, 0.5f);
        }
    }

    void DisconnectFromDevice()
    {
        if (!string.IsNullOrEmpty(selectedDeviceAddress))
        {
            SetState(States.Unsubscribe, 0.1f);
        }
    }

    void AddDeviceToList(string address, string name)
    {
        GameObject listItem = Instantiate(deviceListItemPrefab, devicesListParent);
        listItem.GetComponentInChildren<TextMeshProUGUI>().text = name; // Accede al componente TextMeshPro para establecer el texto
        Button listItemButton = listItem.GetComponentInChildren<Button>(); // Accede al botón dentro del prefab
        listItemButton.onClick.AddListener(() => SelectDevice(address)); // Asigna un listener de evento al botón
        deviceAddresses.Add(address);
    }

    void SelectDevice(string address)
    {
        BluetoothLEHardwareInterface.StopScan();
        selectedDeviceAddress = address;
        BluetoothStatus.text = "Selected: " + address;
    }

    void ClearDevicesList()
    {
        foreach (Transform child in devicesListParent)
        {
            Destroy(child.gameObject);
        }
        deviceAddresses.Clear();
    }

    void SetState(States newState, float timeout)
    {
        _state = newState;
        _timeout = timeout;
    }

    string FullUUID(string uuid)
    {
        return "0000" + uuid + "-0000-1000-8000-00805F9B34FB";
    }

    bool IsEqual(string uuid1, string uuid2)
    {
        if (uuid1.Length == 4)
            uuid1 = FullUUID(uuid1);
        if (uuid2.Length == 4)
            uuid2 = FullUUID(uuid2);

        return (uuid1.ToUpper().Equals(uuid2.ToUpper()));
    }

    void SendString(string value)
    {
        var data = Encoding.UTF8.GetBytes(value);
        BluetoothLEHardwareInterface.WriteCharacteristic(selectedDeviceAddress, ServiceUUID, Characteristic, data, data.Length, false, (characteristicUUID) =>
        {
            BluetoothLEHardwareInterface.Log("Write Succeeded");
        });
    }

    void SendByte(byte value)
    {
        byte[] data = new byte[] { value };
        BluetoothLEHardwareInterface.WriteCharacteristic(selectedDeviceAddress, ServiceUUID, Characteristic, data, data.Length, false, (characteristicUUID) =>
        {
            BluetoothLEHardwareInterface.Log("Write Succeeded");
        });
    }
}
