﻿using InvvardDev.EZLayoutDisplay.Desktop.Model.Service.Interface;
using InvvardDev.EZLayoutDisplay.Desktop.View;
using InvvardDev.EZLayoutDisplay.Desktop.ViewModel;
using Moq;
using Xunit;

namespace InvvardDev.EZLayoutDisplay.Tests
{
    public class MainViewModelTest
    {
        [ Fact ]
        public void MainViewModelConstructor()
        {
            //Arrange
            var mockWindowService = new Mock<IWindowService>();
            var mockApplicationService = new Mock<IApplicationService>();

            //Act
            var mainViewModel = new MainViewModel(mockWindowService.Object, mockApplicationService.Object);

            //Assert
            Assert.Equal("Show Layout", mainViewModel.TrayMenuShowLayoutCommandLabel);
            Assert.Equal("Settings", mainViewModel.TrayMenuShowSettingsCommandLabel);
            Assert.Equal("Exit", mainViewModel.TrayMenuExitCommandLabel);
        }

        [ Fact ]
        public void ShowLayoutCommand()
        {
            //Arrange
            var mockWindowService = new Mock<IWindowService>();
            mockWindowService.Setup(w => w.ShowWindow<DisplayLayoutWindow>()).Verifiable();
            var mockApplicationService = new Mock<IApplicationService>();

            //Act
            var mainViewModel = new MainViewModel(mockWindowService.Object, mockApplicationService.Object);
            mainViewModel.ShowLayoutCommand.Execute(null);

            //Assert
            mockWindowService.Verify(w => w.ShowWindow<DisplayLayoutWindow>(), Times.AtLeastOnce);
        }

        [ Fact ]
        public void ShowSettingsCommand()
        {
            //Arrange
            var mockWindowService = new Mock<IWindowService>();
            mockWindowService.Setup(w => w.ShowWindow<SettingsWindow>()).Verifiable();
            var mockApplicationService = new Mock<IApplicationService>();

            //Act
            var mainViewModel = new MainViewModel(mockWindowService.Object, mockApplicationService.Object);
            mainViewModel.ShowSettingsCommand.Execute(null);

            //Assert
            mockWindowService.Verify(w => w.ShowWindow<SettingsWindow>(), Times.AtLeastOnce);
        }

        [ Fact ]
        public void ExitApplicationCommand()
        {
            //Arrange
            var mockWindowService = new Mock<IWindowService>();
            var mockApplicationService = new Mock<IApplicationService>();
            mockApplicationService.Setup(a => a.ShutdownApplication()).Verifiable();

            //Act
            var mainViewModel = new MainViewModel(mockWindowService.Object, mockApplicationService.Object);
            mainViewModel.ExitApplicationCommand.Execute(null);

            //Assert
            mockApplicationService.Verify(w => w.ShutdownApplication(), Times.Once);
        }
    }
}