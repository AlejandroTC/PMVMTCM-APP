using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using TMPro;

public class ESP32BLEManager : MonoBehaviour
{
    public string ServiceUUID = "4FAFC201-1FB5-459E-8FCC-C5C9C331914B";  // UUID del servicio del ESP32
    public string CharacteristicUUID = "BEB5483E-36E1-4688-B7F5-EA07361B26A8";  // UUID de la característica del ESP32

    public TMP_Text BluetoothStatus; // Asigna el objeto texto en el inspector
    public TMP_Text DataReceivedText; // Asigna el objeto texto en el inspector
    public Button scanButton;  // Asigna el botón en el inspector
    public Button connectButton;  // Asigna el botón en el inspector
    public Button disconnectButton;  // Asigna el botón en el inspector
    public Transform devicesListParent;  // Asigna el objeto padre en el inspector
    public GameObject deviceListItemPrefab;  // Prefab de la lista de dispositivos
    private DataSender dataSender; // Referencia al script DataSender

    private List<string> deviceAddresses = new List<string>();  // Lista de direcciones de dispositivos
    private string selectedDeviceAddress;  // Dirección del dispositivo seleccionado

    enum States  // Estados de la máquina de estados
    {
        None,  // Ninguno
        Scan,  // Escaneo
        Connect,  // Conexión
        RequestMTU,  // Solicitud de MTU
        Subscribe,  // Suscripción
        Unsubscribe,  // Desuscripción
        Disconnect,  // Desconexión
        Communication,  // Comunicación
    }

    private bool _connected = false;  // Estado de conexión
    private float _timeout = 0f;  // Tiempo de espera
    private States _state = States.None;  // Estado actual

    // Método de inicialización
    void Start()
    {
        scanButton.onClick.AddListener(StartScan);  // Asigna un listener de evento al botón
        connectButton.onClick.AddListener(ConnectToDevice);  // Asigna un listener de evento al botón
        disconnectButton.onClick.AddListener(DisconnectFromDevice);  // Asigna un listener de evento al botón

        BluetoothLEHardwareInterface.Initialize(true, false, () =>  // Inicializa el módulo Bluetooth
        {
            BluetoothStatus.text = "Bluetooth Inicializado";
        }, (error) =>
        {
            BluetoothLEHardwareInterface.Log("Error: " + error);  // Muestra un mensaje de error
        });
    }

