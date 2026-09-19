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

## Notes and ideas

_Add your own here._
