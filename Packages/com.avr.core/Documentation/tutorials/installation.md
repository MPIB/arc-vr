@logo

# Installation {#Installation}

##  Pre-Setup

You will have to perform a couple of simple steps before getting started with arc-vr:

- For your arc-vr project to run smoothly, make sure that you're using Unity version \unityversion.

- In addition, I highly recommend setting your fixed timestep to be 90Hz (standard refresh rate of most HMDs). To do this, head on over to `Edit > Project Settings` and under `Time > Fixed Timestep` set it to `0.01111111`.

No further steps are required to make your project arc-vr compatible. However, if you are planning on using SteamVR (HTC Vive or Valve Index) you will have to make Unity's XR Plug-in Manager compatible with the SteamVR input system (see next step).

## SteamVR/HTC Vive Compatibility

Arc-vr uses Unity's [XR Plug-in Manager](https://docs.unity3d.com/2020.2/Documentation/Manual/com.unity.xr.management.html) as a general API. Unfortunately, the XR Manager does not include a default plug-in for the HTC Vive or Valve Index. If you plan on using either of these (or using SteamVR in general), you'll need to perform a couple of extra steps before everything is in place.

You can try using Valve's own [OpenVR plugin](https://github.com/ValveSoftware/unity-xr-plugin), but I recommend to use Khronos' [OpenXR](https://docs.unity3d.com/Packages/com.unity.xr.openxr@0.1/manual/index.html) instead, as it supports a wider range of devices and generally has more features. The following steps will describe how to set up a project intended for the HTC Vive with OpenXR:

1. Open `Windows > Package Manager`. Display all packages from the 'Unity Registry', then find and select 'OpenXR Plugin'. Click 'Install'.

2. Open the `Edit > Project Settings`, and select XR Plug-in Management. Enable the 'OpenXR' option.

3. Under 'XR Plug-in Management' you should now be able to select 'OpenXR'.

4. Select 'OpenXR > Features' and tick 'HTC Vive Controller Profile' to enable compatibility with HTC Vive Controllers.

5. The OpenXR Plugin will likely be displayed with a red exclamation mark. Click on it to open the OpenXR Project Validation Window. Here, OpenXR will ask you to change a few project settings. You can click 'Fix All' or manually change these one-by-one.

\note If these steps are out of date or you're having trouble with OpenXR, refer to their official ['Getting Started' documentation](https://docs.unity3d.com/Packages/com.unity.xr.openxr@0.1/manual/index.html).

\htmlonly\endhtmlonly

\note If your project uses custom shaders that are not single-pass compatible, you will have to set 'Render Mode' to 'Multi-Pass'. Note that this will dramatically reduce performance, so only do this if you know that you have to. All shaders included in arc-vr are compatible with single-pass instanced rendering.

## Installing arc-vr packages

Your first step should be to choose which arc-vr packages you require for your project. Look at the [overview](@ref Overview) for a feature-list.

Once you've chosen which packages you need, head over to the [github release tab](https://github.com/MPIB/arc-vr/releases) and download the respective `.tgz` files.

Under `Windows > Package Manager` click the plus sign and select 'Add package from tarball'. Browse to the downloaded '.tgz' file and select it.

![img](../../res/images/install_tarball.jpg)

\note All arc-vr packages have arc-vr-core as a dependency. Install arc-vr-core first.
