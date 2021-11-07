![Logo](https://github.com/MPIB/arc-vr/blob/main/Packages/com.avr.core/Package_Resources/avr_logo_bw_wide.png?raw=true)

ARC-VR is a Unity3D framework that aims to ameliorate the development process of VR applications or games by providing any and all basic VR features, such as movement, physics-interaction and UI. Thus ensuring the development process doesn't get congested at an early stage with the implementation of redundant, basic VR functionalities.

Built in-house at the [Max Planck Institute for Human Development](https://www.mpib-berlin.mpg.de/en), the framework is primarily intended to simplify and streamline the building process of VR-based psychological experiments and studies.

This project is licensed under the [GNU General Public License v3.0](https://github.com/MPIB/arc-vr/blob/main/LICENSE).

The framework is still work-in-progress, largely untested and prone to bugs. Feel free to report any issues you encounter!

Here's a brief demonstration of the systems capabilities:

[![Video-Link](https://img.youtube.com/vi/NHDEzg9Detg/0.jpg)](https://www.youtube.com/watch?v=NHDEzg9Detg)

# How to get started?

Most of the docs and tutorials are accessible through the Unity Editor once the arc-vr-core package is installed. Follow these simple steps to get started:
- Create a new project with Unity 2020.3.21
- Download *com.avr.core-1.0.0.tgz* from the [github releases](https://github.com/MPIB/arc-vr/releases)
- In your unity project open `Window > Package Manager`, click on the plus sign in the top left and select `Add package from tarball`. Browse to and select the file you downloaded in the previous step.
- When the import is done, click on `AVR > Documentation` in the top menubar to open the docs in your browser.

# Packages

ARC-VR consists of a set of packages, all of which are dependent on a central `arc-vr-core` package, but otherwise function completely independently.
This allows you to pick and choose which features you need for your project and avoid unnecessary dependencies.

Here's an overview of what each package provides:

**arc-vr-core**:
- Documentation
- Trackable VR Controllers
- Base VR PlayerRig
- Integrated developer's console
- Integrated settings system
- Property logger
- Utilities

**arc-vr-motion**:
- Movement Providers (Locomotion/Teleportation etc.)
- Turn Provider
- Customizable shaders and effects for aiming reticules

**arc-vr-ui**:
- VR UI Interaction Provider
- Base UI Elements
- Object-Propery to UI-Element Linkage

**arc-vr-avatar**:
- Player-Avatar-Integration
- Basic Pose-Naturalizer

**arc-vr-phys**:
- Physics Interaction Providers

**arc-vr-net**:
- Multiplayer integration through MLAPI
- Player-Spawning System
- Basic 3rd-Person CharacterController
