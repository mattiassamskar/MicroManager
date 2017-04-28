using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace MicroManager
{
  public class IconViewModel : ViewModelBase
  {
    private Brush _overlayColor;

    public IconViewModel(IObservable<IEnumerable<string>> statesObservable)
    {
      statesObservable.Subscribe(
        serviceInfos =>
          {
            if (!serviceInfos.Any())
            {
              OverlayColor = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
              OverlayColor = serviceInfos.All(si => si == "Running")
                               ? new SolidColorBrush(Color.FromRgb(34, 177, 76))
                               : new SolidColorBrush(Color.FromRgb(237, 28, 36));
            }
          });
    }

    public Brush OverlayColor
    {
      get
      {
        return _overlayColor;
      }

      set
      {
        value.Freeze();
        _overlayColor = value;
        OnPropertyChanged();
      }
    }
  }
}