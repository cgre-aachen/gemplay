# GemPlay Unity Project




## File structure
Git version control is configured with `.gitignore`.
The remaining files follow the typical Unity project structure:
- `ProjectSettings/` contains many YAML files which configure various aspects of how Unity behaves with this project open, including behavior in the Unity Editor as well as build settings.
- `Packages/` contains files specifying the Unity packages which are imported via the Unity Package Manager (UPM).
- Everything else goes into `Assets`.




## Set up

### GemPy server

Set the address to the GemPy server host in `project/Assets/Resources/GemPlay/ScriptableObjects/ServerSettings.asset`.

### REST client

There is an open issue for making our preferred `RestClient` package installable via UPM: https://github.com/proyecto26/RestClient/issues/146

In the meantime, do the following:

#### Install the `RestClient` package manually using NuGet
Download NuGet CLI executable from https://www.nuget.org/downloads.

Download the package using the NuGET CLI, e.g. 

    .\nuget install Proyecto26.RestClient -Version 2.6.1
    
That downloads `Proyecto26.RestClient.2.6.1` and `RSG.Promise.3.0.1`.
    
Delete `RSG.Promise.3.0.1/lib/netstandard2.0`, letting the `lib/net35` version remain.
Otherwise Unity will stumble over the multiple definitions.
    
Move/copy `Proyecto26.RestClient.2.6.1` and `RSG.Promise.3.0.1` to `project/Assets/Plugins`.

#### Assets

Some external assets must be imported via the Unity Asset Store, because they are not yet compatible with the Unity Package Manager (UPM).
These are:
- [Lunar Mobile Console - FREE](https://assetstore.unity.com/packages/tools/gui/lunar-mobile-console-free-82881)
