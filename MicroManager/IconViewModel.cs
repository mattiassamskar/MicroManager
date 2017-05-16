using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace MicroManager
{
  public class IconViewModel : ViewModelBase
  {
    private readonly List<StateEvaluator> _stateEvaluators = new List<StateEvaluator>();
    private Brush _iconBrush;

    public IconViewModel(IObservable<IEnumerable<string>> statesObservable)
    {
      _stateEvaluators.Add(new StateEvaluator(states => !states.Any(), Colors.Transparent));
      _stateEvaluators.Add(new StateEvaluator(states => states.All(s => s == "Running"), Color.FromRgb(34, 177, 76)));
      _stateEvaluators.Add(new StateEvaluator(states => states.All(s => s == "Stopped"), Color.FromRgb(237, 28, 36)));
      _stateEvaluators.Add(new StateEvaluator(_ => true, Colors.Yellow));

      statesObservable.Subscribe(
        states => IconBrush = new SolidColorBrush(_stateEvaluators.First(e => e.Evaluator(states)).Color));
    }

    public Brush IconBrush
    {
      get { return _iconBrush; }

      set
      {
        value.Freeze();
        _iconBrush = value;
        OnPropertyChanged();
      }
    }

    private class StateEvaluator
    {
      public StateEvaluator(Func<IEnumerable<string>, bool> evaluator, Color color)
      {
        Evaluator = evaluator;
        Color = color;
      }

      public Func<IEnumerable<string>, bool> Evaluator { get; }

      public Color Color { get; }
    }
  }
}