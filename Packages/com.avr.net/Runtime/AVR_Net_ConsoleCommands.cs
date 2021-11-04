using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Netcode;

using AVR.Core;

namespace AVR.Net
{
    /// <summary>
    /// arc-vr-net console commands.
    /// </summary>
    [ExecuteInEditMode]
    public class AVR_Net_ConsoleCommands
    {
        [RuntimeInitializeOnLoadMethod]
        static void InitCallbacks()
        {
            try {
                NetworkManager.Singleton.OnClientConnectedCallback += (id) => AVR_DevConsole.csuccess("Client #"+id+" connected.", "NetworkManager");

                NetworkManager.Singleton.OnClientDisconnectCallback += (id) => AVR_DevConsole.cprint("Client #" + id + " disconnected.", "NetworkManager");

                NetworkManager.Singleton.OnServerStarted += () => {
                    AVR_DevConsole.csuccess("Server started.", "NetworkManager");
                    AVR_DevConsole.command("getaddress", false);
                    AVR_DevConsole.command("getport", false);
                };

                NetworkManager.Singleton.OnServerStarted += () => {
                    foreach(var c in Object.FindObjectsOfType<AVR_Component>()) {
                        c.onNetworkStart();
                    }
                };
            } catch(System.Exception) {
                AVR_DevConsole.warn("arc-vr-net is present in the project but there is no MLAPI Networkmanager! arc-vr-net callback functions will be disabled.");
            }
        }

        #if UNITY_EDITOR
        [InitializeOnLoadMethod]
        #endif
        [RuntimeInitializeOnLoadMethod]
        static void InitCommands()
        {
            AVR_DevConsole.register_command("start_host", (s) => {
                NetworkManager.Singleton.StartHost();
            }, 0, "Hosts a game.");

            AVR_DevConsole.register_command("start_host", (s) => {
                AVR_DevConsole.command("setport " + s[0], false);
                NetworkManager.Singleton.StartHost();
            }, 1, "Hosts a game at given portnumber.");

            AVR_DevConsole.register_command("start_client", (s) => {
                NetworkManager.Singleton.StartClient();
            }, 0, "Tries to connect to remote server.");

            AVR_DevConsole.register_command("start_client", (s) => {
                AVR_DevConsole.command("setaddress " + s[0], false);
                AVR_DevConsole.command("setport " + s[1], false);
                NetworkManager.Singleton.StartClient();
            }, 2, "Tries to connect to remote server at given ip address and port.");

            AVR_DevConsole.register_command("start_server", (s) => {
                NetworkManager.Singleton.StartServer();
            });

            AVR_DevConsole.register_command("stop_host", (s) => {
                NetworkManager.Singleton.Shutdown();
            });

            AVR_DevConsole.register_command("stop_server", (s) => {
                NetworkManager.Singleton.Shutdown();
            });

            AVR_DevConsole.register_command("stop_client", (s) => {
                NetworkManager.Singleton.Shutdown();
            });

            AVR_DevConsole.register_command("disconnect", (s) => {
                NetworkManager.Singleton.Shutdown();
            });

            AVR_DevConsole.register_command("spawnas", (s) => {
                AVR_Settings.set("/net/playerPrefabHashGenerator", s[0]);
                AVR_DevConsole.cprint("Set /net/playerPrefabHashGenerator to "+s[0], "AVR_Settings");
            }, 1, "Sets the /net/playerPrefabHashGenerator setting to the given argument to be used by a playerSpawn object.");

            AVR_DevConsole.register_command("getport", (s) =>
            {
                /* TODO
                try {
                    MLAPI.Transports.UNET.UNetTransport unetT = (MLAPI.Transports.UNET.UNetTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
                    AVR_DevConsole.print("Port: " + unetT.ServerListenPort);
                }
                catch (System.InvalidCastException) {
                    AVR_DevConsole.error("Could not get UNetTransport from NetworkManager. Are you using a different transport type?");
                }
                catch (System.Exception) {
                    AVR_DevConsole.error("Could not set Port.");
                }
                */
            }, 0, "Print NetworkManager ConnectPort.");

            AVR_DevConsole.register_command("getaddress", (s) => {
                /*
                try {
                    MLAPI.Transports.UNET.UNetTransport unetT = (MLAPI.Transports.UNET.UNetTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
                    AVR_DevConsole.print("Address: " + unetT.ConnectAddress);
                }
                catch (System.InvalidCastException) {
                    AVR_DevConsole.error("Could not get UNetTransport from NetworkManager. Are you using a different transport type?");
                }
                catch (System.Exception) {
                    AVR_DevConsole.error("Could not set Port.");
                }
                */
            }, 0, "Print NetworkManager ConnectPort.");

            AVR_DevConsole.register_command("setport", (s) => {
                /*
                try {
                    MLAPI.Transports.UNET.UNetTransport unetT = (MLAPI.Transports.UNET.UNetTransport) NetworkManager.Singleton.NetworkConfig.NetworkTransport;
                    unetT.ConnectPort = unetT.ServerListenPort = int.Parse(s[0]);
                }
                catch(System.InvalidCastException) {
                    AVR_DevConsole.error("Could not get UNetTransport from NetworkManager. Are you using a different transport type?");
                }
                catch(System.Exception) {
                    AVR_DevConsole.error("Could not set Port.");
                }
                */
            }, 1, "Set NetworkManager ConnectPort and ServerListenPort to given parameter.");

            AVR_DevConsole.register_command("setaddress", (s) =>
            {
                /*
                try
                {
                    MLAPI.Transports.UNET.UNetTransport unetT = (MLAPI.Transports.UNET.UNetTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
                    unetT.ConnectAddress = s[0];
                }
                catch (System.InvalidCastException)
                {
                    AVR_DevConsole.error("Could not get UNetTransport from NetworkManager. Are you using a different transport type?");
                }
                catch (System.Exception)
                {
                    AVR_DevConsole.error("Could not set Address.");
                }
                */
            }, 1, "Set NetworkManager ConnectAddress to given parameter.");

            AVR_DevConsole.register_command("getaddress", (s) =>
            {
                /*
                try {
                    MLAPI.Transports.UNET.UNetTransport unetT = (MLAPI.Transports.UNET.UNetTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
                    AVR_DevConsole.print("Current Transport Address: " + unetT.ConnectAddress + ":" + unetT.ConnectPort);
                }
                catch (System.InvalidCastException)
                {
                    AVR_DevConsole.error("Could not get UNetTransport from NetworkManager. Are you using a different transport type?");
                }
                catch (System.Exception)
                {
                    AVR_DevConsole.error("Could not gat Address.");
                }
                */
            });
        }
    }
}
