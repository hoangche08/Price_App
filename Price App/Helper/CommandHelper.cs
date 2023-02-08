using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Price_App.Helper;

public static class CommandHelper
{
    private static readonly SemaphoreSlim CommandLock = new(1);
    public static AsyncCommand CreateAsyncCommand(Func<Task> taskFunc) => new(CommandLock, taskFunc);
    public static AsyncCommand<T> CreateAsyncCommand<T>(Func<T, Task> taskFunc) => new(CommandLock, taskFunc);
}

public class AsyncCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly SemaphoreSlim _semaphoreSlim;

    public AsyncCommand(SemaphoreSlim semaphoreSlim, Func<Task> execute)
    {
        _semaphoreSlim = semaphoreSlim;
        _execute = execute;
    }

    public bool CanExecute(object? parameter) => true;


    private async Task ExecuteAsync()
    {
        try
        {
            if (!await _semaphoreSlim.WaitAsync(50)) return;
            Mouse.OverrideCursor = Cursors.Wait;
            
            await _execute();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        } 
        finally
        {
            Mouse.OverrideCursor = null;
            _semaphoreSlim.Release();
        }
        
    }

    public async void Execute(object? parameter)
    {
        await ExecuteAsync();
    }

    public event EventHandler? CanExecuteChanged;
}

public class AsyncCommand<T> : ICommand
{
    private readonly Func<T, Task> _execute;
    private readonly SemaphoreSlim _semaphoreSlim;

    public AsyncCommand(SemaphoreSlim semaphoreSlim, Func<T, Task> execute)
    {
        _semaphoreSlim = semaphoreSlim;
        _execute = execute;
    }

    public bool CanExecute(object? parameter) => true;


    private async Task ExecuteAsync(T parameter)
    {
        try
        {
            if (!await _semaphoreSlim.WaitAsync(50)) return;
            
            await _execute(parameter);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        } 
        finally
        {
            _semaphoreSlim.Release();
        }
        
    }

    public async void Execute(object? parameter)
    {
        await ExecuteAsync((T) parameter);
    }

    public event EventHandler? CanExecuteChanged;
}