    // Método de actualización
    void Update()
    {
        if (_timeout > 0f)  // Si el tiempo de espera es mayor a 0
        {
            _timeout -= Time.deltaTime;  // Disminuye el tiempo de espera
            if (_timeout <= 0f)  // Si el tiempo de espera es menor o igual a 0
            {
                _timeout = 0f;  // Establece el tiempo de espera a 0

                switch (_state)  // Evalúa el estado actual
                {
                    case States.None:  // Si el estado es None
                        break;

                    case States.Scan:  // Si el estado es Scan
                        StartScanning();  // Inicia el escaneo
                        break;

                    case States.Connect:  // Si el estado es Connect
                        StartConnecting();  // Inicia la conexión
                        break;

                    case States.RequestMTU:  // Si el estado es RequestMTU
                        RequestMTU();  // Solicita el MTU
                        break;

                    case States.Subscribe:  // Si el estado es Subscribe
                        SubscribeToCharacteristic();  // Se suscribe a la característica
                        break;

                    case States.Unsubscribe:  // Si el estado es Unsubscribe
                        UnsubscribeFromCharacteristic();  // Se desuscribe de la característica
                        break;

                    case States.Disconnect:  // Si el estado es Disconnect
                        DisconnectDevice();  // Desconecta el dispositivo
                        break;
                }
            }
        }
    }
    // Método para iniciar el escaneo
    void StartScan()
    {
        ClearDevicesList();  // Limpia la lista de dispositivos
        SetState(States.Scan, 0.1f);  // Establece el estado de la máquina de estados
    }
    // Método para iniciar la conexión
    void StartScanning()
    {
        BluetoothStatus.text = "Buscando dispositivos...";

        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) =>  // Escanea los dispositivos
        {
            if (!name.Contains("No Name"))  // Si el nombre del dispositivo no contiene "No Name"
            {
                if (!deviceAddresses.Contains(address))  // Si la lista de direcciones de dispositivos no contiene la dirección
                {
                    AddDeviceToList(address, name);  // Agrega el dispositivo a la lista
                }
            }
        }, null, false, false);
    }

    // Método para conectar con el dispositivo
    void ConnectToDevice()
    {
        if (!string.IsNullOrEmpty(selectedDeviceAddress))   // Si la dirección del dispositivo seleccionado no está vacía
        {
            SetState(States.Connect, 0.5f); // Establece el estado de la máquina de estados
        }
    }

    // Método para iniciar la conexión
    void StartConnecting()
    {
        BluetoothLEHardwareInterface.ConnectToPeripheral(selectedDeviceAddress, null, null, (address, serviceUUID, characteristicUUID) =>  // Conecta con el dispositivo
        {
            if (IsEqual(serviceUUID, ServiceUUID) && IsEqual(characteristicUUID, CharacteristicUUID))  // Si el UUID del servicio y de la característica son iguales
            {
                _connected = true;  // Establece el estado de conexión a verdadero
                SetState(States.RequestMTU, 2f);  // Establece el estado de la máquina de estados
                BluetoothStatus.text = "Conectado";  // Muestra un mensaje en la UI
            }
        }, (disconnectedAddress) =>  // Si se desconecta el dispositivo
        {
            BluetoothLEHardwareInterface.Log("Device disconnected: " + disconnectedAddress);  // Muestra un mensaje en la consola
            BluetoothStatus.text = "Desconectado";  // Muestra un mensaje en la UI
            _connected = false;  // Establece el estado de conexión a falso
        });
    }

    // Método para solicitar el MTU
    void RequestMTU()
    {
        BluetoothLEHardwareInterface.RequestMtu(selectedDeviceAddress, 185, (address, newMTU) =>  // Solicita el MTU
        {
            SetState(States.Subscribe, 0.1f);  // Establece el estado de la máquina de estados
        });
    }

    // Método para suscribirse a la característica
    void SubscribeToCharacteristic()
    {
        _state = States.None;  // Establece el estado a None
        // Se suscribe a la característica
        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(selectedDeviceAddress, ServiceUUID, CharacteristicUUID, null, (address, characteristicUUID, bytes) =>
        {
            string receivedData = Encoding.UTF8.GetString(bytes);   // Convierte los bytes a string
            DataReceivedText.text = receivedData; // Mostrar los datos recibidos en la UI
            PersistentDataManager.Instance.SetReceivedData(receivedData);
            Debug.Log("Received data: " + receivedData);    // Muestra los datos recibidos en la consola
        });
    }

    // Método para desuscribirse de la característica
    void UnsubscribeFromCharacteristic()
    {
        // Se desuscribe de la característica
        BluetoothLEHardwareInterface.UnSubscribeCharacteristic(selectedDeviceAddress, ServiceUUID, CharacteristicUUID, null);
        SetState(States.Disconnect, 4f);  // Establece el estado de la máquina de estados
    }

    void DisconnectDevice()
    {
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
    }

    // Método para desconectar el dispositivo
    void DisconnectFromDevice()
    {
        if (!string.IsNullOrEmpty(selectedDeviceAddress))   // Si la dirección del dispositivo seleccionado no está vacía
        {
            SetState(States.Unsubscribe, 0.1f); // Establece el estado de la máquina de estados
        }
    }

    void AddDeviceToList(string address, string name)
    {
        GameObject listItem = Instantiate(deviceListItemPrefab, devicesListParent);  // Instancia el prefab de la lista de dispositivos
        listItem.GetComponentInChildren<TextMeshProUGUI>().text = name; // Accede al componente TextMeshPro para establecer el texto
        Button listItemButton = listItem.GetComponentInChildren<Button>(); // Accede al botón dentro del prefab
        listItemButton.onClick.AddListener(() => SelectDevice(address)); // Asigna un listener de evento al botón
        deviceAddresses.Add(address);
    }

    void SelectDevice(string address)  // Método para seleccionar un dispositivo
    {
        BluetoothLEHardwareInterface.StopScan();  // Detiene el escaneo
        selectedDeviceAddress = address;  // Establece la dirección del dispositivo seleccionado
        BluetoothStatus.text = "Selected: " + address;  // Muestra un mensaje en la UI
    }

    void ClearDevicesList()  // Método para limpiar la lista de dispositivos
    {
        foreach (Transform child in devicesListParent)  // Recorre los objetos hijos del padre
        {
            Destroy(child.gameObject);  // Destruye los objetos hijos
        }
        deviceAddresses.Clear();  // Limpia la lista de direcciones de dispositivos
    }

    void SetState(States newState, float timeout)  // Método para establecer el estado de la máquina de estados
    {
        _state = newState;  // Establece el estado de la máquina de estados
        _timeout = timeout;  // Establece el tiempo de espera
    }

    string FullUUID(string uuid)  // Método para completar los UUID
    {
        return "0000" + uuid + "-0000-1000-8000-00805F9B34FB";      // Completa el UUID
    }

    bool IsEqual(string uuid1, string uuid2)  // Método para comparar los UUID
    {
        if (uuid1.Length == 4)  // Comprueba si los UUID tienen 4 caracteres
            uuid1 = FullUUID(uuid1);  // Completar los UUID
        if (uuid2.Length == 4)  // Comprueba si los UUID tienen 4 caracteres
            uuid2 = FullUUID(uuid2);    // Completar los UUID

        return uuid1.ToUpper().Equals(uuid2.ToUpper());  // Compara los UUID
    }
}
