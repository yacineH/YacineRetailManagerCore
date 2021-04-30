using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using TRMDesktopUI.EventModels;
using TRMDesktopUILibrary.API;
using TRMDesktopUILibrary.Model;

namespace TRMDesktopUI.ViewModels
{
    public class ShellViewModel:Conductor<object>,IHandle<LogOnEvent>
    {
        //private LoginViewModel _loginVM;
        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        private ILoggedInUserModel _user;
        private IAPIHelper _apiHelper;
        //private SimpleContainer _container;

        public ShellViewModel(IEventAggregator events,SalesViewModel salesVM,
                              ILoggedInUserModel user,IAPIHelper apiHelper)
                              //SimpleContainer container)
        {
            //_loginVM = loginVM;
            _salesVM = salesVM;
            _events = events;
            _user = user;
            _apiHelper = apiHelper;
            //_container = container;

            _events.Subscribe(this);

            //parceque a chaque fois sellview generer on obtien
            //un nouveau LoginViewModel
            //ActivateItem(_loginVM);
            //ActivateItem(_container.GetInstance<LoginViewModel>());

            //remplacer par ca 
            ActivateItem(IoC.Get<LoginViewModel>());
        }

        public bool IsLoggedIn
        {
            get
            {
                bool output = false;
                if (string.IsNullOrWhiteSpace(_user.Token) == false)
                {
                    output = true;
                }
                return output;
            }
        }

        public void ExitApplication()
        {
            TryClose();
        }

        public void UserManagement()
        {
            ActivateItem(IoC.Get<UserDisplayViewModel>());
        }

        public void LogOut()
        {
            _user.ResetUserModel();
            _apiHelper.LogOffUser();
            ActivateItem(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
        public void Handle(LogOnEvent message)
        {
            ActivateItem(_salesVM);
            NotifyOfPropertyChange(()=> IsLoggedIn);
            //_loginVM = _container.GetInstance<LoginViewModel>();
        }
    }
}
