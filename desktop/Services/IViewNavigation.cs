using desktop.Models;
using desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services
{
    public interface IViewNavigation
    {
        public void GoTo<VM>(Bundle bundle = null) where VM : ViewModelBase;
        public void GoToAndCloseCurrent<VM>(ViewModelBase currentViewModel, Bundle bundle = null) where VM : ViewModelBase;
        public void GoToAndCloseOthers<VM>(Bundle bundle = null) where VM: ViewModelBase;
        public void Close(ViewModelBase viewModel);

    }
}
