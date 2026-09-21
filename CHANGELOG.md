## [1.4.1] - 2026-09-21

- No code changes. Independent development continues from this release, outside of Bigpoint; 1.4.0 was the last version developed there.

## [1.3.8] - 2025-04-07
- Added Roslyn analyzer that makes sure that the null conditional operator is used for `Log()` calls.

## [1.3.7] - 2024-07-25
- Improved exception logging, adding clickable links to the stack trace for the editor console

## [1.3.6] - 2024-06-07
- Added separators in LogLevelWindow

## [1.3.5] - 2024-06-05
- Fixed log level overrides not saving if they were changed through the log level window's checkboxes

## [1.3.4] - 2024-02-29
- Added default unity log actions to log levels "debug" and "trace", so they can be activated through the log level window without adding extra code

## [1.3.3] - 2024-01-17
- Switched from EditorPrefs to PlayerPrefs for log level window
- Improved performance of LogEntry.ToString

## [1.3.2] - 2024-01-17
- Improved editor performance by caching LogCategory colors
- Changed log level window to reflect the global max log level

## [1.3.1] - 2024-01-17
- Fixed log level Window
- Updated log level window to forcibly override any other max log level
- Improved LogCategory handling

## [1.3.0] - 2024-01-09
- Added LoggingSetup methods for globally setting a max log level, including disabling logging completely

## [1.2.0] - 2024-01-04
- Added option to add flags to logs

## [1.1.0] - 2024-01-04
- Added option to add additional data to logs

## [1.0.0] - 2024-01-03
- Applied performance improvements

## [0.4.0] - 2024-01-03
- Disabled default UnityEngine.Debug logging behavior for channels that have a default logging action

## [0.3.4] - 2024-01-02
- Removed logging related lines from callstack

## [0.3.2] - 2023-11-29
- Bumped dependency version

## [0.3.1] - 2023-11-14
- Added log level window
- Added deterministically generated colors for LogCategories

## [0.3.0] - 2023-11-13
- Renamed "LogChannel" to "LogLevel"
- Added options to set max log levels for or mute LogCategories
- Added UnityLogHandler for piping Unity logs into the system
- Split default Unity logging and default log action, so they can be set up independently
- Improved smaller details

## [0.2.0] - 2023-09-27
- Updated logging syntax
- Added many more setup options

## [0.1.0] - 2023-09-25
- Initial version
