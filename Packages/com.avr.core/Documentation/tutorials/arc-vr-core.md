@logo

# arc-vr-core {#arc-vr-core}

Arc-vr-core is the core package of the arc-vr framework. It contains all base classes and components that the other packages make use of. These include all rudimentary VR features such as spatial tracking, controller-inputs and other utilities. In many cases, this package might be all you need for your project.

Classes are intentionally built to be inherited, derived and modified. Don't shy away from taking a look into the source code and adapting it to better suit your project.

# Feature List

## Documentation {#Documentation}

The documentation you are looking at right now, as well as docs for all other packages are included in arc-vr-core.
If you take a look inside `Packages/com.avr.core/Documentation/docs` you will find all html, css and resource files of the docs.

Each arc-vr component will have a button inside the Unity Editor that opens the respective documentation page in the browser (See (Editor Untilities)[@ref EditorUtilities]). If you are creating your own `AVR_Component`-objects and want them to link to the proper page, take a look at the (DocumentationUrl attribute)[@ref DocumentationUrlAttribute] to see how to set this link yourself.

These files are generated using [Doxygen 1.8.7](https://www.doxygen.nl/index.html). Under `Packages/com.avr.core/Documentation` you will find the doxygen executable as well as the respective [doxygen configuration file](https://www.doxygen.nl/manual/config.html) called `avr_dxy_config`.

You can re-generate the whole projects documentation any time by simply running `.\doxygen_1_8_7.exe .\avr_dxy_config` inside the Documentation folder. Doxygen will automatically extract code-summaries and produce up-to-date html files.

## Base classes

TODO

## Custom Editor-Attributes

TODO

## Utilities

TODO

## Editor Utilities

TODO

## Developers Console

TODO

## Settings system

TODO

## Package Builder

TODO

## Base class for Trial-Experiments

TODO

## Rig and Controller Components for tracking, input etc.

TODO
