using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

namespace AVR.Net {
    /// <summary>
    /// Lets you run the start_host, start_client and start_server commands through a very simple UI.
    /// </summary>
    public class AVR_BasicNetworkMenu : MonoBehaviour
    {
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
            }

            GUILayout.EndArea();
        }

        static void StartButtons()
        {
            if (GUILayout.Button("Host")) AVR.Core.AVR_DevConsole.command("start_host");
            if (GUILayout.Button("Client")) AVR.Core.AVR_DevConsole.command("start_client");
            if (GUILayout.Button("Server")) AVR.Core.AVR_DevConsole.command("start_server");
        }

        static void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);

            if(NetworkManager.Singleton.IsServer)
            {
                if (GUILayout.Button("Shutdown Server")) NetworkManager.Singleton.StopServer();
            }
            else if (NetworkManager.Singleton.IsHost)
            {
                if (GUILayout.Button("Stop Host")) NetworkManager.Singleton.StopHost();
            }
            else if (NetworkManager.Singleton.IsClient)
            {
                if (GUILayout.Button("Disconnect")) NetworkManager.Singleton.StopClient();
            }
        }
    }
}