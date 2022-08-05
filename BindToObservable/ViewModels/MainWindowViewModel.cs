using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using ReactiveUI;

namespace BindToObservable.ViewModels;


public class MainWindowViewModel : ObservableObject
{
    private readonly ReadOnlyObservableCollection<MyViewModel> myCollection;

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

        changes.WhenPropertyChanged(x => x.IsSelected, false)
            .DistinctUntilChanged()
            .Subscribe(_ => IsSomethingSelected = MyCollection.Any(x => x.IsSelected));

        changes.Connect();
    }


    bool _IsSomethingSelected;

    public bool IsSomethingSelected
    {
        get => _IsSomethingSelected;
        private set => SetProperty(ref _IsSomethingSelected, value);
    }


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