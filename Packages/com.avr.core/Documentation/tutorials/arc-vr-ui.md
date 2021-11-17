@logo

# arc-vr-ui {#arc-vr-ui}

@youtube{HFI-RG5FEqw}

\note If you replace the StandaloneInputModule on your EventSystem object with a InputSystemUIModule (as you might be prompted to do), the UIInteractionProvider might not work as intended. (It will take input bindings from the InputSystemUIModule as opposed to the UIInteractionProvider.)
Reason: At the moment arc-vr-ui uses the old Unity InputSystem, while OpenXR uses the new one. If you are using the ui-package, you will have to set `Project Settings > Player > Active Input Handling` to "Both", for it to work as intended. There are plans to migrate arc-vr-ui to the new InputSystem in the near future.
