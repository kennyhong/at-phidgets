using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phidget22;
using UnityEngine.UI;
using Phidget22.Events;
using ChartAndGraph;
using System.Diagnostics;

public class AppManager : MonoBehaviour {

    VoltageRatioInput ratio = null;
    public Slider dataIntervalSlider;
    public Text ipAddressBox;
    public Text portBox;
    public Text minVal;
    public Text maxVal;
    public Text currVal;
    public Text errorText;
    public Text deviceReading;
    public GameObject errorBox;
    bool isRemote = false;
    public string phidgetPassword;
    public string phidgetServerName;
    VoltageRatioInput attachedDevice = null;
    bool notAttached = true;
    public string ipAddress = "";
    public int port = 8000;
    // Use this for initialization

    public GraphChart Graph;
    public int TotalPoints = 5;
    float lastTime = 0f;
    float lastX = 0f;
    public int currentTime = 0;
    Stopwatch stopwatch = new Stopwatch();

    private void Awake()
    {
        ratio = new VoltageRatioInput();
        ratio.SensorChange += Ratio_SensorChange;
        ratio.Attach += Ratio_Attach;
    }

    private void Ratio_SensorChange(object sender, VoltageRatioInputSensorChangeEventArgs e)
    {
        
    }

    void Start () {
        try
        { //set all the values grabbed from command line.  these values have defaults that are set in ExampleUtils.cs, you can check there to see them.
            ratio.Channel = 0; //selects the channel on the device to open
            ratio.DeviceSerialNumber = 538053; //selects the device or hub to open
            ratio.HubPort = 0; //selects the port on the hub to open
            ratio.IsHubPortDevice = true; //is the device a port on a VINT hub?

            if (isRemote) //are we trying to open a remote device?
            {
                ratio.IsRemote = true;
                Net.EnableServerDiscovery(ServerType.Device); //turn on network scan
                if (phidgetPassword != null && phidgetServerName != null)
                    Net.SetServerPassword(phidgetServerName, phidgetPassword); //set the password if there is one
            }
            else
                ratio.IsLocal = true;

            ratio.Open(); //open the device specified by the above parameters
        }
        catch (PhidgetException ex) { errorText.text = ("Error opening device: " + ex.Message); errorBox.SetActive(true); }

        dataIntervalSlider.onValueChanged.AddListener(delegate { slider_value_change(); });

        errorBox.SetActive(false);
        ipAddressBox.text = ipAddress;
        portBox.text = port.ToString();

        if (Graph == null) // the ChartGraph info is obtained via the inspector
            return;
        stopwatch.Start();
        Graph.DataSource.StartBatch(); // calling StartBatch allows changing the graph data without redrawing the graph for every change
        Graph.DataSource.ClearCategory("Pressure"); // clear the "Player 1" category. this category is defined using the GraphChart inspector
    }

    void Ratio_Attach(object sender, Phidget22.Events.AttachEventArgs e)
    {
        attachedDevice = (VoltageRatioInput)sender;

        try
        {
            attachedDevice.SensorType = VoltageRatioSensorType.PN_1139;
        }
        catch (PhidgetException ex) { errorText.text = "Error initializing device: " + ex.Message; errorBox.SetActive(true); }

    }

    // Update is called once per frame
    void Update () {
		if(attachedDevice != null && notAttached)
        {
            notAttached = false;
            setSliderValues();
        }
        deviceReading.text = attachedDevice.SensorValue.ToString() + " " + attachedDevice.SensorUnit.Symbol;
        float time = Time.time;
        if (lastTime + 0.5f < time)
        {
            lastTime = time;
            //            System.DateTime t = ChartDateUtility.ValueToDate(lastX);
            Graph.DataSource.AddPointToCategory("Pressure", stopwatch.ElapsedMilliseconds, attachedDevice.SensorValue * 1000f); // each time we call AddPointToCategory
        }
        Graph.DataSource.EndBatch(); // finally we call EndBatch , this will cause the GraphChart to redraw itself
    }

    private void setSliderValues()
    {
        minVal.text = attachedDevice.MinDataInterval.ToString() + " ms";
        dataIntervalSlider.minValue = attachedDevice.MinDataInterval;
        maxVal.text = attachedDevice.MaxDataInterval.ToString() + " ms";
        dataIntervalSlider.maxValue = attachedDevice.MaxDataInterval;
        currVal.text = attachedDevice.DataInterval.ToString() + " ms";
        dataIntervalSlider.value = attachedDevice.DataInterval;
    }

    void slider_value_change ()
    {
        ratio.DataInterval = (int) dataIntervalSlider.value;
    }


    void ratio_detach(object sender, Phidget22.Events.DetachEventArgs e)
    {
        
    }


}
