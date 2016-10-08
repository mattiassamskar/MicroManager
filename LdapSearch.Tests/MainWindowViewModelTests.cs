using NSubstitute;
using System.Collections.Generic;
using Xunit;

namespace LdapSearch.Tests
{
  public class MainWindowViewModelTests
  {
    [Fact]
    public void Should_not_search_without_search_string()
    {
      // Arrange
      var ldapHandler = Substitute.For<ILdapHandler>();
      var mainWindowViewModel = new MainWindowViewModel(ldapHandler);
      mainWindowViewModel.SearchString = string.Empty;

      // Act
      mainWindowViewModel.SearchCommandExecuted();

      // Assert
      ldapHandler.DidNotReceive().GetUsers(Arg.Any<string>());
    }

    [Fact]
    public void Should_remove_old_search_result_when_searching_again()
    {
      // Arrange
      var ldapHandler = Substitute.For<ILdapHandler>();
      ldapHandler.GetUsers(Arg.Any<string>()).Returns(new List<User> { new User { SamAccountName = "b" } });

      var mainWindowViewModel = new MainWindowViewModel(ldapHandler);
      mainWindowViewModel.SearchString = "some string";
      mainWindowViewModel.Users.Add(new User { SamAccountName = "a" });

      // Act
      mainWindowViewModel.SearchCommandExecuted();

      // Assert
      Assert.True(mainWindowViewModel.Users.Count == 1);
      Assert.True(mainWindowViewModel.Users[0].SamAccountName == "b");
    }

    [Fact]
    public void Should_handle_multiple_search_strings()
    {
      // Arrange
      var ldapHandler = Substitute.For<ILdapHandler>();
      var mainWindowViewModel = new MainWindowViewModel(ldapHandler);
      mainWindowViewModel.SearchString = "a,b;c";

      // Act
      mainWindowViewModel.SearchCommandExecuted();

      // Assert
      ldapHandler.Received().GetUsers("a");
      ldapHandler.Received().GetUsers("b");
      ldapHandler.Received().GetUsers("c");
    }

    [Fact]
    public void Should_set_first_user_as_selected()
    {
      // Arrange
      var ldapHandler = Substitute.For<ILdapHandler>();
      ldapHandler.GetUsers(Arg.Any<string>()).Returns(new List<User> { new User { SamAccountName = "a" }, new User { SamAccountName = "b" } });

      var mainWindowViewModel = new MainWindowViewModel(ldapHandler);
      mainWindowViewModel.SearchString = "some string";

      // Act
      mainWindowViewModel.SearchCommandExecuted();

      // Assert
      Assert.True(mainWindowViewModel.SelectedUser.SamAccountName == "a");
    }

    [Fact]
    public void Should_fire_on_property_changed_event_when_changing_selected_user()
    {
      // Arrange
      var ldapHandler = Substitute.For<ILdapHandler>();
      var wasCalled = false;

      var mainWindowViewModel = new MainWindowViewModel(ldapHandler);
      mainWindowViewModel.PropertyChanged += (sender, args) => wasCalled = true;

      // Act
      mainWindowViewModel.SelectedUser = new User();

      // Assert
      Assert.True(wasCalled);
    }

    [Fact]
    public void Should_fire_on_property_changed_event_when_changing_search_string()
    {
      // Arrange
      var ldapHandler = Substitute.For<ILdapHandler>();
      var wasCalled = false;

      var mainWindowViewModel = new MainWindowViewModel(ldapHandler);
      mainWindowViewModel.PropertyChanged += (sender, args) => wasCalled = true;

      // Act
      mainWindowViewModel.SearchString = "some string";

      // Assert
      Assert.True(wasCalled);
    }
  }
}
