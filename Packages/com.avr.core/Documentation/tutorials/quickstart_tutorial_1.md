@logo

# Intro

This quickstart tutorial should acquaint you with most arc-vr features as well as familiarize you with the intended workflow.

You'll need to know the basics of working with Unity and C#. If you need a refresher, refer to the official [Unity tutorials](https://learn.unity.com/).

For this tutorial we'll be building a simple VR experiment/game where the user will draw colored spheres from an urn after guessing which color he will draw.
We'll be targeting HTC Vive hardware and using the standard Unity rendering pipeline.
Here's a brief demonstration of what we'll be building:

TODO: Video

After reading, you should be able to:
- Set up a rudimentary VR PlayerRig with spatially tracked HMD and controller.
- Set up basic movement mechanics within the virtual environment.
- Let the player grab and interact with physics-items.
- Let the player interact with virtual UI elements.
- (Optional) Setting up an automated trial-based experiment.
- (Optional) Setting up data collection/logging.

The last two bullet points are optional, as they are intended for use in VR experiments/studies. If you're looking to build simple VR games, feel free to skip these.

# Video

If you prefer watching a video over reading text, here's the complete quickstart guide in video form:

TODO: Video

# Step 1: Setup

## Creating a new Unity project

As you might expect, our first step will be to create a new Unity project. The easiest way to go about this is to use Unity Hub. If you don't have already, download and install tha latest version of Unity Hub [here](https://unity3d.com/get-unity/download).

Arc-vr is intended to be used with \unityversion. Using a different version may very well lead to errors or malfunctioning features. You can download \unityversion for the hub from the [Unity download archive](https://unity3d.com/get-unity/download/archive).

Once you've downloaded \unityversion, you should be able to create a new project with that version using Unity Hub. HDRP, SRP and URP are all fully supported by arc-vr, but for the purposes of this tutorial, we'll be using the [Standard Rendering Pipeline](https://unity.com/srp) (select '3D' from Templates).

![img](../../res/images/qs_new_proj.jpg)

## Preparing our project

With your new, blank \unityversion project open, we'll need to perform a couple of steps before importing arc-vr packages.

### Time.fixedDeltaTime

[Time.fixedDeltaTime](https://docs.unity3d.com/ScriptReference/Time-fixedDeltaTime.html) determines the frequency at which Unity conducts physics updates for rigidbodies. By default this is set to 0.02s (ie 50Hz). Since most HMDs have a refresh-rate of at least 90Hz, physics-interactions will appear jagged when a player grabs and moves objects (which we intend to do).

In order to fix this, we simply set the frequency of fixed updates to also be 90Hz. To do this, head on over to `Edit > Project Settings` and under `Time > Fixed Timestep` set it to `0.01111111`.

## Importing OpenXR

Unfortunately, current native compatibility between SteamVR/OpenVR and Unity's XR Plugin Manager is a little lackluster. We'll be using [OpenXR](https://docs.unity3d.com/Packages/com.unity.xr.openxr@0.1/manual/index.html) in order to enable HTC Vive hardware to be used with the XR Plugin Manager.

Import and install OpenXR into your project by following these steps:
1. Open `Windows > Package Manager`. Display all packages from the 'Unity Registry', then find and select 'OpenXR Plugin'. Click 'Install'.

2. Open the `Edit > Project Settings`, and select XR Plug-in Management. Enable the 'OpenXR' option.

3. Under 'XR Plug-in Management' you should now be able to select 'OpenXR'.

4. Select 'OpenXR > Features' and tick 'HTC Vive Controller Profile' to enable compatibility with HTC Vive Controllers.

5. The OpenXR Plugin will likely be displayed with a red exclamation mark. Click on it to open the OpenXR Project Validation Window. Here, OpenXR will ask you to change a few project settings. You can click 'Fix All' or manually change these one-by-one.

\note If these steps are out of date or you're having trouble with OpenXR, refer to their official ['Getting Started' documentation](https://docs.unity3d.com/Packages/com.unity.xr.openxr@0.1/manual/index.html).

\htmlonly\endhtmlonly

\note This is merely a SteamVR (HTV Vive / Valve Index) and Unity XR compatibility issue. OpenXR is *not* a dependency for arc-vr. If you manage to get Vive/Index to work with the Unity XR Plugin Manager some other way (including input system), or you're using non-SteamVR hardware, you won't need to install OpenXR.

# Step 2: Importing arc-vr-core

Finally, we're ready to import arc-vr into our project. The arc-vr framework consists of a series of mutually-agnostic packages which you are listed [here](@ref Packages). All of them are dependent on the central `arc-vr-core` package, which provides all fundamental VR functionalities.

Head on over to [github release tab](https://github.com/MPIB/arc-vr/releases) and download the .tgz of the arc-vr-core package.

Once downloaded, open up the Unity Package Manager (`Windows > Package Manager`) and click the plus sign in the top left. Select 'Add package from tarball', browse to the downloaded '.tgz' file and select it. The Unity Package Manager should automatically import and install the package.

![img](../../res/images/install_tarball.jpg)

# Step 3: Getting Familiar

Just so we have something to look at, create a 3D plane in the center of the scene and add some geometry to it. Something like this will do just fine:

![img](../../res/images/qs_basic_scene.jpg)

Let's take a quick look at arc-vr's in-built developer's console:

The menu bar of your Unity Editor should now have an additional 'AVR' option. Click on `AVR > arc-vr` to view a small arc-vr welcome window.

![img](../../res/images/qs_top_menu.jpg)

It should list the arc-vr packages you have installed (only arc-vr-core) and present you with 4 buttons. Click the 4th (the right-most one) to open an 'ARC-VR Console' window. Alternatively, you can also just click on `AVR > Open DevConsole`.

This developer's console is a useful tool which is fully available inside the Unity Editor but also during runtime. It can also allow you to make some on-the-fly changes during runtime, should you have to. Type, for instance, `toggle Plane` and press Enter. If you have an object called 'Plane' in your scene it should be toggled from active, to inactive. If you want to add commands of your own or get more familiar with this useful tool, take a look at the [DevConsole documentation page](https://www.TODO.com).



Continue in [tutorial 2](tutorials/quickstart_tutorial_2.md).
