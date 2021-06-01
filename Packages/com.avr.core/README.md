For setup:

- Create a project with Unity 2020.2.2f1
- Import OpenXR package, select it under Project Settings > XR Plug-in Management
- Fix OpenXR issues and (for HTC Vive) set OpenXR Runtime to SteamVR with HTC Controller Configuration
- Adjust Time.fixedTimestep to 0.011111f
- Edit >> Project Settings >> Player >> Other Settings >> Active Input Handling set that to "Both"

import arc-vr-core