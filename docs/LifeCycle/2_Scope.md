# LifeCycle - Scope

#### Introduction

On of the compelling reasons to use scope's in ReactiveUI is their inherit ability for disposal of resources. Each time we subscribe to an observable or create a binding, we create constructs that will eventually have to be disposed of. 



#### Sample

When the `UserControl` is added to the Visual Tree (i.e. when it is rendered), the bindings are created. If we, however, would allow these bindings to remain in place until the app shuts down, we could be unnecessarily keeping the `UserControl` alive, as a view usually outlives its view model. This view could never be garbage collected and as a result needed memory accumulates. ReactiveUI solves this by tearing down the bindings when the `CompositeDisposable`, passed as argument to the scope's `Action`, is disposed of. In practice this will happen when the `UserControl` is removed from the Visual Tree.

```c#
public partial class ScoreView : RxUserControl<ScoreViewModel>
{
    public ScoreView()
    {
        InitializeComponent();

        this.WhenLoaded(disposables =>
        {
            this.OneWayBind(ViewModel,
                    viewModel => viewModel.Score,
                    view => view.ScoreLabel.Content)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel,
                    viewModel => viewModel.DecrementScoreCommand,
                    view => view.DecrBtn)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel,
                    viewModel => viewModel.IncrementScoreCommand,
                    view => view.IncrBtn)
                .DisposeWith(disposables);
        });
    }
}
```



#### ViewSink

The base `WhenLoaded` behavior is provided by an implementation of `ILoadedForViewFetcher`, and mapped to the ViewModel layer through the exposed `ViewSink` on `IViewAware` view models.

Inferno ships with only one implementation: `LoadedForLoadedViewFetcher`.

The minimal behavior applies to view components descending from `FrameworkElement` and will execute / dispose of the `WhenLoaded` scope, when the view is respectively added / removed from the Visual Tree.

However, if the view component implements `IViewFor<TViewModel>` and the contained view model implements `IActivate` (such as `Screen`), the `WhenLoaded` scope will:
- Execute when *both* the view model (including all conducted children) `IsActive` *and* the view is added to the Visual Tree.
- Disposed when *either* the view model is deactivated *or* the view is removed from the Visual Tree.

In other words, the view becomes aware of the viewmodel's lifecycle and will hold of execution of the `WhenLoaded` scope, until the viewmodel becomes active.



#### Activator

The `WhenActivated` scope is the easiest to grasp, as activation is enforced by a `Conductor` its boundaries are clearly defined.

The `WhenInitialized` scope's purpose may not be apparent on first sight. However, a `Conductor` can not only deactivate a child, it may also choose to close a child and remove it from it's `Items` collection. At this point both the child's `WhenActivated` scope's `disposables` (assuming the child was still active) and its `WhenInitialized` scope's `disposables` will be cleaned up.



#### Switch

Another kind of construct that may come in handy are scope based switches. Inferno ships with two default implementations `WhenActivatedSwitch` and `WhenInitializedSwitch`.

You can use them to avoid race conditions during activation. Take the following code sample, here the original problem was that we would get a null ref exception if `WhenAnyValue(x => x.Score)` was evaluated before `_score` was assigned a value. By wrapping the code with `WhenInitializedSwitch` and providing an initial value of `false`, the first evaluation of `x.Score` will only happen after the `WhenInitialized` scope has completed.

```c#
public class ScoreViewModel : Screen
{
    private RxPropertyHelper<int> _score;

    public ScoreViewModel(string backgroundColor)
    {
        this.WhenInitialized(disposables =>
        {
            IncrementScoreCommand = ReactiveCommand.Create(() => 1).DisposeWith(disposables);

            CanDecrement =
                this.WhenInitializedSwitch(
                    this
                        .WhenAnyValue(x => x.Score)
                        .Select(score => score > 0),
                    false);

            DecrementScoreCommand = ReactiveCommand.Create(() => -1, CanDecrement).DisposeWith(disposables);

            _score =
                Observable.Merge(
                        IncrementScoreCommand,
                        DecrementScoreCommand)
                    .Scan(0, (acc, delta) => acc + delta)
                    .ToProperty(this, x => x.Score)
                    .DisposeWith(disposables);
        })
    }

    public int Score => _score.Value;

    public IObservable<bool> CanDecrement { get; private set; }
    public ReactiveCommand<Unit, int> IncrementScoreCommand { get; private set; }
    public ReactiveCommand<Unit, int> DecrementScoreCommand { get; private set; }
}
```



#### Next

[DialogManager](../DialogManager/DialogManager.md)

