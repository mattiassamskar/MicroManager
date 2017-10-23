using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace MicroManager
{
  public partial class IconViewModel : ViewModelBase
  {
    private Brush _iconBrush;

    public IconViewModel(IObservable<IEnumerable<string>> statesObservable)
    {
      var colorEvaluator = new ColorEvaluator();

      statesObservable.Subscribe(
        states => IconBrush = new SolidColorBrush(colorEvaluator.Evaluate(states)));
    }

    public Brush IconBrush
    {
      get => _iconBrush;
      set
      {
        value.Freeze();
        _iconBrush = value;
        OnPropertyChanged();
      }
    }
  }
}