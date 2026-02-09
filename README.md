# Feature Flag Engine – (C# / .NET 8)

## Overview

This repository contains a **CLI-based Feature Flag Engine** implemented in **C# (.NET 8)**.

The system models how feature flags are commonly used in production environments to control functionality at runtime without redeployments. Features can be enabled or disabled globally, or overridden for specific users, groups, or regions, with deterministic evaluation rules.

The solution focuses on:
- Correctness of behavior
- Clear separation of concerns
- Predictable, testable logic
- Pragmatic tradeoffs suitable for a time-boxed exercise

---

## How to Run

### Prerequisites
- .NET 8 SDK installed

### Setup

```bash
dotnet restore
dotnet run --project FeatureFlagsCli -- init
```

---

## Usage Examples

### Create a Feature
```bash
dotnet run --project FeatureFlagsCli -- create-feature new_ui true "New UI rollout"
```

### User Override
```bash
dotnet run --project FeatureFlagsCli -- add-override new_ui user alice false
```

### Group Override
```bash
dotnet run --project FeatureFlagsCli -- add-override new_ui group beta_testers true
```

### Region Override
```bash
dotnet run --project FeatureFlagsCli -- add-override new_ui region eu true
```

---

## Evaluation Precedence

```
User → Group → Region → Global
```

---

## Running Tests

```bash
dotnet test
```

---

## Tradeoffs & Next Steps

- CLI-only (no UI)
- No percentage rollouts
- SQLite for simplicity

Future improvements:
- In-memory caching
- Parameterized tests
- Percentage rollouts

---

Thank you for reviewing.
