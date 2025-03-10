using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Transport.Sockets.Client.Helpers;

namespace HotChocolate.Transport.Sockets.Client.Protocols;

internal sealed class DataMessageObserver : IObserver<IOperationMessage>, IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(0);
    private readonly ConcurrentQueue<IDataMessage> _messages = new();
    private readonly string _id;
    private Exception? _error;
    private bool _disposed;

    public DataMessageObserver(string id)
    {
        _id = id;
    }

    public async ValueTask<IDataMessage?> TryReadNextAsync(CancellationToken ct)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException($"{nameof(DataMessageObserver)} is disposed.");
        }

        await _semaphore.WaitAsync(ct);

        if (_error is not null)
        {
            throw _error;
        }

        _messages.TryDequeue(out var message);
        return message;
    }

    public void OnNext(IOperationMessage value)
    {
        if (value is IDataMessage message && message.Id.EqualsOrdinal(_id))
        {
            _messages.Enqueue(message);
            _semaphore.Release();
        }
    }

    public void OnError(Exception error)
    {
        _error = error;
        _semaphore.Release();
    }

    public void OnCompleted()
        => _semaphore.Release();

    public void Dispose()
    {
        if (!_disposed)
        {
            _semaphore.Dispose();
            _disposed = true;
        }
    }
}
