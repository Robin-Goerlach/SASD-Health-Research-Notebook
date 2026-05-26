# Milestone 1 Implementation Notes

## Implemented areas

- Application shell with WPF
- Dashboard cards
- Navigation placeholder
- Health topic list
- Create-health-topic wizard shell
- JSON persistence
- Small backup-before-write mechanism
- Smoke-test console runner

## Layering

### Domain

Contains the central `HealthTopic` entity and enums for status and organization priority.

### Application

Contains use-case-oriented services and contracts. The UI calls the `HealthTopicService` instead of directly writing JSON.

### Infrastructure

Contains the JSON repository. Later, SQLite can be added here behind the same repository interface.

### WPF

Contains the UI shell, view models and the first wizard dialog.

## Security and privacy notes

- No cloud access.
- No telemetry.
- No health data logging.
- Generic UI error messages.
- JSON is not encrypted yet. Encryption belongs to a later milestone and should be treated as important before productive real-world use.

## Why JSON first?

JSON is transparent, easy to inspect and easy to debug. It is a good first persistence layer while the domain and UI are still evolving.

SQLite should become the next serious storage step once the model stabilizes.
