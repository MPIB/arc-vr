using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MLAPI;

using AVR.Core;

namespace AVR.Net
{
    [ExecuteInEditMode]
    public class AVR_Net_ConsoleCommands
    {
        #if UNITY_EDITOR
        [InitializeOnLoadMethod]
        #endif
        [RuntimeInitializeOnLoadMethod]
        static void InitCommands()
        {
            AVR_DevConsole.register_command("start_host", (s) => {
                NetworkManager.Singleton.StartHost();
            });

            AVR_DevConsole.register_command("start_client", (s) => {
                NetworkManager.Singleton.StartClient();
            });

            AVR_DevConsole.register_command("start_server", (s) => {
                NetworkManager.Singleton.StartServer();
            });

            AVR_DevConsole.register_command("stop_host", (s) => {
                NetworkManager.Singleton.StopHost();
            });

            AVR_DevConsole.register_command("stop_server", (s) => {
                NetworkManager.Singleton.StopServer();
            });

            AVR_DevConsole.register_command("stop_client", (s) => {
                NetworkManager.Singleton.StopClient();
            });

            AVR_DevConsole.register_command("disconnect", (s) => {
                NetworkManager.Singleton.StopClient();
            });
        }
    }
}
