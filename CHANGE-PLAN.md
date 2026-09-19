# Change plan

> Draft. This file tracks what changed on the way to this repo and what I plan to change next. Edit freely.

## Origin

Originally developed at Bigpoint. Published here with Bigpoint's permission for research, education and other noncommercial use. Copyright (c) 2026 Bigpoint; see [LICENSE.md](LICENSE.md).

## Changes made before publishing

- Namespaces are now `TeaSpoons.*` and the package id is `com.tea-spoons.logging` (assemblies renamed to match).
- Internal build, registry and tracker references were removed; the repo uses GitHub Actions (`CI` and `Release`) built on `unity-ci-kit`.
- Added `LICENSE.md` (PolyForm Noncommercial 1.0.0), an install section in the README, and package metadata (author, license and documentation URLs).
- Removed the prebuilt `LoggingRoslynAnalyzer.dll`: its source is not part of this package.
- 1.4.0: `com.tea-spoons.package-core` is no longer a dependency. The editor assembly uses it for the menu root and `GUIColor` when the project has it (`TEASPOONS_PACKAGE_CORE`, set from the asmdef `versionDefines`) and otherwise uses the same `TeaSpoons/` string and a built-in `GUIColor`. Logging now has no dependencies.

## Planned changes

- [x] Tag and publish `v1.3.8` with the Release workflow.
- [x] Tag and publish `v1.4.0` with the Release workflow.
- [ ] Run this package's tests in CI with `unity-ci-kit` (needs a small test-project helper in the kit).
- [ ] Recreate the removed logging analyzer as original code, or drop it.
<!-- review-items:start -->
- [ ] **P1** Make category registration thread safe (a lock or `ConcurrentDictionary`, and a `[ThreadStatic]` builder for the prefix), with a test that creates 100 categories in parallel.
- [ ] **P1** Decide what a duplicate category name does (return the existing instance through `LogCategory.Get(name)`, or throw) and test it.
- [ ] **P1** Declares `unity: 2022.3`, but only Unity 6000.3.8f1 was tested. Add a Unity version matrix to CI once package tests run there (see the `unity-ci-kit` plan), or raise the minimum.
- [ ] **P2** Add an optional rolling file sink and a JSON sink built on `ILogData`, off by default (Unity Logging and ZLogger both offer these).
- [ ] **P2** Add a scripting define that strips logging calls from release builds, mirroring what `ams` does for its fallback.
- [ ] **P2** Evaluate an adapter to `Microsoft.Extensions.Logging`, so libraries can log to either.
<!-- review-items:end -->

<!-- review:start -->
## Review (September 2026)

Reviewed as a senior Unity engineer would: I read the code and compared the package with similar open-source projects (September 2026). Those projects are listed for ideas only. Nothing was copied from them, and their licenses are noted in case code is ever reused. Priorities: **P0** correctness bug or broken metadata, **P1** should be done soon, **P2** nice to have.

### Compared with

| Project | License | Worth noting |
|---|---|---|
| [Unity Logging (com.unity.logging)](https://docs.unity3d.com/Packages/com.unity.logging@1.2/manual/index.html) | Unity package | Structured, asynchronous logging with sinks (standard out, text or JSON files, `Debug.Log`, custom), callable from Burst code, with source generators. The docs say it is not a full replacement for `Debug.Log`. |
| [Cysharp/ZLogger](https://github.com/Cysharp/ZLogger) | not checked | Zero-allocation text and structured logger on top of Microsoft.Extensions.Logging, using string interpolation and a source generator. Its Unity build does not support structured logging. |

### Findings from reading the code

- **[Thread safety]** `LogEntry.ToString` locks its static `StringBuilder` (`LogEntry.cs`, line 53), but `LogCategory` does not: the registration `_allCategories[name] = this` (`LogCategory.cs`, line 123) and `GenerateLogPrefix`, which uses another static builder, are unsynchronized. Creating categories from several threads can corrupt them.
- **[Behavior]** Two categories with the same name silently replace each other in `_allCategories`, while `Equals` and `GetHashCode` treat them as equal. The last one wins in the log level window.
- **[Good]** The `category.Debug?.Log(...)` pattern does not evaluate the message when the level is off, because `?.` short-circuits the call and its arguments.
- **[Gap]** Output goes to the Unity console or to custom actions. There is no file sink, no rotation, no structured output beyond `ILogData`.
<!-- review:end -->

## Notes and ideas

_Add your own here._
