# Changelog

## [2.1.1] - 2022-09-12
* Fix a typo in the changelog headers.

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