# GemPlay

Most of GemPlay's C# code is divided into assemblies.
In GemPlay, each assembly has its own folder with its code residing in its own namespace.
Each assembly other than `Core` is either a Module or an Integration (respectively residing in the `Modules/` or `Integrations/` folder.
The only GemPlay assembly on which a Module should depend is the `Core` assembly, i.e. no Module should depend on any other Module.
Integrations, generally required for producing any useful user-facing features, may depend on any combination of assemblies.

`Applications` is where Unity scenes are finally made for the purpose of developing, building and deploying applications to various platforms and devices.
Using GemPlay's C# code in a Unity scene requires instantiating some combination of Modules and Integrations from a `MonoBehaviour` attached to a game object in that scene.
