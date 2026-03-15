using FluentAssertions;
using YandexSandbox.Api.Services;

namespace YandexSandbox.Tests.Unit.Api.Services;

public class InMemoryLockTests
{
    private readonly InMemoryLock _sut = new();

    [Fact]
    public async Task AcquireAsync_SameKey_BlocksUntilReleased()
    {
        var order = new List<int>();

        await using (await _sut.AcquireAsync("key-1"))
        {
            var blocked = Task.Run(async () =>
            {
                await using (await _sut.AcquireAsync("key-1"))
                {
                    order.Add(2);
                }
            });

            await Task.Delay(100);
            order.Add(1);
        }

        await Task.Delay(100);
        order.Should().Equal(1, 2);
    }

    [Fact]
    public async Task AcquireAsync_DifferentKeys_DoNotBlock()
    {
        await using (await _sut.AcquireAsync("key-a"))
        {
            var other = Task.Run(async () =>
            {
                await using (await _sut.AcquireAsync("key-b")) { }
            });

            var completed = await Task.WhenAny(other, Task.Delay(1000));
            completed.Should().BeSameAs(other);
        }
    }

    [Fact]
    public async Task AcquireAsync_ReleasedLock_CanBeReacquired()
    {
        await using (await _sut.AcquireAsync("key-1")) { }
        await using (await _sut.AcquireAsync("key-1")) { }
    }

    [Fact]
    public async Task AcquireAsync_CancellationToken_ThrowsWhenCancelled()
    {
        using var cts = new CancellationTokenSource();

        await using (await _sut.AcquireAsync("key-1"))
        {
            cts.CancelAfter(50);

            Func<Task> act = () => _sut.AcquireAsync("key-1", cts.Token);

            await act.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}
