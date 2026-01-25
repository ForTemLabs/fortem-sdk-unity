# ForTem Core - API Documentation

## Overview

ForTem Core provides a lightweight, extensible framework for Unity game development.

## Namespaces

### `ForTem.Core`
Main namespace containing core framework functionality.

#### Classes

- **Core** - Main entry point for the framework
  - `Version` - Current version of ForTem Core
  - `Initialize()` - Initializes the framework

### `ForTem.Core.Editor`
Editor-specific utilities and tools.

## Getting Started

1. Import the ForTem Core package
2. Call `Core.Initialize()` in your game's startup code
3. Explore the API and extend as needed

## Examples

### Basic Initialization

```csharp
using ForTem.Core;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        Core.Initialize();
    }
}
```

## Contributing

To extend the framework:
1. Add new classes to the appropriate namespace
2. Document with XML comments
3. Add tests in the `Tests` folder
4. Update this documentation
