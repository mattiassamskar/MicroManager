using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Windows.Media;
using MicroManager;
using Xunit;

namespace LdapSearch.Tests
{
  public class IconViewModelTests
  {
    [Fact]
    public void Icon_should_be_green_when_all_states_are_running()
    {
      // Arrange
      var observable = new Subject<IEnumerable<string>>();
      var iconViewModel = new IconViewModel(observable);

      // Act
      observable.OnNext(new List<string> { "Running", "Running", "Running" });

      // Assert
      Assert.Equal(Color.FromRgb(34, 177, 76), ((SolidColorBrush)iconViewModel.IconBrush).Color);
    }

    [Fact]
    public void Icon_should_be_yellow_when_some_states_are_running()
    {
      // Arrange
      var observable = new Subject<IEnumerable<string>>();
      var iconViewModel = new IconViewModel(observable);

      // Act
      observable.OnNext(new List<string> { "Running", "Running", "Stopped" });

      // Assert
      Assert.Equal(Colors.Yellow, ((SolidColorBrush)iconViewModel.IconBrush).Color);
    }


    [Fact]
    public void Icon_should_be_red_when_no_states_are_running()
    {
      // Arrange
      var observable = new Subject<IEnumerable<string>>();
      var iconViewModel = new IconViewModel(observable);

      // Act
      observable.OnNext(new List<string> { "Stopped", "Stopped", "Stopped" });

      // Assert
      Assert.Equal(Color.FromRgb(237, 28, 36), ((SolidColorBrush)iconViewModel.IconBrush).Color);
    }

    [Fact]
    public void Icon_should_be_transparent_when_no_states_in_list()
    {
      // Arrange
      var observable = new Subject<IEnumerable<string>>();
      var iconViewModel = new IconViewModel(observable);

      // Act
      observable.OnNext(new List<string>());

      // Assert
      Assert.Equal(Colors.Transparent, ((SolidColorBrush)iconViewModel.IconBrush).Color);
    }
  }
}
