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
    private ObservableAsPropertyHelper<bool> isSomethingSelected;

    public MainWindowViewModel()
    {
        var changes = Observable.Range(1, 4)
            .Select(i => new MyViewModel(i))
            .ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Publish();

        changes
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out myCollection)
            .Subscribe();

        var selectedChanges = changes
            .AutoRefresh(x => x.IsSelected)
            .ToCollection()
            .Select(x => x.Any(model => model.IsSelected));

        selectedChanges
            .ToProperty(this, x => x.IsSomethingSelected, out isSomethingSelected);

        selectedChanges.Subscribe(v => Debug.WriteLine($"Something selected: {v}"));

        changes.Connect();
    }

    public bool IsSomethingSelected => isSomethingSelected.Value;

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