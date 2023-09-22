using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.InputSystem;

public class ImmersiveAttackManager : MonoBehaviour
{
    [SerializeField] private InputActionReference disorientInputAction;
    [SerializeField] private InputActionReference chaperoneInputAction;
    [SerializeField] private InputActionReference terminalInputAction;
    [SerializeField] private InputActionReference overlayInputAction;

    // Use this for initialization
    [SerializeField] private Camera[] cameras;
    [SerializeField] private GameObject overlayCanvas;
    [SerializeField] private GameObject overlayElementAd;
    [SerializeField] private GameObject overlayElementVideo;

    private bool mIsChaperoneActive = true;
    private int mOverlayActive = 0;
    private int currentCamera = 0;
    private string mKeyboardText = "";
    private TouchScreenKeyboard mKeyboard;

    // Use this for initialization
    void Start()
    {
        overlayElementAd.SetActive(false);
        overlayElementVideo.SetActive(false);
        overlayCanvas.SetActive(false);
        mKeyboard = null;

        //cameras[0].gameObject.SetActive(false);
        cameras[0].enabled = true;
        for (int i = 1; i<cameras.Length;i++)
        {
            cameras[i].enabled = false;
            //[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update() { 
    
        if(currentCamera == 1){
            cameras[1].transform.Translate(Vector3.forward * Time.deltaTime);
        }
        
        /*if (mKeyboard != null)
        {
            if(mKeyboard.status == TouchScreenKeyboard.Status.Done)
            {
                mKeyboardText = mKeyboard.text;
                RunCommand(mKeyboardText);
            }
        }*/
    }

    private void OnEnable()
    {
        disorientInputAction.action.started += Disorient;
        chaperoneInputAction.action.started += Chaperone;
        terminalInputAction.action.started += Terminal;
        overlayInputAction.action.started += Overlay;
    }

    private void OnDisable()
    {
        disorientInputAction.action.started -= Disorient;
        chaperoneInputAction.action.started -= Chaperone;
        terminalInputAction.action.started -= Terminal;
        overlayInputAction.action.started -= Overlay;
    }

    private void Disorient(InputAction.CallbackContext context)
    {
        int old = currentCamera;
        currentCamera = (currentCamera + 1) % cameras.Length;
        Debug.Log($"Switching from {cameras[old].name} to {cameras[currentCamera].name}.");
        if (currentCamera == 1)
            cameras[currentCamera].transform.position = cameras[0].transform.position;
        //cameras[currentCamera].gameObject.SetActive(true);
        //cameras[old].gameObject.SetActive(false);
        cameras[currentCamera].enabled = true;
        cameras[old].enabled = false;

        /*if (normalCamera.isActiveAndEnabled)
        {
            Debug.Log("Enable Disorientation Attack");
            normalCamera.gameObject.SetActive(false);
            disorientCamera.gameObject.SetActive(true);
            disorientCamera.transform.position = normalCamera.transform.position;
        }
        else
        {
            Debug.Log("Disable Disorientation Attack");
            normalCamera.gameObject.SetActive(true);
            disorientCamera.gameObject.SetActive(false);
            disorientCamera.transform.TransformPoint(normalCamera.transform.position);
        }*/

    }

    private void Chaperone(InputAction.CallbackContext context)
    {
        
        string lAdbCommand = "";
        if (mIsChaperoneActive)
        {
            Debug.Log("Enable Chaperone");
            mIsChaperoneActive = true;
            lAdbCommand = "setprop debug.oculus.guardian_pause 1";
        }
        else
        {
            Debug.Log("Disable Chaperone");
            mIsChaperoneActive = false;
            lAdbCommand = "setprop debug.oculus.guardian_pause 0";
        }

        // Starte den Befehl als Prozess
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        startInfo.FileName = "adb";
        startInfo.Arguments = "shell " + lAdbCommand;
        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        Debug.Log("Created StartInfo");
        process.StartInfo = startInfo;
        Debug.Log("Loaded StartInfo");
        process.Start();
        Debug.Log("Started Process");

        // Lese die Ausgabe des Befehls
        string output = process.StandardOutput.ReadToEnd();
        Debug.Log("ADB output: " + output);

        process.WaitForExit();

    }

    private void Overlay(InputAction.CallbackContext context)
    {
        switch (mOverlayActive)
        {
            case 0:
                Debug.Log("Enable Overlay Ad");
                overlayCanvas.SetActive(true);
                overlayElementAd.SetActive(true);
                mOverlayActive = 1;
                break;
            case 1:
                Debug.Log("Enable Overlay Video");
                overlayElementAd.SetActive(false);
                overlayElementVideo.SetActive(true);
                mOverlayActive = 2;
                break;
            default:
                Debug.Log("Disable Overlay");
                overlayCanvas.SetActive(false);
                overlayElementAd.SetActive(false);
                overlayElementVideo.SetActive(false);
                mOverlayActive = 0;
                break;
        }   
    }

    private void Terminal(InputAction.CallbackContext context)
    {
        if (mKeyboard == null)
        {
            Debug.Log("Show Keyboard");
            mKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.ASCIICapable, false, true, false, false, "Input");
        }
        else
            mKeyboard = null;
    }

    private void RunCommand(string input)
    {
        Debug.Log("Running command with input " + input);
        System.Diagnostics.Process process;
        System.Diagnostics.ProcessStartInfo startInfo;
        string output;

        process = new System.Diagnostics.Process();
        startInfo = new System.Diagnostics.ProcessStartInfo();
        startInfo.FileName = input.Split(" ")[0];
        startInfo.Arguments = input.Substring(startInfo.FileName.Length + 1);
        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        process.StartInfo = startInfo;
        Debug.Log("Running command with FileName=" + startInfo.FileName + " and Arguments=" + startInfo.Arguments);
        process.Start();

        // Lese die Ausgabe des Befehls
        output = process.StandardOutput.ReadToEnd();
        Debug.Log("Output: " + output);
    }
}
