# Ramstack.Structures

[![NuGet](https://img.shields.io/nuget/v/Ramstack.Structures.svg)](https://nuget.org/packages/Ramstack.Structures)
[![MIT](https://img.shields.io/github/license/rameel/ramstack.structures)](https://github.com/rameel/ramstack.structures/blob/main/LICENSE)

Ramstack.Structures is a .NET library providing various data structures and utilities.

## Installation

To install the `Ramstack.Structures` [NuGet package](https://www.nuget.org/packages/Ramstack.Structures) to your project, use the following command:

```console
dotnet add package Ramstack.Structures
```

## Features

The current release includes primary types:

* `Ramstack.Text.StringView`:<br>Represents a read-only view of a string, providing safe and efficient access to a subset of its characters.
* `Ramstack.Collections.ArrayView<T>`:<br>A generic read-only view of an array, allowing safe and efficient access to a subset of its elements.
* `Ramstack.Collections.ReadOnlyArray<T>`:<br>A generic read-only array wrapper, similar to `ImmutableArray<T>`, but with some performance improvements and better code generation in certain cases.

Additionally, the library provides the `StringViewComparer` class, which offers various comparators accessible through corresponding properties:

| Property                       | Description                                                                                                                                       |
|--------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------|
| **Ordinal**                    | Gets a **StringViewComparer** object that performs a case-sensitive ordinal string comparison.                                                    |
| **OrdinalIgnoreCase**          | Gets a **StringViewComparer** object that performs a case-insensitive ordinal string comparison.                                                  |
| **CurrentCulture**             | Gets a **StringViewComparer** object that performs case-sensitive string comparisons using the word comparison rules of the current culture.      |
| **CurrentCultureIgnoreCase**   | Gets a **StringViewComparer** object that performs case-insensitive string comparisons using the word comparison rules of the current culture.    |
| **InvariantCulture**           | Gets a **StringViewComparer** object that performs a case-sensitive string comparison using the word comparison rules of the invariant culture.   |
| **InvariantCultureIgnoreCase** | Gets a **StringViewComparer** object that performs a case-insensitive string comparison using the word comparison rules of the invariant culture. |

## Notes

The `ArrayView<T>` and `ReadOnlyArray<T>` types have a `ref readonly` indexer, which provides efficient access to elements of the underlying array,
avoiding expensive copying in the case of large structures.

This same optimization is also applied to the enumerators of these types. The `Current` property has a `ref readonly` signature,
which enables the following pattern when necessary:

```csharp
foreach (ref readonly HeavyStruct s in view)
{
    ...
}

```

## Supported versions

|      | Version    |
|------|------------|
| .NET | 6, 7, 8, 9 |

## Contributions

Bug reports and contributions are welcome.

## License
This package is released as open source under the **MIT License**.

See the [LICENSE](LICENSE) file for more details.
