using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DocumentFormat.OpenXml.Math;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services
{
    public class DesktopDialogService : IDialogService
    {
        public async Task<IDialogService.DialogResult> ShowDialog(string title, string text, IDialogService.DialogType dialogType)
        {
            var icon = new Bitmap(AssetLoader.Open(new Uri("avares://desktop/Assets/icon.ico")));
            var boxYesNo = MessageBoxManager
                        .GetMessageBoxCustom(
                            new MessageBoxCustomParams
                            {
                                ButtonDefinitions = new List<ButtonDefinition>
                                {
                                    new ButtonDefinition { Name = "Да", },
                                    new ButtonDefinition { Name = "Нет", },
                                },
                                ContentTitle = title,
                                ContentMessage = text,
                                CanResize = false,
                                ShowInCenter = true,
                                Topmost = true,
                                SystemDecorations = SystemDecorations.Full,
                                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                                ImageIcon = icon,
                                WindowIcon = new WindowIcon(icon),                               
                            });
            var dialogStan = new MessageBoxStandardParams()
            {
                Topmost = true,
                ButtonDefinitions = ButtonEnum.Ok,
                ContentTitle = title,
                ContentMessage = text,
                WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner,
                WindowIcon = new WindowIcon(icon)
            };
            if(dialogType == IDialogService.DialogType.Standard)
            {
                var box = MessageBoxManager.GetMessageBoxStandard(dialogStan);
                return (IDialogService.DialogResult)await box.ShowAsync();
            }
            else
            {
                var result = await boxYesNo.ShowAsync();
                if (result== "Да") return IDialogService.DialogResult.Yes;
                else if(result == "Нет") return IDialogService.DialogResult.No;
            }
            return IDialogService.DialogResult.Ok;
            

        }
    }
}
