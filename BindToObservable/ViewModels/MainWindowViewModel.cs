using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;

namespace BindToObservable.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<MyViewModel> myCollection;

    public MainWindowViewModel()
    {
        var changes = Observable.Defer(() => Observable.Range(1, 4))
            .Select(i => new MyViewModel(i))
            .ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Replay()
            .RefCount();

        changes
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out myCollection)
            .Subscribe();

        IsSomethingSelected = changes
            .AutoRefresh(x => x.IsSelected)
            .ToCollection()
            .Select(x => x.Any(model => model.IsSelected));
    }

    public IObservable<bool> IsSomethingSelected { get; }

    public ReadOnlyObservableCollection<MyViewModel> MyCollection => myCollection;
}

public class MyViewModel : ReactiveObject
{
    private bool isSelected;

    public MyViewModel(int i)
    {
        Number = i;
    }

    public int Number { get; }

    public bool IsSelected
    {
        get => isSelected;
        set => this.RaiseAndSetIfChanged(ref isSelected, value);
    }
}