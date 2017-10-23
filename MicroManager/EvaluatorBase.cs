using System;
using System.Collections.Generic;

namespace MicroManager
{
  public partial class IconViewModel
  {
    public class EvaluatorBase<TIn, TOut>
    {
      private Dictionary<Predicate<TIn>, TOut> Subjects { get; } = new Dictionary<Predicate<TIn>, TOut>();

      protected void Add(Predicate<TIn> predicate, TOut result) => Subjects.Add(predicate, result);

      public TOut Evaluate(TIn value)
      {
        return Subjects.FirstOrDefault(s => s.Key(value)).Value;
      }
    }
  }
}