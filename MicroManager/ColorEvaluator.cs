using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace MicroManager
{
  public partial class IconViewModel
  {
    public class ColorEvaluator : EvaluatorBase<IEnumerable<string>, Color>
    {
      public ColorEvaluator()
      {
        Add(states => !states.Any(), Colors.Transparent);
        Add(states => states.All(s => s == "Running"), Color.FromRgb(34, 177, 76));
        Add(states => states.All(s => s == "Stopped"), Color.FromRgb(237, 28, 36));
        Add(_ => true, Colors.Yellow);
      }
    }
  }
}