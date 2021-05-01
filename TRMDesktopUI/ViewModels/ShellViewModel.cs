using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

            _events.SubscribeOnPublishedThread(this);

            //parceque a chaque fois sellview generer on obtien
            //un nouveau LoginViewModel
            //ActivateItem(_loginVM);
            //ActivateItem(_container.GetInstance<LoginViewModel>());

            //remplacer par ca 
            ActivateItemAsync(IoC.Get<LoginViewModel>(),new CancellationToken());
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
            TryCloseAsync();
        }

        public async Task UserManagement()
        {
           await ActivateItemAsync(IoC.Get<UserDisplayViewModel>(),new CancellationToken());
        }

        public async Task LogOut()
        {
            _user.ResetUserModel();
            _apiHelper.LogOffUser();
            await  ActivateItemAsync(IoC.Get<LoginViewModel>(),new CancellationToken());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
        //public void Handle(LogOnEvent message)
        //{
        //    ActivateItem(_salesVM);
        //    NotifyOfPropertyChange(()=> IsLoggedIn);
        //    //_loginVM = _container.GetInstance<LoginViewModel>();
        //}

        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(_salesVM,cancellationToken);
            NotifyOfPropertyChange(() => IsLoggedIn);

        }
    }
}
