using System;
using System.Threading;
using System.Threading.Tasks;
using TriLibCore;
using TriLibCore.General;
using TriLibCore.Interfaces;
using TriLibCore.Utils;

public static class ConstructorThreadUtils
{
    public static Task RequestNewThreadFor<T>(
        T context,
        CancellationToken cancellationToken,
        Action<T> onStart,
        Action<T> onComplete = null,
        Action<IContextualizedError> onError = null,
        int timeout = 0,
        string name = null,
        bool startImmediately = true,
        Action<T> onCompleteSameThread = null)
        where T : class, IAssetLoaderContext
    {

        CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        
        if (timeout != 0)
            cancellationTokenSource.CancelAfter(timeout * 1000);
        
        cancellationToken = cancellationTokenSource.Token;
        context.Context.CancellationTokenSource = cancellationTokenSource;

        if (context.Context.Async)
        {
            Task task = new Task((Action)(() =>
            {
                if (!string.IsNullOrWhiteSpace(name))
                    Thread.CurrentThread.Name = name;
                try
                {
                    new ContextualizedAction<T>(onStart, context).Invoke();
                    if (onCompleteSameThread != null)
                        onCompleteSameThread(context);
                    if (onComplete == null)
                        return;
                    Dispatcher.InvokeAsync<T>(onComplete, context);
                }
                catch (Exception ex)
                {
                    if (!(ex is ContextualizedError<T> contextualizedError2))
                        contextualizedError2 = new ContextualizedError<T>(ex, context);
                    ContextualizedError<T> context1 = contextualizedError2;
                    Dispatcher.InvokeAsyncUnchecked<IContextualizedError>(
                        onError ?? new Action<IContextualizedError>(ReThrow),
                        (IContextualizedError)context1);
                }
            }), cancellationToken);
            if (startImmediately)
                task.Start();
            context.Context.Tasks.Add(task);
            return task;
        }

        try
        {
            onStart(context);
            if (onCompleteSameThread != null)
                onCompleteSameThread(context);
            if (onComplete != null)
                onComplete(context);
        }
        catch (Exception ex)
        {
            if (onError != null)
            {
                if (!(ex is ContextualizedError<T> contextualizedError3))
                    contextualizedError3 = new ContextualizedError<T>(ex, context);
                ContextualizedError<T> contextualizedError4 = contextualizedError3;
                onError((IContextualizedError)contextualizedError4);
            }
            else
            {
                ContextualizedError<T> contextualizedError;
                contextualizedError = new ContextualizedError<T>(ex, context);
                throw contextualizedError;
            }
        }

        return (Task)null;
    }

    public static Task RunThreadSimple(
        Action onStart,
        int timeout = 0,
        string name = null,
        bool startImmediately = true)
    {
        Task task = new Task((Action)(() =>
        {
            if (!string.IsNullOrWhiteSpace(name))
                Thread.CurrentThread.Name = name;
            onStart();
        }));
        task.Start();
        return task;
    }

    public static Task RunThread<T>(
        T context,
        ref CancellationToken cancellationToken,
        Action<T> onStart,
        Action<T> onComplete = null,
        Action<IContextualizedError> onError = null,
        int timeout = 0,
        string name = null,
        bool startImmediately = true)
        where T : IAssetLoaderContext
    {
        if (cancellationToken == CancellationToken.None)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            if (timeout != 0)
                cancellationTokenSource.CancelAfter(timeout * 1000);
            cancellationToken = cancellationTokenSource.Token;
            context.Context.CancellationTokenSource = cancellationTokenSource;
        }

        Task task = new Task((Action)(() =>
        {
            if (!string.IsNullOrWhiteSpace(name))
                Thread.CurrentThread.Name = name;
            try
            {
                new ContextualizedAction<T>(onStart, context).Invoke();
                if (onComplete == null)
                    return;
                Dispatcher.InvokeAsync<T>(onComplete, context);
            }
            catch (Exception ex)
            {
                if (onError != null)
                {
                    if (!(ex is ContextualizedError<T> context3))
                        context3 = new ContextualizedError<T>(ex, context);
                    Dispatcher.InvokeAsyncUnchecked<IContextualizedError>(onError, (IContextualizedError)context3);
                }
                else
                {
                    if (!(ex is ContextualizedError<T> context4))
                        context4 = new ContextualizedError<T>(ex, context);
                    Dispatcher.InvokeAsyncUnchecked<ContextualizedError<T>>(
                        new Action<ContextualizedError<T>>(ReThrow), context4);
                }
            }
        }), cancellationToken);
        if (startImmediately)
            task.Start();
        context.Context.Tasks.Add(task);
        return task;
    }

    private static void ReThrow(IContextualizedError contextualizedError) =>
        throw contextualizedError.GetInnerException();
}