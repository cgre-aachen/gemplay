# GemPlay.Applications.Main




## Platforms


### Unity Editor, Windows and Oculus Link

#### Building in the Unity Editor
- In Build Settings, set Platform to Standalone (e.g. Windows).
- At *Edit > Project Settings > XR Plug-in Management > Android settings*, *Initialize XR on Startup* should be disabled.
- Further, in *Plug-in Providers*, only Oculus should be enabled. (Soon we should also be using Windows Mixed Reality here with the HoloLens 2.)
- Select the Main game object in the Scene Hierarchy and make sure that the option to *Stop XR Subsystems after Start* is enabled.

#### Using the app
The workflow begins with the desktop app.

Enter immersive XR mode by clicking the XR button.


### Android handheld AR

#### Building in the Unity Editor
- In Build Settings, set Platform to Android.
- At *Edit > Project Settings > XR Plug-in Management > Android settings*, *Initialize XR on Startup* should be enabled.
- Further, in *Plug-in Providers*, only ARCore should be enabled.
- Select the Main game object in the Scene Hierarchy and make sure that the option to *Stop XR Subsystems after Start* is disabled.

#### Using the app
The workflow begins with the same view as in the Unity Editor.

Enter handheld AR mode by clicking the AR button.
