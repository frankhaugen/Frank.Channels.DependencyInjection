# Library design

## Registration

`ServiceCollectionExtensions.AddChannel<T>()` registers:

- `Channel<T>` as a singleton
- `ChannelReader<T>` and `ChannelWriter<T>` as singletons resolved from that channel

`IChannelFactory` is registered once (singleton) when the first channel is added.

## Constraints

- **Type parameter**: `T` must be a **reference type** (`where T : class`).
- **Duplicates**: Registering the same `Channel<T>` twice throws `InvalidOperationException` (checked before adding the new registration).

## Channel kinds

- **Unbounded** — default overload; uses `ChannelType.Unbounded` with default `ChannelSettings`.
- **Bounded** — `ChannelType.Bounded` with `ChannelSettings` controlling capacity, full mode, and single reader/writer flags.

Implementation details live in `ChannelFactory` and `IChannelFactory`.

## Consumer alignment

The library targets `Microsoft.Extensions.DependencyInjection` **10.x** alongside **net10.0**. Applications on older runtimes should reference an older major version of this package that matches their stack.
