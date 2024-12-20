# Changelog

## [2.3.1] - 2024-11-30
* Scripts generated via editor tools now use four spaces for indentation instead of tabs.
* Fixed an issue where the quick start core wizard failed to create a prefab for the core project context in Unity 6.

## [2.3.0] - 2024-11-26
Added Extras folder with additional tools and utilities:
* `FluentBinding` and `ContainerFluentExtensions` - Provides fluent binding syntax for easier configuration.
* `UnityObjectMonoInstaller` - A variant of MonoInstaller that supports binding a single Unity object (e.g., a MonoBehaviour or ScriptableObject) assigned through the inspector.
* `ChildrenCompositeMonoInstaller` - A variant of MonoInstaller that finds and installs other MonoInstallers within the same GameObject and its children.
* `CoreBehaviour` - A base class for objects that can be enabled or disabled, and integrate with the core loading callbacks for initialization or cleanup.

## [2.2.0] - 2022-10-25
* Added editor tools (the quick start core wizard and the core management window)

## [2.1.1] - 2022-09-12
* Fixed a typo in the changelog headers.

## [2.1.0] - 2022-07-13
* Reworked loading processes (internally).
* Added `CoreController.OnLoadablesProgressed(LoadablesProgress)`.
* Added `CoreController.OnKeepUnloading(OperationHandle)`.
* Added automated dynamic injection (`DynamicInjector<T>` and `DynamicInjectorBase`).
* Improved documentation comments.
* Improved [the documentation page](https://kempnymaciej.github.io/alchemy-core/).

## [2.0.0] - 2022-06-05
* Removed the `ICoreState` interface.
* `CoreController` no longer has a built-in state machine.
* Added a hierarchical finite state machine (`AlchemyBow.Core.States`).
* Improved documentation comments.
* Improved [the documentation page](https://kempnymaciej.github.io/alchemy-core/).


## [1.0.0] - 2022-03-31
This is the first official release of the AlchemyBow.Core package.
* Added reflection baking.
* Added deep reflection type lookup for injection attributes.
* Added the dynamic collection binding feature.
* Normalized names of callback methods.
* Simplified container bind methods.
* Removed update methods form the `ICoreState` interface.
* Added documentation comments for all public and protected members.
* Added [the documentation page](https://kempnymaciej.github.io/alchemy-core/).