# Contributing to GemPlay

Here should be documented information and best practices that may facilitate GemPlay's development.




## Version control

Most Unity .meta files are not commited to this repo.
This acts as a barrier to excessive dependence on file GUIDs.
Such dependence can otherwise make it difficult to collaborate on Unity projects.
See `.gitignore` for the .meta files which are not ignored (using the `!` pattern).


### git LFS

Before committing anything to your new branch, [set up git LFS for your local user account](https://git-lfs.github.com/).
This is required because, "Note that defining the file types Git LFS should track will not, by itself, convert any pre-existing files to Git LFS, such as files on other branches or in your prior commit history.".


### Pull Requests

The master branch should only be modified via Pull Requests (PRs).
A PR must be approved by the owners of any code it modifies before it can be merged to master.


### Unity version

Upgrading Unity always makes an entire new installation of Unity.
These installations are large.
It quickly becomes impractical to keep many versions of Unity installed.

The Unity version used to open a Unity project in the editor is specified in `project/ProjectSettings/ProjectVersion.txt`.
A Unity project can generally be opened in a newer version than that already specified version, but not in an older version.
When opening the project in UnityHub, if this exact version of Unity is not already installed, there are two options.
Either you can install the exact version of Unity, or you can open this project with a later version.

We want to control which version of Unity is being used for this project's development.
When opening this project with a later version of Unity than the one specified in `project/ProjectSettings/ProjectVersion.txt`,
Unity will then automatically re-import many packages and make other changes (e.g. changing the versoin in `project/ProjectSettings/ProjectVersion.txt`).
Do not commit any of these changes unless you intend to increment the oldest version of Unity which we can use to develop this project.




## Guidelines


### Unity scripting

Avoid creating unnecessary game objects and `MonoBehaviour` classes.
Most logic can be written in C# classes which do not inherit from `MonoBehaviour`.


### C# code

Microsoft's documentation of C# is very good.
Try to follow their naming conventions and other guidelines where appropriate.
Also, IDEs such as Visual Studio and Rider can often provide useful recommendations.


### Assembly definitions

When adding Assembly Definition References in an assembly definition, make sure that "Use GUIDs" is disabled.


### Tests

GemPlay uses Unity's test framework.
It is best to run the tests throughout development, whenever changing code.
Merging most PRs will require all tests to pass.

Some of the tests depend on a GemPy server which, by default, is configured to be on the local host.
